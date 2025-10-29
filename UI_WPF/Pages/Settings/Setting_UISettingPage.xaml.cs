using Another_Mirai_Native.Config;
using Another_Mirai_Native.UI.Controls;
using Another_Mirai_Native.UI.Models;
using Microsoft.Win32;
using ModernWpf;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Interop;
using System.Windows.Media;

namespace Another_Mirai_Native.UI.Pages
{
    /// <summary>
    /// Setting_UISettingPage.xaml 的交互逻辑
    /// </summary>
    public partial class Setting_UISettingPage : Page, INotifyPropertyChanged
    {
        public Setting_UISettingPage()
        {
            InitializeComponent();
            DataContext = this;
            // TODO: 缓存自动清理
        }

        public bool FormLoaded { get; set; }

        public Array ThemeValues { get; set; } = Enum.GetValues(typeof(SystemTheme));

        public Array MaterialValues { get; set; } = Enum.GetValues(typeof(WindowMaterial));

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (FormLoaded)
            {
                return;
            }
            FormLoaded = true;
            SetConfigToControl();
            AutoStartup.Toggled = CheckHasStartupRegistry();
#if NET48
            WebUIAutoStart.IsEnabled = false;
#endif
        }

        private void SetConfigToControl()
        {
            var properties = UIConfig.Instance.GetType().GetProperties();
            foreach (var field in GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance))
            {
                var control = field.GetValue(this);
                if (!(control?.GetType().Name ?? "").StartsWith("SettingItem_"))
                {
                    continue;
                }
                SettingItem_ComboBox? comboBox = null;
                SettingItem_TextBox? textBox = null;
                SettingItem_ToggleButton? toggleButton = null;
                if(control is SettingItem_ComboBox c)
                {
                    comboBox = c;
                }
                else if(control is SettingItem_TextBox t)
                {
                    textBox = t;
                }
                else if(control is SettingItem_ToggleButton b)
                {
                    toggleButton = b;
                }
                else
                {
                    continue;
                }
                foreach (var property in properties)
                {
                    if (property.Name == field.Name)
                    {
                        object? value = property.GetValue(UIConfig.Instance);
                        if (value == null)
                        {
                            break;
                        }
                        if (comboBox != null)
                        {
                            comboBox.SelectedItem = value;
                        }
                        else if(textBox != null)
                        {
                            textBox.Data = value;
                        }
                        else if (toggleButton != null && value is bool b)
                        {
                            toggleButton.Toggled = b;
                        }
                    }
                }
            }
        }

        private static bool CheckHasStartupRegistry() => Registry.CurrentUser?.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true)?.GetValueNames().Any(x => x == "Another-Mirai-Native2") ?? false;

        private static void EditStartupProgram(bool add)
        {
            string name = "";
#if NET48
            string? path = Assembly.GetEntryAssembly().Location;
#else
            string? path = Environment.ProcessPath;
#endif
            if (string.IsNullOrEmpty(path))
            {
                DialogHelper.ShowSimpleDialog("设置开机启动", "无法获取程序启动路径");
                return;
            }
            RegistryKey? rk = Registry.CurrentUser?.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            if (rk == null)
            {
                DialogHelper.ShowSimpleDialog("设置开机启动", "自启动项添加失败，请检查运行权限。");
                return;
            }
            if (add)
            {
                rk.SetValue(name, path);
            }
            else
            {
                rk.DeleteValue(name);
            }
        }

        private void SettingItem_Toggled(object sender, bool value)
        {
            if (!FormLoaded || sender is not SettingItem_ToggleButton toggleButton)
            {
                return;
            }
            var properties = UIConfig.Instance.GetType().GetProperties();
            foreach (var property in properties)
            {
                if (property.Name != toggleButton.Name)
                {
                    continue;
                }
                property.SetValue(UIConfig.Instance, value);
                UIConfig.Instance.SetConfig(property.Name, value);
            }
            AfterConfigSet(toggleButton.Name, value);
        }

        private void SettingItem_TextBox_DataChanged(object sender, object value)
        {
            if (!FormLoaded || sender is not SettingItem_TextBox textBox)
            {
                return;
            }
            var properties = UIConfig.Instance.GetType().GetProperties();
            foreach (var property in properties)
            {
                if (property.Name != textBox.Name)
                {
                    continue;
                }
                property.SetValue(UIConfig.Instance, value);
                UIConfig.Instance.SetConfig(property.Name, value);
            }
            AfterConfigSet(textBox.Name, value);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName) =>
                 PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private void ThemeSelector_SelectedItemChanged(object sender, object value)
        {
            if (!FormLoaded || sender is not SettingItem_ComboBox comboBox)
            {
                return;
            }
            var properties = UIConfig.Instance.GetType().GetProperties();
            foreach (var property in properties)
            {
                if (property.Name != comboBox.Name)
                {
                    continue;
                }
                property.SetValue(UIConfig.Instance, value);
                UIConfig.Instance.SetConfig(property.Name, value);
            }
            AfterConfigSet(comboBox.Name, value);
        }

        private void AfterConfigSet(string name, object value)
        {
            switch (name)
            {
                case "AutoStartup":
                    if(value is bool autoStartup)
                    {
                        EditStartupProgram(autoStartup);
                    }
                    break;

                case "Theme":
                    ThemeManager.Current.ApplicationTheme =  UIConfig.Instance.Theme switch
                    { 
                        SystemTheme.Dark => ApplicationTheme.Dark,
                        SystemTheme.Light => ApplicationTheme.Light,
                        _ => null
                    };
                    break;

                case "WindowMaterial":
                    MainWindow.Instance.ChangeMaterial(UIConfig.Instance.WindowMaterial switch
                    {
                        Models.WindowMaterial.Mica => MainWindow.Material.Mica,
                        Models.WindowMaterial.Acrylic => MainWindow.Material.Acrylic,
                        Models.WindowMaterial.Tabbed => MainWindow.Material.Tabbed,
                        _ => MainWindow.Material.None
                    });
                    break;

                default:
                    break;
            }
        }
    }
}