# Danh sách UC đã làm + Biểu đồ tuần tự (khớp tài liệu ca sử dụng và mã nguồn)

## 1) Danh sách UC đã triển khai

### Chung
- UC-1: Đăng nhập

### Thủ thư
- UC-2: Quản lý danh mục sách
- UC-3: Quản lý đầu sách
- UC-4: Quản lý sách (quyển sách)
- UC-5: Xử lý trả sách
- UC-6: Duyệt mượn sách
- UC-7: Quản lý thông tin độc giả
- UC-8: Gửi thông báo
- UC-9: Tra cứu sách

### Độc giả
- UC-10: Mượn/Trả sách
- UC-11: Xem lịch sử mượn
- UC-15: Nhận thông báo

### Quản trị viên
- UC-12: Quản lý người dùng
- UC-13: Báo cáo và thống kê

### Chưa thấy triển khai
- UC-14: Gia hạn mượn

---

## UC-1: Đăng nhập
```puml
@startuml
title UC-1: Đăng nhập

actor NguoiDung as "Người dùng"
boundary ManHinhDangNhap as "Màn hình đăng nhập"
control XuLyDangNhap as "Xử lý đăng nhập"
database DanhSachTaiKhoan as "Dữ liệu tài khoản"

NguoiDung -> ManHinhDangNhap: Nhập mã người dùng/mật khẩu + nhấn Đăng nhập
ManHinhDangNhap -> XuLyDangNhap: Kiểm tra thông tin đăng nhập
XuLyDangNhap -> DanhSachTaiKhoan: Tìm tài khoản hợp lệ và đang hoạt động
DanhSachTaiKhoan --> XuLyDangNhap: Trả tài khoản hoặc rỗng

alt Đăng nhập thành công
  XuLyDangNhap --> ManHinhDangNhap: Trả thông tin người dùng
  ManHinhDangNhap --> NguoiDung: Chuyển vào màn hình theo vai trò
else Đăng nhập thất bại
  XuLyDangNhap --> ManHinhDangNhap: Trả thông báo lỗi
  ManHinhDangNhap --> NguoiDung: Hiển thị lỗi đăng nhập
end
@enduml
```

## UC-2: Quản lý danh mục sách
```puml
@startuml
title UC-2: Quản lý danh mục sách

actor ThuThu as "Thủ thư"
boundary ManHinhDanhMuc as "Màn hình quản lý danh mục"
control XuLyDanhMuc as "Xử lý danh mục"
database DuLieuDanhMuc as "Dữ liệu danh mục"

ThuThu -> ManHinhDanhMuc: Nhập thông tin + Thêm/Cập nhật/Xóa/Tìm kiếm
ManHinhDanhMuc -> XuLyDanhMuc: Gửi yêu cầu xử lý danh mục
XuLyDanhMuc -> DuLieuDanhMuc: Kiểm tra trùng mã/tên, ràng buộc đầu sách
DuLieuDanhMuc --> XuLyDanhMuc: Trả kết quả xử lý
XuLyDanhMuc --> ManHinhDanhMuc: Thành công/Thất bại + thông báo
ManHinhDanhMuc --> ThuThu: Hiển thị kết quả và tải lại danh sách
@enduml
```

## UC-3: Quản lý đầu sách
```puml
@startuml
title UC-3: Quản lý đầu sách

actor ThuThu as "Thủ thư"
boundary ManHinhDauSach as "Màn hình quản lý đầu sách"
control XuLyDauSach as "Xử lý đầu sách"
database DuLieuDauSach as "Dữ liệu đầu sách"
database DuLieuQuyenSach as "Dữ liệu quyển sách"

ThuThu -> ManHinhDauSach: Nhập dữ liệu + Thêm/Cập nhật/Xóa/Tìm kiếm
ManHinhDauSach -> XuLyDauSach: Gửi yêu cầu xử lý đầu sách
XuLyDauSach -> XuLyDauSach: Kiểm tra mã sách, ISBN, năm xuất bản, số lượng

alt Dữ liệu hợp lệ
  XuLyDauSach -> DuLieuDauSach: Lưu đầu sách
  XuLyDauSach -> DuLieuQuyenSach: Đồng bộ số lượng quyển
  XuLyDauSach --> ManHinhDauSach: Trả kết quả thành công
else Dữ liệu không hợp lệ
  XuLyDauSach --> ManHinhDauSach: Trả thông báo lỗi
end

ManHinhDauSach --> ThuThu: Hiển thị kết quả
@enduml
```

