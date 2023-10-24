using ModernWpf.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Another_Mirai_Native.UI.Controls
{
    /// <summary>
    /// SimpleMessageBox.xaml 的交互逻辑
    /// </summary>
    public partial class SimpleMessageBox : ContentDialog
    {
        public SimpleMessageBox()
        {
            InitializeComponent();
        }

        public string ErrorHint { get; set; } = "哎呀...发生错误了";

        public string ErrorMessage { get; set; } = "";

        public string ErrorDetail { get; set; } = "";

        public string CopyMessage { get; set; } = "";

        private void ContentDialog_Loaded(object sender, RoutedEventArgs e)
        {
            DetailExpander.Visibility = !string.IsNullOrEmpty(ErrorDetail) ? Visibility.Visible : Visibility.Collapsed;
            DataContext = this;
        }

        private void CopyDetail_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Clipboard.SetText(ErrorDetail);
                CopyMessage = "复制成功";
            }
            catch
            {
                CopyMessage = "复制失败";
            }
            ButtonFlyout.ShowAt(CopyDetail);
        }
    }
}