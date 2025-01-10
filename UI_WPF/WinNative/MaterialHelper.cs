using ModernWpf;
using ModernWpf.Controls;
using System;
using System.Resources;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;

namespace Another_Mirai_Native.UI
{
    public partial class MainWindow
    {
        public enum Material
        {
            None,

            Mica = 2,

            Acrylic = 3,

            Tabbed = 4
        }

        public static int ExtendFrame(IntPtr hwnd, ParameterTypes.MARGINS margins)
        {
            return DwmExtendFrameIntoClientArea(hwnd, ref margins);
        }

        public static OSVersion GetOSVersion()
        {
            OSVersion osVersionInfo = new OSVersion { dwOSVersionInfoSize = (uint)Marshal.SizeOf(typeof(OSVersion)) };
            RtlGetVersion(ref osVersionInfo);
            return osVersionInfo;
        }

        [DllImport("ntdll.dll")]
        public static extern int RtlGetVersion(ref OSVersion osVersionInfo);

        public static int SetWindowAttribute(IntPtr hwnd, ParameterTypes.DWMWINDOWATTRIBUTE attribute, int parameter)
        {
            return DwmSetWindowAttribute(hwnd, attribute, ref parameter, Marshal.SizeOf<int>());
        }

        public void ChangeMaterial(Material material)
        {
            var osVersion = GetOSVersion();
            if (!(osVersion.dwMajorVersion >= 10 && osVersion.dwBuildNumber >= 22000))
            {
                UIConfig.Instance.WindowMaterial = "None";
                UIConfig.Instance.SetConfig("WindowMaterial", "None");
                throw new NotSupportedException("更换材质只支持Windows11 21H2版本以上。");
            }
            Background = Brushes.Transparent;
            Resources["SymbolThemeFontFamily"] = new FontFamily("Segoe Fluent Icons");
            int value = (int)material;
            SetWindowAttribute(new WindowInteropHelper(this).Handle, ParameterTypes.DWMWINDOWATTRIBUTE.DWMWA_SYSTEMBACKDROP_TYPE,
                value);
        }

        public static void SetNavigationViewTransparent(NavigationView navigationView)
        {
            UIElement? GetPaneRoot(DependencyObject element, NavigationViewPaneDisplayMode findMode)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
                {
                    var e = VisualTreeHelper.GetChild(element, i);
                    if (e is Grid grid && grid.Name == "PaneRoot" && findMode != NavigationViewPaneDisplayMode.Top)
                    {
                        return grid;
                    }
                    if (e is StackPanel top && top.Name == "TopNavArea" && findMode == NavigationViewPaneDisplayMode.Top)
                    {
                        return top;
                    }
                    else if (e is Grid ignoreGrid && ignoreGrid.Name == "ContentRoot")
                    {
                        continue;
                    }
                    else
                    {
                        var result = GetPaneRoot(e, findMode);
                        if (result != null)
                        {
                            return result;
                        }
                    }
                }
                return null;
            }

            var paneRoot = GetPaneRoot(navigationView, navigationView.PaneDisplayMode);
            if (paneRoot != null)
            {
                if(paneRoot is StackPanel stackPanel)
                {
                    stackPanel.Background = Brushes.Transparent;
                }
                else if(paneRoot is Grid grid)
                {
                    grid.Background = Brushes.Transparent;
                }
            }
        }

        [DllImport("DwmApi.dll")]
        private static extern int DwmExtendFrameIntoClientArea(
            IntPtr hwnd,
            ref ParameterTypes.MARGINS pMarInset);

        [DllImport("DwmApi.dll")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, ParameterTypes.DWMWINDOWATTRIBUTE dwAttribute, ref int pvAttribute, int cbAttribute);

        private void RefreshDarkMode()
        {
            var isDark = ThemeManager.Current.ActualApplicationTheme == ApplicationTheme.Dark;
            int flag = isDark ? 1 : 0;
            SetWindowAttribute(
                new WindowInteropHelper(this).Handle,
                ParameterTypes.DWMWINDOWATTRIBUTE.DWMWA_USE_IMMERSIVE_DARK_MODE,
                flag);
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

        [StructLayout(LayoutKind.Sequential)]
        public struct OSVersion
        {
            public uint dwOSVersionInfoSize;

            public uint dwMajorVersion;

            public uint dwMinorVersion;

            public uint dwBuildNumber;

            public uint dwPlatformId;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string szCSDVersion;

            public ushort wServicePackMajor;

            public ushort wServicePackMinor;

            public ushort wSuiteMask;

            public byte wProductType;

            public byte wReserved;
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