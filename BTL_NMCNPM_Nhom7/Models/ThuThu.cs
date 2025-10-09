using System.ComponentModel.DataAnnotations;

namespace YourProject.Models
{
    public class ThuThu
    {
        [Key]
        public int MaThuThu { get; set; }

        [Required]
        [StringLength(100)]
        public string HoTen { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string TenDangNhap { get; set; } = string.Empty;

        [Required]
        public string MatKhau { get; set; } = string.Empty;
        
        [StringLength(50)]
        public string? QuyenHan { get; set; }
    }
}