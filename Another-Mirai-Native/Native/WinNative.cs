using System.Runtime.InteropServices;

namespace Another_Mirai_Native.Native
{
    public static class WinNative
    {
        [DllImport("user32.dll")]
        public static extern bool SetProcessDPIAware();

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetProcAddress(IntPtr lib, string funcName);

        [DllImport("kernel32.dll")]
        public static extern int GetLastError();

        [DllImport("kernel32.dll")]
        public static extern IntPtr LoadLibrary(string path);
    }
}
