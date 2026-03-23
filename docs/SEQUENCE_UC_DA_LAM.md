# Danh sách UC đã làm + Biểu đồ tuần tự

## 1) Danh sách UC đã triển khai

### Chung
- UC-1: Đăng nhập

### Thủ thư
- UC-2: Quản lý sách
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
- UC-17: Quản lý hệ thống (mức khung: truy cập các phân hệ quản trị)

---

## UC-1: Đăng nhập (Người dùng)
```puml
@startuml
title UC-1: Đăng nhập

actor NguoiDung as "Người dùng"
boundary LoginUI as "Màn hình Đăng nhập"
control AuthService as "Dịch vụ Xác thực"
database UserStore as "Cơ sở dữ liệu"

NguoiDung -> LoginUI: Nhập tên đăng nhập + mật khẩu\nNhấn Đăng nhập
LoginUI -> AuthService: Xác thực(tên đăng nhập, mật khẩu)
AuthService -> UserStore: Tìm tài khoản theo tên đăng nhập
UserStore --> AuthService: Trả thông tin tài khoản + mật khẩu đã băm
AuthService -> AuthService: So khớp mật khẩu và kiểm tra trạng thái

alt Hợp lệ
  AuthService --> LoginUI: Thành công + vai trò/phiên đăng nhập
  LoginUI --> NguoiDung: Chuyển vào màn hình theo quyền
else Không hợp lệ / bị khóa
  AuthService --> LoginUI: Thông báo lỗi
  LoginUI --> NguoiDung: Hiển thị lỗi đăng nhập
end
@enduml
```

## UC-2: Quản lý sách (Thủ thư)
```puml
@startuml
title UC-2: Quản lý sách (Thủ thư)

actor ThuThu as "Thủ thư"
boundary BookManagementUI as "Màn hình Quản lý sách"
control BookService as "Dịch vụ Hệ thống thư viện"
database LibraryDB as "Cơ sở dữ liệu"

ThuThu -> BookManagementUI: Nhập thông tin đầu sách (PTTK)\nvà thông tin sách (quyển)\nChọn Thêm/Sửa/Xóa
BookManagementUI -> BookService: Lưu đầu sách()/Lưu sách()/Xóa sách()
BookService -> BookService: Kiểm tra mã đầu sách, ISBN, năm xuất bản,\nmã sách (quyển), ngày nhập, trạng thái

alt Dữ liệu hợp lệ
  BookService -> LibraryDB: Cập nhật dữ liệu đầu sách
  BookService -> LibraryDB: Cập nhật dữ liệu sách (mỗi quyển có mã riêng,\nthuộc 1 đầu sách, có ngày nhập/trạng thái)
  BookService --> BookManagementUI: Thành công
  BookManagementUI --> ThuThu: Thông báo thành công + tải lại danh sách
else Dữ liệu không hợp lệ
  BookService --> BookManagementUI: Thông báo lỗi
  BookManagementUI --> ThuThu: Hiển thị lỗi nhập liệu
end
@enduml
```

## UC-3: Tìm kiếm phiếu mượn quá hạn (Thủ thư)
```puml
@startuml
title UC-3: Tìm kiếm phiếu mượn quá hạn (Thủ thư)

actor ThuThu as "Thủ thư"
boundary OverdueUI as "Màn hình Phiếu quá hạn"
control OverdueService as "Dịch vụ Tra cứu quá hạn"
database BorrowDB as "Cơ sở dữ liệu"

ThuThu -> OverdueUI: Nhập bộ lọc\n(Mã ĐG, Tên ĐG, Mã đầu sách, Mã sách (quyển), min ngày trễ)
OverdueUI -> OverdueService: Tra cứu quá hạn(bộ lọc)
OverdueService -> BorrowDB: Truy vấn phiếu mượn theo điều kiện
BorrowDB --> OverdueService: Danh sách phiếu phù hợp
OverdueService -> OverdueService: Tính số ngày trễ + tiền phạt\nSắp xếp giảm dần theo số ngày trễ
OverdueService --> OverdueUI: Kết quả quá hạn
OverdueUI --> ThuThu: Hiển thị danh sách phiếu quá hạn
@enduml
```

## UC-4: Xác nhận thu phạt (Thủ thư)
```puml
@startuml
title UC-4: Xác nhận thu phạt (Thủ thư)

actor ThuThu as "Thủ thư"
boundary OverdueUI as "Màn hình Phiếu quá hạn"
control FineService as "Dịch vụ Hệ thống thư viện"
database BorrowDB as "Cơ sở dữ liệu"

ThuThu -> OverdueUI: Chọn phiếu quá hạn\nNhấn Xác nhận thu phạt
OverdueUI -> FineService: Thu phạt(mã mượn)
FineService -> BorrowDB: Tìm phiếu mượn theo mã mượn
BorrowDB --> FineService: Thông tin phiếu mượn
FineService -> FineService: Tính tiền phạt = số ngày trễ * đơn giá
FineService -> BorrowDB: Cập nhật tiền phạt + trạng thái đã thu phạt

alt Cập nhật thành công
  FineService --> OverdueUI: Thành công
  OverdueUI --> ThuThu: Thông báo thu phạt thành công
else Cập nhật thất bại
  FineService --> OverdueUI: Thông báo lỗi
  OverdueUI --> ThuThu: Thông báo lỗi thu phạt
end
@enduml
```

