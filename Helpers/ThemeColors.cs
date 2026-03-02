using System.Drawing;
using System.Drawing.Drawing2D;

namespace LibraryManagement.Helpers
{
    public static class ThemeColors
    {
        // Primary colors
        public static Color Primary = ColorTranslator.FromHtml("#2563EB");
        public static Color PrimaryDark = ColorTranslator.FromHtml("#1D4ED8");
        public static Color PrimaryLight = ColorTranslator.FromHtml("#3B82F6");

        // Background
        public static Color Background = ColorTranslator.FromHtml("#F5F7FA");
        public static Color CardBackground = Color.White;
        public static Color SidebarBackground = ColorTranslator.FromHtml("#1E293B");
        public static Color SidebarHover = ColorTranslator.FromHtml("#334155");
        public static Color SidebarActive = ColorTranslator.FromHtml("#2563EB");
        public static Color HeaderBackground = Color.White;

        // Text
        public static Color TextPrimary = ColorTranslator.FromHtml("#1E293B");
        public static Color TextSecondary = ColorTranslator.FromHtml("#64748B");
        public static Color TextLight = Color.White;
        public static Color TextMuted = ColorTranslator.FromHtml("#94A3B8");

        // Status
        public static Color Success = ColorTranslator.FromHtml("#10B981");
        public static Color SuccessLight = ColorTranslator.FromHtml("#D1FAE5");
        public static Color Warning = ColorTranslator.FromHtml("#F59E0B");
        public static Color WarningLight = ColorTranslator.FromHtml("#FEF3C7");
        public static Color Danger = ColorTranslator.FromHtml("#EF4444");
        public static Color DangerLight = ColorTranslator.FromHtml("#FEE2E2");
        public static Color Info = ColorTranslator.FromHtml("#3B82F6");
        public static Color InfoLight = ColorTranslator.FromHtml("#DBEAFE");

        // Borders
        public static Color Border = ColorTranslator.FromHtml("#E2E8F0");
        public static Color BorderLight = ColorTranslator.FromHtml("#F1F5F9");

        // DataGridView
        public static Color GridAlternateRow = ColorTranslator.FromHtml("#F8FAFC");
        public static Color GridSelectedRow = ColorTranslator.FromHtml("#DBEAFE");
        public static Color GridHeaderBackground = ColorTranslator.FromHtml("#F1F5F9");

        // Fonts
        public static Font HeaderFont = new Font("Segoe UI", 20, FontStyle.Bold);
        public static Font TitleFont = new Font("Segoe UI Semibold", 14);
        public static Font SubTitleFont = new Font("Segoe UI Semibold", 12);
        public static Font BodyFont = new Font("Segoe UI", 10);
        public static Font SmallFont = new Font("Segoe UI", 9);
        public static Font ButtonFont = new Font("Segoe UI Semibold", 10);
        public static Font SidebarFont = new Font("Segoe UI", 11);
        public static Font StatValueFont = new Font("Segoe UI", 28, FontStyle.Bold);
        public static Font StatLabelFont = new Font("Segoe UI", 10);
        public static Font InputFont = new Font("Segoe UI", 11);

        // Dimensions
        public static int BorderRadius = 10;
        public static int SidebarWidth = 250;
        public static int HeaderHeight = 60;
        public static int CardPadding = 24;

        public static GraphicsPath GetRoundedRect(Rectangle bounds, int radius)
        {
            int diameter = radius * 2;
            GraphicsPath path = new GraphicsPath();
            path.AddArc(bounds.X, bounds.Y, diameter, diameter, 180, 90);
            path.AddArc(bounds.Right - diameter, bounds.Y, diameter, diameter, 270, 90);
            path.AddArc(bounds.Right - diameter, bounds.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(bounds.X, bounds.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();
            return path;
        }
    }
}
