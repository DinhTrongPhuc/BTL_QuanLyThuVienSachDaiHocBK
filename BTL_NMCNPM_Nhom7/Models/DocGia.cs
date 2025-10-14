using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YourProject.Models 
{
    [Table("DocGia")] 
    public class DocGia
    {
        [Key]
        public int Id { get; set; } 

        [Required(ErrorMessage = "Vui lòng nhập họ và tên.")]
        [StringLength(100)]
        public string HoTen { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng chọn ngày sinh.")]
        public DateTime? NgaySinh { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ.")]
        [StringLength(255)]
        public string DiaChi { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại.")]
        [RegularExpression(@"^0\d{9}$", ErrorMessage = "Số điện thoại phải bắt đầu bằng số 0 và có đúng 10 chữ số.")]
        public string SoDienThoai { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập email.")]
        [EmailAddress(ErrorMessage = "Định dạng email không hợp lệ.")]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập tên đăng nhập.")]
        [StringLength(50)]
        public string TenDangNhap { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu.")]
        public string MatKhau { get; set; } = string.Empty;
        
        public DateTime? NgayLapThe { get; set; }

        // Navigation property
        public ICollection<PhieuMuon>? PhieuMuons { get; set; }
    }
}