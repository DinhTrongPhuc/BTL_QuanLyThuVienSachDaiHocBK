using Microsoft.EntityFrameworkCore;
using BTL_NMCNPM_Nhom7.Models;

namespace BTL_NMCNPM_Nhom7.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // Khai báo các bảng sẽ được tạo trong CSDL
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
    
            // --- THÊM VÀO ĐÂY ---
            // Cấu hình cho cột TienPhat
        modelBuilder.Entity<ChiTietPhieuMuon>()
            .Property(ctpm => ctpm.TienPhat)
            .HasColumnType("decimal(18, 2)");
            }
        
    }
}