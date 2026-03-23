# Danh sách UC đã làm + Biểu đồ tuần tự

## 1) Danh sách UC đã triển khai

### Thủ thư
- UC-2: Quản lý đầu sách
- UC-3: Tìm kiếm phiếu mượn quá hạn
- UC-4: Xác nhận thu phạt
- UC-5: Xác nhận trả sách
- UC-9: Tra cứu sách

### Độc giả
- UC-12: Lập phiếu mượn
- UC-13: Trả sách
- UC-16: Xem lịch sử mượn

### Quản trị viên
- UC-18: Quản lý người dùng
- UC-19: Báo cáo và thống kê
- UC-17: Quản lý hệ thống (mức khung: truy cập các module quản trị)

---

## UC-2: Quản lý đầu sách (Thủ thư)
```mermaid
sequenceDiagram
    actor ThuThu as Thủ thư
    participant UI as BookTitlePanel
    participant SVC as LibraryDataService
    participant DATA as SampleData

    ThuThu->>UI: Nhập thông tin sách + Thêm/Sửa/Xóa
    UI->>SVC: SaveBook()/DeleteBook()
    SVC->>SVC: Validate mã sách, ISBN, năm XB
    alt Dữ liệu hợp lệ
        SVC->>DATA: Cập nhật Books/BookLots
        SVC-->>UI: Success
        UI-->>ThuThu: Thông báo thành công + reload danh sách
    else Dữ liệu không hợp lệ
        SVC-->>UI: Error message
        UI-->>ThuThu: Hiển thị lỗi
    end
```

## UC-3: Tìm kiếm phiếu mượn quá hạn (Thủ thư)
```mermaid
sequenceDiagram
    actor ThuThu as Thủ thư
    participant UI as OverduePanel
    participant DATA as BorrowRecords

    ThuThu->>UI: Nhập bộ lọc (Mã ĐG, Tên ĐG, Mã sách, Mã lô, min ngày trễ)
    UI->>DATA: Lọc phiếu quá hạn theo AND
    UI->>UI: Tính số ngày trễ + tiền phạt
    UI->>UI: Sắp xếp giảm dần theo số ngày trễ
    UI-->>ThuThu: Hiển thị danh sách quá hạn
```

## UC-4: Xác nhận thu phạt (Thủ thư)
```mermaid
sequenceDiagram
    actor ThuThu as Thủ thư
    participant UI as OverduePanel
    participant SVC as LibraryDataService
    participant DATA as BorrowRecords

    ThuThu->>UI: Chọn phiếu quá hạn + Xác nhận thu phạt
    UI->>SVC: CollectFine(maMuon)
    SVC->>DATA: Tìm BorrowRecord
    SVC->>SVC: Tính phạt = ngày trễ * đơn giá
    SVC->>DATA: Cập nhật TienPhat + DaThuPhat=true
    SVC-->>UI: Success/Fail
    UI-->>ThuThu: Thông báo kết quả + refresh bảng
```

## UC-5: Xác nhận trả sách (Thủ thư)
```mermaid
sequenceDiagram
    actor ThuThu as Thủ thư
    participant UI as OverduePanel/BorrowReturnPanel
    participant SVC as LibraryDataService
    participant DATA as BorrowRecords + BookLots

    ThuThu->>UI: Chọn phiếu/sách + Xác nhận trả
    UI->>SVC: ReturnBook()/CompleteReturn()
    SVC->>SVC: Kiểm tra phiếu đang mượn
    SVC->>SVC: Nếu quá hạn thì phải DaThuPhat=true
    alt Đủ điều kiện
        SVC->>DATA: Cập nhật phiếu => Đã trả
        SVC->>DATA: Tăng tồn lô tương ứng
        SVC-->>UI: Success
        UI-->>ThuThu: Trả sách thành công
    else Chưa đủ điều kiện
        SVC-->>UI: Lỗi chưa thu phạt
        UI-->>ThuThu: Hiển thị cảnh báo
    end
```

