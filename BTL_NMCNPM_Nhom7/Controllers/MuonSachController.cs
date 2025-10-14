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

   // Trang danh sách sách để mượn (đã sửa)
public async Task<IActionResult> Index(string searchString)
{
        // Lưu lại từ khóa tìm kiếm để hiển thị lại trên ô input
    ViewData["CurrentFilter"] = searchString;

    // Bắt đầu câu truy vấn, lấy sách và bao gồm cả Tác giả, Thể loại
    var sachQuery = _context.Sach.Include(s => s.TacGia).Include(s => s.TheLoai).AsQueryable();

    // Nếu có từ khóa tìm kiếm, thực hiện lọc
    if (!String.IsNullOrEmpty(searchString))
    {
        // Chuyển từ khóa về chữ thường để tìm kiếm không phân biệt hoa/thường
        var keyword = searchString.ToLower();
        
        sachQuery = sachQuery.Where(s => 
            s.TenSach.ToLower().Contains(keyword) || 
            s.TacGia.TenTacGia.ToLower().Contains(keyword) || 
            s.TheLoai.TenTheLoai.ToLower().Contains(keyword)
        );
    }

    // Thực thi câu lệnh truy vấn và lấy danh sách kết quả
    var sachList = await sachQuery.ToListAsync();

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
