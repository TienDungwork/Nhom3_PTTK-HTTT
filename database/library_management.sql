PRAGMA foreign_keys = ON;

DROP TABLE IF EXISTS borrow_records;
DROP TABLE IF EXISTS book_lots;
DROP TABLE IF EXISTS books;
DROP TABLE IF EXISTS book_categories;
DROP TABLE IF EXISTS readers;
DROP TABLE IF EXISTS app_users;

CREATE TABLE book_categories (
    ma_danh_muc TEXT PRIMARY KEY,
    ten_danh_muc TEXT NOT NULL UNIQUE,
    mo_ta TEXT DEFAULT '',
    vi_tri_ke TEXT DEFAULT '',
    dang_su_dung INTEGER NOT NULL DEFAULT 1 CHECK (dang_su_dung IN (0, 1))
);

CREATE TABLE books (
    ma_sach TEXT PRIMARY KEY,
    ten_sach TEXT NOT NULL,
    tac_gia TEXT NOT NULL DEFAULT '',
    ma_danh_muc TEXT NOT NULL,
    chu_de TEXT NOT NULL DEFAULT '',
    nam_xuat_ban INTEGER NOT NULL DEFAULT 2024,
    nha_xuat_ban TEXT NOT NULL DEFAULT '',
    uri TEXT NOT NULL DEFAULT '',
    isbn TEXT NOT NULL DEFAULT '',
    bo_suu_tap TEXT NOT NULL DEFAULT '',
    anh_bia TEXT NOT NULL DEFAULT '',
    vi_tri_kho TEXT NOT NULL DEFAULT '',
    nha_cung_cap TEXT NOT NULL DEFAULT '',
    trang_thai TEXT NOT NULL DEFAULT 'Co san',
    FOREIGN KEY (ma_danh_muc) REFERENCES book_categories(ma_danh_muc)
);

CREATE TABLE book_lots (
    ma_lo TEXT PRIMARY KEY,
    ma_sach TEXT NOT NULL,
    ngay_nhap TEXT NOT NULL,
    so_luong_nhap INTEGER NOT NULL CHECK (so_luong_nhap >= 0),
    so_luong_con INTEGER NOT NULL CHECK (so_luong_con >= 0 AND so_luong_con <= so_luong_nhap),
    tinh_trang TEXT NOT NULL DEFAULT 'Moi',
    nha_cung_cap TEXT NOT NULL DEFAULT '',
    ghi_chu TEXT NOT NULL DEFAULT '',
    FOREIGN KEY (ma_sach) REFERENCES books(ma_sach)
);

CREATE TABLE readers (
    ma_doc_gia TEXT PRIMARY KEY,
    ho_ten TEXT NOT NULL,
    email TEXT NOT NULL DEFAULT '',
    sdt TEXT NOT NULL DEFAULT '',
    dia_chi TEXT NOT NULL DEFAULT '',
    ngay_dang_ky TEXT NOT NULL
);

CREATE TABLE app_users (
    ma_tk TEXT PRIMARY KEY,
    username TEXT NOT NULL UNIQUE,
    password TEXT NOT NULL,
    ho_ten TEXT NOT NULL,
    email TEXT NOT NULL DEFAULT '',
    sdt TEXT NOT NULL DEFAULT '',
    role TEXT NOT NULL CHECK (role IN ('Admin', 'ThuThu', 'DocGia')),
    is_active INTEGER NOT NULL DEFAULT 1 CHECK (is_active IN (0, 1)),
    ngay_tao TEXT NOT NULL
);

CREATE TABLE borrow_records (
    ma_muon TEXT PRIMARY KEY,
    ma_doc_gia TEXT NOT NULL,
    ten_doc_gia TEXT NOT NULL,
    ma_sach TEXT NOT NULL,
    ma_lo TEXT NOT NULL,
    ten_sach TEXT NOT NULL,
    so_luong INTEGER NOT NULL DEFAULT 1 CHECK (so_luong > 0),
    ngay_muon TEXT NOT NULL,
    ngay_hen_tra TEXT NOT NULL,
    ngay_tra_thuc TEXT,
    trang_thai TEXT NOT NULL DEFAULT 'Dang muon' CHECK (trang_thai IN ('Dang muon', 'Da tra')),
    tien_phat REAL NOT NULL DEFAULT 0,
    FOREIGN KEY (ma_doc_gia) REFERENCES readers(ma_doc_gia),
    FOREIGN KEY (ma_sach) REFERENCES books(ma_sach),
    FOREIGN KEY (ma_lo) REFERENCES book_lots(ma_lo)
);

