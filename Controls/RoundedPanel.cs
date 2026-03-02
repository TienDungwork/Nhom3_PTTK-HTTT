using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using LibraryManagement.Helpers;

namespace LibraryManagement.Controls
{
    public class RoundedPanel : Panel
    {
        public int Radius { get; set; } = ThemeColors.BorderRadius;
        public Color ShadowColor { get; set; } = Color.FromArgb(15, 0, 0, 0);
        public bool ShowShadow { get; set; } = true;
        public Color BorderColor { get; set; } = Color.Transparent;
        public int ShadowOffset { get; set; } = 4;

        public RoundedPanel()
        {
            DoubleBuffered = true;
            BackColor = Color.Transparent;
            Padding = new Padding(ThemeColors.CardPadding);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            Rectangle shadowRect = new Rectangle(
                ShadowOffset, ShadowOffset,
                Width - ShadowOffset * 2, Height - ShadowOffset * 2);

            Rectangle cardRect = new Rectangle(
                2, 2,
                Width - ShadowOffset * 2 - 2, Height - ShadowOffset * 2 - 2);

            // Shadow
            if (ShowShadow)
            {
                using (GraphicsPath shadowPath = ThemeColors.GetRoundedRect(shadowRect, Radius))
                using (SolidBrush shadowBrush = new SolidBrush(ShadowColor))
                {
                    g.FillPath(shadowBrush, shadowPath);
                }
            }

            // Card background
            using (GraphicsPath cardPath = ThemeColors.GetRoundedRect(cardRect, Radius))
            using (SolidBrush cardBrush = new SolidBrush(ThemeColors.CardBackground))
            {
                g.FillPath(cardBrush, cardPath);

                if (BorderColor != Color.Transparent)
                {
                    using (Pen pen = new Pen(BorderColor, 1))
                    {
                        g.DrawPath(pen, cardPath);
                    }
                }
            }
        }
    }
}
