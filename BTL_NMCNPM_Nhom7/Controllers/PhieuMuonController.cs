using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YourProject.Models;   
using YourProject.Data;

namespace YourProject.Controllers
{
	public class PhieuMuonController : Controller
	{
		private readonly LibraryContext _context;


		public PhieuMuonController(LibraryContext context)
		{
			_context = context;
		}

	
		public IActionResult Index()
		{
		
			return RedirectToAction("Create");
		
		}

	
		public IActionResult Create()
		{
			ViewBag.DocGias = _context.DocGias.ToList();
			ViewBag.ThuThus = _context.ThuThus.ToList();
			ViewBag.Sachs = _context.Sachs.Where(s => s.SoLuong > 0).ToList();

			ViewBag.Message = TempData["Message"];
			ViewBag.Error = TempData["Error"];

			return View();
		}
	


	
	[HttpPost]
	public IActionResult Create(int MaDocGia, int MaThuThu, DateTime NgayHenTra, string TrangThai, string MaSachsInput)
	{
		try
		{
			var phieuMuon = new PhieuMuon
			{
				MaDocGia = MaDocGia,
				MaThuThu = MaThuThu,
				NgayMuon = DateTime.Now,
				NgayHenTra = NgayHenTra,
				TrangThai = TrangThai
			};

			_context.PhieuMuons.Add(phieuMuon);
			_context.SaveChanges();


				var maSachs = MaSachsInput
					.Split(',')
					.Select(s => s.Trim())
					.Where(s => int.TryParse(s, out _))
					.Select(int.Parse)
					.ToList();

				foreach (var maSach in maSachs)
				{
					var sach = _context.Sachs.Find(maSach);
					if (sach != null && sach.SoLuong > 0)
					{
						sach.SoLuong -= 1;
						_context.ChiTietPhieuMuons.Add(new ChiTietPhieuMuon
						{
							MaPhieuMuon = phieuMuon.MaPhieuMuon, 
							MaSach = maSach,
							Sach=sach
						});
					}
				}


				_context.SaveChanges();


			TempData["Message"] = "Tạo phiếu mượn thành công!";
			return RedirectToAction("DanhSach");
		}
		catch (Exception ex)
		{
			TempData["Error"] = "Lỗi khi tạo phiếu: " + ex.Message;
			return RedirectToAction("Create");
		}
	}


	public IActionResult DanhSach()
	{
		var danhSach = _context.PhieuMuons
			.Include(p => p.DocGia)
			.Include(p => p.ThuThu)
			.Include(p => p.ChiTietPhieuMuons)
			.ThenInclude(c => c.Sach)
			.ToList();

		return View("Index", danhSach);
	}



		[HttpGet]
		public IActionResult ChiTiet(int id)
		{
			var phieu = _context.PhieuMuons
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
				var phieuMuon = _context.PhieuMuons
					.Include(p => p.ChiTietPhieuMuons)
					.FirstOrDefault(p => p.MaPhieuMuon == id);

				if (phieuMuon == null)
				{
					return NotFound();
				}

				
				foreach (var ct in phieuMuon.ChiTietPhieuMuons)
				{
					var sach = _context.Sachs.Find(ct.MaSach);
					if (sach != null)
					{
						sach.SoLuong += 1; 
					}

					_context.ChiTietPhieuMuons.Remove(ct);
				}

			
				_context.PhieuMuons.Remove(phieuMuon);
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
