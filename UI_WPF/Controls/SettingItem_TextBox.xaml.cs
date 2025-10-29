using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Another_Mirai_Native.UI.Controls
{
    /// <summary>
    /// SettingItem.xaml 的交互逻辑
    /// </summary>
    public partial class SettingItem_TextBox : ContentControl
    {
        public SettingItem_TextBox()
        {
            InitializeComponent();
        }

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(SettingItem_TextBox), new PropertyMetadata(""));

        public string Subtitle
        {
            get { return (string)GetValue(SubtitleProperty); }
            set { SetValue(SubtitleProperty, value); }
        }

        public static readonly DependencyProperty SubtitleProperty =
            DependencyProperty.Register("Subtitle", typeof(string), typeof(SettingItem_TextBox), new PropertyMetadata(""));

        public object Data
        {
            get { return (object)GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }

        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register("Data", typeof(object), typeof(SettingItem_TextBox), new PropertyMetadata(null));



        public string Unit
        {
            get { return (string)GetValue(UnitProperty); }
            set { SetValue(UnitProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Unit.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UnitProperty =
            DependencyProperty.Register("Unit", typeof(string), typeof(SettingItem_TextBox), new PropertyMetadata(""));



        public event Action<object, object> DataChanged;

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (TryParse(Input.Text, out object data))
            {
                Data = data;
                DataChanged?.Invoke(this, Data);
            }
        }

        private bool TryParse(string text, out object data)
        {
            data = null;
            if (Data is int
                && int.TryParse(text, out int int_value))
            {
                data = int_value;
                return true;
            }
            else if (Data is string s)
            {
                data = s;
                return true;
            }
            else if (Data is float
                && float.TryParse(text, out float float_value))
            {
                data = float_value;
                return true;
            }
            else if (Data is double
                && double.TryParse(text, out double double_value))
            {
                data = double_value;
                return true;
            }

            return false;
        }
    }
}