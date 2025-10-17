using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using YourProject.Data;
using YourProject.Helpers;
using YourProject.Models;

namespace YourProject.Controllers
{

	public class PhieuMuonController : Controller
	{
		private readonly LibraryContext _context;

		public PhieuMuonController(LibraryContext context)
		{
			_context = context;
		}

		[Authorize]
		public async Task<IActionResult> Index()
		{
			var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value;

			if (roleClaim == "Admin")
			{
				var phieuMuons = await _context.PhieuMuon
					.Include(p => p.DocGia)
					.Include(p => p.ThuThu)
					.Include(p => p.ChiTietPhieuMuons)
						.ThenInclude(ct => ct.Sach)
					.ToListAsync();

				return View("Index", phieuMuons);
			}

			return RedirectToAction("Create");
		}




		public async Task<IActionResult> Create()
		{
			var userIdClaim = User.FindFirst("UserId");
			if (userIdClaim == null)
				return RedirectToAction("Login", "Account");

			int docGiaId = int.Parse(userIdClaim.Value);
			var docGia = await _context.DocGia.FirstOrDefaultAsync(d => d.Id == docGiaId);
			if (docGia == null)
				return RedirectToAction("Login", "Account");

			ViewBag.DocGia = docGia;

			var cart = HttpContext.Session.GetObjectFromJson<List<Sach>>("Cart") ?? new List<Sach>();
			ViewBag.Cart = cart;

			var thuThus = await _context.ThuThu.ToListAsync();
			ViewBag.ThuThus = new SelectList(thuThus, "MaThuThu", "HoTen");

			return View();
		}


[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Create(DateTime? NgayHenTra, int MaThuThu, List<int> MaSachList)
{
    var userIdClaim = User.FindFirst("UserId");
    if (userIdClaim == null)
        return RedirectToAction("Login", "Account");

    int docGiaId = int.Parse(userIdClaim.Value);

    if (NgayHenTra == null)
    {
        TempData["ErrorMessage"] = "Vui lòng chọn ngày hẹn trả.";
        return RedirectToAction("Create");
    }

    var thuThu = await _context.ThuThu.FindAsync(MaThuThu);
    if (thuThu == null)
    {
        TempData["ErrorMessage"] = "Thủ thư không hợp lệ.";
        return RedirectToAction("Create");
    }

    var cart = HttpContext.Session.GetObjectFromJson<List<Sach>>("Cart");
    if (cart == null || !cart.Any())
    {
        TempData["ErrorMessage"] = "Giỏ sách trống, không thể tạo phiếu mượn!";
        return RedirectToAction("Create");
    }

    // Tạo phiếu mượn mới
    var phieuMuon = new PhieuMuon
    {
        MaDocGia = docGiaId,
        MaThuThu = MaThuThu,
        NgayMuon = DateTime.Now,
        NgayHenTra = NgayHenTra.Value,
        TrangThai = "Chưa trả",
        DaTra = false
    };
    _context.PhieuMuon.Add(phieuMuon);
    await _context.SaveChangesAsync();

    // Duyệt qua danh sách sách được chọn
    foreach (var maSach in MaSachList)
    {
        var sachChon = cart.FirstOrDefault(s => s.MaSach == maSach);
        if (sachChon != null)
        {
            // Thêm chi tiết phiếu mượn
            _context.ChiTietPhieuMuon.Add(new ChiTietPhieuMuon
            {
                MaPhieuMuon = phieuMuon.MaPhieuMuon,
                MaSach = sachChon.MaSach
            });

            // Giảm số lượng sách
            sachChon.SoLuong -= 1;
        }
    }

    // Cập nhật lại giỏ: loại bỏ những sách có số lượng = 0
    cart.RemoveAll(s => s.SoLuong <= 0);
    HttpContext.Session.SetObjectAsJson("Cart", cart);

    await _context.SaveChangesAsync();

    ViewBag.SuccessMessage = $"Tạo phiếu mượn thành công cho {MaSachList.Count} sách.";
    ViewBag.DocGia = await _context.DocGia.FirstOrDefaultAsync(d => d.Id == docGiaId);
    ViewBag.Cart = cart;
    ViewBag.ThuThus = new SelectList(await _context.ThuThu.ToListAsync(), "MaThuThu", "HoTen");

    return View();
}




		// Danh sách phiếu mượn

		[HttpGet]
		public IActionResult ChiTiet(int id)
		{
			var phieu = _context.PhieuMuon
				.Include(p => p.ChiTietPhieuMuons)
					.ThenInclude(ct => ct.Sach)
				.FirstOrDefault(p => p.MaPhieuMuon == id);

			if (phieu == null)
				return Content("<p>Không tìm thấy phiếu mượn.</p>", "text/html");

			var chiTiet = phieu.ChiTietPhieuMuons.Select(ct => new
			{
				TenSach = ct.Sach.TenSach,
				NgayTra = ct.NgayTra
			}).ToList();

			string html = $@"
        <p><b>Ngày mượn:</b> {phieu.NgayMuon:yyyy-MM-dd}</p>
        <p><b>Ngày hẹn trả:</b> {phieu.NgayHenTra:yyyy-MM-dd}</p>
        <p><b>Trạng thái:</b> {phieu.TrangThai}</p>
        <hr />
        <table style='width:100%; border-collapse: collapse;'>
            <tr>
                <th style='border:1px solid #ccc; padding:6px;'>Tên sách</th>
                <th style='border:1px solid #ccc; padding:6px;'>Ngày trả</th>
            </tr>";

			foreach (var item in chiTiet)
			{
				html += $@"
            <tr>
                <td style='border:1px solid #ccc; padding:6px;'>{item.TenSach}</td>
                <td style='border:1px solid #ccc; padding:6px;'>{(item.NgayTra?.ToString("yyyy-MM-dd") ?? "Chưa trả")}</td>
            </tr>";
			}

			html += "</table>";

			return Content(html, "text/html");
		}


		[HttpPost]
		public IActionResult Delete(int id)
		{
			try
			{
				var phieuMuon = _context.PhieuMuon
					.Include(p => p.ChiTietPhieuMuons)
					.FirstOrDefault(p => p.MaPhieuMuon == id);

				if (phieuMuon == null)
				{
					return NotFound();
				}


				foreach (var ct in phieuMuon.ChiTietPhieuMuons)
				{
					var sach = _context.Sach.Find(ct.MaSach);
					if (sach != null)
					{
						sach.SoLuong += 1;
					}

					_context.ChiTietPhieuMuon.Remove(ct);
				}


				_context.PhieuMuon.Remove(phieuMuon);
				_context.SaveChanges();

				return Ok();
			}
			catch (Exception ex)
			{
				return BadRequest("Lỗi khi xóa phiếu: " + ex.Message);
			}

		}
	}
}



