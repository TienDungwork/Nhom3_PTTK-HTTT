PRAGMA foreign_keys = ON;

DROP TABLE IF EXISTS notifications;
DROP TABLE IF EXISTS borrow_records;
DROP TABLE IF EXISTS borrow_requests;
DROP TABLE IF EXISTS book_copies;
DROP TABLE IF EXISTS books;
DROP TABLE IF EXISTS book_categories;
DROP TABLE IF EXISTS app_users;
DROP TABLE IF EXISTS readers;

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
    ma_doc_gia TEXT,
    username TEXT NOT NULL UNIQUE,
    password TEXT NOT NULL,
    ho_ten TEXT NOT NULL,
    email TEXT NOT NULL DEFAULT '',
    sdt TEXT NOT NULL DEFAULT '',
    role TEXT NOT NULL CHECK (role IN ('Admin', 'ThuThu', 'DocGia')),
    is_active INTEGER NOT NULL DEFAULT 1 CHECK (is_active IN (0, 1)),
    ngay_tao TEXT NOT NULL,
    FOREIGN KEY (ma_doc_gia) REFERENCES readers(ma_doc_gia)
);

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
    so_luong INTEGER NOT NULL DEFAULT 0 CHECK (so_luong >= 0),
    so_luong_dang_muon INTEGER NOT NULL DEFAULT 0 CHECK (so_luong_dang_muon >= 0),
    so_luong_mat_hong INTEGER NOT NULL DEFAULT 0 CHECK (so_luong_mat_hong >= 0),
    trang_thai TEXT NOT NULL DEFAULT 'Co san' CHECK (trang_thai IN ('Co san', 'Het sach', 'Ngung kinh doanh')),
    FOREIGN KEY (ma_danh_muc) REFERENCES book_categories(ma_danh_muc)
);

CREATE TABLE book_copies (
    ma_quyen_sach TEXT PRIMARY KEY,
    ma_sach TEXT NOT NULL,
    ngay_nhap TEXT NOT NULL,
    trang_thai TEXT NOT NULL DEFAULT 'Co san'
        CHECK (trang_thai IN ('Co san', 'Dang muon', 'Hong', 'Mat', 'Bao tri')),
    nha_cung_cap TEXT NOT NULL DEFAULT '',
    ghi_chu TEXT NOT NULL DEFAULT '',
    FOREIGN KEY (ma_sach) REFERENCES books(ma_sach)
);

CREATE TABLE borrow_requests (
    ma_yeu_cau TEXT PRIMARY KEY,
    ma_doc_gia TEXT NOT NULL,
    ten_doc_gia TEXT NOT NULL,
    ma_sach TEXT NOT NULL,
    ten_sach TEXT NOT NULL,
    ma_quyen_sach_yeu_cau TEXT,
    ngay_muon_du_kien TEXT NOT NULL,
    so_ngay_muon INTEGER NOT NULL DEFAULT 14 CHECK (so_ngay_muon > 0),
    ngay_tao_yeu_cau TEXT NOT NULL,
    trang_thai TEXT NOT NULL DEFAULT 'Cho duyet' CHECK (trang_thai IN ('Cho duyet', 'Da duyet', 'Tu choi')),
    nguoi_duyet TEXT NOT NULL DEFAULT '',
    ly_do_tu_choi TEXT NOT NULL DEFAULT '',
    FOREIGN KEY (ma_doc_gia) REFERENCES readers(ma_doc_gia),
    FOREIGN KEY (ma_sach) REFERENCES books(ma_sach),
    FOREIGN KEY (ma_quyen_sach_yeu_cau) REFERENCES book_copies(ma_quyen_sach)
);