## UC-5: Xác nhận trả sách (Thủ thư)
```puml
@startuml
title UC-5: Xác nhận trả sách (Thủ thư)

actor ThuThu as "Thủ thư"
boundary ReturnUI as "Màn hình Phiếu quá hạn/Mượn-Trả sách"
control ReturnService as "Dịch vụ Hệ thống thư viện"
database BorrowDB as "Cơ sở dữ liệu"
database BookCopyDB as "Cơ sở dữ liệu"

ThuThu -> ReturnUI: Chọn phiếu/sách\nNhấn Xác nhận trả
ReturnUI -> ReturnService: Xử lý trả sách()/Hoàn tất trả()
ReturnService -> BorrowDB: Kiểm tra phiếu đang mượn
BorrowDB --> ReturnService: Thông tin phiếu mượn hiện tại
ReturnService -> ReturnService: Kiểm tra điều kiện thu phạt nếu quá hạn

alt Đủ điều kiện trả
  ReturnService -> BorrowDB: Cập nhật trạng thái phiếu = Đã trả
  ReturnService -> BookCopyDB: Cập nhật trạng thái sách (quyển) = Có sẵn
  ReturnService --> ReturnUI: Thành công
  ReturnUI --> ThuThu: Trả sách thành công
else Chưa đủ điều kiện
  ReturnService --> ReturnUI: Lỗi chưa thu phạt
  ReturnUI --> ThuThu: Hiển thị cảnh báo
end
@enduml
```

## UC-9: Tra cứu sách (Thủ thư)
```puml
@startuml
title UC-6: Tra cứu sách (Thủ thư)

actor ThuThu as "Thủ thư"
boundary SearchUI as "Màn hình Tra cứu sách"
control BookSearchService as "Dịch vụ Hệ thống thư viện"
database LibraryDB as "Cơ sở dữ liệu"

ThuThu -> SearchUI: Nhập tiêu chí tra cứu\n(tác giả, nhan đề, chủ đề, mã sách)
SearchUI -> BookSearchService: Tra cứu sách(tiêu chí)
BookSearchService -> LibraryDB: Truy vấn danh sách sách phù hợp
LibraryDB --> BookSearchService: Danh sách đầu sách + danh sách sách (quyển)
BookSearchService -> BookSearchService: Tính số lượng có sẵn theo trạng thái từng quyển
BookSearchService --> SearchUI: Trả kết quả tìm kiếm
SearchUI --> ThuThu: Hiển thị danh sách sách phù hợp
@enduml
```

## UC-12: Lập phiếu mượn (Độc giả)
```puml
@startuml
title UC-7: Lập phiếu mượn (Độc giả)

actor DocGia as "Độc giả"
boundary BorrowUI as "Màn hình Mượn/Trả sách"
control BorrowService as "Dịch vụ Hệ thống thư viện"
database LibraryDB as "Cơ sở dữ liệu"
database BorrowDB as "Cơ sở dữ liệu"

DocGia -> BorrowUI: Chọn đầu sách/quyển sách\nNhập số ngày mượn
BorrowUI -> BorrowService: Lập phiếu mượn(mã đầu sách, mã sách (quyển), mã độc giả, ...)
BorrowService -> LibraryDB: Kiểm tra trạng thái sẵn sàng của quyển sách
LibraryDB --> BorrowService: Thông tin quyển sách hiện tại

alt Còn sách
  BorrowService -> LibraryDB: Cập nhật trạng thái sách (quyển) = Đang mượn
  BorrowService -> BorrowDB: Tạo phiếu mượn (Đang mượn)
  BorrowService --> BorrowUI: Thành công + mã sách (quyển)
  BorrowUI --> DocGia: Hiển thị hạn trả
else Hết sách
  BorrowService --> BorrowUI: Báo lỗi không còn khả dụng
  BorrowUI --> DocGia: Hiển thị lỗi
end
@enduml
```

