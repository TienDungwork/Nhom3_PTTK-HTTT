using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using LibraryManagement.Controls;
using LibraryManagement.Helpers;
using LibraryManagement.Models;

namespace LibraryManagement.Forms.Panels
{
    public class SettingsPanel : UserControl
    {
        private TextBox txtMaxBooks = null!;
        private TextBox txtBorrowDays = null!;
        private TextBox txtFeePerDay = null!;
        private TextBox txtLibraryName = null!;
        private TextBox txtLibraryContact = null!;
        private CheckBox chkBorrowRequest = null!;
        private CheckBox chkInventory = null!;
        private CheckBox chkAutoNotify = null!;

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
            Panel card = new Panel { Location = new Point(32, 100), Size = new Size(760, 480), BackColor = Color.Transparent };
            card.Paint += (s, e) =>
            {
                var g = e.Graphics; g.SmoothingMode = SmoothingMode.AntiAlias;
                var rect = new Rectangle(2, 2, card.Width - 6, card.Height - 6);
                using (var path = ThemeColors.GetRoundedRect(rect, 12))
                using (var bg = new SolidBrush(Color.White)) g.FillPath(bg, path);
            };
            Controls.Add(card);

            int y = 24;
            AddSettingGroup(card, "Cài đặt chung", ref y);
            txtBorrowDays = AddSettingRow(card, "Số ngày mượn mặc định", LibraryDataService.GetSetting("default_borrow_days", "14"), ref y);
            txtFeePerDay = AddSettingRow(card, "Tiền phạt mỗi ngày (VNĐ)", LibraryDataService.GetSetting("late_fee_per_day", "5000"), ref y);
            txtMaxBooks = AddSettingRow(card, "Số sách mượn tối đa / độc giả", LibraryDataService.GetSetting("max_borrow_books", "5"), ref y);

            y += 20;
            AddSettingGroup(card, "Thông tin thư viện", ref y);
            txtLibraryName = AddSettingRow(card, "Tên thư viện", LibraryDataService.GetSetting("library_name", "Thư viện Đại học Thủy Lợi"), ref y);
            txtLibraryContact = AddSettingRow(card, "Thông tin liên hệ", LibraryDataService.GetSetting("library_contact", "library@tlu.edu.vn"), ref y);

            y += 20;
            AddSettingGroup(card, "Bật / tắt tính năng", ref y);
            chkBorrowRequest = AddToggle(card, "Cho phép đặt mượn", LibraryDataService.GetFeatureToggle("borrow_request", true), ref y);
            chkInventory = AddToggle(card, "Cho phép kiểm kê kho", LibraryDataService.GetFeatureToggle("inventory_check", true), ref y);
            chkAutoNotify = AddToggle(card, "Tự động nhắc hạn/quá hạn", LibraryDataService.GetFeatureToggle("auto_notify", true), ref y);

            y += 20;
            RoundedButton btnSave = new RoundedButton
            {
                Text = "Lưu cài đặt",
                Size = new Size(160, 44), Location = new Point(24, y),
                ButtonColor = ThemeColors.Primary, Font = ThemeColors.ButtonFont
            };
            btnSave.Click += (s, e) =>
            {
                LibraryDataService.SetSetting("default_borrow_days", txtBorrowDays.Text.Trim());
                LibraryDataService.SetSetting("late_fee_per_day", txtFeePerDay.Text.Trim());
                LibraryDataService.SetSetting("max_borrow_books", txtMaxBooks.Text.Trim());
                LibraryDataService.SetSetting("library_name", txtLibraryName.Text.Trim());
                LibraryDataService.SetSetting("library_contact", txtLibraryContact.Text.Trim());
                LibraryDataService.SetFeatureToggle("borrow_request", chkBorrowRequest.Checked);
                LibraryDataService.SetFeatureToggle("inventory_check", chkInventory.Checked);
                LibraryDataService.SetFeatureToggle("auto_notify", chkAutoNotify.Checked);
                MessageBox.Show("Lưu cài đặt thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };
            card.Controls.Add(btnSave);
        }

        private void AddSettingGroup(Panel parent, string title, ref int y)
        {
            parent.Controls.Add(new Label { Text = title, Font = ThemeColors.SubTitleFont, ForeColor = ThemeColors.TextPrimary, Location = new Point(24, y), Size = new Size(540, 28), BackColor = Color.Transparent });
            y += 34;
        }

        private TextBox AddSettingRow(Panel parent, string label, string value, ref int y)
        {
            parent.Controls.Add(new Label { Text = label, Font = ThemeColors.BodyFont, ForeColor = ThemeColors.TextSecondary, Location = new Point(24, y + 8), Size = new Size(240, 22), BackColor = Color.Transparent });
            var txt = new TextBox { Text = value, Location = new Point(270, y), Size = new Size(460, 32), Font = ThemeColors.BodyFont, BorderStyle = BorderStyle.FixedSingle };
            parent.Controls.Add(txt);
            y += 48;
            return txt;
        }

        private CheckBox AddToggle(Panel parent, string label, bool value, ref int y)
        {
            var chk = new CheckBox
            {
                Text = label,
                Checked = value,
                Location = new Point(24, y),
                Size = new Size(360, 28),
                Font = ThemeColors.BodyFont
            };
            parent.Controls.Add(chk);
            y += 34;
            return chk;
        }
    }
}
