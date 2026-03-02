using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using LibraryManagement.Controls;
using LibraryManagement.Helpers;
using LibraryManagement.Models;

namespace LibraryManagement.Forms.Dialogs
{
    public class BookInfoDialog : Form
    {
        public BookInfoDialog(Book book)
        {
            Text = "Thông tin sách";
            Size = new Size(520, 540);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            BackColor = Color.White;
            Font = ThemeColors.BodyFont;

            // Cover image area
            var coverPanel = new Panel { Location = new Point(20, 20), Size = new Size(140, 190), BackColor = ThemeColors.Background };
            coverPanel.Paint += (s, e) =>
            {
                var g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                using var path = ThemeColors.GetRoundedRect(new Rectangle(0, 0, coverPanel.Width, coverPanel.Height), 10);
                using var bg = new SolidBrush(Color.FromArgb(230, 235, 245));
                g.FillPath(bg, path);
                // Default book icon
                using var iconBrush = new SolidBrush(ThemeColors.Primary);
                g.DrawString("\uE736", new Font("Segoe MDL2 Assets", 40), iconBrush, 30, 55);
            };
            Controls.Add(coverPanel);

            // Book details
            int x = 180, y = 20;
            AddField("Nhan đề:", book.TenSach, x, ref y, true);
            AddField("Tác giả:", book.TacGia, x, ref y);
            AddField("Chủ đề:", book.ChuDe, x, ref y);
            AddField("Năm xuất bản:", book.NamXuatBan.ToString(), x, ref y);
            AddField("Nhà xuất bản:", book.NhaXuatBan, x, ref y);
            AddField("ISBN:", book.ISBN, x, ref y);

            // Second column below image
            y = 230;
            AddField("URI:", book.URI, 20, ref y);
            AddField("Bộ sưu tập:", book.BoSuuTap, 20, ref y);
            AddField("Thể loại:", book.TheLoai, 20, ref y);
            AddField("Mã sách:", book.MaSach, 20, ref y);
            AddField("Số lượng hiện có:", book.SoLuongHienCo.ToString() + " / " + book.SoLuong.ToString(), 20, ref y);
            AddField("Vị trí kho:", book.ViTriKho, 20, ref y);

            // Status badge
            var lblStatus = new Label
            {
                Text = book.TrangThai,
                Font = new Font("Segoe UI Semibold", 10),
                ForeColor = book.TrangThai == "Có sẵn" ? ThemeColors.Success : ThemeColors.Danger,
                BackColor = book.TrangThai == "Có sẵn" ? ThemeColors.SuccessLight : ThemeColors.DangerLight,
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(100, 28),
                Location = new Point(20, y + 8)
            };
            Controls.Add(lblStatus);

            // Close button
            var btnClose = new RoundedButton
            {
                Text = "Đóng",
                Size = new Size(120, 40),
                Location = new Point(375, y + 4),
                ButtonColor = ThemeColors.TextSecondary,
                Font = ThemeColors.ButtonFont
            };
            btnClose.Click += (s, e) => Close();
            Controls.Add(btnClose);
        }

        private void AddField(string label, string value, int x, ref int y, bool bold = false)
        {
            Controls.Add(new Label
            {
                Text = label,
                Font = ThemeColors.SmallFont,
                ForeColor = ThemeColors.TextSecondary,
                Location = new Point(x, y),
                Size = new Size(120, 18),
                BackColor = Color.Transparent
            });
            Controls.Add(new Label
            {
                Text = value,
                Font = bold ? ThemeColors.SubTitleFont : ThemeColors.BodyFont,
                ForeColor = ThemeColors.TextPrimary,
                Location = new Point(x, y + 18),
                Size = new Size(300, bold ? 26 : 22),
                BackColor = Color.Transparent
            });
            y += bold ? 48 : 44;
        }
    }
}