CREATE TABLE borrow_records (
    ma_muon TEXT PRIMARY KEY,
    ma_yeu_cau TEXT,
    ma_doc_gia TEXT NOT NULL,
    ten_doc_gia TEXT NOT NULL,
    ma_sach TEXT NOT NULL,
    ma_quyen_sach TEXT NOT NULL,
    ten_sach TEXT NOT NULL,
    so_luong INTEGER NOT NULL DEFAULT 1 CHECK (so_luong > 0),
    ngay_muon TEXT NOT NULL,
    ngay_hen_tra TEXT NOT NULL,
    ngay_tra_thuc TEXT,
    trang_thai TEXT NOT NULL DEFAULT 'Dang muon' CHECK (trang_thai IN ('Dang muon', 'Da tra')),
    tien_phat REAL NOT NULL DEFAULT 0,
    da_thu_phat INTEGER NOT NULL DEFAULT 0 CHECK (da_thu_phat IN (0, 1)),
    FOREIGN KEY (ma_yeu_cau) REFERENCES borrow_requests(ma_yeu_cau),
    FOREIGN KEY (ma_doc_gia) REFERENCES readers(ma_doc_gia),
    FOREIGN KEY (ma_sach) REFERENCES books(ma_sach),
    FOREIGN KEY (ma_quyen_sach) REFERENCES book_copies(ma_quyen_sach)
);

CREATE TABLE notifications (
    ma_thong_bao TEXT PRIMARY KEY,
    ma_doc_gia TEXT NOT NULL,
    nguoi_gui TEXT NOT NULL DEFAULT 'Thu thu',
    tieu_de TEXT NOT NULL,
    noi_dung TEXT NOT NULL,
    thoi_gian TEXT NOT NULL,
    da_doc INTEGER NOT NULL DEFAULT 0 CHECK (da_doc IN (0, 1)),
    FOREIGN KEY (ma_doc_gia) REFERENCES readers(ma_doc_gia)
);

CREATE INDEX idx_books_ten_sach ON books(ten_sach);
CREATE INDEX idx_books_tac_gia ON books(tac_gia);
CREATE INDEX idx_books_isbn ON books(isbn);
CREATE INDEX idx_book_copies_ma_sach ON book_copies(ma_sach);
CREATE INDEX idx_book_copies_trang_thai ON book_copies(trang_thai);
CREATE INDEX idx_borrow_request_ma_doc_gia ON borrow_requests(ma_doc_gia);
CREATE INDEX idx_borrow_request_trang_thai ON borrow_requests(trang_thai);
CREATE INDEX idx_borrow_ma_doc_gia ON borrow_records(ma_doc_gia);
CREATE INDEX idx_borrow_ma_sach ON borrow_records(ma_sach);
CREATE INDEX idx_borrow_ma_quyen_sach ON borrow_records(ma_quyen_sach);
CREATE INDEX idx_borrow_trang_thai ON borrow_records(trang_thai);
CREATE INDEX idx_notifications_ma_doc_gia ON notifications(ma_doc_gia);
CREATE UNIQUE INDEX idx_borrow_open_copy ON borrow_records(ma_quyen_sach) WHERE trang_thai = 'Dang muon';
CREATE UNIQUE INDEX idx_user_docgia ON app_users(ma_doc_gia) WHERE ma_doc_gia IS NOT NULL;

INSERT INTO readers (ma_doc_gia, ho_ten, email, sdt, dia_chi, ngay_dang_ky) VALUES
('DG001', 'Nguyen Van Minh', 'minh@email.com', '0901234567', 'Ha Noi', '2024-01-15'),
('DG002', 'Tran Thi Lan', 'lan@email.com', '0912345678', 'TP.HCM', '2024-02-20'),
('DG003', 'Le Hoang Nam', 'nam@email.com', '0923456789', 'Da Nang', '2024-03-10');

INSERT INTO app_users (ma_tk, ma_doc_gia, username, password, ho_ten, email, sdt, role, is_active, ngay_tao) VALUES
('TK001', NULL, 'admin', 'admin123', 'Nguyen Quan Tri', 'admin@thuvien.vn', '0901234567', 'Admin', 1, '2024-01-01'),
('TK002', NULL, 'thuthu', 'thuthu123', 'Tran Thi Thu Thu', 'thuthu@thuvien.vn', '0912345678', 'ThuThu', 1, '2024-01-01'),
('TK003', 'DG001', 'docgia', 'docgia123', 'Nguyen Van Minh', 'minh@email.com', '0901234567', 'DocGia', 1, '2024-01-01'),
('TK004', 'DG002', 'docgia2', 'docgia123', 'Tran Thi Lan', 'lan@email.com', '0912345678', 'DocGia', 1, '2024-01-01');

INSERT INTO book_categories (ma_danh_muc, ten_danh_muc, mo_ta, vi_tri_ke, dang_su_dung) VALUES
('DM001', 'Cong nghe', 'Sach cong nghe thong tin, lap trinh va du lieu', 'Ke A', 1),
('DM002', 'Van hoc', 'Tac pham van hoc Viet Nam va the gioi', 'Ke B', 1),
('DM003', 'Lich su', 'Tai lieu lich su va nghien cuu xa hoi', 'Ke B', 1);

