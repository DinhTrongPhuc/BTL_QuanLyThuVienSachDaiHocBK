using System.ComponentModel.DataAnnotations;

namespace YourProject.Models
{
    public class TacGia
    {
        [Key]
        public int MaTacGia { get; set; }

        [Required]
        [StringLength(100)]
        public string TenTacGia { get; set; } = string.Empty;

        // Navigation property: Một tác giả có nhiều sách
        public ICollection<Sach>? Sachs { get; set; }
    }
}