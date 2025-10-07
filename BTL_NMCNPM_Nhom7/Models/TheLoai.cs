using System.ComponentModel.DataAnnotations;

namespace BTL_NMCNPM_Nhom7.Models
{
    public class TheLoai
    {
        [Key]
        public int MaTheLoai { get; set; }

        [Required]
        [StringLength(100)]
        public string TenTheLoai { get; set; } = string.Empty;

        // Navigation property: Một thể loại có nhiều sách
        public ICollection<Sach>? Sachs { get; set; }
    }
}