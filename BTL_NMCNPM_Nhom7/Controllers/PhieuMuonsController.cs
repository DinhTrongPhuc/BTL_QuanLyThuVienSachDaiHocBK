using YourProject.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YourProject.Data;
using YourProject.Helpers;
using YourProject.Models;

public class PhieuMuonsController : Controller
{
	private readonly LibraryContext _context;

	public PhieuMuonsController(LibraryContext context)
	{
		_context = context;
	}

	// Trang danh sách yêu cầu mượn sách
	public IActionResult Index1()
	{
		var readersWithCart = new List<ReaderCartViewModel>();

		var docGiaList = _context.DocGia.ToList();

		foreach (var docGia in docGiaList)
		{
			var cart = HttpContext.Session.GetObjectFromJson<List<Sach>>($"Cart_{docGia.Id}");
			if (cart != null && cart.Any())
			{
				readersWithCart.Add(new ReaderCartViewModel
				{
					MaDocGia = docGia.Id,
					TenDocGia = docGia.HoTen,
					SachMuon = cart
				});
			}
		}

		return View(readersWithCart);
	}

	
}
