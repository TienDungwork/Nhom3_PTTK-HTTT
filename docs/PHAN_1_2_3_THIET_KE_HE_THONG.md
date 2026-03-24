# PHẦN 1. GIỚI THIỆU TỔNG QUAN HỆ THỐNG

## 1.1. Mô tả chung về bài toán
Hệ thống quản lý thư viện được phân tích theo mô hình hướng đối tượng với các phân hệ chính: quản lý người dùng, quản lý sách, mượn/trả, và quản trị hệ thống.

Bài toán cần giải quyết:
- Quản lý nhiều loại người dùng: quản trị viên, thủ thư, độc giả.
- Quản lý danh mục sách, đầu sách, từng quyển sách vật lý.
- Quản lý quy trình đặt mượn, duyệt đặt mượn, chuyển sang phiếu mượn thực tế.
- Theo dõi trạng thái mượn trả theo từng quyển và tính tiền phạt.
- Ghi nhận nhật ký hệ thống, thông báo và cấu hình vận hành.

## 1.2. Phạm vi nghiệp vụ
- Nhóm người dùng (`NguoiDung`, `ThuThu`, `DocGia`, `QuanTriVien`).
- Nhóm quản lý sách (`DanhMucSach`, `DauSach`, `QuyenSach`).
- Nhóm mượn trả (`PhieuDatMuon`, `PhieuMuon`, `PhieuMuonChiTiet`).
- Nhóm hệ thống (`ThongBao`, `NhatKyHeThong`, `CauHinhHeThong`).

## 1.3. Mục tiêu hệ thống
- Chuẩn hóa dữ liệu nghiệp vụ thư viện.
- Đảm bảo toàn vẹn trạng thái quyển sách theo luồng mượn/trả.
- Tách vai trò người dùng rõ ràng để phân quyền xử lý.
- Hỗ trợ mở rộng báo cáo, nhắc hạn, kiểm soát vận hành.

---

# PHẦN 2. THIẾT KẾ CHI TIẾT LỚP ĐỐI TƯỢNG

## 2.1. Danh sách lớp theo biểu đồ lớp

### 2.1.1. Package `NguoiDung`
- `NguoiDung` (abstract)
- `ThuThu`
- `DocGia`
- `QuanTriVien`
- `VaiTro` (enum)

### 2.1.2. Package `QuanLySach`
- `DanhMucSach`
- `DauSach`
- `QuyenSach`
- `TrangThaiQuyenSach` (enum)

### 2.1.3. Package `MuonTra`
- `PhieuDatMuon`
- `PhieuMuon`
- `PhieuMuonChiTiet`
- `TrangThaiPhieuDat` (enum)
- `TrangThaiPhieuMuon` (enum)
- `TrangThaiChiTiet` (enum)

### 2.1.4. Package `HeThong`
- `ThongBao`
- `NhatKyHeThong`
- `CauHinhHeThong`

## 2.2. Thuộc tính và phương thức từng lớp

### 2.2.1. Lớp `NguoiDung` (abstract)
Thuộc tính:
- `maTK: String`
- `hoTen: String`
- `email: String`
- `soDienThoai: String`
- `tenDangNhap: String`
- `matKhau: String`
- `isActive: boolean`
- `vaiTro: VaiTro`

Phương thức:
- `dangNhap(tenDangNhap, matKhau): boolean`
- `doiMatKhau(matKhauCu, matKhauMoi): boolean`

### 2.2.2. Lớp `ThuThu` kế thừa `NguoiDung`
Thuộc tính:
- `maThuThu: String`

Phương thức:
- `duyetPhieuDat(maPhieuDat): boolean`
- `lapPhieuMuon(docGia, danhSachQuyen): PhieuMuon`
- `xacNhanTraSach(maPhieuMuon): boolean`

### 2.2.3. Lớp `DocGia` kế thừa `NguoiDung`
Thuộc tính:
- `maDocGia: String`
- `diaChi: String`
- `ngayDangKy: Date`

Phương thức:
- `taoPhieuDat(danhSachSach): PhieuDatMuon`
- `xemLichSuMuon(): List<PhieuMuon>`

### 2.2.4. Lớp `QuanTriVien` kế thừa `NguoiDung`
Thuộc tính:
- `maQuanTri: String`

Phương thức:
- `quanLyTaiKhoan(): void`
- `capNhatCauHinh(cauHinhMoi): boolean`
- `xemNhatKyHeThong(): List<NhatKyHeThong>`