## UC-4: Quản lý sách (quyển sách)
```puml
@startuml
title UC-4: Quản lý sách (quyển)

actor ThuThu as "Thủ thư"
boundary ManHinhQuyenSach as "Màn hình quản lý quyển sách"
control XuLyQuyenSach as "Xử lý quyển sách"
database DuLieuQuyenSach as "Dữ liệu quyển sách"
database DuLieuMuonTra as "Dữ liệu mượn trả"

ThuThu -> ManHinhQuyenSach: Nhập/chọn quyển + Thêm/Cập nhật/Xóa/Tìm kiếm
ManHinhQuyenSach -> XuLyQuyenSach: Gửi yêu cầu xử lý quyển sách
XuLyQuyenSach -> DuLieuQuyenSach: Kiểm tra mã quyển, trạng thái, đầu sách liên quan
XuLyQuyenSach -> DuLieuMuonTra: Kiểm tra ràng buộc giao dịch mượn

alt Hợp lệ
  XuLyQuyenSach -> DuLieuQuyenSach: Cập nhật dữ liệu quyển
  XuLyQuyenSach --> ManHinhQuyenSach: Trả kết quả thành công
else Không hợp lệ
  XuLyQuyenSach --> ManHinhQuyenSach: Trả thông báo lỗi
end

ManHinhQuyenSach --> ThuThu: Thông báo + làm mới danh sách
@enduml
```

## UC-5: Xử lý trả sách
```puml
@startuml
title UC-5: Xử lý trả sách

actor ThuThu as "Thủ thư"
boundary ManHinhXuLyTra as "Màn hình xử lý trả sách"
control XuLyTraSach as "Xử lý trả sách"
database DuLieuMuonTra as "Dữ liệu mượn trả"
database DuLieuQuyenSach as "Dữ liệu quyển sách"

ThuThu -> ManHinhXuLyTra: Chọn phiếu mượn cần xử lý

alt Thu phạt
  ManHinhXuLyTra -> XuLyTraSach: Xác nhận thu phạt
  XuLyTraSach -> DuLieuMuonTra: Tính tiền phạt và cập nhật đã thu phạt
  XuLyTraSach --> ManHinhXuLyTra: Trả kết quả thu phạt
else Xác nhận trả sách
  ManHinhXuLyTra -> XuLyTraSach: Xác nhận trả sách
  XuLyTraSach -> DuLieuMuonTra: Kiểm tra điều kiện trả sách
  XuLyTraSach -> DuLieuMuonTra: Cập nhật trạng thái phiếu = Đã trả
  XuLyTraSach -> DuLieuQuyenSach: Cập nhật trạng thái quyển = Có sẵn
  XuLyTraSach --> ManHinhXuLyTra: Trả kết quả trả sách
end

ManHinhXuLyTra --> ThuThu: Hiển thị thông báo
@enduml
```

## UC-6: Duyệt mượn sách
```puml
@startuml
title UC-6: Duyệt mượn sách

actor ThuThu as "Thủ thư"
boundary ManHinhDuyetMuon as "Màn hình duyệt mượn"
control XuLyDuyetMuon as "Xử lý duyệt mượn"
database DuLieuYeuCau as "Dữ liệu yêu cầu mượn"
database DuLieuMuonTra as "Dữ liệu mượn trả"
database DuLieuQuyenSach as "Dữ liệu quyển sách"
database DuLieuThongBao as "Dữ liệu thông báo"

ThuThu -> ManHinhDuyetMuon: Chọn phiếu yêu cầu + Duyệt/Từ chối

alt Duyệt phiếu
  ManHinhDuyetMuon -> XuLyDuyetMuon: Duyệt phiếu yêu cầu
  XuLyDuyetMuon -> DuLieuYeuCau: Kiểm tra trạng thái chờ duyệt
  XuLyDuyetMuon -> DuLieuQuyenSach: Chọn quyển còn sẵn
  XuLyDuyetMuon -> DuLieuMuonTra: Tạo phiếu mượn
  XuLyDuyetMuon -> DuLieuYeuCau: Cập nhật trạng thái đã duyệt
  XuLyDuyetMuon -> DuLieuThongBao: Gửi thông báo cho độc giả
  XuLyDuyetMuon --> ManHinhDuyetMuon: Trả kết quả thành công
else Từ chối phiếu
  ManHinhDuyetMuon -> XuLyDuyetMuon: Từ chối phiếu yêu cầu
  XuLyDuyetMuon -> DuLieuYeuCau: Cập nhật trạng thái từ chối + lý do
  XuLyDuyetMuon -> DuLieuThongBao: Gửi thông báo từ chối
  XuLyDuyetMuon --> ManHinhDuyetMuon: Trả kết quả thành công
end

ManHinhDuyetMuon --> ThuThu: Thông báo + tải lại danh sách
@enduml
```

