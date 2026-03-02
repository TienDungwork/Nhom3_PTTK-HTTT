using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using LibraryManagement.Controls;
using LibraryManagement.Helpers;
using LibraryManagement.Models;

namespace LibraryManagement.Forms.Panels
{
    public class AccountPanel : UserControl
    {
        private DataGridView dgv = null!;
        private TextBox txtSearch = null!;

        public AccountPanel()
        {
            Dock = DockStyle.Fill;
            BackColor = ThemeColors.Background;

            Controls.Add(new Label { Text = "QUẢN LÝ TÀI KHOẢN", Font = ThemeColors.HeaderFont, ForeColor = ThemeColors.TextPrimary, Location = new Point(32, 20), Size = new Size(400, 40), BackColor = Color.Transparent });
            Controls.Add(new Label { Text = "Quản lý tài khoản Admin, Thủ thư, Độc giả", Font = ThemeColors.BodyFont, ForeColor = ThemeColors.TextSecondary, Location = new Point(32, 60), Size = new Size(500, 22), BackColor = Color.Transparent });

            // Toolbar
            var toolbar = new Panel { Location = new Point(32, 92), Size = new Size(920, 52), BackColor = Color.White, Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };
            toolbar.Padding = new Padding(8);
            toolbar.Paint += (s, e) =>
            {
                using var path = ThemeColors.GetRoundedRect(new Rectangle(0, 0, toolbar.Width - 2, toolbar.Height - 2), 12);
                using var bg = new SolidBrush(Color.White);
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                e.Graphics.FillPath(bg, path);
            };

            txtSearch = new TextBox { Location = new Point(16, 12), Size = new Size(260, 28), Font = ThemeColors.BodyFont, BorderStyle = BorderStyle.FixedSingle, PlaceholderText = "Tìm kiếm theo tên, username, vai trò..." };
            txtSearch.TextChanged += (s, e) => FilterAccounts();
            toolbar.Controls.Add(txtSearch);

            var btnAdd = new RoundedButton { Text = "Thêm TK", Size = new Size(110, 36), Location = new Point(296, 8), ButtonColor = ThemeColors.Success, Font = ThemeColors.ButtonFont };
            btnAdd.Click += BtnAdd_Click;
            toolbar.Controls.Add(btnAdd);

            var btnEdit = new RoundedButton { Text = "Sửa", Size = new Size(80, 36), Location = new Point(416, 8), ButtonColor = ThemeColors.Warning, Font = ThemeColors.ButtonFont };
            btnEdit.Click += BtnEdit_Click;
            toolbar.Controls.Add(btnEdit);

            var btnDelete = new RoundedButton { Text = "Xóa", Size = new Size(80, 36), Location = new Point(506, 8), ButtonColor = ThemeColors.Danger, Font = ThemeColors.ButtonFont };
            btnDelete.Click += BtnDelete_Click;
            toolbar.Controls.Add(btnDelete);

            var btnLock = new RoundedButton { Text = "Khóa/Mở", Size = new Size(110, 36), Location = new Point(596, 8), ButtonColor = ThemeColors.TextSecondary, Font = ThemeColors.ButtonFont };
            btnLock.Click += BtnLock_Click;
            toolbar.Controls.Add(btnLock);

            var btnChangePass = new RoundedButton { Text = "Đổi MK", Size = new Size(100, 36), Location = new Point(716, 8), ButtonColor = ThemeColors.Primary, Font = ThemeColors.ButtonFont };
            btnChangePass.Click += BtnChangePass_Click;
            toolbar.Controls.Add(btnChangePass);

            Controls.Add(toolbar);

            // DGV
            dgv = new DataGridView { Location = new Point(32, 156), Size = new Size(920, 490), Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom };
            dgv.Columns.Add("Username", "Tên đăng nhập");
            dgv.Columns.Add("HoTen", "Họ tên");
            dgv.Columns.Add("Role", "Vai trò");
            dgv.Columns.Add("Email", "Email");
            dgv.Columns.Add("SDT", "Số ĐT");
            dgv.Columns.Add("IsActive", "Trạng thái");
            ModernDataGridView.ApplyStyle(dgv);
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            LoadAccounts();
            Controls.Add(dgv);
        }

        private void LoadAccounts()
        {
            dgv.Rows.Clear();
            foreach (var u in UserStore.Users)
            {
                string status = u.IsActive ? "Hoạt động" : "Bị khóa";
                int rowIdx = dgv.Rows.Add(u.Username, u.HoTen, u.RoleDisplay, u.Email, u.SDT, status);
                if (!u.IsActive)
                {
                    dgv.Rows[rowIdx].DefaultCellStyle.ForeColor = ThemeColors.TextMuted;
                    dgv.Rows[rowIdx].DefaultCellStyle.BackColor = Color.FromArgb(248, 248, 248);
                }
            }
        }

