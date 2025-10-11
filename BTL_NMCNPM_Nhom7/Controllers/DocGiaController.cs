using Microsoft.AspNetCore.Mvc;
using YourProject.Data;
using YourProject.Models;
using Microsoft.EntityFrameworkCore;

namespace YourProject.Controllers
{
    public class DocGiaController : Controller
    {
        private readonly LibraryContext _context;

        public DocGiaController(LibraryContext context)
        {
            _context = context;
        }

        //search
        public IActionResult MainHome_search(string searchString)
        {
            var query = _context.DocGia.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(d => d.HoTen.Contains(searchString) || d.Email.Contains(searchString));
            }

            ViewData["SearchString"] = searchString;
            var result = query.ToList();
            return View(result);
        }


        // GET: /DocGia/MainHome
        public async Task<IActionResult> MainHome()
        {
            var danhSachDocGia = await _context.DocGia.ToListAsync();
            return View(danhSachDocGia);
        }

        // GET: /DocGia/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /DocGia/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DocGia docGia)
        {
            if (ModelState.IsValid)
            {
                _context.DocGia.Add(docGia);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Độc giả đã được thêm thành công!";
                return RedirectToAction("MainHome");
            }
            return View(docGia);
        }

        // GET: /DocGia/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var docGia = await _context.DocGia.FindAsync(id);
            if (docGia == null) return NotFound();
            return View(docGia);
        }

        // POST: /DocGia/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DocGia docGia)
        {
            if (id != docGia.Id) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(docGia);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Độc giả đã được cập nhật thành công!";
                return RedirectToAction("MainHome");
            }
            return View(docGia);
        }

        // GET: /DocGia/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var docGia = await _context.DocGia.FindAsync(id);
            if (docGia != null)
            {
                _context.DocGia.Remove(docGia);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Độc giả đã được xóa thành công!";
            }
            return RedirectToAction("MainHome");
        }
    }
}