## UC-7: Quản lý thông tin độc giả
```puml
@startuml
title UC-7: Quản lý thông tin độc giả

actor ThuThu as "Thủ thư"
boundary ManHinhDocGia as "Màn hình quản lý độc giả"
database DuLieuDocGia as "Dữ liệu độc giả"
database DuLieuMuonTra as "Dữ liệu mượn trả"
database DuLieuYeuCau as "Dữ liệu yêu cầu mượn"

ThuThu -> ManHinhDocGia: Thêm/Sửa/Xóa độc giả

alt Thêm
  ManHinhDocGia -> DuLieuDocGia: Kiểm tra trùng mã và thêm mới
else Sửa
  ManHinhDocGia -> DuLieuDocGia: Cập nhật thông tin độc giả
else Xóa
  ManHinhDocGia -> DuLieuMuonTra: Kiểm tra lịch sử mượn trả
  ManHinhDocGia -> DuLieuYeuCau: Kiểm tra yêu cầu đang chờ duyệt
  alt Không có ràng buộc
    ManHinhDocGia -> DuLieuDocGia: Xóa độc giả
  else Có ràng buộc
    ManHinhDocGia --> ThuThu: Báo không thể xóa
  end
end

ManHinhDocGia --> ThuThu: Hiển thị kết quả
@enduml
```

## UC-8: Gửi thông báo
```puml
@startuml
title UC-8: Gửi thông báo

actor ThuThu as "Thủ thư"
boundary ManHinhThongBao as "Màn hình gửi thông báo"
control XuLyThongBao as "Xử lý thông báo"
database DuLieuThongBao as "Dữ liệu thông báo"

ThuThu -> ManHinhThongBao: Chọn độc giả + nhập nội dung + nhấn Gửi
ManHinhThongBao -> XuLyThongBao: Gửi thông báo cho độc giả
XuLyThongBao -> DuLieuThongBao: Tạo mã thông báo và lưu dữ liệu
DuLieuThongBao --> XuLyThongBao: Xác nhận lưu thành công
XuLyThongBao --> ManHinhThongBao: Trả kết quả gửi
ManHinhThongBao --> ThuThu: Báo gửi thành công
@enduml
```

## UC-9: Tra cứu sách
```puml
@startuml
title UC-9: Tra cứu sách

actor NguoiDung as "Thủ thư/Độc giả"
boundary ManHinhTraCuu as "Màn hình tra cứu sách"
control XuLyTraCuu as "Xử lý tra cứu sách"
database DuLieuDauSach as "Dữ liệu đầu sách"
database DuLieuQuyenSach as "Dữ liệu quyển sách"

NguoiDung -> ManHinhTraCuu: Nhập tiêu chí (tác giả, nhan đề, chủ đề, mã sách)
ManHinhTraCuu -> XuLyTraCuu: Gửi yêu cầu tra cứu
XuLyTraCuu -> DuLieuDauSach: Lọc dữ liệu theo các tiêu chí
DuLieuDauSach --> XuLyTraCuu: Trả danh sách đầu sách
XuLyTraCuu -> DuLieuQuyenSach: Tổng hợp số quyển và trạng thái hiện có
XuLyTraCuu --> ManHinhTraCuu: Trả kết quả tra cứu
ManHinhTraCuu --> NguoiDung: Hiển thị danh sách hoặc thông báo không tìm thấy
@enduml
```

## UC-10: Mượn/Trả sách (Độc giả)
```puml
@startuml
title UC-10: Mượn/Trả sách (Độc giả)

actor DocGia as "Độc giả"
boundary ManHinhMuonTra as "Màn hình mượn/trả sách"
control XuLyMuonTra as "Xử lý mượn trả"
database DuLieuYeuCau as "Dữ liệu yêu cầu mượn"
database DuLieuMuonTra as "Dữ liệu mượn trả"

DocGia -> ManHinhMuonTra: Chọn sách + nhập ngày mượn/số ngày

alt Lập phiếu mượn
  ManHinhMuonTra -> XuLyMuonTra: Tạo yêu cầu mượn
  XuLyMuonTra -> DuLieuYeuCau: Kiểm tra và tạo yêu cầu chờ duyệt
  XuLyMuonTra --> ManHinhMuonTra: Trả kết quả lập phiếu
else Trả sách
  ManHinhMuonTra -> XuLyMuonTra: Gửi yêu cầu trả sách
  XuLyMuonTra -> DuLieuMuonTra: Tìm phiếu mượn hợp lệ và kiểm tra điều kiện
  XuLyMuonTra --> ManHinhMuonTra: Trả kết quả trả sách
end

ManHinhMuonTra --> DocGia: Hiển thị thông báo xử lý
@enduml
```

