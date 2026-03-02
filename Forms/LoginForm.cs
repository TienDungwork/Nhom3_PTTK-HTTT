using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using LibraryManagement.Controls;
using LibraryManagement.Helpers;
using LibraryManagement.Models;

namespace LibraryManagement.Forms
{
    public class LoginForm : Form
    {
        private RoundedTextBox txtUsername = null!;
        private RoundedTextBox txtPassword = null!;
        private RoundedButton btnLogin = null!;

        public LoginForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            Text = "Đăng nhập - Hệ thống Quản lý Thư viện";
            Size = new Size(1000, 650);
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.None;
            BackColor = ThemeColors.Primary;
            DoubleBuffered = true;

            Panel mainPanel = new Panel { Dock = DockStyle.Fill, BackColor = Color.Transparent };
            mainPanel.Paint += MainPanel_Paint;
            Controls.Add(mainPanel);

            Panel cardPanel = new Panel { Size = new Size(420, 500), BackColor = Color.Transparent };
            mainPanel.Controls.Add(cardPanel);
            mainPanel.Resize += (s, e) => cardPanel.Location = new Point((mainPanel.Width - cardPanel.Width) / 2, (mainPanel.Height - cardPanel.Height) / 2);
            cardPanel.Location = new Point((mainPanel.Width - cardPanel.Width) / 2, (mainPanel.Height - cardPanel.Height) / 2);
            cardPanel.Paint += CardPanel_Paint;

            // Icon
            cardPanel.Controls.Add(new Label { Text = "📚", Font = new Font("Segoe UI", 40), Size = new Size(420, 70), Location = new Point(0, 30), TextAlign = ContentAlignment.MiddleCenter, BackColor = Color.Transparent });

            // Title
            cardPanel.Controls.Add(new Label { Text = "ĐĂNG NHẬP HỆ THỐNG", Font = new Font("Segoe UI", 18, FontStyle.Bold), ForeColor = ThemeColors.TextPrimary, Size = new Size(420, 40), Location = new Point(0, 105), TextAlign = ContentAlignment.MiddleCenter, BackColor = Color.Transparent });

            // Subtitle
            cardPanel.Controls.Add(new Label { Text = "Hệ thống Quản lý Thư viện", Font = ThemeColors.BodyFont, ForeColor = ThemeColors.TextSecondary, Size = new Size(420, 25), Location = new Point(0, 145), TextAlign = ContentAlignment.MiddleCenter, BackColor = Color.Transparent });

            // Username
            cardPanel.Controls.Add(new Label { Text = "Tên đăng nhập", Font = ThemeColors.SubTitleFont, ForeColor = ThemeColors.TextPrimary, Location = new Point(50, 190), Size = new Size(320, 22), BackColor = Color.Transparent });
            txtUsername = new RoundedTextBox { Placeholder = "Nhập tên đăng nhập", Location = new Point(50, 216), Size = new Size(320, 46) };
            cardPanel.Controls.Add(txtUsername);

            // Password
            cardPanel.Controls.Add(new Label { Text = "Mật khẩu", Font = ThemeColors.SubTitleFont, ForeColor = ThemeColors.TextPrimary, Location = new Point(50, 276), Size = new Size(320, 22), BackColor = Color.Transparent });
            txtPassword = new RoundedTextBox { Placeholder = "Nhập mật khẩu", IsPassword = true, Location = new Point(50, 302), Size = new Size(320, 46) };
            cardPanel.Controls.Add(txtPassword);

            // Login button
            btnLogin = new RoundedButton { Text = "Đăng nhập", Size = new Size(320, 48), Location = new Point(50, 375), Font = new Font("Segoe UI Semibold", 12), ButtonColor = ThemeColors.Primary, HoverColor = ThemeColors.PrimaryDark };
            btnLogin.Click += BtnLogin_Click;
            cardPanel.Controls.Add(btnLogin);

