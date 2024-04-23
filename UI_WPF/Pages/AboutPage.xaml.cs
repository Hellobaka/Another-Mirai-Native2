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

namespace Another_Mirai_Native.UI.Pages
{
    /// <summary>
    /// AboutPage.xaml 的交互逻辑
    /// </summary>
    public partial class AboutPage : Page
    {
        public AboutPage()
        {
            InitializeComponent();
            // TODO: 实现此页面
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            int flag = int.Parse((string)((RadioButton)sender).Tag);
            MainWindow.Instance.ChangeMaterial((MainWindow.Material)flag);
            MainWindow.Instance.Background = Brushes.Transparent;
        }
    }
}
