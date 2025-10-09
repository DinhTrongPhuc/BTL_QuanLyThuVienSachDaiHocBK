using System.ComponentModel.DataAnnotations.Schema;

namespace YourProject.Models
{
    public class ChiTietPhieuMuon
    {
        public int MaPhieuMuon { get; set; }
        public int MaSach { get; set; }
        
        public DateTime? NgayTra { get; set; }
        public decimal TienPhat { get; set; }
        public string? GhiChu { get; set; }

        // Navigation properties
        [ForeignKey("MaPhieuMuon")]
        public PhieuMuon? PhieuMuon { get; set; }

        [ForeignKey("MaSach")]
        public Sach? Sach { get; set; }
    }
}