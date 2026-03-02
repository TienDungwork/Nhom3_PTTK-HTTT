using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using LibraryManagement.Helpers;

namespace LibraryManagement.Controls
{
    public class RoundedTextBox : UserControl
    {
        private TextBox _textBox;
        private string _placeholder = "";
        private bool _showPlaceholder = true;
        private bool _isFocused = false;
        private bool _isPassword = false;

        public string Placeholder
        {
            get => _placeholder;
            set { _placeholder = value; UpdatePlaceholder(); }
        }

        public bool IsPassword
        {
            get => _isPassword;
            set
            {
                _isPassword = value;
                _textBox.UseSystemPasswordChar = value;
            }
        }

        public override string Text
        {
            get => _showPlaceholder ? "" : _textBox.Text;
            set
            {
                _textBox.Text = value;
                _showPlaceholder = string.IsNullOrEmpty(value);
                UpdatePlaceholder();
            }
        }

        public string InputText => _showPlaceholder ? "" : _textBox.Text;

        public int Radius { get; set; } = ThemeColors.BorderRadius;

        public RoundedTextBox()
        {
            DoubleBuffered = true;
            Size = new Size(300, 46);
            BackColor = Color.Transparent;

            _textBox = new TextBox
            {
                BorderStyle = BorderStyle.None,
                Font = ThemeColors.InputFont,
                ForeColor = ThemeColors.TextMuted,
                BackColor = Color.White,
                Location = new Point(16, 13),
                Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top
            };

            _textBox.GotFocus += (s, e) =>
            {
                _isFocused = true;
                if (_showPlaceholder)
                {
                    _showPlaceholder = false;
                    _textBox.Text = "";
                    _textBox.ForeColor = ThemeColors.TextPrimary;
                    if (_isPassword) _textBox.UseSystemPasswordChar = true;
                }
                Invalidate();
            };

            _textBox.LostFocus += (s, e) =>
            {
                _isFocused = false;
                if (string.IsNullOrEmpty(_textBox.Text))
                {
                    _showPlaceholder = true;
                    _textBox.UseSystemPasswordChar = false;
                    _textBox.ForeColor = ThemeColors.TextMuted;
                    _textBox.Text = _placeholder;
                }
                Invalidate();
            };

            _textBox.TextChanged += (s, e) =>
            {
                if (!_showPlaceholder)
                    OnTextChanged(EventArgs.Empty);
            };

            Controls.Add(_textBox);
            UpdatePlaceholder();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            _textBox.Width = Width - 32;
            _textBox.Location = new Point(16, (Height - _textBox.Height) / 2);
        }

        private void UpdatePlaceholder()
        {
            if (_showPlaceholder && !_isFocused)
            {
                _textBox.UseSystemPasswordChar = false;
                _textBox.ForeColor = ThemeColors.TextMuted;
                _textBox.Text = _placeholder;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            Rectangle rect = new Rectangle(1, 1, Width - 3, Height - 3);
            using (GraphicsPath path = ThemeColors.GetRoundedRect(rect, Radius))
            {
                using (SolidBrush bg = new SolidBrush(Color.White))
                {
                    g.FillPath(bg, path);
                }

                Color borderColor = _isFocused ? ThemeColors.Primary : ThemeColors.Border;
                float borderWidth = _isFocused ? 2f : 1f;
                using (Pen pen = new Pen(borderColor, borderWidth))
                {
                    g.DrawPath(pen, path);
                }
            }
        }
    }
}
