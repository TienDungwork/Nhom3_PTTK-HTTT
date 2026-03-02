using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using LibraryManagement.Controls;
using LibraryManagement.Helpers;

namespace LibraryManagement.Forms.Panels
{
    public class SettingsPanel : UserControl
    {
        public SettingsPanel()
        {
            DoubleBuffered = true;
            Dock = DockStyle.Fill;
            BackColor = ThemeColors.Background;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            Controls.Add(new Label { Text = "CÀI ĐẶT", Font = ThemeColors.HeaderFont, ForeColor = ThemeColors.TextPrimary, Location = new Point(32, 20), Size = new Size(400, 40), BackColor = Color.Transparent });
            Controls.Add(new Label { Text = "Tùy chỉnh hệ thống thư viện", Font = ThemeColors.BodyFont, ForeColor = ThemeColors.TextSecondary, Location = new Point(32, 62), Size = new Size(400, 22), BackColor = Color.Transparent });

            // Settings card
            Panel card = new Panel { Location = new Point(32, 100), Size = new Size(600, 420), BackColor = Color.Transparent };
            card.Paint += (s, e) =>
            {
                var g = e.Graphics; g.SmoothingMode = SmoothingMode.AntiAlias;
                var rect = new Rectangle(2, 2, card.Width - 6, card.Height - 6);
                using (var path = ThemeColors.GetRoundedRect(rect, 12))
                using (var bg = new SolidBrush(Color.White)) g.FillPath(bg, path);
            };
            Controls.Add(card);

            int y = 24;
            AddSettingGroup(card, "⚙️  Cài đặt chung", ref y);
            AddSettingRow(card, "Số ngày mượn tối đa", "14", ref y);
            AddSettingRow(card, "Tiền phạt mỗi ngày (VNĐ)", "5,000", ref y);
            AddSettingRow(card, "Số sách mượn tối đa / độc giả", "5", ref y);

            y += 20;
            AddSettingGroup(card, "🏢  Thông tin thư viện", ref y);
            AddSettingRow(card, "Tên thư viện", "Thư viện Đại học ABC", ref y);
            AddSettingRow(card, "Địa chỉ", "123 Đường XYZ, TP.HCM", ref y);
            AddSettingRow(card, "Số điện thoại", "028-1234-5678", ref y);
            AddSettingRow(card, "Email", "library@abc.edu.vn", ref y);

            y += 20;
            RoundedButton btnSave = new RoundedButton
            {
                Text = "Lưu cài đặt", IconText = "💾",
                Size = new Size(160, 44), Location = new Point(24, y),
                ButtonColor = ThemeColors.Primary, Font = ThemeColors.ButtonFont
            };
            btnSave.Click += (s, e) => MessageBox.Show("Lưu cài đặt thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
            card.Controls.Add(btnSave);
        }

        private void AddSettingGroup(Panel parent, string title, ref int y)
        {
            parent.Controls.Add(new Label { Text = title, Font = ThemeColors.SubTitleFont, ForeColor = ThemeColors.TextPrimary, Location = new Point(24, y), Size = new Size(540, 28), BackColor = Color.Transparent });
            y += 34;
        }

        private void AddSettingRow(Panel parent, string label, string value, ref int y)
        {
            parent.Controls.Add(new Label { Text = label, Font = ThemeColors.BodyFont, ForeColor = ThemeColors.TextSecondary, Location = new Point(24, y + 8), Size = new Size(240, 22), BackColor = Color.Transparent });
            var txt = new RoundedTextBox { Text = value, Location = new Point(270, y), Size = new Size(290, 40) };
            parent.Controls.Add(txt);
            y += 48;
        }
    }
}
