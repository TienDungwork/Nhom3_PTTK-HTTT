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
        private Label lblError = null!;

        public LoginForm()
        {
            Text = "Đăng nhập - Thư viện ĐH Thủy Lợi";
            Size = new Size(480, 620);
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            BackColor = ThemeColors.Background;
            Font = ThemeColors.BodyFont;

            // --- Main card panel ---
            var card = new Panel
            {
                Size = new Size(400, 480),
                Location = new Point(40, 60),
                BackColor = Color.White,
                Anchor = AnchorStyles.None
            };
            card.Paint += (s, e) =>
            {
                var g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                using var shadow = new SolidBrush(Color.FromArgb(15, 0, 0, 0));
                using var shadowPath = ThemeColors.GetRoundedRect(new Rectangle(4, 4, card.Width - 4, card.Height - 4), 16);
                g.FillPath(shadow, shadowPath);
                using var bg = new SolidBrush(Color.White);
                using var bgPath = ThemeColors.GetRoundedRect(new Rectangle(0, 0, card.Width - 4, card.Height - 4), 16);
                g.FillPath(bg, bgPath);
            };
            Controls.Add(card);

            // --- Logo icon ---
            var lblLogo = new Label
            {
                Text = "\uE736",
                Font = new Font("Segoe MDL2 Assets", 36),
                ForeColor = ThemeColors.Primary,
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(80, 80),
                Location = new Point(160, 24),
                BackColor = Color.Transparent
            };
            card.Controls.Add(lblLogo);

            // --- Title ---
            var lblTitle = new Label
            {
                Text = "ĐĂNG NHẬP HỆ THỐNG",
                Font = new Font("Segoe UI Semibold", 16),
                ForeColor = ThemeColors.TextPrimary,
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(360, 32),
                Location = new Point(20, 108),
                BackColor = Color.Transparent
            };
            card.Controls.Add(lblTitle);

            var lblSubtitle = new Label
            {
                Text = "Thư viện Trường Đại học Thủy Lợi",
                Font = ThemeColors.BodyFont,
                ForeColor = ThemeColors.TextSecondary,
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(360, 22),
                Location = new Point(20, 142),
                BackColor = Color.Transparent
            };
            card.Controls.Add(lblSubtitle);

            // --- Mã người dùng ---
            var lblUser = new Label
            {
                Text = "Mã người dùng",
                Font = ThemeColors.BodyFont,
                ForeColor = ThemeColors.TextSecondary,
                Location = new Point(40, 186),
                Size = new Size(320, 20),
                BackColor = Color.Transparent
            };
            card.Controls.Add(lblUser);

            txtUsername = new RoundedTextBox
            {
                Location = new Point(40, 208),
                Size = new Size(320, 44),
                Placeholder = "Nhập mã người dùng"
            };
            card.Controls.Add(txtUsername);

            // --- Mật khẩu ---
            var lblPass = new Label
            {
                Text = "Mật khẩu",
                Font = ThemeColors.BodyFont,
                ForeColor = ThemeColors.TextSecondary,
                Location = new Point(40, 264),
                Size = new Size(320, 20),
                BackColor = Color.Transparent
            };
            card.Controls.Add(lblPass);

            txtPassword = new RoundedTextBox
            {
                Location = new Point(40, 286),
                Size = new Size(320, 44),
                Placeholder = "Nhập mật khẩu",
                IsPassword = true
            };
            card.Controls.Add(txtPassword);

            // --- Error label ---
            lblError = new Label
            {
                Text = "",
                Font = ThemeColors.SmallFont,
                ForeColor = ThemeColors.Danger,
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(40, 340),
                Size = new Size(320, 22),
                BackColor = Color.Transparent,
                Visible = false
            };
            card.Controls.Add(lblError);

            // --- Login button ---
            var btnLogin = new RoundedButton
            {
                Text = "Đăng nhập",
                Size = new Size(320, 48),
                Location = new Point(40, 370),
                ButtonColor = ThemeColors.Primary,
                Font = new Font("Segoe UI Semibold", 12),
                ForeColor = Color.White,
                Cursor = Cursors.Hand
            };
            btnLogin.Click += BtnLogin_Click;
            card.Controls.Add(btnLogin);

            // --- Hint ---
            var lblHint = new Label
            {
                Text = "admin / admin123  |  thuthu / thuthu123  |  docgia / docgia123",
                Font = ThemeColors.SmallFont,
                ForeColor = ThemeColors.TextMuted,
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(20, 434),
                Size = new Size(360, 20),
                BackColor = Color.Transparent
            };
            card.Controls.Add(lblHint);

            // Enter key
            txtPassword.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) BtnLogin_Click(s, e); };
            txtUsername.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) txtPassword.Focus(); };
        }

        private void BtnLogin_Click(object? sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();

            // Validation
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ShowError("Vui lòng nhập đầy đủ thông tin!");
                return;
            }

            var user = UserStore.Login(username, password);
            if (user == null)
            {
                ShowError("Mã người dùng hoặc mật khẩu không đúng!");
                return;
            }

            UserStore.CurrentUser = user;

            Form mainForm = user.Role switch
            {
                UserRole.ThuThu => new LibrarianForm(),
                UserRole.DocGia => new ReaderForm(),
                UserRole.Admin => new AdminForm(),
                _ => new LibrarianForm()
            };

            Hide();
            mainForm.FormClosed += (s, args) =>
            {
                txtPassword.Text = "";
                lblError.Visible = false;
                Show();
            };
            mainForm.Show();
        }

        private void ShowError(string message)
        {
            lblError.Text = message;
            lblError.Visible = true;
        }
    }
}