## UC-9: Tra cứu sách (Thủ thư)
```mermaid
sequenceDiagram
    actor ThuThu as Thủ thư
    participant UI as SearchBookPanel
    participant SVC as LibraryDataService
    participant DATA as Books + BookLots

    ThuThu->>UI: Nhập tiêu chí (tác giả, nhan đề, chủ đề, mã sách)
    UI->>SVC: SearchBooks(...)
    SVC->>DATA: Truy vấn danh sách phù hợp
    SVC->>SVC: Tính tồn hiện có, số lô
    SVC-->>UI: Kết quả tìm kiếm
    UI-->>ThuThu: Hiển thị danh sách sách phù hợp
```

## UC-12: Lập phiếu mượn (Độc giả)
```mermaid
sequenceDiagram
    actor DocGia as Độc giả
    participant UI as BorrowReturnPanel
    participant SVC as LibraryDataService
    participant DATA as Books + BookLots + BorrowRecords

    DocGia->>UI: Chọn sách/lô + nhập số ngày mượn
    UI->>SVC: BorrowBook(maSach, maDocGia, ...)
    SVC->>DATA: Kiểm tra tồn kho theo lô
    alt Còn sách
        SVC->>DATA: Trừ tồn lô
        SVC->>DATA: Tạo BorrowRecord (Đang mượn)
        SVC-->>UI: Thành công + mã lô xuất
        UI-->>DocGia: Hiển thị hạn trả
    else Hết sách
        SVC-->>UI: Báo lỗi không còn khả dụng
        UI-->>DocGia: Hiển thị lỗi
    end
```

## UC-13: Trả sách (Độc giả)
```mermaid
sequenceDiagram
    actor DocGia as Độc giả
    participant UI as BorrowReturnPanel
    participant SVC as LibraryDataService
    participant DATA as BorrowRecords + BookLots

    DocGia->>UI: Chọn sách đang mượn + Trả sách
    UI->>SVC: ReturnBook(maSach, maDocGia, now)
    SVC->>DATA: Tìm phiếu đang mượn gần nhất
    SVC->>DATA: Cập nhật phiếu => Đã trả
    SVC->>DATA: Hoàn kho về lô
    SVC-->>UI: Kết quả
    UI-->>DocGia: Thông báo trả sách
```

## UC-16: Xem lịch sử mượn (Độc giả)
```mermaid
sequenceDiagram
    actor DocGia as Độc giả
    participant UI as BorrowHistoryPanel
    participant DATA as BorrowRecords

    DocGia->>UI: Mở màn hình lịch sử mượn
    UI->>DATA: Lấy phiếu mượn theo mã độc giả
    UI->>UI: Sắp xếp theo ngày mượn, gắn trạng thái
    UI-->>DocGia: Hiển thị lịch sử đầy đủ
```

## UC-18: Quản lý người dùng (Admin)
```mermaid
sequenceDiagram
    actor Admin
    participant UI as AccountPanel
    participant STORE as UserStore

    Admin->>UI: Thêm/Sửa/Xóa/Khóa tài khoản
    UI->>STORE: Validate + cập nhật Users
    alt Hợp lệ
        STORE-->>UI: Success
        UI-->>Admin: Danh sách tài khoản cập nhật
    else Không hợp lệ
        STORE-->>UI: Error
        UI-->>Admin: Thông báo lỗi
    end
```

## UC-19: Báo cáo và thống kê (Admin)
```mermaid
sequenceDiagram
    actor Admin
    participant UI as ReportPanel
    participant DATA as Books + BorrowRecords

    Admin->>UI: Mở báo cáo thống kê
    UI->>DATA: Lấy dữ liệu sách, mượn/trả
    UI->>UI: Tổng hợp số liệu tồn kho, lượt mượn
    UI-->>Admin: Hiển thị bảng thống kê
```

## UC-17: Quản lý hệ thống (Admin - mức khung)
```mermaid
sequenceDiagram
    actor Admin
    participant UI as AdminForm
    participant M1 as AccountPanel
    participant M2 as RolePanel
    participant M3 as SettingsPanel
    participant M4 as BackupPanel
    participant M5 as ActivityLogPanel

    Admin->>UI: Đăng nhập quyền quản trị
    UI-->>Admin: Hiển thị dashboard quản trị
    Admin->>UI: Chọn module quản lý
    UI->>M1: Quản lý tài khoản
    UI->>M2: Phân quyền
    UI->>M3: Cấu hình hệ thống
    UI->>M4: Sao lưu/Phục hồi
    UI->>M5: Nhật ký hoạt động
```
