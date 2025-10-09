using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YourProject.Models
{

	[Table("ChiTietPhieuMuon")]
	public class ChiTietPhieuMuon
	{
		[ForeignKey("PhieuMuon")]
		public int MaPhieuMuon { get; set; }
		public PhieuMuon PhieuMuon { get; set; }

		[ForeignKey("Sach")]
		public int MaSach { get; set; }
		public Sach Sach { get; set; }

		public DateTime? NgayTra { get; set; }

		public string GhiChu { get; set; }
	}
}