using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using LibraryManagement.Helpers;

namespace LibraryManagement.Controls
{
    public class RoundedPanel : Panel
    {
        public int Radius { get; set; } = ThemeColors.BorderRadius;
        public Color BorderColor { get; set; } = ThemeColors.Border;
        public float BorderWidth { get; set; } = 1f;

        public RoundedPanel()
        {
            DoubleBuffered = true;
            ResizeRedraw = true;
            BackColor = Color.White;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            var rect = new Rectangle(0, 0, Width - 1, Height - 1);

            using var path = ThemeColors.GetRoundedRect(rect, Radius);
            using var bg = new SolidBrush(BackColor);
            using var pen = new Pen(BorderColor, BorderWidth);

            e.Graphics.FillPath(bg, path);
            e.Graphics.DrawPath(pen, path);
        }
    }
}
