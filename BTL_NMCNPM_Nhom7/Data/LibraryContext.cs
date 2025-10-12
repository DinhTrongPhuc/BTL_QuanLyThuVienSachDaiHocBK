using Microsoft.EntityFrameworkCore;
using YourProject.Models;

namespace YourProject.Data
{
    public class LibraryContext : DbContext
    {
        public LibraryContext(DbContextOptions<LibraryContext> options) : base(options)
        {
        }
        public DbSet<Sach> Sach { get; set; }
        public DbSet<DocGia> DocGia { get; set; }
        public DbSet<PhieuMuon> PhieuMuon { get; set; }
        public DbSet<ChiTietPhieuMuon> ChiTietPhieuMuon { get; set; }
        public DbSet<TheLoai> TheLoai { get; set; }
        public DbSet<TacGia> TacGia { get; set; }
        public DbSet<ThuThu> ThuThu { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Cấu hình khóa chính phức hợp cho ChiTietPhieuMuon
            modelBuilder.Entity<ChiTietPhieuMuon>()
                .HasKey(ctpm => new { ctpm.MaPhieuMuon, ctpm.MaSach });
        }
    }
}
