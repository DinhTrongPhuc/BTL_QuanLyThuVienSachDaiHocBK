using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using YourProject.Data;

namespace BTL_NMCNPM_Nhom7.Controllers
{
    public class BaoCaoTongHopController : Controller
    {
        private readonly LibraryContext _context;

        public BaoCaoTongHopController(LibraryContext context)
        {
            _context = context;
        }

        // Trang chính tổng hợp báo cáo
        public IActionResult Index()
        {
            return View();
        }

        // 1️⃣ Báo cáo Sách được mượn nhiều nhất
        public async Task<IActionResult> SachMuonNhieu()
        {
            var data = await _context.ChiTietPhieuMuon
                .Include(c => c.Sach)
                .GroupBy(c => c.MaSach)
                .Select(g => new
                {
                    TenSach = g.First().Sach.TenSach,
                    SoLanMuon = g.Count()
                })
                .OrderByDescending(x => x.SoLanMuon)
                .Take(10)
                .ToListAsync();

            return View(data);
        }
    

        // 2️⃣ Báo cáo Sách quá hạn
        public async Task<IActionResult> SachQuaHan()
        {
            var today = DateTime.Now;
            var data = await _context.PhieuMuon
                .Include(p => p.DocGia)
                .Include(p => p.ChiTietPhieuMuons)
                .ThenInclude(ct => ct.Sach)
                .Where(p => p.NgayTra < today && !p.DaTra)
                .SelectMany(p => p.ChiTietPhieuMuons.Select(ct => new
                {
                    MaPhieu = p.MaPhieuMuon,
                    TenSach = ct.Sach.TenSach,
                    DocGia = p.DocGia.HoTen,
                    NgayMuon = p.NgayMuon,
                    HanTra = p.NgayTra
                }))
                .ToListAsync();

            return View(data);
        }

        // 3️⃣ Báo cáo Độc giả quá hạn
        public async Task<IActionResult> DocGiaQuaHan()
        {
            var today = DateTime.Now;
            var data = await _context.PhieuMuon
                .Include(p => p.DocGia)
                .Where(p => p.NgayTra < today && !p.DaTra)
                .GroupBy(p => p.DocGia.HoTen)
                .Select(g => new
                {
                    DocGia = g.Key,
                    SoSachQuaHan = g.Count()
                })
                .OrderByDescending(x => x.SoSachQuaHan)
                .ToListAsync();

            return View(data);
        }

        // 4️⃣ Báo cáo Sách tồn kho
        public async Task<IActionResult> SachTonKho()
        {
            var data = await _context.Sach
                .Select(s => new
                {
                    TenSach = s.TenSach,
                    SoLuongCon = s.SoLuong
                })
                .OrderByDescending(s => s.SoLuongCon)
                .ToListAsync();

            return View(data);
        }

        // 🧾 Xuất Excel (áp dụng chung)
        public async Task<FileResult> ExportExcel(string type)
        {
            var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("BaoCao");

            if (type == "SachMuonNhieu")
            {
                var data = await _context.ChiTietPhieuMuon
                    .Include(c => c.Sach)
                    .GroupBy(c => c.MaSach)
                    .Select(g => new
                    {
                        TenSach = g.First().Sach.TenSach,
                        SoLanMuon = g.Count()
                    })
                    .OrderByDescending(x => x.SoLanMuon)
                    .ToListAsync();

                ws.Cell(1, 1).Value = "Tên sách";
                ws.Cell(1, 2).Value = "Số lần mượn";
                ws.Cell(2, 1).InsertData(data);
            }

            using (var stream = new MemoryStream())
            {
                workbook.SaveAs(stream);
                var content = stream.ToArray();
                return File(content,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    $"{type}_BaoCao.xlsx");
            }
        }
    }
}
