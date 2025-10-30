using Another_Mirai_Native.UI.Controls;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
#if NET5_0_OR_GREATER
using Another_Mirai_Native.BlazorUI;
#endif

namespace Another_Mirai_Native.UI.Pages
{
    /// <summary>
    /// Setting_UISettingPage.xaml 的交互逻辑
    /// </summary>
    public partial class Setting_WebUISettingPage : Page, INotifyPropertyChanged
    {
        public Setting_WebUISettingPage()
        {
            InitializeComponent();
            DataContext = this;
        }

        public bool FormLoaded { get; set; }

        private PropertyInfo[] AppConfigProperties { get; set; } = [];

        private Debouncer Debouncer { get; set; } = new();

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (FormLoaded)
            {
                return;
            }
#if NET5_0_OR_GREATER
            AppConfigProperties = typeof(Blazor_Config).GetProperties();
            SetConfigToControl();
#endif
            FormLoaded = true;
        }

        private void SetConfigToControl()
        {
#if NET5_0_OR_GREATER
            var properties = Blazor_Config.Instance.GetType().GetProperties();
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
                        object? value = property.GetValue(Blazor_Config.Instance);
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
#endif
        }

        private void SettingItem_TextBox_DataChanged(object sender, object value)
        {
#if NET5_0_OR_GREATER
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
                var currentValue = property.GetValue(Blazor_Config.Instance);
                if (currentValue?.ToString() != value.ToString())
                {
                    property.SetValue(Blazor_Config.Instance, value);
                    Blazor_Config.Instance.SetConfig(property.Name, value);
                }
            }
            AfterConfigSet(textBox.Name, value);
#endif
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName) =>
                 PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private void AfterConfigSet(string name, object value)
        {
            switch (name)
            {
                default:
                    break;
            }
        }
    }
}