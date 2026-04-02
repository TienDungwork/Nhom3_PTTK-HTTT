using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

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
        public string MaDocGia { get; set; } = "";
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";
        public string PasswordHash { get; set; } = "";
        public string PasswordSalt { get; set; } = "";
        public string HoTen { get; set; } = "";
        public string Email { get; set; } = "";
        public string SDT { get; set; } = "";
        public UserRole Role { get; set; }
        public bool IsActive { get; set; } = true;
        public int FailedLoginCount { get; set; } = 0;
        public DateTime? LockedUntil { get; set; }
        public DateTime? LastActiveAt { get; set; }
        public string EmploymentStatus { get; set; } = "Đang làm";
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
        public string LoaiSuKien { get; set; } = "System";
    }

    public class Notification
    {
        public string MaThongBao { get; set; } = "";
        public string MaDocGia { get; set; } = "";
        public string NguoiGui { get; set; } = "";
        public DateTime ThoiGian { get; set; }
        public string LoaiThongBao { get; set; } = "HeThong";
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

    public class SystemSetting
    {
        public string SettingKey { get; set; } = "";
        public string SettingValue { get; set; } = "";
    }

    public class FeatureToggle
    {
        public string FeatureKey { get; set; } = "";
        public bool Enabled { get; set; } = true;
    }

    public class NotificationTemplate
    {
        public string TemplateKey { get; set; } = "";
        public bool Enabled { get; set; } = true;
        public string Content { get; set; } = "";
    }

    public class RolePermission
    {
        public string Role { get; set; } = "";
        public string Module { get; set; } = "";
        public string Action { get; set; } = "";
        public bool Allowed { get; set; } = true;
    }

    public class LoginResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = "";
        public AppUser? User { get; set; }
    }

    public static class PasswordHasher
    {
        public static (string Hash, string Salt) HashPassword(string password)
        {
            byte[] saltBytes = RandomNumberGenerator.GetBytes(16);
            byte[] hashBytes = PBKDF2(password, saltBytes);
            return (Convert.ToBase64String(hashBytes), Convert.ToBase64String(saltBytes));
        }

        public static bool VerifyPassword(string password, string hashBase64, string saltBase64)
        {
            if (string.IsNullOrWhiteSpace(hashBase64) || string.IsNullOrWhiteSpace(saltBase64))
                return false;
            byte[] salt = Convert.FromBase64String(saltBase64);
            byte[] expected = Convert.FromBase64String(hashBase64);
            byte[] actual = PBKDF2(password, salt);
            return CryptographicOperations.FixedTimeEquals(expected, actual);
        }

        private static byte[] PBKDF2(string password, byte[] salt)
        {
            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100000, HashAlgorithmName.SHA256);
            return pbkdf2.GetBytes(32);
        }
    }

    public static class UserStore
    {
        private const int MaxFailedLogin = 5;
        private static readonly TimeSpan LockDuration = TimeSpan.FromMinutes(15);

        public static List<AppUser> Users = new List<AppUser>
        {
            new AppUser { MaTK = "TK001", Username = "admin",   Password = "admin123",   HoTen = "Nguyễn Quản Trị",   Email = "admin@thuvien.vn",   SDT = "0901234567", Role = UserRole.Admin },
            new AppUser { MaTK = "TK002", Username = "thuthu",  Password = "thuthu123",  HoTen = "Trần Thị Thủ Thư",  Email = "thuthu@thuvien.vn",  SDT = "0912345678", Role = UserRole.ThuThu },
            new AppUser { MaTK = "TK003", MaDocGia = "DG001", Username = "docgia",  Password = "docgia123",  HoTen = "Nguyễn Văn Minh",  Email = "minh@email.com",   SDT = "0901234567", Role = UserRole.DocGia },
            new AppUser { MaTK = "TK004", Username = "thuthu2", Password = "thuthu123",  HoTen = "Lê Văn Thủ Thư 2",  Email = "thuthu2@thuvien.vn", SDT = "0934567890", Role = UserRole.ThuThu },
            new AppUser { MaTK = "TK005", MaDocGia = "DG002", Username = "docgia2", Password = "docgia123",  HoTen = "Trần Thị Lan",  Email = "lan@email.com",  SDT = "0912345678", Role = UserRole.DocGia },
        };

        public static AppUser? CurrentUser { get; set; }

        static UserStore()
        {
            InitializeSecurityData();
        }

        private static void InitializeSecurityData()
        {
            foreach (var user in Users)
            {
                if (string.IsNullOrWhiteSpace(user.PasswordHash))
                {
                    var hashed = PasswordHasher.HashPassword(string.IsNullOrWhiteSpace(user.Password) ? "123456" : user.Password);
                    user.PasswordHash = hashed.Hash;
                    user.PasswordSalt = hashed.Salt;
                }
            }
        }

        public static LoginResult Login(string username, string password)
        {
            var user = Users.FirstOrDefault(u => string.Equals(u.Username, username, StringComparison.OrdinalIgnoreCase));
            if (user == null)
            {
                AddLog("Ẩn danh", "Đăng nhập thất bại", $"Không tồn tại tài khoản {username}", "Auth");
                return new LoginResult { Success = false, Message = "Mã người dùng hoặc mật khẩu không đúng!" };
            }

            if (!user.IsActive)
            {
                AddLog(user.HoTen, "Đăng nhập thất bại", "Tài khoản đã bị vô hiệu hóa", "Auth");
                return new LoginResult { Success = false, Message = "Tài khoản không hoạt động." };
            }

            if (user.LockedUntil.HasValue && user.LockedUntil.Value > DateTime.Now)
            {
                AddLog(user.HoTen, "Đăng nhập thất bại", "Tài khoản đang bị khóa tạm thời", "Auth");
                return new LoginResult { Success = false, Message = $"Tài khoản bị khóa đến {user.LockedUntil:HH:mm dd/MM/yyyy}." };
            }

            bool valid = PasswordHasher.VerifyPassword(password, user.PasswordHash, user.PasswordSalt);
            if (!valid)
            {
                user.FailedLoginCount++;
                if (user.FailedLoginCount >= MaxFailedLogin)
                {
                    user.LockedUntil = DateTime.Now.Add(LockDuration);
                    user.FailedLoginCount = 0;
                    AddLog(user.HoTen, "Khóa tài khoản", "Khóa tự động do đăng nhập sai nhiều lần", "Security");
                    return new LoginResult { Success = false, Message = "Tài khoản đã bị khóa tạm thời do nhập sai nhiều lần." };
                }

                AddLog(user.HoTen, "Đăng nhập thất bại", $"Sai mật khẩu lần {user.FailedLoginCount}", "Auth");
                return new LoginResult { Success = false, Message = "Mã người dùng hoặc mật khẩu không đúng!" };
            }

            user.FailedLoginCount = 0;
            user.LockedUntil = null;
            user.LastActiveAt = DateTime.Now;
            CurrentUser = user;
            AddLog(user.HoTen, "Đăng nhập", "Đăng nhập thành công", "Auth");
            return new LoginResult { Success = true, Message = "Đăng nhập thành công.", User = user };
        }

        public static void LogoutCurrentUser()
        {
            if (CurrentUser != null)
                AddLog(CurrentUser.HoTen, "Đăng xuất", "Người dùng đăng xuất hệ thống", "Auth");
            CurrentUser = null;
        }

        public static bool SetPassword(string username, string newPassword)
        {
            var user = Users.FirstOrDefault(u => u.Username == username);
            if (user == null)
                return false;
            var hashed = PasswordHasher.HashPassword(newPassword);
            user.PasswordHash = hashed.Hash;
            user.PasswordSalt = hashed.Salt;
            user.Password = "";
            AddLog(CurrentUser?.HoTen ?? "Hệ thống", "Đổi mật khẩu", $"Đổi mật khẩu cho tài khoản {username}", "Security");
            return true;
        }

        public static bool HasPermission(UserRole role, string module, string action)
        {
            var roleName = role.ToString();
            var match = RolePermissions.FirstOrDefault(p =>
                string.Equals(p.Role, roleName, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(p.Module, module, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(p.Action, action, StringComparison.OrdinalIgnoreCase));
            return match?.Allowed ?? false;
        }

        public static void AddLog(string nguoiDung, string hanhDong, string chiTiet, string loai = "System")
        {
            Logs.Insert(0, new ActivityLog
            {
                ThoiGian = DateTime.Now,
                NguoiDung = string.IsNullOrWhiteSpace(nguoiDung) ? "Hệ thống" : nguoiDung,
                HanhDong = hanhDong,
                ChiTiet = chiTiet,
                LoaiSuKien = loai
            });
        }

        public static List<ActivityLog> Logs = new List<ActivityLog>
        {
            new ActivityLog { ThoiGian = DateTime.Now.AddHours(-1), NguoiDung = "Trần Thị Thủ Thư", HanhDong = "Cho mượn sách", ChiTiet = "Đắc Nhân Tâm - Phạm Văn Độc Giả", LoaiSuKien = "Business" },
            new ActivityLog { ThoiGian = DateTime.Now.AddHours(-3), NguoiDung = "Trần Thị Thủ Thư", HanhDong = "Nhận trả sách", ChiTiet = "Lập Trình C# - Ngô Thị Minh Anh", LoaiSuKien = "Business" },
        };

        public static List<Notification> Notifications = new List<Notification>
        {
            new Notification { MaThongBao = "TB001", MaDocGia = "DG001", NguoiGui = "Hệ thống", LoaiThongBao = "NhacHan", ThoiGian = DateTime.Now.AddHours(-2), TieuDe = "Sắp đến hạn trả sách", NoiDung = "Sách 'Đắc Nhân Tâm' sẽ đến hạn trả trong 2 ngày.", DaDoc = false },
            new Notification { MaThongBao = "TB002", MaDocGia = "DG002", NguoiGui = "Thủ thư", LoaiThongBao = "QuaHan", ThoiGian = DateTime.Now.AddDays(-1), TieuDe = "Nhắc nhở trả sách", NoiDung = "Vui lòng kiểm tra các sách đã mượn và trả đúng hạn.", DaDoc = false },
            new Notification { MaThongBao = "TB003", MaDocGia = "DG001", NguoiGui = "Thủ thư", LoaiThongBao = "MuonSach", ThoiGian = DateTime.Now.AddDays(-3), TieuDe = "Sách mới có sẵn", NoiDung = "Thư viện vừa bổ sung đầu sách mới về Công nghệ thông tin.", DaDoc = true },
        };

        public static List<ExtensionRequest> ExtensionRequests = new List<ExtensionRequest>
        {
            new ExtensionRequest { MaYeuCau = "GH001", MaDocGia = "DG001", TenSach = "Đắc Nhân Tâm", NgayYeuCau = DateTime.Now.AddDays(-1), SoNgayGiaHan = 7, TrangThai = "Chờ duyệt" },
            new ExtensionRequest { MaYeuCau = "GH002", MaDocGia = "DG002", TenSach = "Lập Trình C#", NgayYeuCau = DateTime.Now.AddDays(-3), SoNgayGiaHan = 7, TrangThai = "Đã duyệt" },
        };

        public static List<SystemSetting> SystemSettings = new List<SystemSetting>
        {
            new SystemSetting { SettingKey = "max_borrow_books", SettingValue = "5" },
            new SystemSetting { SettingKey = "default_borrow_days", SettingValue = "14" },
            new SystemSetting { SettingKey = "late_fee_per_day", SettingValue = "5000" },
            new SystemSetting { SettingKey = "max_overdue_days", SettingValue = "30" },
            new SystemSetting { SettingKey = "library_name", SettingValue = "Thư viện Đại học Thủy Lợi" },
            new SystemSetting { SettingKey = "library_contact", SettingValue = "library@tlu.edu.vn" },
        };

        public static List<FeatureToggle> FeatureToggles = new List<FeatureToggle>
        {
            new FeatureToggle { FeatureKey = "borrow_request", Enabled = true },
            new FeatureToggle { FeatureKey = "inventory_check", Enabled = true },
            new FeatureToggle { FeatureKey = "auto_notify", Enabled = true },
        };

        public static List<NotificationTemplate> NotificationTemplates = new List<NotificationTemplate>
        {
            new NotificationTemplate { TemplateKey = "approved", Enabled = true, Content = "Yêu cầu mượn của bạn đã được duyệt." },
            new NotificationTemplate { TemplateKey = "rejected", Enabled = true, Content = "Yêu cầu mượn của bạn đã bị từ chối." },
            new NotificationTemplate { TemplateKey = "overdue", Enabled = true, Content = "Bạn có sách đang quá hạn, vui lòng xử lý sớm." },
        };

        public static List<RolePermission> RolePermissions = new List<RolePermission>
        {
            new RolePermission { Role = "Admin", Module = "Accounts", Action = "View", Allowed = true },
            new RolePermission { Role = "Admin", Module = "Accounts", Action = "Edit", Allowed = true },
            new RolePermission { Role = "Admin", Module = "Reports", Action = "View", Allowed = true },
            new RolePermission { Role = "Admin", Module = "Settings", Action = "Edit", Allowed = true },
            new RolePermission { Role = "ThuThu", Module = "Reports", Action = "View", Allowed = true },
            new RolePermission { Role = "ThuThu", Module = "Accounts", Action = "View", Allowed = false },
            new RolePermission { Role = "DocGia", Module = "Reports", Action = "View", Allowed = false },
        };
    }
}
