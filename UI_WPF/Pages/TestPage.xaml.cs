using Another_Mirai_Native.Config;
using Another_Mirai_Native.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace Another_Mirai_Native.UI.Pages
{
    /// <summary>
    /// TestPage.xaml 的交互逻辑
    /// </summary>
    public partial class TestPage : Page
    {
        public TestPage()
        {
            InitializeComponent();
            Instance = this;
        }

        public static TestPage Instance { get; set; }

        public static CQPluginProxy? CurrentPlugin { get; private set; }

        private Dictionary<string, object> PageCache { get; set; } = new();

        private void NavigationView_SelectionChanged(ModernWpf.Controls.NavigationView sender, ModernWpf.Controls.NavigationViewSelectionChangedEventArgs args)
        {
            var selectedItem = (ModernWpf.Controls.NavigationViewItem)args.SelectedItem;
            if (selectedItem != null)
            {
                string selectedItemTag = (string)selectedItem.Tag;
                if (PageCache.ContainsKey(selectedItemTag))
                {
                    MainFrame.Navigate(PageCache[selectedItemTag]);
                }
                else
                {
                    Type? pageType = typeof(MainWindow).Assembly.GetType("Another_Mirai_Native.UI.Pages." + selectedItemTag)
                        ?? typeof(MainWindow).Assembly.GetType("Another_Mirai_Native.UI.Pages.Setting_CustomPage");
                    if (pageType == null)
                    {
                        return;
                    }
                    var obj = Activator.CreateInstance(pageType);
                    PageCache.Add(selectedItemTag, obj);
                    MainFrame.Navigate(obj);
                }
            }
        }

        private void Page_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            CurrentPlugin = PluginManagerProxy.Proxies.FirstOrDefault(x => x.AppInfo.AuthCode == AppConfig.TestingAuthCode);
            if (CurrentPlugin == null)
            {
                DialogHelper.ShowSimpleDialog("嗯？", "当前没有插件被标记为测试，请前往插件窗口选择测试插件");
                MainWindow.Instance.PluginMenuItem.IsSelected = true;
                return;
            }
            PageTitle.Text = $"当前测试插件: {CurrentPlugin.PluginName}";
        }
    }
}