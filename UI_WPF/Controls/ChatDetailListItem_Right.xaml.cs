using Another_Mirai_Native.Model;
using Another_Mirai_Native.UI.Pages;
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
    public partial class ChatDetailListItem_Right : UserControl
    {
        public ChatDetailListItem_Right()
        {
            InitializeComponent();
            DataContext = this;
        }

        public string Message { get; set; } = "";
        public ChatAvatar.AvatarTypes AvatarType { get; set; } = ChatAvatar.AvatarTypes.Fallback;
        public DetailItemType DetailItemType { get; set; }
        public string DisplayName { get; set; }
        public DateTime Time { get; set; }
        public long Id { get; set; }
        public string GUID { get; set; }
        public bool ControlLoaded { get; set; }

        public void ParseAndBuildDetail()
        {
            Avatar.Item = new ChatListItemViewModel
            {
                AvatarType = AvatarType,
                GroupName = DisplayName,
                Id = Id
            };
            var ls = CQCode.Parse(Message);
            string msg = Message;
            foreach (var item in ls)
            {
                msg = msg.Replace(item.ToString(), "<!cqCode!>");// 将CQ码的位置使用占空文本替换
            }
            var p = msg.Split("<!cqCode!>");
            int cqCode_index = 0;
            for (int i = 0; i < p.Length; i++)
            {
                if (p[i] == "<!cqCode!>")
                {
                    var item = ls[cqCode_index];
                    if (item.Function == Model.Enums.CQCodeType.Image)
                    {

                    }
                    else if (item.Function == Model.Enums.CQCodeType.Record)
                    {

                    }
                    else if (item.Function == Model.Enums.CQCodeType.Rich)
                    {

                    }
                    else
                    {
                        DetailContainer.Children.Add(BuildTextElement(item.ToSendString()));
                    }
                    cqCode_index++;
                }
                else
                {
                    DetailContainer.Children.Add(BuildTextElement(p[i]));
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

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (ControlLoaded)
            {
                return;
            }
            ControlLoaded = true;
            ParseAndBuildDetail();
            ChatPage.WindowSizeChanged += ChatPage_WindowSizeChanged;
        }

        private void ChatPage_WindowSizeChanged(SizeChangedEventArgs e)
        {
            MaxWidth = e.NewSize.Width * 0.6;
        }
    }
}
