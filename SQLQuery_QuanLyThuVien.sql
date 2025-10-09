Create Database SQLQuery_QuanLyThuVien
go 
use SQLQuery_QuanLyThuVien
go

CREATE TABLE TheLoai (
    MaTheLoai INT PRIMARY KEY IDENTITY(1,1),
    TenTheLoai NVARCHAR(100) NOT NULL
);
go
CREATE TABLE TacGia (
    MaTacGia INT PRIMARY KEY IDENTITY(1,1),
    TenTacGia NVARCHAR(100) NOT NULL,
    TieuSu TEXT NULL
);
go
CREATE TABLE ThuThu (
    MaThuThu INT PRIMARY KEY IDENTITY(1,1),
    HoTen NVARCHAR(100) NOT NULL,
    TenDangNhap VARCHAR(50) NOT NULL UNIQUE,
    MatKhau VARCHAR(255) NOT NULL,
    QuyenHan NVARCHAR(50) NOT NULL
);
go
CREATE TABLE DocGia (
    Id INT PRIMARY KEY IDENTITY(1,1),
    HoTen NVARCHAR(100) NOT NULL,
    NgaySinh DATE NULL,
    DiaChi NVARCHAR(255) NULL,
    SoDienThoai VARCHAR(15) NULL,
    Email VARCHAR(100) UNIQUE,
    TenDangNhap VARCHAR(50) NOT NULL UNIQUE,
    MatKhau VARCHAR(255) NOT NULL,
    NgayLapThe DATE DEFAULT (GETDATE())
);
go
CREATE TABLE Sach (
    MaSach INT PRIMARY KEY IDENTITY(1,1),
    TenSach NVARCHAR(255) NOT NULL,
    MaTheLoai INT,
    MaTacGia INT,
    NamXuatBan INT NULL,
    SoLuong INT NOT NULL DEFAULT 0,
    MoTa TEXT NULL,
    FOREIGN KEY (MaTheLoai) REFERENCES TheLoai(MaTheLoai),
    FOREIGN KEY (MaTacGia) REFERENCES TacGia(MaTacGia)
);
go
CREATE TABLE PhieuMuon (
    MaPhieuMuon INT PRIMARY KEY IDENTITY(1,1),
    MaDocGia INT,
    MaThuThu INT,
    NgayMuon DATETIME DEFAULT CURRENT_TIMESTAMP,
    NgayHenTra DATE NOT NULL,
    TrangThai NVARCHAR(50) NOT NULL,
    FOREIGN KEY (MaDocGia) REFERENCES DocGia(Id),
    FOREIGN KEY (MaThuThu) REFERENCES ThuThu(MaThuThu)
);
go
CREATE TABLE ChiTietPhieuMuon (
    MaPhieuMuon INT,
    MaSach INT,
    NgayTra DATETIME NULL,
    TienPhat DECIMAL(10, 2) DEFAULT 0.00,
    GhiChu NVARCHAR(255) NULL,
    PRIMARY KEY (MaPhieuMuon, MaSach),
    FOREIGN KEY (MaPhieuMuon) REFERENCES PhieuMuon(MaPhieuMuon),
    FOREIGN KEY (MaSach) REFERENCES Sach(MaSach)
);