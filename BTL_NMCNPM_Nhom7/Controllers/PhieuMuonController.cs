using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YourProject.Data;
using YourProject.Helpers;
using YourProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace YourProject.Controllers
{
    public class PhieuMuonController : Controller
    {
        private readonly LibraryContext _context;

        public PhieuMuonController(LibraryContext context)
        {
            _context = context;
        }

        // GET: PhieuMuon (Hiển thị danh sách phiếu mượn)
        public async Task<IActionResult> Index()
        {
           var phieuMuons = await _context.PhieuMuon
                                     .Include(p => p.DocGia) 
                                     .ToListAsync();
           return View(phieuMuons);
}
       // GET: PhieuMuon/Details/5
public async Task<IActionResult> Details(int? id)
{
    if (id == null)
    {
        return NotFound();
    }

    var phieuMuon = await _context.PhieuMuon
        .Include(p => p.DocGia) // Nối đến bảng Độc Giả để lấy tên
        .Include(p => p.ThuThu) // Nối đến bảng Thủ Thư để lấy tên
        .Include(p => p.ChiTietPhieuMuons)
            .ThenInclude(ct => ct.Sach) // Từ chi tiết, nối tiếp đến bảng Sách để lấy tên sách
        .FirstOrDefaultAsync(m => m.MaPhieuMuon == id);

    if (phieuMuon == null)
    {
        return NotFound();
    }

    return View(phieuMuon);
}
        // GET: PhieuMuon/Create (Hiển thị form để tạo phiếu mới)
        public IActionResult Create()
        {
            // Chuẩn bị danh sách cho các dropdown
            ViewData["MaDocGia"] = new SelectList(_context.DocGia, "Id", "HoTen");
            ViewData["SachList"] = new SelectList(_context.Sach.Where(s => s.SoLuong > 0), "MaSach", "TenSach");
            return View();
        }

		// POST: PhieuMuon/Create (Xử lý việc tạo phiếu mượn)
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create([Bind("MaDocGia")] PhieuMuon phieuMuon, List<int> MaSachs)
		{
			if (MaSachs == null || !MaSachs.Any())
			{
				ModelState.AddModelError("", "Vui lòng chọn ít nhất một cuốn sách để mượn.");
			}

			if (ModelState.IsValid)
			{
				// Gán các giá trị mặc định cho phiếu mượn
				phieuMuon.NgayMuon = DateTime.Now;
				phieuMuon.NgayHenTra = DateTime.Now.AddDays(14); // Hẹn trả sau 14 ngày
				phieuMuon.TrangThai = "Đang mượn";
				phieuMuon.MaThuThu = 1; // Giả sử Thủ thư/Admin có ID=1 là người xử lý

				_context.Add(phieuMuon);
				await _context.SaveChangesAsync(); // Lưu để lấy được ID của phiếu mượn

				// Thêm các sách đã chọn vào chi tiết phiếu mượn và cập nhật số lượng
				foreach (var maSach in MaSachs)
				{
					var sach = await _context.Sach.FindAsync(maSach);
					if (sach != null && sach.SoLuong > 0)
					{
						var chiTiet = new ChiTietPhieuMuon
						{
							MaPhieuMuon = phieuMuon.MaPhieuMuon,
							MaSach = maSach
						};
						_context.ChiTietPhieuMuon.Add(chiTiet);
						sach.SoLuong--; // Giảm số lượng sách trong kho
					}
				}

				await _context.SaveChangesAsync();
				TempData["SuccessMessage"] = "Tạo phiếu mượn thành công!";
				return RedirectToAction(nameof(Index));
			}

			// Nếu có lỗi, tải lại danh sách cho dropdown và hiển thị lại form
			ViewData["Id"] = new SelectList(_context.DocGia, "MaDocGia", "HoTen", phieuMuon.MaDocGia);
			ViewData["SachList"] = new SelectList(_context.Sach.Where(s => s.SoLuong > 0), "MaSach", "TenSach");
			return View(phieuMuon);
		}
	// POST: PhieuMuon/Duyet/5
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Duyet(int id)
{
    var phieuMuon = await _context.PhieuMuon
                                  .Include(p => p.ChiTietPhieuMuons) // Lấy kèm chi tiết để biết mượn sách gì
                                  .FirstOrDefaultAsync(p => p.MaPhieuMuon == id);
    if (phieuMuon == null)
    {
        return NotFound();
    }

    // Kiểm tra số lượng sách trước khi duyệt
    foreach (var chiTiet in phieuMuon.ChiTietPhieuMuons)
    {
        var sach = await _context.Sach.FindAsync(chiTiet.MaSach);
        if (sach == null || sach.SoLuong <= 0)
        {
            TempData["ErrorMessage"] = $"Không thể duyệt: Sách có mã {chiTiet.MaSach} đã hết hàng.";
            return RedirectToAction(nameof(Index));
        }
    }

    // Nếu tất cả sách đều còn, tiến hành duyệt
    phieuMuon.TrangThai = "Đang mượn";

    // Trừ số lượng sách trong kho
    foreach (var chiTiet in phieuMuon.ChiTietPhieuMuons)
    {
        var sach = await _context.Sach.FindAsync(chiTiet.MaSach);
        if (sach != null)
        {
            sach.SoLuong--; // Giảm số lượng đi 1
        }
    }

    await _context.SaveChangesAsync();
    TempData["SuccessMessage"] = "Đã duyệt phiếu mượn thành công!";
    return RedirectToAction(nameof(Index));
}

		// POST: PhieuMuon/TuChoi/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> TuChoi(int id)
		{
			var phieuMuon = await _context.PhieuMuon.FindAsync(id);
			if (phieuMuon == null)
			{
				return NotFound();
			}

			// Cập nhật trạng thái phiếu mượn
			phieuMuon.TrangThai = "Đã từ chối";

			await _context.SaveChangesAsync();
			TempData["InfoMessage"] = "Đã từ chối phiếu mượn.";
			return RedirectToAction(nameof(Index));
		}
		
		// POST: PhieuMuon/XacNhanTra/5
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> XacNhanTra(int id)
{
    var phieuMuon = await _context.PhieuMuon
                                  .Include(p => p.ChiTietPhieuMuons)
                                  .FirstOrDefaultAsync(p => p.MaPhieuMuon == id);
    if (phieuMuon == null)
    {
        return NotFound();
    }

    phieuMuon.TrangThai = "Đã trả";

    // Cập nhật ngày trả và tăng lại số lượng sách
    foreach (var chiTiet in phieuMuon.ChiTietPhieuMuons)
    {
        chiTiet.NgayTra = DateTime.Now; // Ghi nhận ngày trả

        // Tính phạt nếu có
        if (chiTiet.NgayTra > phieuMuon.NgayHenTra)
        {
            chiTiet.TienPhat = (decimal)(chiTiet.NgayTra.Value - phieuMuon.NgayHenTra).TotalDays * 5000;
        }

        // Tăng lại số lượng sách trong kho
        var sach = await _context.Sach.FindAsync(chiTiet.MaSach);
        if (sach != null)
        {
            sach.SoLuong++; // Tăng số lượng lên 1
        }
    }

    await _context.SaveChangesAsync();
    TempData["SuccessMessage"] = "Đã xác nhận trả sách thành công!";
    return RedirectToAction(nameof(Index));
}		
// POST: PhieuMuon/TuChoiTra/5
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> TuChoiTra(int id)
{
    var phieuMuon = await _context.PhieuMuon.FindAsync(id);
    if (phieuMuon == null)
    {
        return NotFound();
    }

    // Cập nhật trạng thái phiếu mượn trở lại "Đang mượn"
    phieuMuon.TrangThai = "Đang mượn";

    await _context.SaveChangesAsync();
    TempData["InfoMessage"] = "Đã từ chối yêu cầu trả sách.";
    return RedirectToAction(nameof(Index));
}
    }
}