using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using LibraryManagement.Controls;
using LibraryManagement.Helpers;

namespace LibraryManagement.Forms.Dialogs
{
    public class ReaderDialog : Form
    {
        private RoundedTextBox txtMaDG = null!;
        private RoundedTextBox txtHoTen = null!;
        private RoundedTextBox txtEmail = null!;
        private RoundedTextBox txtSDT = null!;
        private RoundedTextBox txtDiaChi = null!;

        public Models.Reader? ResultReader { get; private set; }
        public bool IsEditMode { get; set; }

        public ReaderDialog(Models.Reader? reader = null)
        {
            IsEditMode = reader != null;
            InitializeComponent();
            if (reader != null) LoadReader(reader);
        }

        private void InitializeComponent()
        {
            Text = IsEditMode ? "Chỉnh sửa độc giả" : "Thêm độc giả mới";
            Size = new Size(500, 520);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.None;
            BackColor = Color.White;
            DoubleBuffered = true;
            ShowInTaskbar = false;

            Paint += (s, e) =>
            {
                var g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                Rectangle rect = new Rectangle(0, 0, Width - 1, Height - 1);
                using (GraphicsPath path = ThemeColors.GetRoundedRect(rect, 12))
                using (Pen pen = new Pen(ThemeColors.Border, 1))
                    g.DrawPath(pen, path);
            };

            // Header
            Panel header = new Panel { Dock = DockStyle.Top, Height = 60, BackColor = Color.White };
            header.Paint += (s, e) =>
            {
                e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
                using (SolidBrush b = new SolidBrush(ThemeColors.TextPrimary))
                    e.Graphics.DrawString(
                        IsEditMode ? "✏️  Chỉnh sửa độc giả" : "👤  Thêm độc giả mới",
                        ThemeColors.TitleFont, b, 24, 18);
                using (Pen p = new Pen(ThemeColors.Border))
                    e.Graphics.DrawLine(p, 0, 59, header.Width, 59);
            };

            Label btnClose = new Label
            {
                Text = "✕", Font = new Font("Segoe UI", 12), ForeColor = ThemeColors.TextSecondary,
                Size = new Size(36, 36), Location = new Point(Width - 48, 12),
                TextAlign = ContentAlignment.MiddleCenter, Cursor = Cursors.Hand, BackColor = Color.Transparent,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            btnClose.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };
            btnClose.MouseEnter += (s, e) => btnClose.ForeColor = ThemeColors.Danger;
            btnClose.MouseLeave += (s, e) => btnClose.ForeColor = ThemeColors.TextSecondary;
            header.Controls.Add(btnClose);
            Controls.Add(header);

            Panel content = new Panel { Dock = DockStyle.Fill, Padding = new Padding(32, 16, 32, 16), BackColor = Color.White };
            Controls.Add(content);
            content.BringToFront();

            int y = 16;
            AddFieldLabel(content, "Mã độc giả", ref y);
            txtMaDG = AddTextField(content, "Nhập mã độc giả", ref y);
            AddFieldLabel(content, "Họ và tên", ref y);
            txtHoTen = AddTextField(content, "Nhập họ tên", ref y);
            AddFieldLabel(content, "Email", ref y);
            txtEmail = AddTextField(content, "Nhập email", ref y);
            AddFieldLabel(content, "Số điện thoại", ref y);
            txtSDT = AddTextField(content, "Nhập số điện thoại", ref y);
            AddFieldLabel(content, "Địa chỉ", ref y);
            txtDiaChi = AddTextField(content, "Nhập địa chỉ", ref y);

            Panel btnPanel = new Panel { Dock = DockStyle.Bottom, Height = 70, BackColor = Color.White };
            btnPanel.Paint += (s, e) => { using (Pen p = new Pen(ThemeColors.Border)) e.Graphics.DrawLine(p, 0, 0, btnPanel.Width, 0); };

            RoundedButton btnSave = new RoundedButton
            {
                Text = "Lưu", IconText = "💾", Size = new Size(120, 42),
                Location = new Point(Width - 290, 14), ButtonColor = ThemeColors.Primary, Anchor = AnchorStyles.Right
            };
            btnSave.Click += BtnSave_Click;

            RoundedButton btnCancel = new RoundedButton
            {
                Text = "Hủy", Size = new Size(120, 42),
                Location = new Point(Width - 160, 14), ButtonColor = ThemeColors.Border,
                TextColor = ThemeColors.TextPrimary, HoverColor = Color.FromArgb(220, 220, 220),
                IsOutline = true, Anchor = AnchorStyles.Right
            };
            btnCancel.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };

            btnPanel.Controls.AddRange(new Control[] { btnSave, btnCancel });
            Controls.Add(btnPanel);
        }

        private void AddFieldLabel(Panel parent, string text, ref int y)
        {
            parent.Controls.Add(new Label
            {
                Text = text, Font = new Font("Segoe UI Semibold", 10),
                ForeColor = ThemeColors.TextPrimary, Location = new Point(32, y),
                Size = new Size(400, 22), BackColor = Color.Transparent
            });
            y += 24;
        }

        private RoundedTextBox AddTextField(Panel parent, string placeholder, ref int y)
        {
            var txt = new RoundedTextBox
            {
                Placeholder = placeholder, Location = new Point(32, y),
                Size = new Size(Width - 96, 44), Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top
            };
            parent.Controls.Add(txt);
            y += 54;
            return txt;
        }

        private void LoadReader(Models.Reader r)
        {
            txtMaDG.Text = r.MaDocGia;
            txtHoTen.Text = r.HoTen;
            txtEmail.Text = r.Email;
            txtSDT.Text = r.SDT;
            txtDiaChi.Text = r.DiaChi;
        }

        private void BtnSave_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtHoTen.InputText) || string.IsNullOrWhiteSpace(txtMaDG.InputText))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin!", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            ResultReader = new Models.Reader
            {
                MaDocGia = txtMaDG.InputText,
                HoTen = txtHoTen.InputText,
                Email = txtEmail.InputText,
                SDT = txtSDT.InputText,
                DiaChi = txtDiaChi.InputText,
                NgayDangKy = DateTime.Now
            };

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
