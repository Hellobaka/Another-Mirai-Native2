using Another_Mirai_Native.UI.ViewModel;
using System.Windows.Controls;
using Another_Mirai_Native.Native;
using Another_Mirai_Native.Model.Enums;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using Another_Mirai_Native.UI.Controls;

namespace Another_Mirai_Native.UI.Pages
{
    /// <summary>
    /// PluginPage.xaml 的交互逻辑
    /// </summary>
    public partial class PluginPage : Page
    {
        public PluginPageViewModel ViewModel { get; set; }

        public PluginPage()
        {
            InitializeComponent();
            ViewModel = new PluginPageViewModel();
            DataContext = ViewModel;
            Unloaded += PluginPage_Unloaded;
        }

        private void PluginPage_Unloaded(object sender, System.Windows.RoutedEventArgs e)
        {
            ViewModel.UnsubscribePluginProxyEvents();
        }

        private void OpenMenuBtn_Click(object sender, RoutedEventArgs e)
        {
            var selectedPlugin = ViewModel.SelectedPlugin;
            if (selectedPlugin == null)
            {
                return;
            }
            var menu = new ContextMenu();
            foreach (var item in selectedPlugin.AppInfo.menu)
            {
                MenuItem menuItem = new();
                menuItem.Header = item.name;
                menuItem.Click += (a, b) =>
                {
                    if (selectedPlugin.Enabled is false)
                    {
                        DialogHelper.ShowSimpleDialog("嗯哼", "当前插件未启用，无法调用窗口事件");
                        return;
                    }
                    Task.Run(() =>
                    {
                        PluginManagerProxy.Instance.InvokeEvent(selectedPlugin, PluginEventType.Menu, item.function);
                    });
                };
                menu.Items.Add(menuItem);
            }
            menu.PlacementTarget = (UIElement)sender;
            menu.Placement = PlacementMode.MousePoint;
            menu.IsOpen = true;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            ViewModel.SubscribePluginProxyEvents();
        }
    }
}
