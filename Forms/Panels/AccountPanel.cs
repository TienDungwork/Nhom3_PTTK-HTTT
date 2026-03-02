using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using LibraryManagement.Controls;
using LibraryManagement.Helpers;
using LibraryManagement.Models;

namespace LibraryManagement.Forms.Panels
{
    public class AccountPanel : UserControl
    {
        private DataGridView dgv = null!;

        public AccountPanel()
        {
            DoubleBuffered = true; Dock = DockStyle.Fill; BackColor = ThemeColors.Background;
            InitializeUI();
        }

        private void InitializeUI()
        {
            Controls.Add(new Label { Text = "QUẢN LÝ TÀI KHOẢN", Font = ThemeColors.HeaderFont, ForeColor = ThemeColors.TextPrimary, Location = new Point(32, 20), Size = new Size(400, 40), BackColor = Color.Transparent });
            Controls.Add(new Label { Text = "Tạo, chỉnh sửa và quản lý tài khoản người dùng", Font = ThemeColors.BodyFont, ForeColor = ThemeColors.TextSecondary, Location = new Point(32, 62), Size = new Size(500, 22), BackColor = Color.Transparent });

            // Toolbar
            var txtSearch = new RoundedTextBox { Placeholder = "🔍  Tìm kiếm tài khoản...", Location = new Point(32, 96), Size = new Size(300, 44) };
            Controls.Add(txtSearch);

            var btnAdd = new RoundedButton { Text = "+ Tạo tài khoản", Size = new Size(160, 42), Location = new Point(350, 98), ButtonColor = ColorTranslator.FromHtml("#8B5CF6"), Font = ThemeColors.ButtonFont };
            btnAdd.Click += (s, e) =>
            {
                MessageBox.Show("Mở form tạo tài khoản mới", "Tạo tài khoản", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };
            Controls.Add(btnAdd);

            // DataGridView
            dgv = new DataGridView { Location = new Point(32, 152), Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom };
            ModernDataGridView.ApplyStyle(dgv);
            dgv.Columns.Add("MaTK", "Mã TK");
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "HoTen", HeaderText = "Họ tên", FillWeight = 180 });
            dgv.Columns.Add("Username", "Tên đăng nhập");
            dgv.Columns.Add("Email", "Email");
            dgv.Columns.Add("SDT", "Số điện thoại");
            dgv.Columns.Add("Role", "Vai trò");
            dgv.Columns.Add("TrangThai", "Trạng thái");
            dgv.Columns.Add(new DataGridViewButtonColumn { Name = "Actions", HeaderText = "", Text = "Sửa", UseColumnTextForButtonValue = true, Width = 60 });

            foreach (var u in UserStore.Users)
            {
                dgv.Rows.Add(u.MaTK, u.HoTen, u.Username, u.Email, u.SDT, u.RoleDisplay, u.IsActive ? "Hoạt động" : "Bị khóa");
            }

            dgv.CellFormatting += (s, e) =>
            {
                if (e.ColumnIndex == dgv.Columns["TrangThai"]!.Index)
                {
                    string val = e.Value?.ToString() ?? "";
                    e.CellStyle.ForeColor = val == "Hoạt động" ? ThemeColors.Success : ThemeColors.Danger;
                    e.CellStyle.Font = new Font(ThemeColors.BodyFont, FontStyle.Bold);
                }
                if (e.ColumnIndex == dgv.Columns["Role"]!.Index)
                {
                    string val = e.Value?.ToString() ?? "";
                    if (val == "Quản trị viên") e.CellStyle.ForeColor = ColorTranslator.FromHtml("#8B5CF6");
                    else if (val == "Thủ thư") e.CellStyle.ForeColor = ThemeColors.Primary;
                    else e.CellStyle.ForeColor = ThemeColors.Success;
                }
            };

            txtSearch.TextChanged += (s, e) =>
            {
                string q = txtSearch.InputText.ToLower();
                dgv.Rows.Clear();
                foreach (var u in UserStore.Users.Where(u => u.HoTen.ToLower().Contains(q) || u.Username.ToLower().Contains(q) || u.Email.ToLower().Contains(q)))
                    dgv.Rows.Add(u.MaTK, u.HoTen, u.Username, u.Email, u.SDT, u.RoleDisplay, u.IsActive ? "Hoạt động" : "Bị khóa");
            };

            Controls.Add(dgv);
            Resize += (s, e) => dgv.Size = new Size(Width - 64, Height - 172);
            dgv.Size = new Size(Width - 64, Height - 172);
        }
    }
}
