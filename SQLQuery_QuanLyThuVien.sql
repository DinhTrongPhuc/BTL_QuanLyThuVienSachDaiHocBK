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

INSERT INTO TacGia (TenTacGia)
VALUES
    (N'Nguyễn Nhật Ánh'), 
    (N'Paulo Coelho'),   
    (N'Dale Carnegie'),  
    (N'Stephen Hawking'),
    (N'J.K. Rowling');   
GO

INSERT INTO TheLoai (TenTheLoai)
VALUES
    (N'Văn học thiếu nhi'), 
    (N'Tiểu thuyết'),       
    (N'Kỹ năng sống'),      
    (N'Khoa học');          
GO
INSERT INTO Sach (TenSach, NamXuatBan, SoLuong, MoTa, MaTheLoai, MaTacGia)
VALUES 
    (N'Tôi thấy hoa vàng trên cỏ xanh', 2015, 20, N'Câu chuyện tuổi thơ ở một làng quê Việt Nam.', 1, 1),
    (N'Nhà giả kim', 2017, 15, N'Hành trình của cậu bé chăn cừu Santiago.', 2, 2),
    (N'Đắc nhân tâm', 2018, 30, N'Sách về nghệ thuật đối nhân xử thế.', 3, 3),
    (N'Lược sử thời gian', 2010, 12, N'Từ Vụ Nổ Lớn đến các hố đen.', 4, 4),
    (N'Harry Potter và Hòn đá Phù thủy', 2000, 25, N'Tập đầu tiên trong bộ truyện Harry Potter.', 2, 5),
    (N'Cho tôi xin một vé đi tuổi thơ', 2010, 18, N'Tác phẩm nổi tiếng của Nguyễn Nhật Ánh.', 1, 1),
    (N'Quẳng gánh lo đi và vui sống', 2019, 22, N'Cuốn sách giúp bạn vượt qua lo lắng.', 3, 3),
    (N'Vũ trụ trong vỏ hạt dẻ', 2011, 8, N'Khám phá các lý thuyết vật lý hiện đại.', 4, 4),
    (N'Harry Potter và Phòng chứa Bí mật', 2001, 23, N'Tập thứ hai trong bộ truyện Harry Potter.', 2, 5),
    (N'Mắt biếc', 2016, 16, N'Một câu chuyện tình lãng mạn và buồn.', 2, 1);
GO

INSERT INTO DocGia (HoTen, NgaySinh, DiaChi, SoDienThoai, Email, TenDangNhap, MatKhau)
VALUES
('Nguyễn Văn A', '1995-05-12', 'Hà Nội', '0987654321', 'nguyenvana@example.com', 'nguyenvana', 'MatKhau123'),
('Trần Thị B', '1998-08-20', 'Hồ Chí Minh', '0912345678', 'tranthib@example.com', 'tranthib', 'MatKhau123'),
('Lê Thế C', '2000-01-15', 'Đà Nẵng', '0978123456', 'lethec@example.com', 'lethec', 'MatKhau123'),
('Phạm Văn D', '1997-12-05', 'Hải Phòng', '0965432187', 'phamvand@example.com', 'phamvand', 'MatKhau123'),
('Nguyễn Thị E', '1999-03-30', 'Cần Thơ', '0932165478', 'nguyenthie@example.com', 'nguyenthie', 'MatKhau123');

ALTER TABLE PhieuMuon
ADD NgayTra DATETIME NULL,
    DaTra BIT NULL;

INSERT INTO ThuThu (HoTen, TenDangNhap, MatKhau, QuyenHan)
VALUES (N'Admin Quản Trị', 'admin', 'admin123', N'Admin');