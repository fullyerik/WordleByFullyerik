using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace WordleByFullyerik
{

    public class BaseAppForm : Form
    {

        protected const int TitleBarHeight = 48;

        private Panel titleBar = null!;
        private Label appTitleLabel = null!;
        private TitleBarButton backButton = null!;
        private TitleBarButton themeButton = null!;
        private TitleBarButton minimizeButton = null!;
        private TitleBarButton closeButton = null!;

        protected Panel ContentPanel { get; private set; } = null!;

        protected bool IsMainMenu { get; set; } = false;

        public BaseAppForm()
        {

            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Font = new Font("Segoe UI", 10F);
            this.MinimumSize = new Size(560, 600);
            this.DoubleBuffered = true;

            BuildContentPanel();
            BuildTitleBar();

            ThemeManager.ThemeChanged += ApplyBaseTheme;
            this.FormClosed += (s, e) => ThemeManager.ThemeChanged -= ApplyBaseTheme;

            this.Load += (s, e) => ApplyBaseTheme();
        }

        private void BuildTitleBar()
        {
            titleBar = new Panel
            {
                Dock = DockStyle.Top,
                Height = TitleBarHeight
            };
            this.Controls.Add(titleBar);

            appTitleLabel = new Label
            {
                Text = "  🟩  Wordle By FULLYERIK",
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleLeft,
                Location = new Point(8, 0),
                Size = new Size(280, TitleBarHeight)
            };
            titleBar.Controls.Add(appTitleLabel);

            titleBar.MouseDown += TitleBar_MouseDown;
            appTitleLabel.MouseDown += TitleBar_MouseDown;

            closeButton = CreateTitleBarButton("✕", 46, isCloseButton: true);
            closeButton.Click += (s, e) => this.Close();
            titleBar.Controls.Add(closeButton);

            minimizeButton = CreateTitleBarButton("—", 46);
            minimizeButton.Click += (s, e) => this.WindowState = FormWindowState.Minimized;
            titleBar.Controls.Add(minimizeButton);

            themeButton = CreateTitleBarButton(GetThemeIcon(), 56);
            themeButton.Font = new Font("Segoe UI Emoji", 14F, FontStyle.Regular);
            themeButton.Click += (s, e) =>
            {
                ThemeManager.Toggle();
                themeButton.Text = GetThemeIcon();
            };
            titleBar.Controls.Add(themeButton);

            backButton = CreateTitleBarButton("←  Menü", 90);
            backButton.Click += (s, e) => this.Close();
            titleBar.Controls.Add(backButton);

            titleBar.Resize += (s, e) => LayoutTitleBarButtons();
            LayoutTitleBarButtons();
        }

        private TitleBarButton CreateTitleBarButton(string text, int width, bool isCloseButton = false)
        {
            return new TitleBarButton
            {
                Text = text,
                Width = width,
                Height = TitleBarHeight,
                IsCloseButton = isCloseButton
            };
        }

        private void LayoutTitleBarButtons()
        {
            int x = titleBar.Width;

            x -= closeButton.Width;
            closeButton.Location = new Point(x, 0);

            x -= minimizeButton.Width;
            minimizeButton.Location = new Point(x, 0);

            x -= themeButton.Width;
            themeButton.Location = new Point(x, 0);

            backButton.Visible = !IsMainMenu;
            if (!IsMainMenu)
            {
                x -= backButton.Width;
                backButton.Location = new Point(x, 0);
            }
        }

        private string GetThemeIcon()
        {

            return ThemeManager.IsDarkMode ? "☀" : "🌙";
        }

        private void BuildContentPanel()
        {
            ContentPanel = new Panel
            {
                Dock = DockStyle.Fill
            };
            this.Controls.Add(ContentPanel);
        }

        [DllImport("user32.dll")]
        private static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HTCAPTION = 0x2;

        private void TitleBar_MouseDown(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(this.Handle, WM_NCLBUTTONDOWN, HTCAPTION, 0);
            }
        }

        protected virtual void ApplyBaseTheme()
        {
            this.BackColor = ThemeManager.Background;
            titleBar.BackColor = ThemeManager.Surface;
            appTitleLabel.ForeColor = ThemeManager.Accent;

            foreach (Control c in titleBar.Controls)
            {
                if (c is TitleBarButton tbb)
                {
                    tbb.NormalBackColor = ThemeManager.Surface;
                    tbb.NormalForeColor = ThemeManager.Foreground;
                    tbb.HoverBackColor = tbb.IsCloseButton
                        ? Color.FromArgb(232, 17, 35)
                        : ThemeManager.BoxBorder;
                    tbb.HoverForeColor = tbb.IsCloseButton ? Color.White : ThemeManager.Foreground;
                    tbb.Invalidate();
                }
            }

            ContentPanel.BackColor = ThemeManager.Background;

            ApplyContentTheme();
        }

        protected virtual void ApplyContentTheme()
        {
        }

        private class TitleBarButton : Label
        {
            public bool IsCloseButton { get; set; }

            private Color normalBack = Color.White;
            private Color normalFore = Color.Black;
            private Color hoverBack = Color.LightGray;
            private Color hoverFore = Color.Black;

            public Color NormalBackColor
            {
                get => normalBack;
                set { normalBack = value; UpdateColors(); }
            }
            public Color NormalForeColor
            {
                get => normalFore;
                set { normalFore = value; UpdateColors(); }
            }
            public Color HoverBackColor
            {
                get => hoverBack;
                set { hoverBack = value; UpdateColors(); }
            }
            public Color HoverForeColor
            {
                get => hoverFore;
                set { hoverFore = value; UpdateColors(); }
            }

            private bool isHovered;

            public TitleBarButton()
            {
                this.AutoSize = false;
                this.TextAlign = ContentAlignment.MiddleCenter;
                this.Cursor = Cursors.Hand;
                this.Font = new Font("Segoe UI", 11F, FontStyle.Regular);

                this.MouseEnter += (s, e) => { isHovered = true; UpdateColors(); };
                this.MouseLeave += (s, e) => { isHovered = false; UpdateColors(); };
            }

            private void UpdateColors()
            {
                this.BackColor = isHovered ? hoverBack : normalBack;
                this.ForeColor = isHovered ? hoverFore : normalFore;
            }
        }
    }
}