CREATE INDEX idx_books_ten_sach ON books(ten_sach);
CREATE INDEX idx_books_tac_gia ON books(tac_gia);
CREATE INDEX idx_books_isbn ON books(isbn);
CREATE INDEX idx_lots_ma_sach ON book_lots(ma_sach);
CREATE INDEX idx_lots_ngay_nhap ON book_lots(ngay_nhap);
CREATE INDEX idx_borrow_ma_doc_gia ON borrow_records(ma_doc_gia);
CREATE INDEX idx_borrow_ma_sach ON borrow_records(ma_sach);
CREATE INDEX idx_borrow_trang_thai ON borrow_records(trang_thai);

INSERT INTO book_categories (ma_danh_muc, ten_danh_muc, mo_ta, vi_tri_ke, dang_su_dung) VALUES
('DM001', 'Cong nghe', 'Sach cong nghe thong tin, lap trinh va du lieu', 'Ke A', 1),
('DM002', 'Van hoc', 'Tac pham van hoc Viet Nam va the gioi', 'Ke B', 1),
('DM003', 'Lich su', 'Tai lieu lich su va nghien cuu xa hoi', 'Ke B', 1),
('DM004', 'Kinh te', 'Giao trinh va tai lieu kinh te', 'Ke C', 1),
('DM005', 'Tam ly', 'Tam ly hoc ung dung va dai cuong', 'Ke C', 1),
('DM006', 'Toan hoc', 'Sach toan co ban va nang cao', 'Ke D', 1),
('DM007', 'Triet hoc', 'Triet hoc, chinh tri va ly luan', 'Ke D', 1);

INSERT INTO books (ma_sach, ten_sach, tac_gia, ma_danh_muc, chu_de, nam_xuat_ban, nha_xuat_ban, isbn, vi_tri_kho, nha_cung_cap, trang_thai) VALUES
('S001', 'Lap trinh C# co ban', 'Nguyen Van A', 'DM001', 'Lap trinh', 2022, 'NXB Giao duc', '978-604-1-00001-0', 'A1-01', 'Fahasa', 'Co san'),
('S002', 'Cau truc du lieu va giai thuat', 'Tran Thi B', 'DM001', 'Khoa hoc may tinh', 2021, 'NXB DHQG', '978-604-1-00002-7', 'A1-02', 'Fahasa', 'Co san'),
('S003', 'Tri tue nhan tao', 'Le Hoang C', 'DM001', 'AI', 2023, 'NXB Bach khoa', '978-604-1-00003-4', 'A1-03', 'Tiki', 'Co san');

INSERT INTO book_lots (ma_lo, ma_sach, ngay_nhap, so_luong_nhap, so_luong_con, tinh_trang, nha_cung_cap, ghi_chu) VALUES
('LO001', 'S001', '2024-01-10', 50, 35, 'Moi', 'Fahasa', 'Lo nhap dau nam'),
('LO002', 'S001', '2025-09-15', 50, 50, 'Moi', 'Fahasa', 'Bo sung hoc ky moi'),
('LO003', 'S002', '2024-02-05', 30, 24, 'Moi', 'Fahasa', ''),
('LO004', 'S003', '2024-03-02', 20, 16, 'Moi', 'Tiki', '');

INSERT INTO readers (ma_doc_gia, ho_ten, email, sdt, dia_chi, ngay_dang_ky) VALUES
('DG001', 'Nguyen Van Minh', 'minh@email.com', '0901234567', 'Ha Noi', '2024-01-15'),
('DG002', 'Tran Thi Lan', 'lan@email.com', '0912345678', 'TP.HCM', '2024-02-20');

INSERT INTO app_users (ma_tk, username, password, ho_ten, email, sdt, role, is_active, ngay_tao) VALUES
('TK001', 'admin', 'admin123', 'Nguyen Quan Tri', 'admin@thuvien.vn', '0901234567', 'Admin', 1, '2024-01-01'),
('TK002', 'thuthu', 'thuthu123', 'Tran Thi Thu Thu', 'thuthu@thuvien.vn', '0912345678', 'ThuThu', 1, '2024-01-01'),
('TK003', 'docgia', 'docgia123', 'Pham Van Doc Gia', 'docgia@gmail.com', '0923456789', 'DocGia', 1, '2024-01-01');

INSERT INTO borrow_records (ma_muon, ma_doc_gia, ten_doc_gia, ma_sach, ma_lo, ten_sach, so_luong, ngay_muon, ngay_hen_tra, ngay_tra_thuc, trang_thai, tien_phat) VALUES
('M001', 'DG001', 'Nguyen Van Minh', 'S001', 'LO001', 'Lap trinh C# co ban', 1, '2026-03-01', '2026-03-15', NULL, 'Dang muon', 0),
('M002', 'DG002', 'Tran Thi Lan', 'S002', 'LO003', 'Cau truc du lieu va giai thuat', 1, '2026-03-03', '2026-03-17', NULL, 'Dang muon', 0);
