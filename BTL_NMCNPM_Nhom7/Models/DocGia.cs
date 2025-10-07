using System.ComponentModel.DataAnnotations;

namespace YourProject.Models
{
    public class DocGia
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Họ và tên")]
        public string HoTen { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Display(Name = "Số điện thoại")]
        public string SoDienThoai { get; set; }
    }
}
