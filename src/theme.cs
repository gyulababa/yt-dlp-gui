using System.Drawing;

namespace YtDlpGuiApp
{
    public static class Theme
    {
        public enum Mode { Light, Dark }

        public static Mode Current = Mode.Light;

        public static Color BackColor =>
            Current == Mode.Dark ? Color.FromArgb(30, 30, 30) : Color.White;

        public static Color ForeColor =>
            Current == Mode.Dark ? Color.White : Color.Black;

        public static Color ButtonBack =>
            Current == Mode.Dark ? Color.FromArgb(60, 60, 60) : Color.LightGray;
    }
}

