using Another_Mirai_Native.UI.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
            get { return (ChatListItemViewModel)GetValue(ItemProperty); }
            set { SetValue(ItemProperty, value); }
        }

        private static void OnItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChatListItem control = (ChatListItem)d;
            ChatListItemViewModel newValue = (ChatListItemViewModel)e.NewValue;

            control.Id = newValue.Id;
            control.GroupName = newValue.GroupName;
            control.Detail = newValue.Detail;
            control.Time = newValue.Time;
        }

        public int ItemId { get; set; }

        public long Id { get; set; }

        public string GroupName { get; set; } = "";

        public string Detail { get; set; } = "";

        public DateTime Time { get; set; } = DateTime.Now;

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
