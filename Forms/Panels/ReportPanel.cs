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
    public class ReportPanel : UserControl
    {
        public ReportPanel()
        {
            Dock = DockStyle.Fill;
            BackColor = ThemeColors.Background;
            AutoScroll = true;

            Controls.Add(new Label { Text = "BÁO CÁO & THỐNG KÊ", Font = ThemeColors.HeaderFont, ForeColor = ThemeColors.TextPrimary, Location = new Point(32, 20), Size = new Size(500, 40), BackColor = Color.Transparent });
            Controls.Add(new Label { Text = "Thống kê sách và tình hình mượn trả", Font = ThemeColors.BodyFont, ForeColor = ThemeColors.TextSecondary, Location = new Point(32, 60), Size = new Size(500, 22), BackColor = Color.Transparent });

            // --- Report 1: Inventory stats ---
            Controls.Add(new Label { Text = "Báo cáo 1: Thống kê số lượng sách", Font = ThemeColors.SubTitleFont, ForeColor = ThemeColors.TextPrimary, Location = new Point(32, 96), Size = new Size(400, 26), BackColor = Color.Transparent });

            var dgv1 = new DataGridView { Location = new Point(32, 126), Size = new Size(920, 250), Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };
            dgv1.Columns.Add("MaSach", "Mã sách");
            dgv1.Columns.Add("TenSach", "Tên sách");
            dgv1.Columns.Add("TacGia", "Tác giả");
            dgv1.Columns.Add("DangMuon", "Đang mượn");
            dgv1.Columns.Add("MatHong", "Mất/hỏng");
            dgv1.Columns.Add("HienCo", "Hiện có");
            dgv1.Columns.Add("ViTri", "Vị trí kho");
            dgv1.Columns.Add("NCC", "Nhà cung cấp");
            ModernDataGridView.ApplyStyle(dgv1);

            foreach (var b in SampleData.Books)
            {
                dgv1.Rows.Add(b.MaSach, b.TenSach, b.TacGia, b.SoLuongDangMuon, b.SoLuongMatHong, b.SoLuongHienCo, b.ViTriKho, b.NhaCungCap);
            }
            Controls.Add(dgv1);

            // --- Report 2: Most borrowed ---
            Controls.Add(new Label { Text = "Báo cáo 2: Sách được mượn nhiều nhất", Font = ThemeColors.SubTitleFont, ForeColor = ThemeColors.TextPrimary, Location = new Point(32, 390), Size = new Size(400, 26), BackColor = Color.Transparent });

            var dgv2 = new DataGridView { Location = new Point(32, 420), Size = new Size(920, 230), Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom };
            dgv2.Columns.Add("TenSach", "Tên sách");
            dgv2.Columns.Add("TacGia", "Tác giả");
            dgv2.Columns.Add("TheLoai", "Thể loại");
            dgv2.Columns.Add("SoLanMuon", "Số lần mượn");
            dgv2.Columns.Add("DangMuon", "Đang mượn");
            ModernDataGridView.ApplyStyle(dgv2);

            var borrowCounts = SampleData.BorrowRecords
                .GroupBy(r => r.MaSach)
                .Select(g => new { MaSach = g.Key, Count = g.Count(), ActiveCount = g.Count(r => r.TrangThai == "Đang mượn") })
                .OrderByDescending(x => x.Count)
                .ToList();

            foreach (var bc in borrowCounts)
            {
                var book = SampleData.Books.FirstOrDefault(b => b.MaSach == bc.MaSach);
                if (book != null)
                    dgv2.Rows.Add(book.TenSach, book.TacGia, book.TheLoai, bc.Count, bc.ActiveCount);
            }
            Controls.Add(dgv2);
        }
    }
}
