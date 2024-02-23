using Another_Mirai_Native.Model;
using Another_Mirai_Native.UI.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
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
    public partial class ChatDetailListItem_Left : UserControl
    {
        public ChatDetailListItem_Left()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty ItemProperty =
            DependencyProperty.Register(
                "Item",
                typeof(ChatDetailItemViewModel),
                typeof(ChatDetailListItem_Left),
                new PropertyMetadata(new ChatDetailItemViewModel(), OnItemChanged));

        public ChatDetailItemViewModel Item
        {
            get { return (ChatDetailItemViewModel)GetValue(ItemProperty); }
            set { SetValue(ItemProperty, value); }
        }

        public string Message { get; set; } = "";
        public DetailItemType DetailItemType { get; private set; }
        public string DisplayName { get; private set; }
        public DateTime Time { get; private set; }
        public long Id { get; private set; }

        private static void OnItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChatDetailListItem_Left control = (ChatDetailListItem_Left)d;
            ChatDetailItemViewModel newValue = (ChatDetailItemViewModel)e.NewValue;

            control.Message = newValue.Content;
            control.DetailItemType = newValue.DetailItemType;
            control.DisplayName = newValue.Nick;
            control.Time = newValue.Time;
            control.Id = newValue.Id;
            control.ParseAndBuildDetail();
        }

        public void ParseAndBuildDetail()
        {
            var ls = CQCode.Parse(Message);
            Avatar.DataContext = new ChatListItemViewModel
            {
                AvatarType = ChatAvatar.AvatarTypes.QQPrivate,
                GroupName = DisplayName,
                Id = Id
            };
            DataContext = this;
            foreach (var item in ls)
            {
                if (item.Function == Model.Enums.CQCodeType.Image)
                {

                }
                else if (item.Function == Model.Enums.CQCodeType.Record)
                {

                }
                else if(item.Function == Model.Enums.CQCodeType.Rich)
                {

                }
                else
                {
                    DetailContainer.Children.Add(BuildTextElement(item.ToSendString()));
                }
            }
        }

        public static TextBox BuildTextElement(string text)
        {
            return new TextBox
            {
                Text = text,
                Padding = new Thickness(10),
                TextWrapping = TextWrapping.Wrap,
                IsReadOnly = true,
                Background = Brushes.Transparent,
                BorderThickness = new Thickness(0)
            };
        }

        public static Image BuildImageElement(CQCode cqCode)
        {
            var img = new Image();
            img.BeginInit();
            img.Source = new BitmapImage(new Uri(Extend.GetImageUrlOrPathFromCQCode(cqCode)));
            img.EndInit();
            return img;
        }
    }
}