### 2.2.5. Lớp `DanhMucSach`
Thuộc tính:
- `maDanhMuc: String`
- `tenDanhMuc: String`
- `moTa: String`
- `viTriKe: String`
- `dangSuDung: boolean`

Phương thức:
- `themDauSach(dauSach): boolean`
- `capNhatDanhMuc(): boolean`

### 2.2.6. Lớp `DauSach`
Thuộc tính:
- `maDauSach: String`
- `tenSach: String`
- `tacGia: String`
- `chuDe: String`
- `nhaXuatBan: String`
- `namXuatBan: int`
- `isbn: String`
- `viTriKho: String`
- `trangThai: String`

Phương thức:
- `layDanhSachQuyen(): List<QuyenSach>`
- `capNhatThongTin(): boolean`

### 2.2.7. Lớp `QuyenSach`
Thuộc tính:
- `maQuyenSach: String`
- `ngayNhap: Date`
- `tinhTrang: String`
- `trangThai: TrangThaiQuyenSach`
- `viTri: String`

Phương thức:
- `capNhatTrangThai(trangThaiMoi): boolean`
- `isSanSangMuon(): boolean`

### 2.2.8. Lớp `PhieuDatMuon`
Thuộc tính:
- `maPhieuDat: String`
- `ngayDat: Date`
- `trangThai: TrangThaiPhieuDat`

Phương thức:
- `chuyenThanhPhieuMuon(): PhieuMuon`
- `huyPhieuDat(): boolean`

### 2.2.9. Lớp `PhieuMuon`
Thuộc tính:
- `maPhieuMuon: String`
- `ngayMuon: Date`
- `hanTra: Date`
- `trangThai: TrangThaiPhieuMuon`
- `tienPhat: double`
- `daThuPhat: boolean`

Phương thức:
- `tinhTienPhat(ngayTra): double`
- `xacNhanThuPhat(): boolean`
- `dongPhieuMuon(): boolean`

### 2.2.10. Lớp `PhieuMuonChiTiet`
Thuộc tính:
- `soLuong: int`
- `ngayTraThuc: Date`
- `trangThai: TrangThaiChiTiet`

Phương thức:
- `xacNhanTra(ngayTra): boolean`

### 2.2.11. Lớp `ThongBao`
Thuộc tính:
- `maThongBao: String`
- `noiDung: String`
- `ngayGui: Date`
- `trangThai: String`

Phương thức:
- `danhDauDaDoc(): void`

### 2.2.12. Lớp `NhatKyHeThong`
Thuộc tính:
- `maNhatKy: String`
- `hanhDong: String`
- `thoiGian: Date`
- `chiTiet: String`

Phương thức:
- `ghiLog(hanhDong, chiTiet): void`

### 2.2.13. Lớp `CauHinhHeThong`
Thuộc tính:
- `soNgayMuonToiDa: int`
- `soSachMuonToiDa: int`
- `phiPhatMoiNgay: double`

Phương thức:
- `capNhatCauHinh(soNgayMuon, soSachMuon, phiPhat): boolean`

## 2.3. Đặc tả các phương thức phức tạp

### 2.3.1. `PhieuDatMuon.chuyenThanhPhieuMuon(): PhieuMuon`
Mục đích: chuyển từ yêu cầu đặt mượn sang phiếu mượn thực tế.

Tiền điều kiện:
- `trangThai = ChoDuyet` hoặc `DaDuyet`.
- Có ít nhất một `QuyenSach` sẵn sàng mượn.

Luồng xử lý:
1. Kiểm tra trạng thái phiếu đặt.
2. Kiểm tra số lượng quyển sách khả dụng.
3. Tạo `PhieuMuon` và các `PhieuMuonChiTiet` tương ứng.
4. Cập nhật trạng thái phiếu đặt thành `DaChuyenPhieuMuon`.
5. Chuyển trạng thái các `QuyenSach` sang `DangMuon`.

Hậu điều kiện:
- Trả về `PhieuMuon` hợp lệ.
- Không còn mâu thuẫn trạng thái giữa phiếu và quyển sách.

### 2.3.2. `PhieuMuon.tinhTienPhat(ngayTra): double`
Mục đích: tính tiền phạt theo số ngày trả trễ.

Tiền điều kiện:
- `ngayTra` hợp lệ.

Công thức:
- Nếu `ngayTra <= hanTra`: `tienPhat = 0`.
- Nếu `ngayTra > hanTra`:  
  `tienPhat = soNgayTre * CauHinhHeThong.phiPhatMoiNgay`.