## UC-11: Xem lịch sử mượn
```puml
@startuml
title UC-11: Xem lịch sử mượn

actor DocGia as "Độc giả"
boundary ManHinhLichSu as "Màn hình lịch sử mượn"
database DuLieuMuonTra as "Dữ liệu mượn trả"

DocGia -> ManHinhLichSu: Mở màn hình lịch sử mượn
ManHinhLichSu -> DuLieuMuonTra: Lấy phiếu theo mã độc giả hiện tại
DuLieuMuonTra --> ManHinhLichSu: Trả danh sách phiếu mượn/trả
ManHinhLichSu -> ManHinhLichSu: Sắp xếp theo ngày mượn giảm dần
ManHinhLichSu --> DocGia: Hiển thị lịch sử và trạng thái
@enduml
```

## UC-12: Quản lý người dùng
```puml
@startuml
title UC-12: Quản lý người dùng

actor QuanTriVien as "Quản trị viên"
boundary ManHinhTaiKhoan as "Màn hình quản lý tài khoản"
control XuLyTaiKhoan as "Xử lý tài khoản"
database DuLieuTaiKhoan as "Dữ liệu tài khoản"

QuanTriVien -> ManHinhTaiKhoan: Thêm/Sửa/Xóa/Khóa-Mở/Đổi mật khẩu

alt Thêm tài khoản
  ManHinhTaiKhoan -> XuLyTaiKhoan: Kiểm tra trùng tên đăng nhập
  XuLyTaiKhoan -> DuLieuTaiKhoan: Thêm tài khoản mới
else Sửa tài khoản
  ManHinhTaiKhoan -> DuLieuTaiKhoan: Cập nhật thông tin tài khoản
else Xóa tài khoản
  ManHinhTaiKhoan -> DuLieuTaiKhoan: Xóa tài khoản
else Khóa/Mở tài khoản
  ManHinhTaiKhoan -> DuLieuTaiKhoan: Cập nhật trạng thái hoạt động
else Đổi mật khẩu
  ManHinhTaiKhoan -> DuLieuTaiKhoan: Cập nhật mật khẩu
end

ManHinhTaiKhoan --> QuanTriVien: Thông báo + tải lại danh sách
@enduml
```

## UC-13: Báo cáo và thống kê
```puml
@startuml
title UC-13: Báo cáo và thống kê

actor QuanTriVien as "Quản trị viên"
boundary ManHinhBaoCao as "Màn hình báo cáo"
control XuLyBaoCao as "Xử lý báo cáo"
database DuLieuDauSach as "Dữ liệu đầu sách"
database DuLieuMuonTra as "Dữ liệu mượn trả"

QuanTriVien -> ManHinhBaoCao: Mở chức năng báo cáo
ManHinhBaoCao -> DuLieuDauSach: Lấy danh sách đầu sách
ManHinhBaoCao -> XuLyBaoCao: Đồng bộ trạng thái sách
ManHinhBaoCao -> DuLieuMuonTra: Tổng hợp số lượt mượn theo mã sách
DuLieuMuonTra --> ManHinhBaoCao: Trả dữ liệu thống kê
ManHinhBaoCao --> QuanTriVien: Hiển thị báo cáo tồn kho và mượn nhiều
@enduml
```

## UC-15: Nhận thông báo
```puml
@startuml
title UC-15: Nhận thông báo

actor DocGia as "Độc giả"
boundary ManHinhNhanThongBao as "Màn hình thông báo"
database DuLieuThongBao as "Dữ liệu thông báo"

DocGia -> ManHinhNhanThongBao: Mở mục Thông báo
ManHinhNhanThongBao -> DuLieuThongBao: Lấy thông báo theo mã độc giả
DuLieuThongBao --> ManHinhNhanThongBao: Trả danh sách thông báo
ManHinhNhanThongBao --> DocGia: Hiển thị thông báo + số chưa đọc

opt Đánh dấu đã đọc tất cả
  DocGia -> ManHinhNhanThongBao: Nhấn Đánh dấu đã đọc tất cả
  ManHinhNhanThongBao -> DuLieuThongBao: Cập nhật trạng thái đã đọc
  ManHinhNhanThongBao --> DocGia: Làm mới danh sách
end
@enduml
```

---

## Ghi chú đối chiếu
- Nội dung biểu đồ đã Việt hóa phần hiển thị.
- Một số quy tắc trong tài liệu ca sử dụng chưa thấy hiện thực đầy đủ trong mã nguồn hiện tại (ví dụ: khóa tài khoản sau nhiều lần nhập sai, gia hạn mượn UC-14).
