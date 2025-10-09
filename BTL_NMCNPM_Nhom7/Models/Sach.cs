using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YourProject.Models
{
	[Table("Sach")]
	public class Sach
	{
		[Key]
		[Display(Name = "Mã sách")]
		public int MaSach { get; set; }

		[Required]
		[Display(Name = "Tên sách")]
		public string TenSach { get; set; }

		[Required]
		[Display(Name = "Số lượng")]
		public int SoLuong { get; set; }
	}
}