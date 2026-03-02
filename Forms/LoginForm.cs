using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using LibraryManagement.Controls;
using LibraryManagement.Helpers;

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
            // Form setup
            Text = "Đăng nhập - Hệ thống Quản lý Thư viện";
            Size = new Size(1000, 650);
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.None;
            BackColor = ThemeColors.Primary;
            DoubleBuffered = true;

            // Main container
            Panel mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent
            };
            mainPanel.Paint += MainPanel_Paint;
            Controls.Add(mainPanel);

            // Login card panel
            Panel cardPanel = new Panel
            {
                Size = new Size(420, 480),
                BackColor = Color.Transparent
            };
            mainPanel.Controls.Add(cardPanel);
            mainPanel.Resize += (s, e) =>
            {
                cardPanel.Location = new Point(
                    (mainPanel.Width - cardPanel.Width) / 2,
                    (mainPanel.Height - cardPanel.Height) / 2);
            };
            cardPanel.Location = new Point(
                (mainPanel.Width - cardPanel.Width) / 2,
                (mainPanel.Height - cardPanel.Height) / 2);
            cardPanel.Paint += CardPanel_Paint;

            // Logo/Icon area
            Label lblIcon = new Label
            {
                Text = "📚",
                Font = new Font("Segoe UI", 40),
                Size = new Size(420, 70),
                Location = new Point(0, 40),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent
            };
            cardPanel.Controls.Add(lblIcon);

            // Title
            Label lblTitle = new Label
            {
                Text = "ĐĂNG NHẬP HỆ THỐNG",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = ThemeColors.TextPrimary,
                Size = new Size(420, 40),
                Location = new Point(0, 115),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent
            };
            cardPanel.Controls.Add(lblTitle);

            // Subtitle
            Label lblSubtitle = new Label
            {
                Text = "Hệ thống Quản lý Thư viện",
                Font = ThemeColors.BodyFont,
                ForeColor = ThemeColors.TextSecondary,
                Size = new Size(420, 25),
                Location = new Point(0, 155),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent
            };
            cardPanel.Controls.Add(lblSubtitle);

            // Username label
            Label lblUser = new Label
            {
                Text = "Tên đăng nhập",
                Font = ThemeColors.SubTitleFont,
                ForeColor = ThemeColors.TextPrimary,
                Location = new Point(50, 200),
                Size = new Size(320, 22),
                BackColor = Color.Transparent
            };
            cardPanel.Controls.Add(lblUser);

            // Username textbox
            txtUsername = new RoundedTextBox
            {
                Placeholder = "Nhập tên đăng nhập",
                Location = new Point(50, 226),
                Size = new Size(320, 46)
            };
            cardPanel.Controls.Add(txtUsername);

            // Password label
            Label lblPass = new Label
            {
                Text = "Mật khẩu",
                Font = ThemeColors.SubTitleFont,
                ForeColor = ThemeColors.TextPrimary,
                Location = new Point(50, 286),
                Size = new Size(320, 22),
                BackColor = Color.Transparent
            };
            cardPanel.Controls.Add(lblPass);

            // Password textbox
            txtPassword = new RoundedTextBox
            {
                Placeholder = "Nhập mật khẩu",
                IsPassword = true,
                Location = new Point(50, 312),
                Size = new Size(320, 46)
            };
            cardPanel.Controls.Add(txtPassword);

            // Login button
            btnLogin = new RoundedButton
            {
                Text = "Đăng nhập",
                Size = new Size(320, 48),
                Location = new Point(50, 385),
                Font = new Font("Segoe UI Semibold", 12),
                ButtonColor = ThemeColors.Primary,
                HoverColor = ThemeColors.PrimaryDark
            };
            btnLogin.Click += BtnLogin_Click;
            cardPanel.Controls.Add(btnLogin);

            // Version info
            Label lblVersion = new Label
            {
                Text = "Phiên bản 1.0 © 2026",
                Font = ThemeColors.SmallFont,
                ForeColor = Color.FromArgb(150, 255, 255, 255),
                Size = new Size(1000, 20),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent,
                Dock = DockStyle.Bottom
            };
            lblVersion.Padding = new Padding(0, 0, 0, 10);
            mainPanel.Controls.Add(lblVersion);

            // Close button
            Label btnClose = new Label
            {
                Text = "✕",
                Font = new Font("Segoe UI", 14),
                ForeColor = Color.FromArgb(200, 255, 255, 255),
                Size = new Size(40, 40),
                Location = new Point(Width - 50, 10),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            btnClose.Click += (s, e) => Application.Exit();
            btnClose.MouseEnter += (s, e) => btnClose.ForeColor = Color.White;
            btnClose.MouseLeave += (s, e) => btnClose.ForeColor = Color.FromArgb(200, 255, 255, 255);
            mainPanel.Controls.Add(btnClose);
            btnClose.BringToFront();

            // Allow drag
            mainPanel.MouseDown += (s, e) =>
            {
                if (e.Button == MouseButtons.Left)
                {
                    NativeMethods.ReleaseCapture();
                    NativeMethods.SendMessage(Handle, 0xA1, 0x2, 0);
                }
            };
        }

        private void MainPanel_Paint(object? sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            using (LinearGradientBrush brush = new LinearGradientBrush(
                ClientRectangle,
                ColorTranslator.FromHtml("#1E3A8A"),
                ColorTranslator.FromHtml("#3B82F6"),
                LinearGradientMode.ForwardDiagonal))
            {
                g.FillRectangle(brush, ClientRectangle);
            }

            // Decorative circles
            using (SolidBrush circleBrush = new SolidBrush(Color.FromArgb(15, 255, 255, 255)))
            {
                g.FillEllipse(circleBrush, -100, -100, 400, 400);
                g.FillEllipse(circleBrush, Width - 200, Height - 200, 400, 400);
                g.FillEllipse(circleBrush, Width / 2 - 150, -200, 300, 300);
            }
        }

        private void CardPanel_Paint(object? sender, PaintEventArgs e)
        {
            Panel panel = (Panel)sender!;
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Shadow
            Rectangle shadowRect = new Rectangle(6, 6, panel.Width - 12, panel.Height - 12);
            using (GraphicsPath shadowPath = ThemeColors.GetRoundedRect(shadowRect, 16))
            using (SolidBrush shadowBrush = new SolidBrush(Color.FromArgb(40, 0, 0, 0)))
            {
                g.FillPath(shadowBrush, shadowPath);
            }

            // Card
            Rectangle cardRect = new Rectangle(4, 4, panel.Width - 14, panel.Height - 14);
            using (GraphicsPath cardPath = ThemeColors.GetRoundedRect(cardRect, 16))
            using (SolidBrush cardBrush = new SolidBrush(Color.White))
            {
                g.FillPath(cardBrush, cardPath);
            }
        }

        private void BtnLogin_Click(object? sender, EventArgs e)
        {
            string username = txtUsername.InputText;
            string password = txtPassword.InputText;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ShowMessage("Vui lòng nhập đầy đủ thông tin đăng nhập!", false);
                return;
            }

            var user = Models.UserStore.Login(username, password);
            if (user != null)
            {
                Models.UserStore.CurrentUser = user;
                Hide();
                MainForm mainForm = new MainForm();
                mainForm.FormClosed += (s, args) => Close();
                mainForm.Show();
            }
            else
            {
                ShowMessage("Tên đăng nhập hoặc mật khẩu không đúng!", false);
            }
        }

        private void ShowMessage(string message, bool isSuccess)
        {
            MessageBox.Show(message,
                isSuccess ? "Thành công" : "Lỗi",
                MessageBoxButtons.OK,
                isSuccess ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
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