Hậu điều kiện:
- Cập nhật đúng thuộc tính `tienPhat`.

### 2.3.3. `ThuThu.lapPhieuMuon(docGia, danhSachQuyen): PhieuMuon`
Mục đích: tạo phiếu mượn từ danh sách quyển cụ thể.

Tiền điều kiện:
- `docGia` đang hoạt động.
- Mỗi `QuyenSach` có `trangThai = SanSang`.
- Số sách mượn không vượt `CauHinhHeThong.soSachMuonToiDa`.

Luồng xử lý:
1. Kiểm tra giới hạn mượn.
2. Tạo `PhieuMuon` và `PhieuMuonChiTiet`.
3. Gán hạn trả theo `CauHinhHeThong.soNgayMuonToiDa`.
4. Cập nhật trạng thái quyển sang `DangMuon`.

Hậu điều kiện:
- Phiếu mượn hợp lệ và có thể truy vết theo chi tiết từng quyển.

### 2.3.4. `QuyenSach.capNhatTrangThai(trangThaiMoi): boolean`
Mục đích: chuyển trạng thái vòng đời của quyển sách.

Tiền điều kiện:
- `trangThaiMoi` thuộc enum `TrangThaiQuyenSach`.
- Không vi phạm quy tắc chuyển trạng thái nghiệp vụ.

Quy tắc:
- `SanSang -> DangMuon` (khi phát sinh mượn).
- `DangMuon -> SanSang` (khi trả xong).
- Bất kỳ trạng thái -> `MatHong` khi có sự cố.

Hậu điều kiện:
- Trạng thái quyển phản ánh đúng thực tế kho.

## 2.4. Ánh xạ các lớp sang ngôn ngữ đích (C#)

### 2.4.1. Quy tắc ánh xạ
- Lớp UML ánh xạ thành `class` trong C#.
- Lớp trừu tượng (`NguoiDung`) ánh xạ thành `abstract class`.
- Quan hệ kế thừa ánh xạ bằng `:` trong C# (`ThuThu : NguoiDung`, ...).
- Enum UML ánh xạ thành `enum`.
- Quan hệ 1-N ánh xạ thành thuộc tính tập hợp `List<T>` ở phía 1.

### 2.4.2. Bảng ánh xạ kiểu dữ liệu

| UML | C# |
|---|---|
| `String` | `string` |
| `int` | `int` |
| `double` | `double` |
| `boolean` | `bool` |
| `Date` | `DateTime` |
| `List<T>` | `List<T>` |

### 2.4.3. Khung lớp C# minh họa
```csharp
public abstract class NguoiDung
{
    public string MaTK { get; set; } = string.Empty;
    public string HoTen { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string SoDienThoai { get; set; } = string.Empty;
    public string TenDangNhap { get; set; } = string.Empty;
    public string MatKhau { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public VaiTro VaiTro { get; set; }
}

public class ThuThu : NguoiDung
{
    public string MaThuThu { get; set; } = string.Empty;
}

public class DocGia : NguoiDung
{
    public string MaDocGia { get; set; } = string.Empty;
    public string DiaChi { get; set; } = string.Empty;
    public DateTime NgayDangKy { get; set; }
}
```

---

# PHẦN 3. THIẾT KẾ CƠ SỞ DỮ LIỆU

## 3.1. Ánh xạ biểu đồ lớp sang các lược đồ CSDL

### 3.1.1. Lớp -> bảng quan hệ

| Lớp | Bảng |
|---|---|
| `NguoiDung` | `nguoi_dung` |
| `ThuThu` | `thu_thu` |
| `DocGia` | `doc_gia` |
| `QuanTriVien` | `quan_tri_vien` |
| `DanhMucSach` | `danh_muc_sach` |
| `DauSach` | `dau_sach` |
| `QuyenSach` | `quyen_sach` |
| `PhieuDatMuon` | `phieu_dat_muon` |
| `PhieuMuon` | `phieu_muon` |
| `PhieuMuonChiTiet` | `phieu_muon_chi_tiet` |
| `ThongBao` | `thong_bao` |
| `NhatKyHeThong` | `nhat_ky_he_thong` |
| `CauHinhHeThong` | `cau_hinh_he_thong` |

