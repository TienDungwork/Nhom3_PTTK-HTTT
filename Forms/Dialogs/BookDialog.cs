using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using LibraryManagement.Controls;
using LibraryManagement.Helpers;

namespace LibraryManagement.Forms.Dialogs
{
    public class BookDialog : Form
    {
        private RoundedTextBox txtMaSach = null!;
        private RoundedTextBox txtTenSach = null!;
        private RoundedTextBox txtTacGia = null!;
        private RoundedTextBox txtISBN = null!;
        private ComboBox cboTheLoai = null!;
        private RoundedTextBox txtSoLuong = null!;

        public Models.Book? ResultBook { get; private set; }
        public bool IsEditMode { get; set; }

        public BookDialog(Models.Book? book = null)
        {
            IsEditMode = book != null;
            InitializeComponent();
            if (book != null) LoadBook(book);
        }

        private void InitializeComponent()
        {
            Text = IsEditMode ? "Chỉnh sửa sách" : "Thêm sách mới";
            Size = new Size(500, 580);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.None;
            BackColor = ThemeColors.Background;
            DoubleBuffered = true;
            ShowInTaskbar = false;

            // Main paint for rounded form border
            Paint += (s, e) =>
            {
                var g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                Rectangle rect = new Rectangle(0, 0, Width - 1, Height - 1);
                using (GraphicsPath path = ThemeColors.GetRoundedRect(rect, 12))
                using (Pen pen = new Pen(ThemeColors.Border, 1))
                {
                    g.DrawPath(pen, path);
                }
            };

            // Header
            Panel header = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = Color.White
            };
            header.Paint += (s, e) =>
            {
                e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
                using (SolidBrush b = new SolidBrush(ThemeColors.TextPrimary))
                    e.Graphics.DrawString(
                        IsEditMode ? "✏️  Chỉnh sửa sách" : "📖  Thêm sách mới",
                        ThemeColors.TitleFont, b, 24, 18);
                using (Pen p = new Pen(ThemeColors.Border))
                    e.Graphics.DrawLine(p, 0, 59, header.Width, 59);
            };

            // Close button
            Label btnClose = new Label
            {
                Text = "✕",
                Font = new Font("Segoe UI", 12),
                ForeColor = ThemeColors.TextSecondary,
                Size = new Size(36, 36),
                Location = new Point(Width - 48, 12),
                TextAlign = ContentAlignment.MiddleCenter,
                Cursor = Cursors.Hand,
                BackColor = Color.Transparent,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            btnClose.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };
            btnClose.MouseEnter += (s, e) => btnClose.ForeColor = ThemeColors.Danger;
            btnClose.MouseLeave += (s, e) => btnClose.ForeColor = ThemeColors.TextSecondary;
            header.Controls.Add(btnClose);
            Controls.Add(header);

            // Content panel
            Panel content = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(32, 16, 32, 16),
                BackColor = Color.White
            };
            Controls.Add(content);
            content.BringToFront();

            int y = 16;
            int labelH = 22;
            int inputH = 44;
            int gap = 10;

            AddFieldLabel(content, "Mã sách", ref y, labelH);
            txtMaSach = AddTextField(content, "Nhập mã sách", ref y, inputH, gap);

            AddFieldLabel(content, "Tên sách", ref y, labelH);
            txtTenSach = AddTextField(content, "Nhập tên sách", ref y, inputH, gap);

            AddFieldLabel(content, "Tác giả", ref y, labelH);
            txtTacGia = AddTextField(content, "Nhập tên tác giả", ref y, inputH, gap);

            AddFieldLabel(content, "ISBN", ref y, labelH);
            txtISBN = AddTextField(content, "Nhập mã ISBN", ref y, inputH, gap);

