# Sơ Đồ ERD Hệ Thống Quản Lý Thư Viện

```mermaid
erDiagram
    BOOK_CATEGORIES ||--o{ BOOKS : "phan_loai"
    BOOKS ||--o{ BOOK_LOTS : "co_lo_nhap"
    READERS ||--o{ BORROW_RECORDS : "muon_sach"
    BOOKS ||--o{ BORROW_RECORDS : "duoc_muon"
    BOOK_LOTS ||--o{ BORROW_RECORDS : "xuat_theo_lo"

    BOOK_CATEGORIES {
        TEXT ma_danh_muc PK
        TEXT ten_danh_muc UK
        TEXT mo_ta
        TEXT vi_tri_ke
        INT dang_su_dung
    }

    BOOKS {
        TEXT ma_sach PK
        TEXT ten_sach
        TEXT tac_gia
        TEXT ma_danh_muc FK
        TEXT chu_de
        INT nam_xuat_ban
        TEXT nha_xuat_ban
        TEXT uri
        TEXT isbn
        TEXT bo_suu_tap
        TEXT anh_bia
        TEXT vi_tri_kho
        TEXT nha_cung_cap
        TEXT trang_thai
    }

    BOOK_LOTS {
        TEXT ma_lo PK
        TEXT ma_sach FK
        DATE ngay_nhap
        INT so_luong_nhap
        INT so_luong_con
        TEXT tinh_trang
        TEXT nha_cung_cap
        TEXT ghi_chu
    }

    READERS {
        TEXT ma_doc_gia PK
        TEXT ho_ten
        TEXT email
        TEXT sdt
        TEXT dia_chi
        DATE ngay_dang_ky
    }

    APP_USERS {
        TEXT ma_tk PK
        TEXT username UK
        TEXT password
        TEXT ho_ten
        TEXT email
        TEXT sdt
        TEXT role
        INT is_active
        DATE ngay_tao
    }

    BORROW_RECORDS {
        TEXT ma_muon PK
        TEXT ma_doc_gia FK
        TEXT ten_doc_gia
        TEXT ma_sach FK
        TEXT ma_lo FK
        TEXT ten_sach
        INT so_luong
        DATE ngay_muon
        DATE ngay_hen_tra
        DATE ngay_tra_thuc
        TEXT trang_thai
        REAL tien_phat
    }
```

## Ghi chú
- `APP_USERS` độc lập với `READERS` trong thiết kế hiện tại (chưa gắn FK trực tiếp).
- `BORROW_RECORDS.ten_doc_gia` và `BORROW_RECORDS.ten_sach` là dữ liệu snapshot lịch sử.
- Quan hệ cốt lõi cho nghiệp vụ lô sách là `BOOKS -> BOOK_LOTS -> BORROW_RECORDS`.
