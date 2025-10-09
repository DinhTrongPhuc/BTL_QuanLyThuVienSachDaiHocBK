using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YourProject.Models
{
	[Table("ThuThu")]
	public class ThuThu
	{
		[Key]
		[Display(Name = "Mã thủ thư")]
		public int MaThuThu { get; set; }

		[Required]
		[Display(Name = "Họ và tên")]
		public string HoTen { get; set; }
	}
}
