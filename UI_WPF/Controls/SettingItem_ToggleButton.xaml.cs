using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Another_Mirai_Native.UI.Controls
{
    /// <summary>
    /// SettingItem.xaml 的交互逻辑
    /// </summary>
    public partial class SettingItem_ToggleButton : ContentControl
    {
        public SettingItem_ToggleButton()
        {
            InitializeComponent();
        }

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(SettingItem_ToggleButton), new PropertyMetadata(""));

        public string Subtitle
        {
            get { return (string)GetValue(SubtitleProperty); }
            set { SetValue(SubtitleProperty, value); }
        }

        public static readonly DependencyProperty SubtitleProperty =
            DependencyProperty.Register("Subtitle", typeof(string), typeof(SettingItem_ToggleButton), new PropertyMetadata(""));



        public bool Toggled
        {
            get { return (bool)GetValue(ToggledProperty); }
            set { SetValue(ToggledProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Toggled.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ToggledProperty =
            DependencyProperty.Register("Toggled", typeof(bool), typeof(SettingItem_ToggleButton), new PropertyMetadata(false));

        public event Action<object, bool> OnToggled;

        private void ToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            Toggled = Selector.IsOn;
            OnToggled?.Invoke(this, Toggled);
        }
    }

    public class StringToVisibilityConverter : IValueConverter
    {
        public static StringToVisibilityConverter Instance { get; } = new StringToVisibilityConverter();

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return string.IsNullOrEmpty(value as string) ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class SubTitleToVerticalAlignmentConverter : IValueConverter
    {
        public static SubTitleToVerticalAlignmentConverter Instance { get; } = new SubTitleToVerticalAlignmentConverter();

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return string.IsNullOrEmpty(value as string) ? VerticalAlignment.Center : VerticalAlignment.Top;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
