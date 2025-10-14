using Microsoft.AspNetCore.Mvc;
using YourProject.Data;
using Microsoft.EntityFrameworkCore;
using YourProject.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace BTL_NMCNPM_Nhom7.Controllers
{
    public class AccountController : Controller
    {
        private readonly LibraryContext _context;

        public AccountController(LibraryContext context)
        {
            _context = context;
        }

        // --- ĐĂNG KÝ ---
        // GET: /Account/Register
         [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(DocGia docGia)
        {
            if (ModelState.IsValid)
            {
                bool tenDangNhapDaTonTai = await _context.DocGia.AnyAsync(u => u.TenDangNhap == docGia.TenDangNhap);
                if (tenDangNhapDaTonTai)
                {
                    // Thêm lỗi vào ModelState và trả về View
                    ModelState.AddModelError("TenDangNhap", "Tên đăng nhập này đã được sử dụng.");
                }

                bool soDienThoaiDaTonTai = await _context.DocGia.AnyAsync(u => u.SoDienThoai == docGia.SoDienThoai);
                if (soDienThoaiDaTonTai)
                {
                    ModelState.AddModelError("SoDienThoai", "Số điện thoại này đã được đăng ký.");
                }


                if (!ModelState.IsValid)
                {
                    return View(docGia);
                }

                docGia.NgayLapThe = DateTime.Now;
                _context.Add(docGia);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Đăng ký thành công! Vui lòng đăng nhập.";
                return RedirectToAction("Login");
            }

            return View(docGia);
        }
        // --- ĐĂNG NHẬP ---
        // GET: /Account/Login
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }
        
       // POST: /Account/Login
        [HttpPost]
[AllowAnonymous]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Login(string tenDangNhap, string matKhau)
{
    if (string.IsNullOrEmpty(tenDangNhap) || string.IsNullOrEmpty(matKhau))
    {
        ViewBag.Error = "Vui lòng nhập đầy đủ thông tin.";
        return View();
    }

    // Kiểm tra Thủ thư (Admin)
    var admin = await _context.ThuThu.FirstOrDefaultAsync(u => u.TenDangNhap == tenDangNhap && u.MatKhau == matKhau);
    if (admin != null)
    {
        // Tạo các "claim" để lưu thông tin người dùng
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, admin.HoTen),
            new Claim("UserId", admin.MaThuThu.ToString()),
            new Claim(ClaimTypes.Role, "Admin") // Gán vai trò là Admin
        };

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

        // Đăng nhập người dùng bằng cookie
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
        
        return RedirectToAction("Index", "Home"); 
    }

    // Kiểm tra Độc giả
    var docGia = await _context.DocGia.FirstOrDefaultAsync(u => u.TenDangNhap == tenDangNhap && u.MatKhau == matKhau);
    if (docGia != null)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, docGia.HoTen),
            new Claim("UserId", docGia.Id.ToString()),
            new Claim(ClaimTypes.Role, "DocGia") // Gán vai trò là Độc giả
        };
        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

        return RedirectToAction("Index", "MuonSach"); 
    }
    
    ViewBag.Error = "Tên đăng nhập hoặc mật khẩu không đúng.";
    return View();
}

public async Task<IActionResult> Logout()
{
    // Đăng xuất người dùng
    await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return RedirectToAction("Login", "Account");
}
    }
}