INSERT INTO books (ma_sach, ten_sach, tac_gia, ma_danh_muc, chu_de, nam_xuat_ban, nha_xuat_ban, isbn, vi_tri_kho, nha_cung_cap, so_luong, so_luong_dang_muon, so_luong_mat_hong, trang_thai) VALUES
('S001', 'Lap trinh C# co ban', 'Nguyen Van A', 'DM001', 'Lap trinh', 2022, 'NXB Giao duc', '978-604-1-00001-0', 'A1-01', 'Fahasa', 3, 1, 0, 'Co san'),
('S002', 'Cau truc du lieu va giai thuat', 'Tran Thi B', 'DM001', 'Khoa hoc may tinh', 2021, 'NXB DHQG', '978-604-1-00002-7', 'A1-02', 'Fahasa', 3, 1, 0, 'Co san'),
('S003', 'Tri tue nhan tao', 'Le Hoang C', 'DM001', 'AI', 2023, 'NXB Bach khoa', '978-604-1-00003-4', 'A1-03', 'Tiki', 2, 0, 0, 'Co san');

INSERT INTO book_copies (ma_quyen_sach, ma_sach, ngay_nhap, trang_thai, nha_cung_cap, ghi_chu) VALUES
('Q001', 'S001', '2024-01-10', 'Dang muon', 'Fahasa', 'Ban sao 1'),
('Q002', 'S001', '2024-01-10', 'Co san', 'Fahasa', 'Ban sao 2'),
('Q003', 'S001', '2025-09-15', 'Co san', 'Fahasa', 'Ban sao bo sung hoc ky moi'),
('Q004', 'S002', '2024-02-05', 'Dang muon', 'Fahasa', ''),
('Q005', 'S002', '2024-02-05', 'Co san', 'Fahasa', ''),
('Q006', 'S002', '2024-02-05', 'Co san', 'Fahasa', ''),
('Q007', 'S003', '2024-03-02', 'Co san', 'Tiki', '');

INSERT INTO borrow_requests (ma_yeu_cau, ma_doc_gia, ten_doc_gia, ma_sach, ten_sach, ma_quyen_sach_yeu_cau, ngay_muon_du_kien, so_ngay_muon, ngay_tao_yeu_cau, trang_thai, nguoi_duyet, ly_do_tu_choi) VALUES
('YC001', 'DG001', 'Nguyen Van Minh', 'S002', 'Cau truc du lieu va giai thuat', NULL, '2026-03-25', 14, '2026-03-24 09:00:00', 'Cho duyet', '', ''),
('YC002', 'DG002', 'Tran Thi Lan', 'S001', 'Lap trinh C# co ban', NULL, '2026-03-25', 7, '2026-03-24 10:30:00', 'Cho duyet', '', '');

INSERT INTO borrow_records (ma_muon, ma_yeu_cau, ma_doc_gia, ten_doc_gia, ma_sach, ma_quyen_sach, ten_sach, so_luong, ngay_muon, ngay_hen_tra, ngay_tra_thuc, trang_thai, tien_phat, da_thu_phat) VALUES
('M001', NULL, 'DG001', 'Nguyen Van Minh', 'S001', 'Q001', 'Lap trinh C# co ban', 1, '2026-03-01', '2026-03-15', NULL, 'Dang muon', 0, 0),
('M002', NULL, 'DG002', 'Tran Thi Lan', 'S002', 'Q004', 'Cau truc du lieu va giai thuat', 1, '2026-03-03', '2026-03-17', NULL, 'Dang muon', 0, 0);

INSERT INTO notifications (ma_thong_bao, ma_doc_gia, nguoi_gui, tieu_de, noi_dung, thoi_gian, da_doc) VALUES
('TB001', 'DG001', 'He thong', 'Sap den han tra sach', 'Sach Lap trinh C# co ban se den han trong 2 ngay.', '2026-03-24 08:00:00', 0),
('TB002', 'DG002', 'Thu thu', 'Nhac nho tra sach', 'Vui long kiem tra va tra sach dung han.', '2026-03-23 15:00:00', 0);