### 3.1.2. Quá trình ánh xạ
1. Ánh xạ mỗi lớp thực thể thành một bảng.
2. Ánh xạ kế thừa `NguoiDung -> ThuThu/DocGia/QuanTriVien` theo chiến lược table-per-subclass:
   - Bảng cha `nguoi_dung` chứa thuộc tính chung.
   - Bảng con chứa khóa chính đồng thời là khóa ngoại tới `nguoi_dung`.
3. Ánh xạ quan hệ 1-N bằng khóa ngoại ở bảng phía N.
4. Ánh xạ enum thành cột `VARCHAR` có `CHECK` theo tập giá trị enum.

### 3.1.3. Quan hệ chính trong mô hình quan hệ
- `danh_muc_sach (1) - (N) dau_sach`
- `dau_sach (1) - (N) quyen_sach`
- `doc_gia (1) - (N) phieu_dat_muon`
- `doc_gia (1) - (N) phieu_muon`
- `thu_thu (1) - (N) phieu_dat_muon` (duyệt)
- `thu_thu (1) - (N) phieu_muon` (lập/xử lý)
- `phieu_dat_muon (1) - (0..1) phieu_muon`
- `phieu_muon (1) - (N) phieu_muon_chi_tiet`
- `phieu_muon_chi_tiet (N) - (1) quyen_sach`
- `doc_gia (1) - (N) thong_bao`
- `quan_tri_vien (1) - (N) nhat_ky_he_thong`

## 3.2. Mô tả chi tiết lược đồ bảng

### 3.2.1. Bảng `phieu_muon_chi_tiet` (ví dụ mẫu đúng form yêu cầu)

| Thuộc tính | Kiểu | Null | Ràng buộc | Mô tả |
|---|---|---|---|---|
| `ma_phieu_muon` | `CHAR(20)` | Không | PK, FK -> `phieu_muon` | Mã phiếu mượn |
| `ma_quyen_sach` | `CHAR(20)` | Không | PK, FK -> `quyen_sach` | Mã quyển sách |
| `so_luong` | `INT` | Không | CHECK > 0 | Số lượng mượn |
| `ngay_tra_thuc` | `DATE` | Có |  | Ngày trả thực tế |
| `trang_thai` | `VARCHAR(20)` | Không | CHECK (`DangMuon`,`DaTra`) | Trạng thái chi tiết |

### 3.2.2. Bảng `danh_muc_sach`

| Thuộc tính | Kiểu | Null | Ràng buộc | Mô tả |
|---|---|---|---|---|
| `ma_danh_muc` | `CHAR(20)` | Không | PK | Mã danh mục |
| `ten_danh_muc` | `VARCHAR(100)` | Không | UNIQUE | Tên danh mục |
| `mo_ta` | `VARCHAR(255)` | Có |  | Mô tả |
| `vi_tri_ke` | `VARCHAR(50)` | Có |  | Vị trí kệ |
| `dang_su_dung` | `BOOLEAN` | Không |  | Trạng thái sử dụng |

### 3.2.3. Bảng `dau_sach`

| Thuộc tính | Kiểu | Null | Ràng buộc | Mô tả |
|---|---|---|---|---|
| `ma_dau_sach` | `CHAR(20)` | Không | PK | Mã đầu sách |
| `ma_danh_muc` | `CHAR(20)` | Không | FK -> `danh_muc_sach` | Mã danh mục |
| `ten_sach` | `VARCHAR(255)` | Không |  | Tên sách |
| `tac_gia` | `VARCHAR(150)` | Không |  | Tác giả |
| `chu_de` | `VARCHAR(100)` | Có |  | Chủ đề |
| `nha_xuat_ban` | `VARCHAR(150)` | Có |  | Nhà xuất bản |
| `nam_xuat_ban` | `INT` | Có |  | Năm xuất bản |
| `isbn` | `VARCHAR(20)` | Có | UNIQUE | ISBN |
| `vi_tri_kho` | `VARCHAR(50)` | Có |  | Vị trí kho |
| `trang_thai` | `VARCHAR(30)` | Không |  | Trạng thái đầu sách |

### 3.2.4. Bảng `quyen_sach`

| Thuộc tính | Kiểu | Null | Ràng buộc | Mô tả |
|---|---|---|---|---|
| `ma_quyen_sach` | `CHAR(20)` | Không | PK | Mã quyển sách |
| `ma_dau_sach` | `CHAR(20)` | Không | FK -> `dau_sach` | Mã đầu sách |
| `ngay_nhap` | `DATE` | Không |  | Ngày nhập |
| `tinh_trang` | `VARCHAR(50)` | Có |  | Mô tả tình trạng vật lý |
| `trang_thai` | `VARCHAR(20)` | Không | CHECK (`SanSang`,`DangMuon`,`MatHong`) | Trạng thái quyển |
| `vi_tri` | `VARCHAR(50)` | Có |  | Vị trí đặt quyển |

