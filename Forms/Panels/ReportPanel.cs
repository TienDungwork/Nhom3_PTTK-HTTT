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
            DoubleBuffered = true;
            Dock = DockStyle.Fill;
            BackColor = ThemeColors.Background;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            Controls.Add(new Label { Text = "BÁO CÁO", Font = ThemeColors.HeaderFont, ForeColor = ThemeColors.TextPrimary, Location = new Point(32, 20), Size = new Size(400, 40), BackColor = Color.Transparent });
            Controls.Add(new Label { Text = "Thống kê và báo cáo hoạt động thư viện", Font = ThemeColors.BodyFont, ForeColor = ThemeColors.TextSecondary, Location = new Point(32, 62), Size = new Size(400, 22), BackColor = Color.Transparent });

            int totalBooks = SampleData.Books.Sum(b => b.SoLuong);
            int borrowed = SampleData.BorrowRecords.Count(r => r.TrangThai == "Đang mượn");
            int returned = SampleData.BorrowRecords.Count(r => r.TrangThai == "Đã trả");
            decimal totalFines = SampleData.BorrowRecords.Sum(r => r.TienPhat);

            Controls.Add(new StatCard { IconText = "📚", Title = "Tổng sách", Value = totalBooks.ToString(), AccentColor = ThemeColors.Primary, Location = new Point(32, 100), Size = new Size(230, 130) });
            Controls.Add(new StatCard { IconText = "📖", Title = "Đang mượn", Value = borrowed.ToString(), AccentColor = ThemeColors.Warning, Location = new Point(282, 100), Size = new Size(230, 130) });
            Controls.Add(new StatCard { IconText = "✅", Title = "Đã trả", Value = returned.ToString(), AccentColor = ThemeColors.Success, Location = new Point(532, 100), Size = new Size(230, 130) });
            Controls.Add(new StatCard { IconText = "💰", Title = "Tổng phạt", Value = $"{totalFines:N0}đ", AccentColor = ThemeColors.Danger, Location = new Point(782, 100), Size = new Size(230, 130) });

            Panel piePanel = new Panel { Location = new Point(32, 250), Size = new Size(480, 320), BackColor = Color.Transparent };
            piePanel.Paint += PiePanel_Paint;
            Controls.Add(piePanel);

            Panel topPanel = new Panel { Location = new Point(530, 250), Size = new Size(480, 320), BackColor = Color.Transparent };
            topPanel.Paint += TopPanel_Paint;
            Controls.Add(topPanel);
        }

        private void PiePanel_Paint(object? sender, PaintEventArgs e)
        {
            Panel p = (Panel)sender!;
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            var rect = new Rectangle(2, 2, p.Width - 6, p.Height - 6);
            using (var path = ThemeColors.GetRoundedRect(rect, 12))
            { using (var bg = new SolidBrush(Color.White)) g.FillPath(bg, path); }

            using (var b = new SolidBrush(ThemeColors.TextPrimary))
                g.DrawString("📊  Phân bố sách theo thể loại", ThemeColors.SubTitleFont, b, 20, 16);

            var cats = SampleData.Books.GroupBy(b => b.TheLoai).Select(gr => new { Name = gr.Key, Count = gr.Sum(b => b.SoLuong) }).OrderByDescending(x => x.Count).ToList();
            int total = cats.Sum(c => c.Count);
            if (total == 0) return;

            Rectangle pieRect = new Rectangle(40, 60, 200, 200);
            float start = 0;
            Color[] colors = { ThemeColors.Primary, ThemeColors.Success, ThemeColors.Warning, ThemeColors.Danger, ThemeColors.Info, Color.FromArgb(139, 92, 246), Color.FromArgb(236, 72, 153) };

            for (int i = 0; i < cats.Count; i++)
            {
                float sweep = (float)cats[i].Count / total * 360f;
                using (var br = new SolidBrush(colors[i % colors.Length])) g.FillPie(br, pieRect, start, sweep);
                start += sweep;
            }
            using (var w = new SolidBrush(Color.White)) g.FillEllipse(w, 90, 110, 100, 100);
            using (var tb = new SolidBrush(ThemeColors.TextPrimary))
            using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                g.DrawString(total.ToString(), new Font("Segoe UI", 20, FontStyle.Bold), tb, new Rectangle(90, 110, 100, 100), sf);

            int ly = 65;
            for (int i = 0; i < cats.Count; i++)
            {
                using (var d = new SolidBrush(colors[i % colors.Length])) g.FillRectangle(d, 280, ly, 14, 14);
                using (var lb = new SolidBrush(ThemeColors.TextPrimary)) g.DrawString($"{cats[i].Name} ({cats[i].Count})", ThemeColors.SmallFont, lb, 300, ly - 1);
                ly += 26;
            }
        }

        private void TopPanel_Paint(object? sender, PaintEventArgs e)
        {
            Panel p = (Panel)sender!;
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            var rect = new Rectangle(2, 2, p.Width - 6, p.Height - 6);
            using (var path = ThemeColors.GetRoundedRect(rect, 12))
            { using (var bg = new SolidBrush(Color.White)) g.FillPath(bg, path); }

            using (var b = new SolidBrush(ThemeColors.TextPrimary))
                g.DrawString("🏆  Độc giả mượn nhiều nhất", ThemeColors.SubTitleFont, b, 20, 16);

            var top = SampleData.BorrowRecords.GroupBy(r => r.TenDocGia).Select(gr => new { Name = gr.Key, Count = gr.Count() }).OrderByDescending(x => x.Count).Take(5).ToList();
            int max = top.Any() ? top.Max(x => x.Count) : 1;
            int iy = 60;
            Color[] bc = { ThemeColors.Primary, ThemeColors.Success, ThemeColors.Warning, ThemeColors.Info, ThemeColors.Danger };

            for (int i = 0; i < top.Count; i++)
            {
                using (var rb = new SolidBrush(ThemeColors.TextSecondary)) g.DrawString($"#{i + 1}", new Font("Segoe UI Semibold", 10), rb, 20, iy + 8);
                using (var nb = new SolidBrush(ThemeColors.TextPrimary)) g.DrawString(top[i].Name, new Font("Segoe UI", 10), nb, 50, iy);
                int bw = (int)((float)top[i].Count / max * 260);
                var br = new Rectangle(50, iy + 24, bw, 16);
                using (var bp = ThemeColors.GetRoundedRect(br, 8))
                using (var bb = new SolidBrush(bc[i % bc.Length])) g.FillPath(bb, bp);
                using (var cb = new SolidBrush(ThemeColors.TextSecondary)) g.DrawString($"{top[i].Count} lần", ThemeColors.SmallFont, cb, br.Right + 8, iy + 22);
                iy += 50;
            }
        }
    }
}
