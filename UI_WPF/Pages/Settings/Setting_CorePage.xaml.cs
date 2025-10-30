using Another_Mirai_Native.Config;
using Another_Mirai_Native.UI.Controls;
using ModernWpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Another_Mirai_Native.UI.Pages
{
    /// <summary>
    /// Setting_CorePage.xaml 的交互逻辑
    /// </summary>
    public partial class Setting_CorePage : Page, INotifyPropertyChanged
    {
        public Setting_CorePage()
        {
            InitializeComponent();
            DataContext = this;
        }

        public string[] Protocols { get; set; }

        private bool FormLoaded { get; set; }

        private PropertyInfo[] AppConfigProperties { get; set; } = [];

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (FormLoaded)
            {
                return;
            }
            AppConfigProperties = typeof(AppConfig).GetProperties();
            Protocols = ProtocolManager.Protocols.Select(x => x.Name).ToArray();
            SetConfigToControl();
            InitDisplay();
            FormLoaded = true;

            OnPropertyChanged(nameof(Protocols));
        }

        private void SetConfigToControl()
        {
            var properties = AppConfig.Instance.GetType().GetProperties();
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
                if (control is SettingItem_ComboBox c)
                {
                    comboBox = c;
                }
                else if (control is SettingItem_TextBox t)
                {
                    textBox = t;
                }
                else if (control is SettingItem_ToggleButton b)
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
                        object? value = property.GetValue(AppConfig.Instance);
                        if (value == null)
                        {
                            break;
                        }
                        if (comboBox != null)
                        {
                            comboBox.SelectedItem = value;
                        }
                        else if (textBox != null)
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

        private void InitDisplay()
        {
            OfflineActionCommandAdd.Text = string.Empty;
            OfflineActionCommands.Items.Clear();
            foreach (var item in AppConfig.Instance.OfflineActionCommands)
            {
                if (string.IsNullOrEmpty(item))
                {
                    continue;
                }
                OfflineActionCommands.Items.Add(item);
            }
        }

        private void OfflineActionCommandAddButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(OfflineActionCommandAdd.Text))
            {
                bool duplicate = false;
                foreach (var item in OfflineActionCommands.Items)
                {
                    if (item.ToString() == OfflineActionCommandAdd.Text)
                    {
                        duplicate = true;
                        break;
                    }
                }
                if (duplicate)
                {
                    DialogHelper.ShowSimpleDialog("嗯？", "已存在相同项");
                    return;
                }
                OfflineActionCommands.Items.Add(OfflineActionCommandAdd.Text);
                UpdateOfflineCommands();
            }
            else
            {
                DialogHelper.ShowSimpleDialog("嗯？", "输入内容格式错误");
            }
        }

        private void OfflineActionCommandRemoveButton_Click(object sender, RoutedEventArgs e)
        {
            if (OfflineActionCommands.SelectedIndex < 0)
            {
                DialogHelper.ShowSimpleDialog("嗯？", "请选择一项");
                return;
            }
            OfflineActionCommands.Items.RemoveAt(OfflineActionCommands.SelectedIndex);
            UpdateOfflineCommands();
        }

        private void UpdateOfflineCommands()
        {
            List<string> commands = [];
            foreach (var item in OfflineActionCommands.Items)
            {
                if(item == null)
                {
                    continue;
                }
                commands.Add(item.ToString());
            }
            AppConfig.Instance.OfflineActionCommands = commands;
            AppConfig.Instance.SetConfig("OfflineActionCommands", commands);
        }

        private void SettingItem_Toggled(object sender, bool value)
        {
            if (!FormLoaded || sender is not SettingItem_ToggleButton toggleButton)
            {
                return;
            }
            foreach (var property in AppConfigProperties)
            {
                if (property.Name != toggleButton.Name)
                {
                    continue;
                }
                property.SetValue(AppConfig.Instance, value);
                AppConfig.Instance.SetConfig(property.Name, value);
            }
            AfterConfigSet(toggleButton.Name, value);
        }

        private void SettingItem_ComboBox_SelectedItemChanged(object sender, object value)
        {
            if (!FormLoaded || sender is not SettingItem_ComboBox comboBox)
            {
                return;
            }
            foreach (var property in AppConfigProperties)
            {
                if (property.Name != comboBox.Name)
                {
                    continue;
                }
                property.SetValue(AppConfig.Instance, value);
                AppConfig.Instance.SetConfig(property.Name, value);
            }
            AfterConfigSet(comboBox.Name, value);
        }

        private void SettingItem_TextBox_DataChanged(object sender, object value)
        {
            if (!FormLoaded || sender is not SettingItem_TextBox textBox)
            {
                return;
            }
            foreach (var property in AppConfigProperties)
            {
                if (property.Name != textBox.Name)
                {
                    continue;
                }
                property.SetValue(AppConfig.Instance, value);
                AppConfig.Instance.SetConfig(property.Name, value);
            }
            AfterConfigSet(textBox.Name, value);
        }

        private void AfterConfigSet(string name, object value)
        {
            switch (name)
            {

                default:
                    break;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName) =>
                 PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}