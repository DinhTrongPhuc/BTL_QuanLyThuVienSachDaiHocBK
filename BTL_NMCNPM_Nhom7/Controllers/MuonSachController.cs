using Microsoft.AspNetCore.Mvc;
using YourProject.Data;
using YourProject.Helpers; 
using YourProject.Models;
using Microsoft.EntityFrameworkCore;


public class MuonSachController : Controller
{
    private readonly LibraryContext _context;

    public MuonSachController(LibraryContext context)
    {
        _context = context;
    }

    // Trang danh sách sách để mượn
    public IActionResult Index()
    {
        var sachList = _context.Sach
            .Include(s => s.TacGia)
            .Include(s => s.TheLoai)
            .ToList();

        return View(sachList);
    }

    // Thêm sách vào giỏ
    [HttpPost]
    public IActionResult AddToCart(int maSach)
    {
        var sach = _context.Sach.FirstOrDefault(s => s.MaSach == maSach);
        if (sach == null)
            return NotFound();

        var cart = HttpContext.Session.GetObjectFromJson<List<Sach>>("Cart") ?? new List<Sach>();
        cart.Add(sach);
        HttpContext.Session.SetObjectAsJson("Cart", cart);

        TempData["SuccessMessage"] = $"Đã thêm '{sach.TenSach}' vào giỏ mượn!";
        return RedirectToAction("Index");
    }

    // Xem giỏ mượn
    public IActionResult Cart()
    {
        var cart = HttpContext.Session.GetObjectFromJson<List<Sach>>("Cart") ?? new List<Sach>();
        return View(cart);
    }

    // Xác nhận mượn
    [HttpPost]
    public IActionResult ConfirmBorrow()
    {
        var cart = HttpContext.Session.GetObjectFromJson<List<Sach>>("Cart");
        if (cart == null || !cart.Any())
        {
            TempData["ErrorMessage"] = "Giỏ mượn trống!";
            return RedirectToAction("Index");
        }

        HttpContext.Session.Remove("Cart");
        TempData["SuccessMessage"] = "Mượn sách thành công!";
        return RedirectToAction("Index");
    }
}
