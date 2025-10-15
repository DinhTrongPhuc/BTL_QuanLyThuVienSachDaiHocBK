using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YourProject.Models
{
    public class PhieuMuon
    {
		[Key]
		public int MaPhieuMuon { get; set; }

		public int MaDocGia { get; set; }
		public int MaThuThu { get; set; }

		// NgayMuon mặc định là DateTime.Now khi tạo phiếu
		public DateTime NgayMuon { get; set; } = DateTime.Now;

		// NgayHenTra bắt buộc
		public DateTime NgayHenTra { get; set; }

		[StringLength(50)]
		public string TrangThai { get; set; } = string.Empty;

		// Navigation properties
		[ForeignKey("MaDocGia")]
		public DocGia? DocGia { get; set; }

		[ForeignKey("MaThuThu")]
		public ThuThu? ThuThu { get; set; }

		public ICollection<ChiTietPhieuMuon>? ChiTietPhieuMuons { get; set; }

		// NgayTra và DaTra có thể null
		public DateTime? NgayTra { get; set; }
		public bool? DaTra { get; set; }

	}
}