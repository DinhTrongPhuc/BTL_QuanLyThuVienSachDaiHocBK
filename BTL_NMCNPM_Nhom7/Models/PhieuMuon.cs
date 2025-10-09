using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YourProject.Models
{
	[Table("PhieuMuon")]
	public class PhieuMuon
	{
		[Key]
		[Column("MaPhieuMuon")] 
		[Display(Name = "Mã phiếu mượn")]
		public int MaPhieuMuon { get; set; }

		[Required]
		[Display(Name = "Độc giả")]
		[ForeignKey("DocGia")]
		public int MaDocGia { get; set; }
		public DocGia DocGia { get; set; }

		[Required]
		[Display(Name = "Thủ thư")]
		[ForeignKey("ThuThu")]
		public int MaThuThu { get; set; }
		public ThuThu ThuThu { get; set; }

		[Required]
		[Display(Name = "Ngày mượn")]
		public DateTime NgayMuon { get; set; } = DateTime.Now;

		[Required]
		[Display(Name = "Ngày hẹn trả")]
		public DateTime NgayHenTra { get; set; }

		[Display(Name = "Trạng thái")]
		public string TrangThai { get; set; }

		// Danh sách chi tiết sách mượn
		[Display(Name = "Chi tiết phiếu mượn")]
		public ICollection<ChiTietPhieuMuon> ChiTietPhieuMuons { get; set; }
	}
}
