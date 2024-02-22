using ModernWpf;
using System;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.Windows.Media;

namespace Another_Mirai_Native.UI
{
    public partial class MainWindow
    {
        public enum Material
        {
            Mica = 2,
            Acrylic = 3,
            Tabbed = 4
        }

        private void RefreshFrame()
        {
            IntPtr mainWindowPtr = new WindowInteropHelper(this).Handle;
            HwndSource mainWindowSrc = HwndSource.FromHwnd(mainWindowPtr);
            mainWindowSrc.CompositionTarget.BackgroundColor = Color.FromArgb(0, 0, 0, 0);

            ParameterTypes.MARGINS margins = new()
            {
                cxLeftWidth = -1,
                cxRightWidth = -1,
                cyTopHeight = -1,
                cyBottomHeight = -1
            };

            ExtendFrame(mainWindowSrc.Handle, margins);
        }

        private void RefreshDarkMode()
        {
            var isDark = ThemeManager.Current.ActualApplicationTheme == ApplicationTheme.Dark;
            int flag = isDark ? 1 : 0;
            SetWindowAttribute(
                new WindowInteropHelper(this).Handle,
                ParameterTypes.DWMWINDOWATTRIBUTE.DWMWA_USE_IMMERSIVE_DARK_MODE,
                flag);
        }

        public void ChangeMaterial(Material material)
        {
            SetWindowAttribute(
                new WindowInteropHelper(this).Handle,
                ParameterTypes.DWMWINDOWATTRIBUTE.DWMWA_SYSTEMBACKDROP_TYPE,
                (int)material);
        }

        [DllImport("DwmApi.dll")]
        private static extern int DwmExtendFrameIntoClientArea(
            IntPtr hwnd,
            ref ParameterTypes.MARGINS pMarInset);

        [DllImport("dwmapi.dll")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, ParameterTypes.DWMWINDOWATTRIBUTE dwAttribute, ref int pvAttribute, int cbAttribute);

        public static int ExtendFrame(IntPtr hwnd, ParameterTypes.MARGINS margins)
        {
            return DwmExtendFrameIntoClientArea(hwnd, ref margins);
        }

        public static int SetWindowAttribute(IntPtr hwnd, ParameterTypes.DWMWINDOWATTRIBUTE attribute, int parameter)
        {
            return DwmSetWindowAttribute(hwnd, attribute, ref parameter, Marshal.SizeOf<int>());
        }
    }

    public class ParameterTypes
    {
        /*
        [Flags]
        enum DWM_SYSTEMBACKDROP_TYPE
        {
            DWMSBT_MAINWINDOW = 2, // Mica
            DWMSBT_TRANSIENTWINDOW = 3, // Acrylic
            DWMSBT_TABBEDWINDOW = 4 // Tabbed
        }
        */

        [Flags]
        public enum DWMWINDOWATTRIBUTE
        {
            DWMWA_USE_IMMERSIVE_DARK_MODE = 20,
            DWMWA_SYSTEMBACKDROP_TYPE = 38
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MARGINS
        {
            public int cxLeftWidth;      // width of left border that retains its size
            public int cxRightWidth;     // width of right border that retains its size
            public int cyTopHeight;      // height of top border that retains its size
            public int cyBottomHeight;   // height of bottom border that retains its size
        };
    }
}