### 3.2.5. Bảng `phieu_dat_muon`

| Thuộc tính | Kiểu | Null | Ràng buộc | Mô tả |
|---|---|---|---|---|
| `ma_phieu_dat` | `CHAR(20)` | Không | PK | Mã phiếu đặt |
| `ma_doc_gia` | `CHAR(20)` | Không | FK -> `doc_gia` | Người tạo phiếu |
| `ma_thu_thu_duyet` | `CHAR(20)` | Có | FK -> `thu_thu` | Thủ thư duyệt |
| `ngay_dat` | `DATE` | Không |  | Ngày đặt |
| `trang_thai` | `VARCHAR(30)` | Không | CHECK (`ChoDuyet`,`DaDuyet`,`TuChoi`,`DaChuyenPhieuMuon`) | Trạng thái phiếu đặt |

### 3.2.6. Bảng `phieu_muon`

| Thuộc tính | Kiểu | Null | Ràng buộc | Mô tả |
|---|---|---|---|---|
| `ma_phieu_muon` | `CHAR(20)` | Không | PK | Mã phiếu mượn |
| `ma_phieu_dat` | `CHAR(20)` | Có | FK UNIQUE -> `phieu_dat_muon` | Phiếu đặt nguồn |
| `ma_doc_gia` | `CHAR(20)` | Không | FK -> `doc_gia` | Độc giả mượn |
| `ma_thu_thu_lap` | `CHAR(20)` | Không | FK -> `thu_thu` | Thủ thư lập phiếu |
| `ngay_muon` | `DATE` | Không |  | Ngày mượn |
| `han_tra` | `DATE` | Không |  | Hạn trả |
| `trang_thai` | `VARCHAR(20)` | Không | CHECK (`DangMuon`,`QuaHan`,`DaTra`) | Trạng thái phiếu mượn |
| `tien_phat` | `DECIMAL(12,2)` | Không | DEFAULT 0 | Tiền phạt |
| `da_thu_phat` | `BOOLEAN` | Không | DEFAULT FALSE | Đã thu phạt chưa |

### 3.2.7. Bảng `nguoi_dung`

| Thuộc tính | Kiểu | Null | Ràng buộc | Mô tả |
|---|---|---|---|---|
| `ma_tk` | `CHAR(20)` | Không | PK | Mã tài khoản |
| `ho_ten` | `VARCHAR(120)` | Không |  | Họ tên |
| `email` | `VARCHAR(120)` | Có |  | Email |
| `so_dien_thoai` | `VARCHAR(20)` | Có |  | Số điện thoại |
| `ten_dang_nhap` | `VARCHAR(60)` | Không | UNIQUE | Tên đăng nhập |
| `mat_khau` | `VARCHAR(255)` | Không |  | Mật khẩu |
| `is_active` | `BOOLEAN` | Không |  | Trạng thái hoạt động |
| `vai_tro` | `VARCHAR(20)` | Không | CHECK (`Admin`,`ThuThu`,`DocGia`) | Vai trò |

### 3.2.8. Bảng `doc_gia`, `thu_thu`, `quan_tri_vien`

| Thuộc tính | Kiểu | Null | Ràng buộc | Mô tả |
|---|---|---|---|---|
| `ma_doc_gia` | `CHAR(20)` | Không | PK, FK -> `nguoi_dung(ma_tk)` | Mã độc giả |
| `dia_chi` | `VARCHAR(255)` | Có |  | Địa chỉ |
| `ngay_dang_ky` | `DATE` | Không |  | Ngày đăng ký |

| Thuộc tính | Kiểu | Null | Ràng buộc | Mô tả |
|---|---|---|---|---|
| `ma_thu_thu` | `CHAR(20)` | Không | PK, FK -> `nguoi_dung(ma_tk)` | Mã thủ thư |

| Thuộc tính | Kiểu | Null | Ràng buộc | Mô tả |
|---|---|---|---|---|
| `ma_quan_tri` | `CHAR(20)` | Không | PK, FK -> `nguoi_dung(ma_tk)` | Mã quản trị viên |

### 3.2.9. Bảng hệ thống

