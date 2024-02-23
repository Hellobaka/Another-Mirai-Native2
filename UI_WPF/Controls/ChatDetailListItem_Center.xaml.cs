using Another_Mirai_Native.Model;
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
    /// ChatDetailListItem.xaml 的交互逻辑
    /// </summary>
    public partial class ChatDetailListItem_Center : UserControl
    {
        public ChatDetailListItem_Center()
        {
            InitializeComponent();
        }


        public static readonly DependencyProperty ItemProperty =
            DependencyProperty.Register(
                "Item",
                typeof(ChatDetailItemViewModel),
                typeof(ChatDetailListItem_Center),
                new PropertyMetadata(new ChatDetailItemViewModel(), OnItemChanged));

        public ChatDetailItemViewModel Item
        {
            get { return (ChatDetailItemViewModel)GetValue(ItemProperty); }
            set { SetValue(ItemProperty, value); }
        }

        public string Message { get; set; } = "";
        public DetailItemType DetailItemType { get; private set; }

        private static void OnItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChatDetailListItem_Center control = (ChatDetailListItem_Center)d;
            ChatDetailItemViewModel newValue = (ChatDetailItemViewModel)e.NewValue;

            control.Message = newValue.Content;
            control.DetailItemType = newValue.DetailItemType;
        }
    }
}