            AddFieldLabel(content, "Thể loại", ref y, labelH);
            cboTheLoai = new ComboBox
            {
                Location = new Point(32, y),
                Size = new Size(Width - 96, 36),
                Font = ThemeColors.InputFont,
                DropDownStyle = ComboBoxStyle.DropDownList,
                FlatStyle = FlatStyle.Flat,
                Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top
            };
            foreach (var cat in Models.SampleData.Categories)
                if (cat != "Tất cả") cboTheLoai.Items.Add(cat);
            if (cboTheLoai.Items.Count > 0) cboTheLoai.SelectedIndex = 0;
            content.Controls.Add(cboTheLoai);
            y += 36 + gap;

            AddFieldLabel(content, "Số lượng", ref y, labelH);
            txtSoLuong = AddTextField(content, "Nhập số lượng", ref y, inputH, gap);

            // Button panel
            Panel btnPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 70,
                BackColor = Color.White,
                Padding = new Padding(32, 0, 32, 0)
            };
            btnPanel.Paint += (s, e) =>
            {
                using (Pen p = new Pen(ThemeColors.Border))
                    e.Graphics.DrawLine(p, 0, 0, btnPanel.Width, 0);
            };

            RoundedButton btnSave = new RoundedButton
            {
                Text = "Lưu",
                IconText = "💾",
                Size = new Size(120, 42),
                Location = new Point(Width - 290, 14),
                ButtonColor = ThemeColors.Primary,
                Anchor = AnchorStyles.Right
            };
            btnSave.Click += BtnSave_Click;

            RoundedButton btnCancel = new RoundedButton
            {
                Text = "Hủy",
                Size = new Size(120, 42),
                Location = new Point(Width - 160, 14),
                ButtonColor = ThemeColors.Border,
                TextColor = ThemeColors.TextPrimary,
                HoverColor = Color.FromArgb(220, 220, 220),
                IsOutline = true,
                Anchor = AnchorStyles.Right
            };
            btnCancel.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };

            btnPanel.Controls.Add(btnSave);
            btnPanel.Controls.Add(btnCancel);
            Controls.Add(btnPanel);
        }

        private void AddFieldLabel(Panel parent, string text, ref int y, int height)
        {
            Label lbl = new Label
            {
                Text = text,
                Font = new Font("Segoe UI Semibold", 10),
                ForeColor = ThemeColors.TextPrimary,
                Location = new Point(32, y),
                Size = new Size(Width - 96, height),
                BackColor = Color.Transparent
            };
            parent.Controls.Add(lbl);
            y += height + 2;
        }

        private RoundedTextBox AddTextField(Panel parent, string placeholder, ref int y, int height, int gap)
        {
            var txt = new RoundedTextBox
            {
                Placeholder = placeholder,
                Location = new Point(32, y),
                Size = new Size(Width - 96, height),
                Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top
            };
            parent.Controls.Add(txt);
            y += height + gap;
            return txt;
        }

        private void LoadBook(Models.Book book)
        {
            txtMaSach.Text = book.MaSach;
            txtTenSach.Text = book.TenSach;
            txtTacGia.Text = book.TacGia;
            txtISBN.Text = book.ISBN;
            txtSoLuong.Text = book.SoLuong.ToString();
            for (int i = 0; i < cboTheLoai.Items.Count; i++)
            {
                if (cboTheLoai.Items[i].ToString() == book.TheLoai)
                {
                    cboTheLoai.SelectedIndex = i;
                    break;
                }
            }
        }

        private void BtnSave_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTenSach.InputText) ||
                string.IsNullOrWhiteSpace(txtMaSach.InputText))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin bắt buộc!",
                    "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int qty = 0;
            int.TryParse(txtSoLuong.InputText, out qty);

            ResultBook = new Models.Book
            {
                MaSach = txtMaSach.InputText,
                TenSach = txtTenSach.InputText,
                TacGia = txtTacGia.InputText,
                ISBN = txtISBN.InputText,
                TheLoai = cboTheLoai.SelectedItem?.ToString() ?? "",
                SoLuong = qty,
                TrangThai = qty > 0 ? "Có sẵn" : "Hết sách"
            };

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