## UC-13: Trả sách (Độc giả)
```puml
@startuml
title UC-8: Trả sách (Độc giả)

actor DocGia as "Độc giả"
boundary ReturnUI as "Màn hình Mượn/Trả sách"
control ReturnService as "Dịch vụ Hệ thống thư viện"
database BorrowDB as "Cơ sở dữ liệu"
database BookCopyDB as "Cơ sở dữ liệu"

DocGia -> ReturnUI: Chọn sách đang mượn\nNhấn Trả sách
ReturnUI -> ReturnService: Trả sách(mã sách, mã độc giả, hiện tại)
ReturnService -> BorrowDB: Tìm phiếu đang mượn gần nhất
BorrowDB --> ReturnService: Phiếu mượn hợp lệ
ReturnService -> BorrowDB: Cập nhật trạng thái phiếu = Đã trả
ReturnService -> BookCopyDB: Cập nhật trạng thái sách (quyển) = Có sẵn
ReturnService --> ReturnUI: Kết quả xử lý
ReturnUI --> DocGia: Thông báo trả sách
@enduml
```

## UC-16: Xem lịch sử mượn (Độc giả)
```puml
@startuml
title UC-9: Xem lịch sử mượn (Độc giả)

actor DocGia as "Độc giả"
boundary HistoryUI as "Màn hình Lịch sử mượn"
control HistoryService as "Dịch vụ Lịch sử mượn"
database BorrowDB as "Cơ sở dữ liệu"

DocGia -> HistoryUI: Mở màn hình lịch sử mượn
HistoryUI -> HistoryService: Lấy lịch sử mượn(mã độc giả)
HistoryService -> BorrowDB: Lấy danh sách phiếu mượn của độc giả
BorrowDB --> HistoryService: Danh sách mượn/trả
HistoryService -> HistoryService: Sắp xếp theo ngày mượn\nGắn trạng thái từng phiếu
HistoryService --> HistoryUI: Dữ liệu lịch sử đã xử lý
HistoryUI --> DocGia: Hiển thị lịch sử đầy đủ
@enduml
```

## UC-18: Quản lý người dùng (Admin)
```puml
@startuml
title UC-10: Quản lý người dùng (Admin)

actor Admin
boundary AccountUI as "Màn hình Quản lý người dùng"
control UserService as "Dịch vụ Quản lý người dùng"
database UserStore as "Cơ sở dữ liệu"

Admin -> AccountUI: Thêm/Sửa/Xóa/Khóa tài khoản
AccountUI -> UserService: Lưu người dùng()/Xóa người dùng()/Khóa người dùng()
UserService -> UserService: Kiểm tra dữ liệu tài khoản

alt Hợp lệ
  UserService -> UserStore: Cập nhật dữ liệu người dùng
  UserService --> AccountUI: Thành công
  AccountUI --> Admin: Danh sách tài khoản được cập nhật
else Không hợp lệ
  UserService --> AccountUI: Thông báo lỗi
  AccountUI --> Admin: Hiển thị lỗi
end
@enduml
```

## UC-19: Báo cáo và thống kê (Admin)
```puml
@startuml
title UC-11: Báo cáo và thống kê (Admin)

actor Admin
boundary ReportUI as "Màn hình Báo cáo"
control ReportService as "Dịch vụ Báo cáo"
database AnalyticsDB as "Cơ sở dữ liệu"

Admin -> ReportUI: Mở màn hình báo cáo thống kê
ReportUI -> ReportService: Tạo báo cáo()
ReportService -> AnalyticsDB: Lấy dữ liệu sách và mượn/trả
AnalyticsDB --> ReportService: Dữ liệu thô
ReportService -> ReportService: Tổng hợp tồn kho, lượt mượn, chỉ số liên quan
ReportService --> ReportUI: Bộ số liệu thống kê
ReportUI --> Admin: Hiển thị bảng/biểu báo cáo
@enduml
```

## UC-17: Quản lý hệ thống (Admin - mức khung)
```puml
@startuml
title UC-12: Quản lý hệ thống (Admin - mức khung)

actor Admin
boundary AdminUI as "Màn hình Quản trị"
control SystemRouter as "Bộ điều hướng hệ thống"
boundary AccountModule as "Màn hình Quản lý người dùng"
boundary RoleModule as "Màn hình Phân quyền"
boundary SettingsModule as "Màn hình Cấu hình hệ thống"
boundary BackupModule as "Màn hình Sao lưu/Phục hồi"
boundary ActivityLogModule as "Màn hình Nhật ký hoạt động"

Admin -> AdminUI: Đăng nhập quyền quản trị
AdminUI --> Admin: Hiển thị bảng điều khiển quản trị
Admin -> AdminUI: Chọn phân hệ quản lý
AdminUI -> SystemRouter: Điều hướng phân hệ(phân hệ)

alt Quản lý tài khoản
  SystemRouter -> AccountModule: Mở phân hệ Quản lý người dùng
else Phân quyền
  SystemRouter -> RoleModule: Mở phân hệ Phân quyền
else Cấu hình hệ thống
  SystemRouter -> SettingsModule: Mở phân hệ Cấu hình hệ thống
else Sao lưu/Phục hồi
  SystemRouter -> BackupModule: Mở phân hệ Sao lưu/Phục hồi
else Nhật ký hoạt động
  SystemRouter -> ActivityLogModule: Mở phân hệ Nhật ký hoạt động
end
@enduml
```
