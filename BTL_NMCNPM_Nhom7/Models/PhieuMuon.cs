using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BTL_NMCNPM_Nhom7.Models
{
    public class PhieuMuon
    {
        [Key]
        public int MaPhieuMuon { get; set; }

        public DateTime NgayMuon { get; set; }
        public DateTime NgayHenTra { get; set; }
        
        [StringLength(50)]
        public string TrangThai { get; set; } = string.Empty;

        // Khóa ngoại
        public int MaDocGia { get; set; }
        public int MaThuThu { get; set; }

        // Navigation properties
        [ForeignKey("MaDocGia")]
        public DocGia? DocGia { get; set; }
        
        [ForeignKey("MaThuThu")]
        public ThuThu? ThuThu { get; set; }

        public ICollection<ChiTietPhieuMuon>? ChiTietPhieuMuons { get; set; }
    }
}