        private void FilterAccounts()
        {
            dgv.Rows.Clear();
            string kw = txtSearch.Text.Trim();
            var filtered = UserStore.Users.Where(u =>
                u.Username.Contains(kw, StringComparison.OrdinalIgnoreCase) ||
                u.HoTen.Contains(kw, StringComparison.OrdinalIgnoreCase) ||
                u.RoleDisplay.Contains(kw, StringComparison.OrdinalIgnoreCase))
                .ToList();

            foreach (var u in filtered)
            {
                string status = u.IsActive ? "Hoạt động" : "Bị khóa";
                int rowIdx = dgv.Rows.Add(u.Username, u.HoTen, u.RoleDisplay, u.Email, u.SDT, status);
                if (!u.IsActive)
                {
                    dgv.Rows[rowIdx].DefaultCellStyle.ForeColor = ThemeColors.TextMuted;
                    dgv.Rows[rowIdx].DefaultCellStyle.BackColor = Color.FromArgb(248, 248, 248);
                }
            }
        }

        private AppUser? GetSelectedUser()
        {
            if (dgv.SelectedRows.Count == 0) { MessageBox.Show("Vui lòng chọn tài khoản!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning); return null; }
            string username = dgv.SelectedRows[0].Cells["Username"].Value?.ToString() ?? "";
            return UserStore.Users.FirstOrDefault(u => u.Username == username);
        }

        private void BtnAdd_Click(object? sender, EventArgs e)
        {
            using var dlg = new Form { Text = "Thêm tài khoản mới", Size = new Size(400, 360), StartPosition = FormStartPosition.CenterParent, BackColor = Color.White, Font = ThemeColors.BodyFont, FormBorderStyle = FormBorderStyle.FixedDialog, MaximizeBox = false };
            var txtUser = new TextBox { Location = new Point(20, 40), Size = new Size(340, 28), Font = ThemeColors.BodyFont, BorderStyle = BorderStyle.FixedSingle };
            dlg.Controls.Add(new Label { Text = "Tên đăng nhập", Font = ThemeColors.SmallFont, ForeColor = ThemeColors.TextSecondary, Location = new Point(20, 20), Size = new Size(200, 18) });
            dlg.Controls.Add(txtUser);
            var txtName = new TextBox { Location = new Point(20, 96), Size = new Size(340, 28), Font = ThemeColors.BodyFont, BorderStyle = BorderStyle.FixedSingle };
            dlg.Controls.Add(new Label { Text = "Họ tên", Font = ThemeColors.SmallFont, ForeColor = ThemeColors.TextSecondary, Location = new Point(20, 76), Size = new Size(200, 18) });
            dlg.Controls.Add(txtName);
            var txtPass = new TextBox { Location = new Point(20, 152), Size = new Size(340, 28), Font = ThemeColors.BodyFont, BorderStyle = BorderStyle.FixedSingle, UseSystemPasswordChar = true };
            dlg.Controls.Add(new Label { Text = "Mật khẩu", Font = ThemeColors.SmallFont, ForeColor = ThemeColors.TextSecondary, Location = new Point(20, 132), Size = new Size(200, 18) });
            dlg.Controls.Add(txtPass);
            var cboRole = new ComboBox { Location = new Point(20, 208), Size = new Size(340, 28), Font = ThemeColors.BodyFont, DropDownStyle = ComboBoxStyle.DropDownList };
            cboRole.Items.AddRange(new object[] { "Quản trị viên", "Thủ thư", "Độc giả" });
            cboRole.SelectedIndex = 2;
            dlg.Controls.Add(new Label { Text = "Vai trò", Font = ThemeColors.SmallFont, ForeColor = ThemeColors.TextSecondary, Location = new Point(20, 188), Size = new Size(200, 18) });
            dlg.Controls.Add(cboRole);

            var btnSave = new RoundedButton { Text = "Thêm", Size = new Size(100, 38), Location = new Point(20, 260), ButtonColor = ThemeColors.Success, Font = ThemeColors.ButtonFont };
            btnSave.Click += (s2, e2) =>
            {
                if (string.IsNullOrWhiteSpace(txtUser.Text) || string.IsNullOrWhiteSpace(txtPass.Text)) { MessageBox.Show("Vui lòng điền đầy đủ!"); return; }
                if (UserStore.Users.Any(u => u.Username == txtUser.Text.Trim())) { MessageBox.Show("Tên đăng nhập đã tồn tại!"); return; }
                UserRole role = cboRole.SelectedIndex switch { 0 => UserRole.Admin, 1 => UserRole.ThuThu, _ => UserRole.DocGia };
                UserStore.Users.Add(new AppUser { Username = txtUser.Text.Trim(), Password = txtPass.Text, HoTen = txtName.Text.Trim(), Role = role });
                MessageBox.Show("Thêm tài khoản thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                dlg.Close();
            };
            dlg.Controls.Add(btnSave);
            dlg.ShowDialog(FindForm());
            LoadAccounts();
        }

        private void BtnEdit_Click(object? sender, EventArgs e)
        {
            var user = GetSelectedUser();
            if (user == null) return;

            using var dlg = new Form { Text = "Sửa tài khoản", Size = new Size(400, 260), StartPosition = FormStartPosition.CenterParent, BackColor = Color.White, Font = ThemeColors.BodyFont, FormBorderStyle = FormBorderStyle.FixedDialog, MaximizeBox = false };
            var txtName = new TextBox { Location = new Point(20, 40), Size = new Size(340, 28), Font = ThemeColors.BodyFont, BorderStyle = BorderStyle.FixedSingle, Text = user.HoTen };
            dlg.Controls.Add(new Label { Text = "Họ tên", Font = ThemeColors.SmallFont, ForeColor = ThemeColors.TextSecondary, Location = new Point(20, 20), Size = new Size(200, 18) });
            dlg.Controls.Add(txtName);
            var cboRole = new ComboBox { Location = new Point(20, 96), Size = new Size(340, 28), Font = ThemeColors.BodyFont, DropDownStyle = ComboBoxStyle.DropDownList };
            cboRole.Items.AddRange(new object[] { "Quản trị viên", "Thủ thư", "Độc giả" });
            cboRole.SelectedIndex = user.Role switch { UserRole.Admin => 0, UserRole.ThuThu => 1, _ => 2 };
            dlg.Controls.Add(new Label { Text = "Vai trò", Font = ThemeColors.SmallFont, ForeColor = ThemeColors.TextSecondary, Location = new Point(20, 76), Size = new Size(200, 18) });
            dlg.Controls.Add(cboRole);

            var btnSave = new RoundedButton { Text = "Lưu", Size = new Size(100, 38), Location = new Point(20, 148), ButtonColor = ThemeColors.Success, Font = ThemeColors.ButtonFont };
            btnSave.Click += (s2, e2) =>
            {
                user.HoTen = txtName.Text.Trim();
                user.Role = cboRole.SelectedIndex switch { 0 => UserRole.Admin, 1 => UserRole.ThuThu, _ => UserRole.DocGia };
                MessageBox.Show("Cập nhật thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                dlg.Close();
            };
            dlg.Controls.Add(btnSave);
            dlg.ShowDialog(FindForm());
            LoadAccounts();
        }

        private void BtnDelete_Click(object? sender, EventArgs e)
        {
            var user = GetSelectedUser();
            if (user == null) return;
            if (MessageBox.Show($"Bạn có chắc chắn muốn xóa tài khoản \"{user.Username}\"?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                UserStore.Users.Remove(user);
                MessageBox.Show("Xóa tài khoản thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadAccounts();
            }
        }

        private void BtnLock_Click(object? sender, EventArgs e)
        {
            var user = GetSelectedUser();
            if (user == null) return;
            user.IsActive = !user.IsActive;
            string status = user.IsActive ? "mở khóa" : "khóa";
            MessageBox.Show($"Đã {status} tài khoản \"{user.Username}\"!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
            LoadAccounts();
        }

        private void BtnChangePass_Click(object? sender, EventArgs e)
        {
            var user = GetSelectedUser();
            if (user == null) return;

            using var dlg = new Form { Text = "Đổi mật khẩu", Size = new Size(380, 200), StartPosition = FormStartPosition.CenterParent, BackColor = Color.White, Font = ThemeColors.BodyFont, FormBorderStyle = FormBorderStyle.FixedDialog, MaximizeBox = false };
            var txtNewPass = new TextBox { Location = new Point(20, 40), Size = new Size(320, 28), Font = ThemeColors.BodyFont, BorderStyle = BorderStyle.FixedSingle, UseSystemPasswordChar = true };
            dlg.Controls.Add(new Label { Text = $"Mật khẩu mới cho \"{user.Username}\"", Font = ThemeColors.SmallFont, ForeColor = ThemeColors.TextSecondary, Location = new Point(20, 20), Size = new Size(320, 18) });
            dlg.Controls.Add(txtNewPass);

            var btnSave = new RoundedButton { Text = "Đổi mật khẩu", Size = new Size(140, 38), Location = new Point(20, 88), ButtonColor = ThemeColors.Primary, Font = ThemeColors.ButtonFont };
            btnSave.Click += (s2, e2) =>
            {
                if (string.IsNullOrWhiteSpace(txtNewPass.Text)) { MessageBox.Show("Vui lòng nhập mật khẩu mới!"); return; }
                user.Password = txtNewPass.Text;
                MessageBox.Show("Đổi mật khẩu thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                dlg.Close();
            };
            dlg.Controls.Add(btnSave);
            dlg.ShowDialog(FindForm());
        }
    }
}
