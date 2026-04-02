using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using LibraryManagement.Controls;
using LibraryManagement.Helpers;
using LibraryManagement.Models;

namespace LibraryManagement.Forms.Panels
{
    public class RolePanel : UserControl
    {
        private readonly string[] modules = { "Accounts", "Reports", "Settings" };
        private readonly string[] actions = { "View", "Edit" };
        private readonly Dictionary<string, CheckBox> checkboxes = new Dictionary<string, CheckBox>();

        public RolePanel()
        {
            DoubleBuffered = true; Dock = DockStyle.Fill; BackColor = ThemeColors.Background; AutoScroll = true;
            InitializeUI();
        }

        private void InitializeUI()
        {
            Controls.Add(new Label { Text = "PHÂN QUYỀN", Font = ThemeColors.HeaderFont, ForeColor = ThemeColors.TextPrimary, Location = new Point(32, 20), Size = new Size(400, 40), BackColor = Color.Transparent });
            Controls.Add(new Label { Text = "Quản lý vai trò và quyền hạn trong hệ thống", Font = ThemeColors.BodyFont, ForeColor = ThemeColors.TextSecondary, Location = new Point(32, 62), Size = new Size(500, 22), BackColor = Color.Transparent });

            int y = 100;
            var roles = new[] { UserRole.Admin, UserRole.ThuThu, UserRole.DocGia };
            foreach (var role in roles)
            {
                Panel card = new Panel { Location = new Point(32, y), Size = new Size(900, 160), BackColor = Color.Transparent };
                Color accent = role switch
                {
                    UserRole.Admin => ColorTranslator.FromHtml("#8B5CF6"),
                    UserRole.ThuThu => ColorTranslator.FromHtml("#2563EB"),
                    _ => ColorTranslator.FromHtml("#10B981")
                };

                card.Paint += (s, e) =>
                {
                    var g = e.Graphics; g.SmoothingMode = SmoothingMode.AntiAlias;
                    var rect = new Rectangle(2, 2, card.Width - 6, card.Height - 6);
                    using (var path = ThemeColors.GetRoundedRect(rect, 12))
                    using (var bg = new SolidBrush(Color.White)) g.FillPath(bg, path);
                    // Left accent bar
                    using (var aPath = ThemeColors.GetRoundedRect(new Rectangle(2, 2, 5, card.Height - 6), 2))
                    using (var ab = new SolidBrush(accent)) g.FillPath(ab, aPath);
                };

                card.Controls.Add(new Label { Text = role switch { UserRole.Admin => "Quản trị viên", UserRole.ThuThu => "Thủ thư", _ => "Độc giả" }, Font = new Font("Segoe UI", 16, FontStyle.Bold), ForeColor = accent, Location = new Point(24, 16), Size = new Size(300, 32), BackColor = Color.Transparent });
                card.Controls.Add(new Label { Text = "Quyền truy cập theo module/action", Font = ThemeColors.BodyFont, ForeColor = ThemeColors.TextSecondary, Location = new Point(24, 52), Size = new Size(700, 22), BackColor = Color.Transparent });

                int px = 24;
                int py = 86;
                foreach (var module in modules)
                {
                    foreach (var action in actions)
                    {
                        bool allowed = UserStore.HasPermission(role, module, action);
                        string key = $"{role}:{module}:{action}";
                        var chk = new CheckBox
                        {
                            Text = $"{module}.{action}",
                            Checked = allowed,
                            Location = new Point(px, py),
                            Size = new Size(130, 24),
                            Font = ThemeColors.SmallFont
                        };
                        checkboxes[key] = chk;
                        card.Controls.Add(chk);
                        px += 138;
                    }
                }

                int count = UserStore.Users.Count(u => u.Role == role);
                card.Controls.Add(new Label { Text = $"{count} người dùng", Font = ThemeColors.SubTitleFont, ForeColor = ThemeColors.TextSecondary, Location = new Point(740, 16), Size = new Size(140, 28), BackColor = Color.Transparent, TextAlign = ContentAlignment.MiddleRight });

                Controls.Add(card);
                y += 180;
            }

            var btnSave = new RoundedButton { Text = "Lưu phân quyền", Size = new Size(160, 40), Location = new Point(32, y), ButtonColor = ThemeColors.Primary, Font = ThemeColors.ButtonFont };
            btnSave.Click += (_, _) =>
            {
                foreach (var kv in checkboxes)
                {
                    var parts = kv.Key.Split(':');
                    var role = (UserRole)Enum.Parse(typeof(UserRole), parts[0]);
                    LibraryDataService.SetRolePermission(role, parts[1], parts[2], kv.Value.Checked);
                }
                MessageBox.Show("Đã lưu cấu hình phân quyền.", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };
            Controls.Add(btnSave);
        }
    }
}
