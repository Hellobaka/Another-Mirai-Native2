using ModernWpf.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Another_Mirai_Native.UI
{
    public static class Helper
    {
        internal static async Task<ContentDialogResult> ShowAndDisableMinimizeToTrayAsync(this ContentDialog contentDialog)
        {
            var mw = (MainWindow)Application.Current.MainWindow;
            var result = await contentDialog.ShowAsync();

            return result;
        }
    }
}