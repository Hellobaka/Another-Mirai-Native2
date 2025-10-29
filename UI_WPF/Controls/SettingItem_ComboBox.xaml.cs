using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Another_Mirai_Native.UI.Controls
{
    /// <summary>
    /// SettingItem_ComboBox.xaml 的交互逻辑
    /// </summary>
    public partial class SettingItem_ComboBox : ContentControl
    {
        public SettingItem_ComboBox()
        {
            InitializeComponent();
        }

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(SettingItem_ComboBox), new PropertyMetadata(""));

        public string Subtitle
        {
            get { return (string)GetValue(SubtitleProperty); }
            set { SetValue(SubtitleProperty, value); }
        }

        public static readonly DependencyProperty SubtitleProperty =
            DependencyProperty.Register("Subtitle", typeof(string), typeof(SettingItem_ComboBox), new PropertyMetadata(""));

        public IEnumerable ItemSources
        {
            get { return (IEnumerable)GetValue(ItemSourcesProperty); }
            set { SetValue(ItemSourcesProperty, value); }
        }

        public static readonly DependencyProperty ItemSourcesProperty =
            DependencyProperty.Register("ItemSources", typeof(IEnumerable), typeof(SettingItem_ComboBox), new PropertyMetadata(null));

        public object SelectedItem
        {
            get { return GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(object), typeof(SettingItem_ComboBox), new PropertyMetadata(null));

        public event Action<object, object> SelectedItemChanged;

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedItemChanged?.Invoke(this, SelectedItem);
        }
    }
}