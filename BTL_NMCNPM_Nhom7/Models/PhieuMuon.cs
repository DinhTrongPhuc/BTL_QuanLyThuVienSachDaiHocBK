using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YourProject.Models
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
        public object Sach { get; internal set; }
        public object NgayTra { get; internal set; }
        public DateTime HanTra { get; internal set; }
    }
}