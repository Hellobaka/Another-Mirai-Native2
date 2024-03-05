using Another_Mirai_Native.UI.Pages;
using Another_Mirai_Native.UI.ViewModel;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Another_Mirai_Native.UI.Controls
{
    /// <summary>
    /// ChatListItem.xaml 的交互逻辑
    /// </summary>
    public partial class ChatListItem : UserControl
    {
        public ChatListItem()
        {
            //DataContext = this;
            InitializeComponent();
        }

        public static readonly DependencyProperty ItemProperty =
            DependencyProperty.Register(
                "Item",
                typeof(ChatListItemViewModel),
                typeof(ChatListItem),
                new PropertyMetadata(new ChatListItemViewModel(), OnItemChanged));

        public ChatListItemViewModel Item
        {
            get => (ChatListItemViewModel)GetValue(ItemProperty);
            set => SetValue(ItemProperty, value);
        }

        private static void OnItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChatListItem control = (ChatListItem)d;
            ChatListItemViewModel newValue = (ChatListItemViewModel)e.NewValue;

            control.ViewModel = newValue;
            control.Id = newValue.Id;
            control.GroupName = newValue.GroupName;
            control.Detail = newValue.Detail;
            control.Time = newValue.Time;
            control.UnreadCount = newValue.UnreadCount;

            control.UpdateControl();
        }

        private void UpdateControl()
        {
            UnreadTip.Visibility = UnreadCount > 0 ? Visibility.Visible : Visibility.Collapsed;
            UnreadCountDisplay.FontSize = UnreadTip.Width * 0.5;
        }

        public int ItemId { get; set; }

        public long Id { get; set; }

        public string GroupName { get; set; } = "";

        public string Detail { get; set; } = "";

        public DateTime Time { get; set; } = DateTime.Now;

        public int UnreadCount { get; set; }

        public ChatListItemViewModel ViewModel { get; set; }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void UnreadTip_MouseDown(object sender, MouseButtonEventArgs e)
        {
            UnreadCount = 0;
            UpdateControl();
            ChatPage.Instance.UpdateUnreadCount(ViewModel);
        }
    }
}
