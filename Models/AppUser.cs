namespace LibraryManagement.Models
{
    public enum UserRole
    {
        Admin,
        ThuThu,
        DocGia
    }

    public class AppUser
    {
        public string MaTK { get; set; } = "";
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";
        public string HoTen { get; set; } = "";
        public string Email { get; set; } = "";
        public string SDT { get; set; } = "";
        public UserRole Role { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime NgayTao { get; set; } = DateTime.Now;

        public string RoleDisplay => Role switch
        {
            UserRole.Admin => "Quản trị viên",
            UserRole.ThuThu => "Thủ thư",
            UserRole.DocGia => "Độc giả",
            _ => "Không xác định"
        };
    }

    public class ActivityLog
    {
        public DateTime ThoiGian { get; set; }
        public string NguoiDung { get; set; } = "";
        public string HanhDong { get; set; } = "";
        public string ChiTiet { get; set; } = "";
    }

    public class Notification
    {
        public DateTime ThoiGian { get; set; }
        public string TieuDe { get; set; } = "";
        public string NoiDung { get; set; } = "";
        public bool DaDoc { get; set; } = false;
    }

    public class ExtensionRequest
    {
        public string MaYeuCau { get; set; } = "";
        public string MaDocGia { get; set; } = "";
        public string TenSach { get; set; } = "";
        public DateTime NgayYeuCau { get; set; }
        public int SoNgayGiaHan { get; set; } = 7;
        public string TrangThai { get; set; } = "Chờ duyệt"; // Chờ duyệt, Đã duyệt, Từ chối
    }

    public static class UserStore
    {
        public static List<AppUser> Users = new List<AppUser>
        {
            new AppUser { MaTK = "TK001", Username = "admin",   Password = "admin123",   HoTen = "Nguyễn Quản Trị",   Email = "admin@thuvien.vn",   SDT = "0901234567", Role = UserRole.Admin },
            new AppUser { MaTK = "TK002", Username = "thuthu",  Password = "thuthu123",  HoTen = "Trần Thị Thủ Thư",  Email = "thuthu@thuvien.vn",  SDT = "0912345678", Role = UserRole.ThuThu },
            new AppUser { MaTK = "TK003", Username = "docgia",  Password = "docgia123",  HoTen = "Phạm Văn Độc Giả",  Email = "docgia@gmail.com",   SDT = "0923456789", Role = UserRole.DocGia },
            new AppUser { MaTK = "TK004", Username = "thuthu2", Password = "thuthu123",  HoTen = "Lê Văn Thủ Thư 2",  Email = "thuthu2@thuvien.vn", SDT = "0934567890", Role = UserRole.ThuThu },
            new AppUser { MaTK = "TK005", Username = "docgia2", Password = "docgia123",  HoTen = "Ngô Thị Minh Anh",  Email = "minhanh@gmail.com",  SDT = "0945678901", Role = UserRole.DocGia },
        };

        public static AppUser? CurrentUser { get; set; }

        public static AppUser? Login(string username, string password)
        {
            return Users.FirstOrDefault(u =>
                u.Username == username && u.Password == password && u.IsActive);
        }

        public static List<ActivityLog> Logs = new List<ActivityLog>
        {
            new ActivityLog { ThoiGian = DateTime.Now.AddHours(-1), NguoiDung = "Trần Thị Thủ Thư", HanhDong = "Cho mượn sách", ChiTiet = "Đắc Nhân Tâm - Phạm Văn Độc Giả" },
            new ActivityLog { ThoiGian = DateTime.Now.AddHours(-3), NguoiDung = "Trần Thị Thủ Thư", HanhDong = "Nhận trả sách", ChiTiet = "Lập Trình C# - Ngô Thị Minh Anh" },
            new ActivityLog { ThoiGian = DateTime.Now.AddDays(-1), NguoiDung = "Nguyễn Quản Trị", HanhDong = "Tạo tài khoản", ChiTiet = "docgia2 - Ngô Thị Minh Anh" },
            new ActivityLog { ThoiGian = DateTime.Now.AddDays(-1), NguoiDung = "Trần Thị Thủ Thư", HanhDong = "Thêm sách mới", ChiTiet = "Clean Code - Robert C. Martin" },
            new ActivityLog { ThoiGian = DateTime.Now.AddDays(-2), NguoiDung = "Nguyễn Quản Trị", HanhDong = "Sao lưu CSDL", ChiTiet = "Sao lưu thành công" },
        };

        public static List<Notification> Notifications = new List<Notification>
        {
            new Notification { ThoiGian = DateTime.Now.AddHours(-2), TieuDe = "Sắp đến hạn trả sách", NoiDung = "Sách 'Đắc Nhân Tâm' sẽ đến hạn trả trong 2 ngày.", DaDoc = false },
            new Notification { ThoiGian = DateTime.Now.AddDays(-1), TieuDe = "Yêu cầu gia hạn đã duyệt", NoiDung = "Yêu cầu gia hạn sách 'Lập Trình C#' đã được duyệt thêm 7 ngày.", DaDoc = true },
            new Notification { ThoiGian = DateTime.Now.AddDays(-3), TieuDe = "Sách mới có sẵn", NoiDung = "Thư viện vừa bổ sung 5 đầu sách mới về Công nghệ thông tin.", DaDoc = true },
        };

        public static List<ExtensionRequest> ExtensionRequests = new List<ExtensionRequest>
        {
            new ExtensionRequest { MaYeuCau = "GH001", MaDocGia = "DG001", TenSach = "Đắc Nhân Tâm", NgayYeuCau = DateTime.Now.AddDays(-1), SoNgayGiaHan = 7, TrangThai = "Chờ duyệt" },
            new ExtensionRequest { MaYeuCau = "GH002", MaDocGia = "DG002", TenSach = "Lập Trình C#", NgayYeuCau = DateTime.Now.AddDays(-3), SoNgayGiaHan = 7, TrangThai = "Đã duyệt" },
        };
    }
}
