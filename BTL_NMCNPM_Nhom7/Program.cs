// Thêm các namespace cần thiết ở đầu file
using Microsoft.EntityFrameworkCore;
using BTL_NMCNPM_Nhom7.Data;

var builder = WebApplication.CreateBuilder(args);

// --- PHẦN BẠN CẦN THÊM BẮT ĐẦU TỪ ĐÂY ---

// 1. Lấy chuỗi kết nối từ appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// 2. Đăng ký ApplicationDbContext với SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// --- KẾT THÚC PHẦN THÊM ---


// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // Sửa lại: Bỏ app.MapStaticAssets() và dùng dòng này

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
    // .WithStaticAssets(); // Dòng này không cần thiết và có thể gây lỗi

app.Run();