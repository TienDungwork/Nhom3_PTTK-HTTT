using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using LibraryManagement.Helpers;

namespace LibraryManagement.Controls
{
    public class StatCard : Control
    {
        private bool _isHovered = false;

        public string IconText { get; set; } = "📚";
        public string Title { get; set; } = "Thống kê";
        public string Value { get; set; } = "0";
        public Color AccentColor { get; set; } = ThemeColors.Primary;

        public StatCard()
        {
            DoubleBuffered = true;
            Size = new Size(240, 130);
            Cursor = Cursors.Hand;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            // Shadow
            Rectangle shadowRect = new Rectangle(4, 4, Width - 8, Height - 8);
            using (GraphicsPath shadowPath = ThemeColors.GetRoundedRect(shadowRect, 12))
            using (SolidBrush shadowBrush = new SolidBrush(Color.FromArgb(_isHovered ? 25 : 15, 0, 0, 0)))
            {
                g.FillPath(shadowBrush, shadowPath);
            }

            // Card
            Rectangle cardRect = new Rectangle(2, 2, Width - 10, Height - 10);
            using (GraphicsPath cardPath = ThemeColors.GetRoundedRect(cardRect, 12))
            using (SolidBrush cardBrush = new SolidBrush(Color.White))
            {
                g.FillPath(cardBrush, cardPath);
            }

            // Accent line at top
            Rectangle accentRect = new Rectangle(2, 2, Width - 10, 4);
            using (GraphicsPath accentPath = ThemeColors.GetRoundedRect(
                new Rectangle(2, 2, Width - 10, 30), 12))
            {
                g.SetClip(accentRect);
                using (SolidBrush accentBrush = new SolidBrush(AccentColor))
                {
                    g.FillPath(accentBrush, accentPath);
                }
                g.ResetClip();
            }

            // Icon background circle
            Rectangle iconBg = new Rectangle(20, 28, 48, 48);
            using (SolidBrush iconBgBrush = new SolidBrush(Color.FromArgb(20, AccentColor)))
            {
                g.FillEllipse(iconBgBrush, iconBg);
            }

            // Icon
            using (StringFormat sf = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            })
            {
                g.DrawString(IconText, new Font("Segoe UI", 18), new SolidBrush(AccentColor), iconBg, sf);
            }

            // Value
            using (SolidBrush valueBrush = new SolidBrush(ThemeColors.TextPrimary))
            {
                g.DrawString(Value, ThemeColors.StatValueFont, valueBrush, 80, 24);
            }

            // Title
            using (SolidBrush titleBrush = new SolidBrush(ThemeColors.TextSecondary))
            {
                g.DrawString(Title, ThemeColors.StatLabelFont, titleBrush, 80, 68);
            }
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            _isHovered = true;
            Invalidate();
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            _isHovered = false;
            Invalidate();
            base.OnMouseLeave(e);
        }
    }
}
