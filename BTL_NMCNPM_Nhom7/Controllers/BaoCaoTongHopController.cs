using YourProject.Data;
using ClosedXML.Excel;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;
using YourProject.Data;
using YourProject.Models;

namespace BTL_NMCNPM_Nhom7.Controllers.BaoCao
{
    public class BaoCaoTongHopController : Controller
    {
        private readonly LibraryContext _context;

        public BaoCaoTongHopController(LibraryContext context)
        {
            _context = context;
        }

        // 📘 1️⃣ Sách được mượn nhiều nhất
        public async Task<IActionResult> SachMuonNhieu()
        {
            var data = await _context.PhieuMuon
                .Include(p => p.Sach)
                .GroupBy(p => p.Sach)
                .Select(g => new
                {
                    Sach = g.Key,
                    SoLuotMuon = g.Count()
                })
                .OrderByDescending(x => x.SoLuotMuon)
                .Take(10)
                .ToListAsync();

            return View(data);
        }

        // ⏰ 2️⃣ Sách quá hạn
        public async Task<IActionResult> SachQuaHan()
        {
            var today = DateTime.Now;
            var data = await _context.PhieuMuon
                .Include(p => p.Sach)
                .Include(p => p.DocGia)
                .Where(p => p.NgayTra == null && p.HanTra < today)
                .ToListAsync();

            return View(data);
        }

        // 👤 3️⃣ Độc giả có sách quá hạn
        public async Task<IActionResult> DocGiaQuaHan()
        {
            var today = DateTime.Now;
            var data = await _context.PhieuMuon
                .Include(p => p.DocGia)
                .Where(p => p.NgayTra == null && p.HanTra < today)
                .GroupBy(p => p.DocGia)
                .Select(g => new
                {
                    DocGia = g.Key,
                    SoSachQuaHan = g.Count()
                })
                .OrderByDescending(x => x.SoSachQuaHan)
                .ToListAsync();

            return View(data);
        }

        // 🏷️ 4️⃣ Thống kê tồn kho
        public async Task<IActionResult> TonKho()
        {
            int tong = await _context.Sach.CountAsync();
            int dangMuon = await _context.PhieuMuon
                .CountAsync(p => p.NgayTra == null);
            int conLai = tong - dangMuon;

            ViewBag.TongSoSach = tong;
            ViewBag.DangMuon = dangMuon;
            ViewBag.ConLai = conLai;
            return View();
        }

        // 📤 Xuất Excel (ví dụ: sách mượn nhiều nhất)
        public async Task<IActionResult> ExportSachMuonNhieuToExcel()
        {
            var data = await _context.PhieuMuon
                .Include(p => p.Sach)
                .GroupBy(p => (Sach)p.Sach)
                .Select(g => new
                {
                    MaSach = ((Sach)g.Key).MaSach,
                    TenSach = ((Sach)g.Key).TenSach,
                    SoLuotMuon = g.Count()
                })
                .OrderByDescending(x => x.SoLuotMuon)
                .ToListAsync();

            using (var wb = new XLWorkbook())
            {
                var ws = wb.Worksheets.Add("SachMuonNhieu");
                ws.Cell(1, 1).Value = "Mã Sách";
                ws.Cell(1, 2).Value = "Tên Sách";
                ws.Cell(1, 3).Value = "Số Lượt Mượn";
                ws.Row(1).Style.Font.Bold = true;

                int row = 2;
                foreach (var item in data)
                {
                    ws.Cell(row, 1).Value = item.MaSach;
                    ws.Cell(row, 2).Value = item.TenSach;
                    ws.Cell(row, 3).Value = item.SoLuotMuon;
                    row++;
                }

                ws.Columns().AdjustToContents();
                using (var stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "BaoCao_SachMuonNhieu.xlsx");
                }
            }
        }

        // 📄 Xuất PDF (ví dụ: sách mượn nhiều nhất)
        public async Task<IActionResult> ExportSachMuonNhieuToPdf()
        {
            var data = await _context.PhieuMuon
                .Include(p => p.Sach)
                .GroupBy(p => (Sach)p.Sach)
                .Select(g => new
                {
                    MaSach = ((Sach)g.Key).MaSach,
                    TenSach = ((Sach)g.Key).TenSach,
                    SoLuotMuon = g.Count()
                })
                .OrderByDescending(x => x.SoLuotMuon)
                .ToListAsync();

            using (var stream = new MemoryStream())
            {
                var doc = new Document(PageSize.A4);
                PdfWriter.GetInstance(doc, stream);
                doc.Open();

                var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16);
                doc.Add(new Paragraph("BÁO CÁO SÁCH MƯỢN NHIỀU NHẤT", titleFont));
                doc.Add(new Paragraph($"Ngày tạo: {DateTime.Now:dd/MM/yyyy}\n\n"));

                PdfPTable table = new PdfPTable(3);
                table.AddCell("Mã Sách");
                table.AddCell("Tên Sách");
                table.AddCell("Số Lượt Mượn");

                foreach (var s in data)
                {
                    table.AddCell(s.MaSach.ToString());
                    table.AddCell(s.TenSach);
                    table.AddCell(s.SoLuotMuon.ToString());
                }

                doc.Add(table);
                doc.Close();

                return File(stream.ToArray(), "application/pdf", "BaoCao_SachMuonNhieu.pdf");
            }
        }

        // Trang chọn loại báo cáo
        public IActionResult Index() => View();
    }
}