| Thuộc tính | Kiểu | Null | Ràng buộc | Mô tả |
|---|---|---|---|---|
| `thong_bao(ma_thong_bao)` | `CHAR(20)` | Không | PK | Mã thông báo |
| `thong_bao.ma_doc_gia` | `CHAR(20)` | Không | FK -> `doc_gia` | Người nhận |
| `thong_bao.noi_dung` | `VARCHAR(500)` | Không |  | Nội dung |
| `thong_bao.ngay_gui` | `DATE` | Không |  | Ngày gửi |
| `thong_bao.trang_thai` | `VARCHAR(20)` | Không |  | Trạng thái đọc |

| Thuộc tính | Kiểu | Null | Ràng buộc | Mô tả |
|---|---|---|---|---|
| `nhat_ky_he_thong(ma_nhat_ky)` | `CHAR(20)` | Không | PK | Mã nhật ký |
| `nhat_ky_he_thong.ma_quan_tri` | `CHAR(20)` | Không | FK -> `quan_tri_vien` | Người ghi |
| `hanh_dong` | `VARCHAR(120)` | Không |  | Hành động |
| `thoi_gian` | `TIMESTAMP` | Không |  | Thời gian |
| `chi_tiet` | `VARCHAR(500)` | Có |  | Chi tiết |

| Thuộc tính | Kiểu | Null | Ràng buộc | Mô tả |
|---|---|---|---|---|
| `cau_hinh_he_thong(id)` | `INT` | Không | PK | Khóa cấu hình |
| `so_ngay_muon_toi_da` | `INT` | Không | CHECK > 0 | Số ngày mượn tối đa |
| `so_sach_muon_toi_da` | `INT` | Không | CHECK > 0 | Số sách mượn tối đa |
| `phi_phat_moi_ngay` | `DECIMAL(12,2)` | Không | CHECK >= 0 | Phí phạt mỗi ngày |

## 3.3. Dạng chuẩn và tối ưu hóa CSDL

### 3.3.1. Kiểm tra chuẩn
- 1NF: đạt vì thuộc tính nguyên tố, không lặp nhóm dữ liệu.
- 2NF: đạt vì bảng chi tiết dùng khóa chính phù hợp; không phụ thuộc bộ phận sai.
- 3NF: đạt với thiết kế tách thực thể chuẩn (`dau_sach`, `quyen_sach`, `phieu_muon`, `nguoi_dung`...).

### 3.3.2. Điểm cần chú ý khi đưa về 3NF
- Không lưu lặp thông tin tên độc giả/tên sách trong bảng giao dịch nếu không cần snapshot.
- Tách kế thừa người dùng thành bảng cha-con như mục 3.1.2 để tránh dư thừa.

### 3.3.3. Phi chuẩn hóa có kiểm soát
- Có thể bổ sung trường snapshot ở `phieu_muon` để phục vụ báo cáo lịch sử (nếu yêu cầu).
- Chỉ thực hiện khi đã đánh giá chi phí join và yêu cầu báo cáo theo thời điểm.

### 3.3.4. Tạo chỉ mục
Chỉ mục đề xuất:
- `idx_dau_sach_ten_sach` trên `dau_sach(ten_sach)`.
- `idx_dau_sach_tac_gia` trên `dau_sach(tac_gia)`.
- `idx_dau_sach_isbn` trên `dau_sach(isbn)`.
- `idx_quyen_sach_ma_dau_sach_trang_thai` trên `quyen_sach(ma_dau_sach, trang_thai)`.
- `idx_phieu_muon_ma_doc_gia_trang_thai` trên `phieu_muon(ma_doc_gia, trang_thai)`.
- `idx_phieu_dat_muon_trang_thai` trên `phieu_dat_muon(trang_thai)`.
- `idx_phieu_muon_chi_tiet_ma_quyen_sach` trên `phieu_muon_chi_tiet(ma_quyen_sach)`.
- `idx_thong_bao_ma_doc_gia_trang_thai` trên `thong_bao(ma_doc_gia, trang_thai)`.

## 3.4. Kết luận
Tài liệu đã được chỉnh để khớp hoàn toàn với biểu đồ lớp trong `LIBRARY_LASS_DIAGRAM.puml`, bao gồm:
- Đúng hệ lớp, enum, kế thừa, và quan hệ nghiệp vụ.
- Đúng luồng đặt mượn -> mượn -> trả theo mô hình lớp cũ.
- Ánh xạ CSDL quan hệ tương ứng, có chuẩn hóa 3NF và chỉ mục tối ưu.
