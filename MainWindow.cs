using System.Runtime.InteropServices;
using System.Text;

namespace Borderless
{
    public partial class MainWindow : Form
    {
        [DllImport("user32.dll")]
        public static extern bool EnumWindows(EnumWindowsProc enumProc, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        [DllImport("user32.dll")]
        public static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern void SwitchToThisWindow(IntPtr hWnd, bool altcheck);

        [DllImport("user32.dll")]
        public static extern long GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        public static extern int SetWindowLong(IntPtr hWnd, int nIndex, long dwNewLong);

        [DllImport("user32.dll")]
        static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        [DllImport("user32.dll")]
        static extern bool GetClientRect(IntPtr hWnd, out RECT lpRect);

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        // Constants for SetWindowPos flags
        const uint SWP_FRAMECHANGED = 0x0020;
        const uint SWP_NOMOVE = 0x0002;
        const uint SWP_NOSIZE = 0x0001;
        const uint SWP_NOZORDER = 0x0004;

        const int GWL_STYLE = -16;
        const long WS_BORDER = 0x00800000L;
        const long WS_CAPTION = 0x00C00000L;
        const long WS_SYSMENU = 0x00080000L;

        public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        public nint CurrentWindowID;
        public List<nint> windowIds = new List<nint>();
        public List<string> windowTitles = new List<string>();

        public MainWindow()
        {
            InitializeComponent();
            update_window_list();
        }

        public void update_window_list()
        {
            EnumWindows((hWnd, lParam) =>
            {
                if (IsWindowVisible(hWnd))
                {
                    int maxLength = 1024;
                    StringBuilder windowTitle = new StringBuilder(maxLength);
                    GetWindowText(hWnd, windowTitle, maxLength);

                    string title = windowTitle.ToString();
                    if (!string.IsNullOrWhiteSpace(title))
                    {
                        windowTitles.Add(title);
                        windowIds.Add(hWnd);
                    }
                }

                return true; // Continue enumeration
            }, IntPtr.Zero);
            comboBox1.DataSource = windowTitles;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            CurrentWindowID = windowIds[comboBox1.SelectedIndex];
            IntPtr hWnd = (IntPtr)CurrentWindowID;
            StringBuilder windowTitle = new StringBuilder(1024);
            GetWindowText(hWnd, windowTitle, 1024);
            textBox1.Text = hWnd + "\0";
            textBox2.Text = windowTitle.ToString();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            IntPtr hWnd = CurrentWindowID;

            // Remove WS_BORDER, WS_CAPTION, and WS_SYSMENU styles to make the window borderless
            long currentStyle = GetWindowLong(hWnd, GWL_STYLE);
            long newStyle = currentStyle & ~(WS_BORDER | WS_CAPTION | WS_SYSMENU);

            // Get the current window position and size
            RECT rect;
            GetClientRect(hWnd, out rect);
            int newWidth = rect.Right;  // Maintain the same width
            int newHeight = rect.Bottom; // Maintain the same height
            int xPos = 0;
            int yPos = 0;
            IntPtr zPos = -1;
            textBox3.Text = newWidth.ToString() + " , " + newHeight.ToString();

            // Update the window position and size and style
            SetWindowLong(hWnd, GWL_STYLE, newStyle);
            SetWindowPos(hWnd, zPos, xPos, yPos, newWidth, newHeight, SWP_FRAMECHANGED | SWP_NOMOVE);

            black_canvas black_canvas = new black_canvas();
            black_canvas.Show();
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            update_window_list();
        }
    }
}
