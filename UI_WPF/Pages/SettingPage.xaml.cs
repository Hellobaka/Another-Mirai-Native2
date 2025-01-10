using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Controls;

namespace Another_Mirai_Native.UI.Pages
{
    /// <summary>
    /// SettingPage.xaml 的交互逻辑
    /// </summary>
    public partial class SettingPage : Page
    {
        public SettingPage()
        {
            InitializeComponent();
            Instance = this;
        }

        private Dictionary<string, object> PageCache { get; set; } = new();

        private List<string> MenuList { get; set; } = new();

        public static SettingPage Instance { get; private set; }

        private void SettingContainer_SelectionChanged(ModernWpf.Controls.NavigationView sender, ModernWpf.Controls.NavigationViewSelectionChangedEventArgs args)
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
                    if(obj == null)
                    {
                        return;
                    }
                    PageCache.Add(selectedItemTag, obj);
                    MainFrame.Navigate(obj);
                }
            }
        }

        private void Page_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            MainWindow.SetNavigationViewTransparent(SettingContainer);
            foreach (var item in Directory.GetFiles("conf", "*.json"))
            {
                if (string.IsNullOrEmpty(item) || item == @"conf\Config.json" || item == @"conf\UIConfig.json")
                {
                    continue;
                }
                string name = item.Replace(@"conf\", "").Replace(".json", "");
                if (MenuList.Any(x => x == name))
                {
                    continue;
                }
                SettingContainer.MenuItems.Add(new ModernWpf.Controls.NavigationViewItem
                {
                    Content = name,
                    Tag = item
                });
                MenuList.Add(name);
            }
        }
    }
}