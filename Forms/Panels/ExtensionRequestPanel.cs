using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using LibraryManagement.Controls;
using LibraryManagement.Helpers;
using LibraryManagement.Models;

namespace LibraryManagement.Forms.Panels
{
    public class ExtensionRequestPanel : UserControl
    {
        private DataGridView dgv = null!;

        public ExtensionRequestPanel()
        {
            DoubleBuffered = true; Dock = DockStyle.Fill; BackColor = ThemeColors.Background;
            InitializeUI();
        }

        private void InitializeUI()
        {
            Controls.Add(new Label { Text = "YÊU CẦU GIA HẠN", Font = ThemeColors.HeaderFont, ForeColor = ThemeColors.TextPrimary, Location = new Point(32, 20), Size = new Size(400, 40), BackColor = Color.Transparent });
            Controls.Add(new Label { Text = "Gửi yêu cầu gia hạn thời gian mượn sách", Font = ThemeColors.BodyFont, ForeColor = ThemeColors.TextSecondary, Location = new Point(32, 62), Size = new Size(500, 22), BackColor = Color.Transparent });

            // New request form
            Panel formCard = new Panel { Location = new Point(32, 96), Size = new Size(420, 280), BackColor = Color.Transparent };
            formCard.Paint += (s, e) =>
            {
                var g = e.Graphics; g.SmoothingMode = SmoothingMode.AntiAlias;
                using (var path = ThemeColors.GetRoundedRect(new Rectangle(2, 2, formCard.Width - 6, formCard.Height - 6), 12))
                using (var bg = new SolidBrush(Color.White)) g.FillPath(bg, path);
            };
            formCard.Controls.Add(new Label { Text = "📝  Tạo yêu cầu mới", Font = ThemeColors.SubTitleFont, ForeColor = ThemeColors.TextPrimary, Location = new Point(20, 16), Size = new Size(380, 28), BackColor = Color.Transparent });

            formCard.Controls.Add(new Label { Text = "Chọn sách cần gia hạn", Font = ThemeColors.BodyFont, ForeColor = ThemeColors.TextSecondary, Location = new Point(20, 56), Size = new Size(380, 20), BackColor = Color.Transparent });
            ComboBox cboBook = new ComboBox { Location = new Point(20, 80), Size = new Size(380, 36), DropDownStyle = ComboBoxStyle.DropDownList, Font = ThemeColors.BodyFont };
            var currentBorrows = SampleData.BorrowRecords.Where(b => b.NgayTraThuc == null).ToList();
            foreach (var b in currentBorrows)
            {
                string name = SampleData.Books.FirstOrDefault(bk => bk.MaSach == b.MaSach)?.TenSach ?? b.MaSach;
                cboBook.Items.Add(name);
            }
            if (cboBook.Items.Count > 0) cboBook.SelectedIndex = 0;
            formCard.Controls.Add(cboBook);

            formCard.Controls.Add(new Label { Text = "Số ngày gia hạn", Font = ThemeColors.BodyFont, ForeColor = ThemeColors.TextSecondary, Location = new Point(20, 128), Size = new Size(380, 20), BackColor = Color.Transparent });
            NumericUpDown numDays = new NumericUpDown { Location = new Point(20, 152), Size = new Size(120, 36), Minimum = 1, Maximum = 30, Value = 7, Font = ThemeColors.BodyFont };
            formCard.Controls.Add(numDays);

            formCard.Controls.Add(new Label { Text = "Lý do", Font = ThemeColors.BodyFont, ForeColor = ThemeColors.TextSecondary, Location = new Point(20, 192), Size = new Size(380, 20), BackColor = Color.Transparent });
            var txtReason = new RoundedTextBox { Placeholder = "Nhập lý do gia hạn...", Location = new Point(20, 216), Size = new Size(380, 44) };
            formCard.Controls.Add(txtReason);

            Controls.Add(formCard);

            // Submit button
            var btnSubmit = new RoundedButton { Text = "Gửi yêu cầu", Size = new Size(180, 44), Location = new Point(32, 390), ButtonColor = ColorTranslator.FromHtml("#10B981"), Font = ThemeColors.ButtonFont };
            btnSubmit.Click += (s, e) =>
            {
                if (cboBook.SelectedItem == null) { MessageBox.Show("Vui lòng chọn sách!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
                MessageBox.Show("Yêu cầu gia hạn đã được gửi thành công!\nVui lòng chờ thủ thư phê duyệt.", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };
            Controls.Add(btnSubmit);

            // Request history
            Controls.Add(new Label { Text = "📋  Lịch sử yêu cầu", Font = ThemeColors.SubTitleFont, ForeColor = ThemeColors.TextPrimary, Location = new Point(470, 96), Size = new Size(300, 28), BackColor = Color.Transparent });

            dgv = new DataGridView { Location = new Point(470, 130), Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom, ReadOnly = true, AllowUserToAddRows = false };
            ModernDataGridView.ApplyStyle(dgv);
            dgv.Columns.Add("TenSach", "Tên sách");
            dgv.Columns.Add("NgayYC", "Ngày yêu cầu");
            dgv.Columns.Add("SoNgay", "Số ngày");
            dgv.Columns.Add("TrangThai", "Trạng thái");
            dgv.Columns["TenSach"]!.FillWeight = 200;

            foreach (var r in UserStore.ExtensionRequests)
                dgv.Rows.Add(r.TenSach, r.NgayYeuCau.ToString("dd/MM/yyyy"), $"{r.SoNgayGiaHan} ngày", r.TrangThai);

            dgv.CellFormatting += (s, e) =>
            {
                if (e.ColumnIndex == dgv.Columns["TrangThai"]!.Index)
                {
                    string val = e.Value?.ToString() ?? "";
                    if (val == "Đã duyệt") e.CellStyle.ForeColor = ThemeColors.Success;
                    else if (val == "Từ chối") e.CellStyle.ForeColor = ThemeColors.Danger;
                    else e.CellStyle.ForeColor = ThemeColors.Warning;
                }
            };

            Controls.Add(dgv);
            Resize += (s, e) => dgv.Size = new Size(Width - 502, Height - 150);
            dgv.Size = new Size(Width - 502, Height - 150);
        }
    }
}
