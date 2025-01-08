using System.Runtime.InteropServices;

namespace Another_Mirai_Native.Native
{
    public static class WinNative
    {
        public const int CTRL_C_EVENT = 0;

        public const int CTRL_CLOSE_EVENT = 2;

        public delegate bool ConsoleEventDelegate(int eventType);

        [DllImport("kernel32.dll")]
        public static extern bool SetConsoleCtrlHandler(ConsoleEventDelegate handler, bool add);

        [DllImport("user32.dll")]
        public static extern bool SetProcessDPIAware();

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetProcAddress(IntPtr lib, string funcName);

        [DllImport("kernel32.dll")]
        public static extern int GetLastError();

        [DllImport("kernel32.dll")]
        public static extern IntPtr LoadLibrary(string path);

        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr GetConsoleWindow();
    }
}