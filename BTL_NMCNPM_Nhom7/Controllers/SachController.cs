using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using YourProject.Data;
using YourProject.Models;

namespace YourProject.Controllers
{
    public class SachController : Controller
    {
        private readonly LibraryContext _context;

        public SachController(LibraryContext context)
        {
            _context = context;
        }

        // GET: Sach
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Sach.Include(s => s.TacGia).Include(s => s.TheLoai);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Sach/Create
        public IActionResult Create()
        {
            ViewData["MaTacGia"] = new SelectList(_context.TacGia, "MaTacGia", "TenTacGia");
            ViewData["MaTheLoai"] = new SelectList(_context.TheLoai, "MaTheLoai", "TenTheLoai");
            return View();
        }
        // POST: Sach/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaSach,TenSach,NamXuatBan,SoLuong,MoTa,MaTheLoai,MaTacGia")] Sach sach)
        {
            if (ModelState.IsValid)
            {
                _context.Add(sach);
                await _context.SaveChangesAsync();
                
                TempData["AlertMessage"] = "Thêm sách thành công!";
                
                return RedirectToAction(nameof(Index));
            }
    TempData["AlertMessage"] = "Thêm sách thất bại, vui lòng kiểm tra lại thông tin.";
    
    ViewData["MaTacGia"] = new SelectList(_context.TacGia, "MaTacGia", "TenTacGia", sach.MaTacGia);
    ViewData["MaTheLoai"] = new SelectList(_context.TheLoai, "MaTheLoai", "TenTheLoai", sach.MaTheLoai);
    return View(sach);
}
        // GET: Sach/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sach = await _context.Sach.FindAsync(id);
            if (sach == null)
            {
                return NotFound();
            }
            ViewData["MaTacGia"] = new SelectList(_context.TacGia, "MaTacGia", "TenTacGia", sach.MaTacGia);
            ViewData["MaTheLoai"] = new SelectList(_context.TheLoai, "MaTheLoai", "TenTheLoai", sach.MaTheLoai);
            return View(sach);
        }

        // POST: Sach/Edit/5
        // POST: Sach/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaSach,TenSach,NamXuatBan,SoLuong,MoTa,MaTheLoai,MaTacGia")] Sach sach)
{
    if (id != sach.MaSach) return NotFound();

    if (ModelState.IsValid)
    {
        try
        {
            _context.Update(sach);
            await _context.SaveChangesAsync();
            // Trả về JSON báo thành công
            return Json(new { success = true });
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!SachExists(sach.MaSach))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }
    }
    
    // Nếu thất bại, trả về lại form với lỗi validation
    ViewData["MaTacGia"] = new SelectList(_context.TacGia, "MaTacGia", "TenTacGia", sach.MaTacGia);
    ViewData["MaTheLoai"] = new SelectList(_context.TheLoai, "MaTheLoai", "TenTheLoai", sach.MaTheLoai);
    return PartialView("Edit", sach);
}

        // GET: Sach/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sach = await _context.Sach
                .Include(s => s.TacGia)
                .Include(s => s.TheLoai)
                .FirstOrDefaultAsync(m => m.MaSach == id);
            if (sach == null)
            {
                return NotFound();
            }

            return View(sach);
        }

        // POST: Sach/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sach = await _context.Sach.FindAsync(id);
            if (sach != null)
            {
                _context.Sach.Remove(sach);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SachExists(int id)
        {
            return _context.Sach.Any(e => e.MaSach == id);
        }
    }
}