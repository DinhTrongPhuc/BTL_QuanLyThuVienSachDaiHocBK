using YourProject.Models;

namespace YourProject.ViewModels
{
	public class ReaderCartViewModel
	{
		public int MaDocGia { get; set; }
		public string TenDocGia { get; set; } = string.Empty;
		public List<Sach> SachMuon { get; set; } = new List<Sach>();
	}
}
