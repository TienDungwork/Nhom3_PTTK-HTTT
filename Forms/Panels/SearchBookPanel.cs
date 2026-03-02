using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using LibraryManagement.Controls;
using LibraryManagement.Helpers;
using LibraryManagement.Models;

namespace LibraryManagement.Forms.Panels
{
    public class SearchBookPanel : UserControl
    {
        private DataGridView dgv = null!;
        private RoundedTextBox txtSearch = null!;
        private ComboBox cboCategory = null!;

        public SearchBookPanel()
        {
            DoubleBuffered = true; Dock = DockStyle.Fill; BackColor = ThemeColors.Background;
            InitializeUI();
        }

        private void InitializeUI()
        {
            Controls.Add(new Label { Text = "TRA CỨU SÁCH", Font = ThemeColors.HeaderFont, ForeColor = ThemeColors.TextPrimary, Location = new Point(32, 20), Size = new Size(400, 40), BackColor = Color.Transparent });
            Controls.Add(new Label { Text = "Tìm kiếm và xem thông tin sách trong thư viện", Font = ThemeColors.BodyFont, ForeColor = ThemeColors.TextSecondary, Location = new Point(32, 62), Size = new Size(500, 22), BackColor = Color.Transparent });

            // Search
            txtSearch = new RoundedTextBox { Placeholder = "🔍  Tìm theo tên sách, tác giả, ISBN...", Location = new Point(32, 96), Size = new Size(400, 44) };
            txtSearch.TextChanged += (s, e) => FilterBooks();
            Controls.Add(txtSearch);

            // Category filter
            cboCategory = new ComboBox { Location = new Point(450, 100), Size = new Size(200, 36), DropDownStyle = ComboBoxStyle.DropDownList, Font = ThemeColors.BodyFont };
            cboCategory.Items.Add("Tất cả thể loại");
            foreach (var cat in SampleData.Categories) cboCategory.Items.Add(cat);
            cboCategory.SelectedIndex = 0;
            cboCategory.SelectedIndexChanged += (s, e) => FilterBooks();
            Controls.Add(cboCategory);

            // DataGridView (read-only, no add/edit/delete)
            dgv = new DataGridView { Location = new Point(32, 152), Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom };
            ModernDataGridView.ApplyStyle(dgv);
            dgv.ReadOnly = true;
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.Columns.Add("MaSach", "Mã sách");
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "TenSach", HeaderText = "Tên sách", FillWeight = 200 });
            dgv.Columns.Add("TacGia", "Tác giả");
            dgv.Columns.Add("TheLoai", "Thể loại");
            dgv.Columns.Add("ISBN", "ISBN");
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "SoLuong", HeaderText = "Còn lại", Width = 80 });
            dgv.Columns.Add("TrangThai", "Trạng thái");

            LoadBooks(SampleData.Books);

            dgv.CellFormatting += (s, e) =>
            {
                if (e.ColumnIndex == dgv.Columns["TrangThai"]!.Index)
                {
                    string val = e.Value?.ToString() ?? "";
                    if (val == "Hết sách") { e.CellStyle.ForeColor = ThemeColors.Danger; e.CellStyle.Font = new Font(ThemeColors.BodyFont, FontStyle.Bold); }
                    else { e.CellStyle.ForeColor = ThemeColors.Success; }
                }
            };

            Controls.Add(dgv);
            Resize += (s, e) => dgv.Size = new Size(Width - 64, Height - 170);
            dgv.Size = new Size(Width - 64, Height - 170);
        }

        private void LoadBooks(List<Book> books)
        {
            dgv.Rows.Clear();
            foreach (var b in books)
                dgv.Rows.Add(b.MaSach, b.TenSach, b.TacGia, b.TheLoai, b.ISBN, b.SoLuong, b.TrangThai);
        }

        private void FilterBooks()
        {
            string search = txtSearch.InputText.ToLower();
            string category = cboCategory.SelectedIndex <= 0 ? "" : cboCategory.SelectedItem?.ToString() ?? "";
            var filtered = SampleData.Books.Where(b =>
                (string.IsNullOrEmpty(search) || b.TenSach.ToLower().Contains(search) || b.TacGia.ToLower().Contains(search) || b.ISBN.ToLower().Contains(search)) &&
                (string.IsNullOrEmpty(category) || b.TheLoai == category)
            ).ToList();
            LoadBooks(filtered);
        }
    }
}
