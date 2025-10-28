using Microsoft.AspNetCore.Mvc;
using YourProject.Data;
using YourProject.Helpers; 
using YourProject.Models;
using Microsoft.EntityFrameworkCore;
using YourProject.ViewModels;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
namespace BTL_NMCNPM_Nhom7.Controllers
{
    public class MuonSachController : Controller
    {
        private readonly LibraryContext _context;

        public MuonSachController(LibraryContext context)
        {
            _context = context;
        }

        // Trang danh sách sách để mượn
        public async Task<IActionResult> Index(string searchString)
        {
            ViewData["CurrentFilter"] = searchString;
            var sachQuery = _context.Sach.Include(s => s.TacGia).Include(s => s.TheLoai).AsQueryable();

            if (!String.IsNullOrEmpty(searchString))
            {
                var keyword = searchString.ToLower();
                sachQuery = sachQuery.Where(s =>
                    s.TenSach.ToLower().Contains(keyword) ||
                    (s.TacGia != null && s.TacGia.TenTacGia.ToLower().Contains(keyword)) ||
                    (s.TheLoai != null && s.TheLoai.TenTheLoai.ToLower().Contains(keyword))
                );
            }
            return View(await sachQuery.ToListAsync());
        }

        // Thêm sách vào giỏ
        [HttpPost]
        public IActionResult AddToCart(int maSach)
        {
            var sach = _context.Sach
                .Include(s => s.TacGia)
                .FirstOrDefault(s => s.MaSach == maSach);

            if (sach == null) return NotFound();

            if (sach.SoLuong <= 0)
            {
                TempData["ErrorMessage"] = $"Sách '{sach.TenSach}' đã hết hàng!";
                return RedirectToAction("Index");
            }

            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();
            var existingItem = cart.FirstOrDefault(item => item.MaSach == maSach);

            if (existingItem == null)
            {
                cart.Add(new CartItem
                {
                    MaSach = sach.MaSach,
                    TenSach = sach.TenSach,
                    TacGia = sach.TacGia?.TenTacGia,
                    SoLuong = 1
                });
                TempData["SuccessMessage"] = $"Đã thêm '{sach.TenSach}' vào giỏ mượn!";
            }
            else
            {
                existingItem.SoLuong++;
                TempData["InfoMessage"] = $"Đã cập nhật số lượng cho '{sach.TenSach}'.";
            }

            HttpContext.Session.SetObjectAsJson("Cart", cart);
            return RedirectToAction("Index");
        }

        // Xem giỏ mượn
        public IActionResult Cart()
        {
            var cartItems = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();
            return View(cartItems);
        }

        // Xác nhận mượn
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmBorrow()
        {
            // 1. Kiểm tra giỏ hàng
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart");
            var userIdString = User.FindFirstValue("UserId");

            if (cart == null || !cart.Any() || string.IsNullOrEmpty(userIdString))
            {
                TempData["ErrorMessage"] = "Giỏ mượn trống hoặc có lỗi xác thực người dùng!";
                return RedirectToAction("Index");
            }

            try
            {
                var phieuMuon = new PhieuMuon
                {
                    MaDocGia = int.Parse(userIdString),
                    NgayMuon = DateTime.Now,
                    NgayHenTra = DateTime.Now.AddDays(14),
                    TrangThai = "Chờ duyệt",
                    MaThuThu = 1
                };
                _context.PhieuMuon.Add(phieuMuon);
                await _context.SaveChangesAsync();

                foreach (var item in cart)
                {
                    var chiTiet = new ChiTietPhieuMuon { MaPhieuMuon = phieuMuon.MaPhieuMuon, MaSach = item.MaSach };
                    _context.ChiTietPhieuMuon.Add(chiTiet);
                    // LƯU Ý: KHÔNG giảm số lượng sách ở bước này. Chỉ giảm khi Thủ thư đã duyệt.
                }

                await _context.SaveChangesAsync();
                HttpContext.Session.Remove("Cart");
                TempData["SuccessMessage"] = "Yêu cầu mượn sách của bạn đã được gửi đi và đang chờ duyệt!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Đã xảy ra lỗi hệ thống khi xử lý yêu cầu.";
                Console.WriteLine(ex.ToString());
                return RedirectToAction("Index");
            }
        }
        public async Task<IActionResult> LichSuMuon()
        {
            // Lấy User ID từ Claims, an toàn và chính xác hơn Session
            var userIdString = User.FindFirstValue("UserId");

            if (string.IsNullOrEmpty(userIdString))
            {
                // Trường hợp này hiếm khi xảy ra vì đã có [Authorize]
                return Challenge(); // Yêu cầu đăng nhập lại
            }

            var maDocGia = int.Parse(userIdString);

            var lichSu = await _context.PhieuMuon
                .Where(p => p.MaDocGia == maDocGia)
                .Include(p => p.ChiTietPhieuMuons)
                    .ThenInclude(ct => ct.Sach)
                .OrderByDescending(p => p.NgayMuon)
                .ToListAsync();

            return View(lichSu);
        }
        [HttpPost]
            public async Task<IActionResult> YeuCauTraSach(int maPhieuMuon)
            {
                var phieuMuon = await _context.PhieuMuon.FindAsync(maPhieuMuon);
                if (phieuMuon != null)
                {
                    phieuMuon.TrangThai = "Chờ trả sách"; // Cập nhật trạng thái
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Đã gửi yêu cầu trả sách. Vui lòng mang sách đến thư viện để thủ thư xác nhận.";
                }
                return RedirectToAction("LichSuMuon");
            }
    }
}
    