using Another_Mirai_Native.DB;
using Another_Mirai_Native.UI.ViewModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Another_Mirai_Native.UI.Controls.Chat
{
    /// <summary>
    /// ChatListItem.xaml 的交互逻辑
    /// </summary>
    public partial class ChatListItem : UserControl
    {
        public ChatListItem()
        {
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

            newValue.Detail = newValue.Detail.Replace("\r", "").Replace("\n", "");
            control.ViewModel = newValue;
        }

        public ChatListItemViewModel ViewModel { get; set; }

        private void UnreadTip_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ViewModel.UnreadCount = 0;
            e.Handled = true;
        }
    }
}
