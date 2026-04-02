# PHẦN 4. THIẾT KẾ CƠ SỞ DỮ LIỆU

Tài liệu này mô tả thiết kế CSDL quan hệ cho hệ thống quản lý thư viện, bám theo mô hình lớp đã triển khai trong mã nguồn và schema SQL tại `database/library_management.sql`.

## 4.1. Ánh xạ biểu đồ lớp thành các lược đồ cơ sở dữ liệu

### 4.1.1. Nguyên tắc ánh xạ
- Mỗi lớp nghiệp vụ chính được ánh xạ thành một bảng quan hệ.
- Quan hệ `1-N` trong biểu đồ lớp được biểu diễn bằng khóa ngoại ở phía `N`.
- Trường tính toán động (ví dụ số lượng hiện có) không lưu trực tiếp nếu có thể suy diễn từ dữ liệu chi tiết.
- Một số trường snapshot tại thời điểm nghiệp vụ (như tên độc giả, tên sách trong phiếu mượn) được giữ lại để phục vụ lịch sử.

### 4.1.2. Ma trận ánh xạ lớp -> bảng

| Lớp miền nghiệp vụ | Bảng CSDL | Ghi chú ánh xạ |
|---|---|---|
| `BookCategory` | `book_categories` | Danh mục sách |
| `Book` | `books` | Đầu sách (metadata) |
| `BookLot` | `book_lots` | Lô nhập theo ngày/tình trạng |
| `Reader` | `readers` | Độc giả |
| `AppUser` | `app_users` | Tài khoản hệ thống |
| `BorrowRecord` | `borrow_records` | Phiếu mượn/trả có truy vết theo lô |

### 4.1.3. Ánh xạ quan hệ giữa các lớp
- `BookCategory (1) --- (N) Book`  
  `books.ma_danh_muc` -> `book_categories.ma_danh_muc`
- `Book (1) --- (N) BookLot`  
  `book_lots.ma_sach` -> `books.ma_sach`
- `Reader (1) --- (N) BorrowRecord`  
  `borrow_records.ma_doc_gia` -> `readers.ma_doc_gia`
- `Book (1) --- (N) BorrowRecord`  
  `borrow_records.ma_sach` -> `books.ma_sach`
- `BookLot (1) --- (N) BorrowRecord`  
  `borrow_records.ma_lo` -> `book_lots.ma_lo`

### 4.1.4. Lý do bổ sung thực thể `BookLot`
Thực thể `BookLot` giúp giải quyết yêu cầu nghiệp vụ quan trọng:
- Phân biệt nhiều lô của cùng một đầu sách (ví dụ 50 quyển nhập đợt 1, 50 quyển nhập đợt 2).
- Theo dõi ngày nhập, tình trạng mới/cũ, nhà cung cấp theo từng lô.
- Quản lý tồn kho và truy vết xuất/nhập theo lô thay vì chỉ tổng số lượng đầu sách.

## 4.2. Mô tả cấu trúc các bảng trong CSDL quan hệ

### 4.2.1. Bảng `book_categories`
- Mục đích: lưu danh mục nghiệp vụ của đầu sách.
- Khóa chính: `ma_danh_muc`.
- Thuộc tính:
  - `ma_danh_muc` (TEXT, PK)
  - `ten_danh_muc` (TEXT, UNIQUE, NOT NULL)
  - `mo_ta` (TEXT)
  - `vi_tri_ke` (TEXT)
  - `dang_su_dung` (INTEGER, CHECK 0/1)

### 4.2.2. Bảng `books`
- Mục đích: lưu thông tin thư mục của đầu sách.
- Khóa chính: `ma_sach`.
- Khóa ngoại: `ma_danh_muc` -> `book_categories`.
- Thuộc tính chính:
  - `ma_sach`, `ten_sach`, `tac_gia`, `isbn`, `nha_xuat_ban`, `nam_xuat_ban`
  - `chu_de`, `bo_suu_tap`, `vi_tri_kho`, `nha_cung_cap`, `uri`, `anh_bia`
  - `trang_thai` (trạng thái tổng quát)

### 4.2.3. Bảng `book_lots`
- Mục đích: lưu chi tiết từng lần nhập kho theo lô.
- Khóa chính: `ma_lo`.
- Khóa ngoại: `ma_sach` -> `books`.
- Thuộc tính chính:
  - `ma_lo`, `ma_sach`
  - `ngay_nhap`
  - `so_luong_nhap` (CHECK >= 0)
  - `so_luong_con` (CHECK >= 0 và <= `so_luong_nhap`)
  - `tinh_trang` (Mới/Cũ)
  - `nha_cung_cap`, `ghi_chu`

### 4.2.4. Bảng `readers`
- Mục đích: quản lý độc giả.
- Khóa chính: `ma_doc_gia`.
- Thuộc tính:
  - `ma_doc_gia`, `ho_ten`, `email`, `sdt`, `dia_chi`, `ngay_dang_ky`

### 4.2.5. Bảng `app_users`
- Mục đích: quản lý tài khoản đăng nhập hệ thống.
- Khóa chính: `ma_tk`.
- Ràng buộc:
  - `username` UNIQUE
  - `role` CHECK trong tập `{Admin, ThuThu, DocGia}`
  - `is_active` CHECK 0/1
- Thuộc tính:
  - `ma_tk`, `username`, `password`, `ho_ten`, `email`, `sdt`, `role`, `is_active`, `ngay_tao`

### 4.2.6. Bảng `borrow_records`
- Mục đích: lưu lịch sử mượn/trả có liên kết độc giả, đầu sách và lô xuất.
- Khóa chính: `ma_muon`.
- Khóa ngoại:
  - `ma_doc_gia` -> `readers`
  - `ma_sach` -> `books`
  - `ma_lo` -> `book_lots`
