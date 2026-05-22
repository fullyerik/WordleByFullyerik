using System;
using System.Drawing;

namespace WordleByFullyerik
{

    public static class ThemeManager
    {

        public static bool IsDarkMode { get; private set; } = false;

        public static event Action? ThemeChanged;

        public static Color CorrectColor => Color.FromArgb(83, 141, 78);
        public static Color PresentColor => Color.FromArgb(181, 159, 59);
        public static Color AbsentColor  => Color.FromArgb(120, 124, 126);

        public static Color Background => IsDarkMode
            ? Color.FromArgb(18, 24, 18)
            : Color.White;

        public static Color Surface => IsDarkMode
            ? Color.FromArgb(28, 38, 28)
            : Color.FromArgb(245, 248, 245);

        public static Color Foreground => IsDarkMode
            ? Color.White
            : Color.FromArgb(20, 30, 20);

        public static Color SubtleForeground => IsDarkMode
            ? Color.FromArgb(180, 190, 180)
            : Color.FromArgb(90, 100, 90);

        public static Color Accent => Color.FromArgb(83, 141, 78);

        public static Color AccentHover => Color.FromArgb(70, 125, 65);

        public static Color Secondary => IsDarkMode
            ? Color.FromArgb(140, 200, 130)
            : Color.FromArgb(40, 90, 35);

        public static Color BoxBorder => IsDarkMode
            ? Color.FromArgb(70, 80, 70)
            : Color.FromArgb(211, 214, 218);

        public static Color BoxBackground => IsDarkMode
            ? Color.FromArgb(28, 38, 28)
            : Color.White;

        public static void Toggle()
        {
            IsDarkMode = !IsDarkMode;
            ThemeChanged?.Invoke();
        }
    }
}
