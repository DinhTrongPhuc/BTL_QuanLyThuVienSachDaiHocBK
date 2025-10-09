using System.ComponentModel.DataAnnotations;

namespace YourProject.Models
{
    public class DocGia
    {
     [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string HoTen { get; set; } = string.Empty;

        public DateTime? NgaySinh { get; set; }

        [StringLength(255)]
        public string? DiaChi { get; set; }

        [StringLength(15)]
        public string? SoDienThoai { get; set; }

        [StringLength(100)]
        public string? Email { get; set; }

        [Required]
        [StringLength(50)]
        public string TenDangNhap { get; set; } = string.Empty;
        
        [Required]
        public string MatKhau { get; set; } = string.Empty;
        
        public DateTime NgayLapThe { get; set; }

        // Navigation property
        public ICollection<PhieuMuon>? PhieuMuons { get; set; }
    }
}