- Thuộc tính chính:
  - `ma_muon`, `ma_doc_gia`, `ma_sach`, `ma_lo`, `so_luong`
  - `ngay_muon`, `ngay_hen_tra`, `ngay_tra_thuc`
  - `trang_thai` (Đang mượn/Đã trả), `tien_phat`
  - `ten_doc_gia`, `ten_sach` (snapshot phục vụ truy vết lịch sử)

### 4.2.7. Ràng buộc toàn vẹn nghiệp vụ chính
- Không thể tạo bản ghi mượn nếu không tồn tại độc giả/đầu sách/lô.
- `so_luong_con` của lô không được âm.
- Không cho phép dữ liệu role/trạng thái ngoài tập hợp chuẩn.
- Dữ liệu thời gian mượn/trả được chuẩn hóa dạng `DATE/TEXT ISO` để dễ so sánh.

## 4.3. Xác định dạng chuẩn và tối ưu hóa CSDL

### 4.3.1. Chuẩn hóa dữ liệu

#### 1NF (First Normal Form)
- Mọi bảng đều có khóa chính.
- Thuộc tính là nguyên tố, không có nhóm lặp đa trị trong một cột.
- Dữ liệu lô được tách thành `book_lots` thay vì lưu danh sách lô trong `books`.

#### 2NF (Second Normal Form)
- Các bảng dùng khóa đơn (PK 1 cột), nên không có phụ thuộc bộ phận vào khóa ghép.
- Mọi thuộc tính không khóa phụ thuộc đầy đủ vào khóa chính bảng tương ứng.

#### 3NF (Third Normal Form)
- Loại bỏ phụ thuộc bắc cầu chính:
  - Danh mục không đặt trực tiếp nhiều bản sao trong bảng lô/phiếu mượn.
  - Thông tin độc giả chuẩn nằm ở `readers`.
  - Thông tin đầu sách chuẩn nằm ở `books`.
- `borrow_records.ten_doc_gia` và `borrow_records.ten_sach` là dữ liệu snapshot có chủ đích (phục vụ lịch sử), chấp nhận như một denormalization kiểm soát được.

### 4.3.2. Đánh giá mức chuẩn đề xuất
- Các bảng chính đạt 3NF.
- Một số bảng có thể xem gần BCNF vì mọi phụ thuộc hàm xác định bởi khóa chính.
- Denormalization chỉ dùng tại bảng nghiệp vụ lịch sử (`borrow_records`) để tăng tính truy vết và ổn định báo cáo theo thời điểm.

### 4.3.3. Tối ưu hóa truy vấn

#### Chỉ mục đã áp dụng
- `books(ten_sach)`, `books(tac_gia)`, `books(isbn)`
- `book_lots(ma_sach)`, `book_lots(ngay_nhap)`
- `borrow_records(ma_doc_gia)`, `borrow_records(ma_sach)`, `borrow_records(trang_thai)`

#### Chỉ mục đề xuất bổ sung
- `book_lots(ma_sach, so_luong_con)` để tối ưu chọn lô khả dụng khi mượn.
- `borrow_records(ma_doc_gia, trang_thai)` để tối ưu tra cứu đang mượn theo độc giả.
- `borrow_records(ma_lo)` để tăng tốc truy vết mượn/trả theo lô.

### 4.3.4. Tối ưu toàn vẹn và giao dịch
- Thao tác mượn/trả nên chạy trong transaction:
  - Mượn: ghi `borrow_records` + giảm `book_lots.so_luong_con`.
  - Trả: cập nhật `borrow_records` + tăng `book_lots.so_luong_con`.
- Bật `PRAGMA foreign_keys = ON` để đảm bảo FK luôn có hiệu lực.
- Đề xuất thêm trigger kiểm soát:
  - Không cho `so_luong_con` vượt `so_luong_nhap`.
  - Không cho mượn khi `so_luong_con = 0`.

### 4.3.5. Tối ưu dung lượng và mở rộng
- Tách metadata đầu sách (`books`) và dữ liệu tồn theo lô (`book_lots`) giúp giảm dư thừa.
- Khi dữ liệu tăng lớn:
  - dùng phân trang ở tầng ứng dụng;
  - áp dụng tìm kiếm có điều kiện thay vì tải toàn bộ;
  - cân nhắc tách bảng lịch sử dài hạn (archive) cho `borrow_records`.

### 4.3.6. Kết luận chuẩn hóa và tối ưu
- Thiết kế hiện tại đáp ứng yêu cầu quản lý kho sách theo lô và truy vết nhập/mượn/trả chi tiết.
- CSDL đạt mức chuẩn hóa tốt (3NF), vẫn đảm bảo hiệu năng nhờ chỉ mục và denormalization có kiểm soát.
- Mô hình đủ linh hoạt để mở rộng thêm báo cáo, kiểm kê định kỳ và tích hợp đồng bộ dữ liệu về sau.

## 4.4. Bổ sung cho đợt triển khai req.md (2.1.4, 2.1.5, 2.1.9)

- Bổ sung cột `notifications.loai_thong_bao` để phân loại thông báo (`MuonTra`, `NhacHan`, `QuaHan`, `HeThong`).
- Bổ sung bảng `inventory_sessions` để quản lý từng đợt kiểm kê.
- Bổ sung bảng `inventory_check_items` để lưu kết quả đối soát theo từng quyển sách trong đợt kiểm kê.
- Bổ sung chỉ mục:
  - `idx_notifications_loai`
  - `idx_inventory_item_dot`
- Các thay đổi đã cập nhật trong `database/library_management.sql`.
