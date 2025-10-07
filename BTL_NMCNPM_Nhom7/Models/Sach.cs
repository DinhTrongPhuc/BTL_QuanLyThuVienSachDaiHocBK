using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BTL_NMCNPM_Nhom7.Models
{
    public class Sach
    {
        [Key]
        public int MaSach { get; set; }

        [Required]
        [StringLength(255)]
        public string TenSach { get; set; } = string.Empty;

        public int NamXuatBan { get; set; }
        public int SoLuong { get; set; }
        public string? MoTa { get; set; }

        // Khóa ngoại
        public int MaTheLoai { get; set; }
        public int MaTacGia { get; set; }

        // Navigation properties
        [ForeignKey("MaTacGia")]
        public TacGia? TacGia { get; set; }

        [ForeignKey("MaTheLoai")]
        public TheLoai? TheLoai { get; set; }
    }
}