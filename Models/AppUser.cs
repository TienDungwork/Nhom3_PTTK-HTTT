namespace LibraryManagement.Models
{
    public enum UserRole
    {
        Admin,
        ThuThu,    // Thủ thư
        NguoiDung  // Người dùng thông thường (chỉ xem)
    }

    public class AppUser
    {
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";
        public string HoTen { get; set; } = "";
        public UserRole Role { get; set; }

        public string RoleDisplay => Role switch
        {
            UserRole.Admin => "Quản trị viên",
            UserRole.ThuThu => "Thủ thư",
            UserRole.NguoiDung => "Người dùng",
            _ => "Không xác định"
        };
    }

    public static class UserStore
    {
        // Tài khoản mặc định
        public static List<AppUser> Users = new List<AppUser>
        {
            new AppUser { Username = "admin",   Password = "admin123",  HoTen = "Nguyễn Quản Trị", Role = UserRole.Admin },
            new AppUser { Username = "thuthu",  Password = "thuthu123", HoTen = "Trần Thị Thủ Thư", Role = UserRole.ThuThu },
            new AppUser { Username = "user",    Password = "user123",   HoTen = "Phạm Văn User",    Role = UserRole.NguoiDung },
        };

        public static AppUser? CurrentUser { get; set; }

        public static AppUser? Login(string username, string password)
        {
            return Users.FirstOrDefault(u =>
                u.Username == username && u.Password == password);
        }
    }
}