            // Hint label
            cardPanel.Controls.Add(new Label { Text = "TK: admin / thuthu / docgia", Font = ThemeColors.SmallFont, ForeColor = ThemeColors.TextSecondary, Size = new Size(320, 18), Location = new Point(50, 432), TextAlign = ContentAlignment.MiddleCenter, BackColor = Color.Transparent });

            // Version
            mainPanel.Controls.Add(new Label { Text = "Phiên bản 2.0 © 2026", Font = ThemeColors.SmallFont, ForeColor = Color.FromArgb(150, 255, 255, 255), Size = new Size(1000, 20), TextAlign = ContentAlignment.MiddleCenter, BackColor = Color.Transparent, Dock = DockStyle.Bottom, Padding = new Padding(0, 0, 0, 10) });

            // Close button
            Label btnClose = new Label { Text = "✕", Font = new Font("Segoe UI", 14), ForeColor = Color.FromArgb(200, 255, 255, 255), Size = new Size(40, 40), Location = new Point(Width - 50, 10), TextAlign = ContentAlignment.MiddleCenter, BackColor = Color.Transparent, Cursor = Cursors.Hand, Anchor = AnchorStyles.Top | AnchorStyles.Right };
            btnClose.Click += (s, e) => Application.Exit();
            btnClose.MouseEnter += (s, e) => btnClose.ForeColor = Color.White;
            btnClose.MouseLeave += (s, e) => btnClose.ForeColor = Color.FromArgb(200, 255, 255, 255);
            mainPanel.Controls.Add(btnClose);
            btnClose.BringToFront();

            // Drag support
            mainPanel.MouseDown += (s, e) => { if (e.Button == MouseButtons.Left) { NativeMethods.ReleaseCapture(); NativeMethods.SendMessage(Handle, 0xA1, 0x2, 0); } };
        }

        private void MainPanel_Paint(object? sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            using (var brush = new LinearGradientBrush(ClientRectangle, ColorTranslator.FromHtml("#1E3A8A"), ColorTranslator.FromHtml("#3B82F6"), LinearGradientMode.ForwardDiagonal))
                g.FillRectangle(brush, ClientRectangle);
            using (var cb = new SolidBrush(Color.FromArgb(15, 255, 255, 255)))
            {
                g.FillEllipse(cb, -100, -100, 400, 400);
                g.FillEllipse(cb, Width - 200, Height - 200, 400, 400);
                g.FillEllipse(cb, Width / 2 - 150, -200, 300, 300);
            }
        }

        private void CardPanel_Paint(object? sender, PaintEventArgs e)
        {
            Panel p = (Panel)sender!;
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            using (var sp = ThemeColors.GetRoundedRect(new Rectangle(6, 6, p.Width - 12, p.Height - 12), 16))
            using (var sb = new SolidBrush(Color.FromArgb(40, 0, 0, 0)))
                g.FillPath(sb, sp);
            using (var cp = ThemeColors.GetRoundedRect(new Rectangle(4, 4, p.Width - 14, p.Height - 14), 16))
            using (var cb = new SolidBrush(Color.White))
                g.FillPath(cb, cp);
        }

        private void BtnLogin_Click(object? sender, EventArgs e)
        {
            string username = txtUsername.InputText;
            string password = txtPassword.InputText;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin đăng nhập!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var user = UserStore.Login(username, password);
            if (user == null)
            {
                MessageBox.Show("Tên đăng nhập hoặc mật khẩu không đúng!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            UserStore.CurrentUser = user;
            Hide();

            Form mainForm = user.Role switch
            {
                UserRole.ThuThu => new LibrarianForm(),
                UserRole.DocGia => new ReaderForm(),
                UserRole.Admin => new AdminForm(),
                _ => new LibrarianForm()
            };

            mainForm.FormClosed += (s, args) => Close();
            mainForm.Show();
        }
    }

    internal static class NativeMethods
    {
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool ReleaseCapture();
    }
}
