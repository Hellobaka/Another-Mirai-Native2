using Another_Mirai_Native.Model;
using Another_Mirai_Native.UI.Pages;
using Another_Mirai_Native.UI.ViewModel;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

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

        public ChatAvatar.AvatarTypes AvatarType { get; set; } = ChatAvatar.AvatarTypes.Fallback;

        public bool ControlLoaded { get; set; }

        public DetailItemType DetailItemType { get; set; }

        public string DisplayName { get; set; }

        public long GroupId { get; set; }

        public string GUID { get; set; }

        public int MsgId { get; set; }

        public long Id { get; set; }

        public string Message { get; set; } = "";

        public DateTime Time { get; set; }

        public bool Recalled { get; set; }

        public void ParseAndBuildDetail()
        {
            Message = UnionAllAtMsg(Message);
            var ls = CQCode.Parse(Message);
            int imageCount = ls.Count(x => x.IsImageCQCode);
            int recordCount = ls.Count(x => x.IsRecordCQCode);
            string msg = Message;
            foreach (var item in ls)
            {
                msg = msg.Replace(item.ToString(), "<!cqCode!>");
            }
            var p = msg.SplitV2("<!cqCode!>");
            int cqCode_index = 0;
            StackPanel imgContainer = imageCount == 1 && p.Length == 1 ? ImageDisplay : DetailContainer;
            for (int i = 0; i < p.Length; i++)
            {
                if (p[i] == "<!cqCode!>")
                {
                    var item = ls[cqCode_index];
                    if (item.Function == Model.Enums.CQCodeType.Image)
                    {
                        if (imageCount == 1 && p.Length == 1)
                        {
                            ImageBorder.Visibility = Visibility.Visible;
                            DetailBorder.Visibility = Visibility.Collapsed;
                        }
                        imgContainer.Children.Add(ChatDetailListItem_Common.BuildImageElement(item, MaxWidth * 0.5));
                    }
                    else
                    {
                        Expander expander = new()
                        {
                            Header = "CQ 码",
                            Margin = new Thickness(10),
                            Content = ChatDetailListItem_Common.BuildTextElement(item.ToSendString())
                        };
                        DetailContainer.Children.Add(expander);
                    }
                    cqCode_index++;
                }
                else
                {
                    DetailContainer.Children.Add(ChatDetailListItem_Common.BuildTextElement(p[i]));
                }
            }
        }

        private string UnionAllAtMsg(string message)
        {
            var ls = CQCode.Parse(message);
            foreach (var item in ls)
            {
                if (item.Function == Model.Enums.CQCodeType.At)
                {
                    if (item.Items.TryGetValue("qq", out string qq) && long.TryParse(qq, out long id))
                    {
                        string nick = AvatarType == ChatAvatar.AvatarTypes.QQGroup
                                ? ChatPage.Instance.GetGroupMemberNick(GroupId, id)
                                    : ChatPage.Instance.GetFriendNick(id);
                        message = message.Replace(item.ToSendString(), $" @{nick} ");
                    }
                }
            }
            return message;
        }

        private void ChatPage_WindowSizeChanged(SizeChangedEventArgs e)
        {
            MaxWidth = e.NewSize.Width * 0.6;
            ImageBorder.MaxWidth = MaxWidth;
        }

        public void Recall()
        {
            Dispatcher.BeginInvoke(() =>
            {
                RecallDisplay.Visibility = Visibility.Visible;
            });
        }

        public void UpdateSendStatus(bool sending)
        {
            SendStatus.Visibility = sending ? Visibility.Visible : Visibility.Collapsed;
            ResendClick.Visibility = Visibility.Collapsed;
        }

        public void SendFail()
        {
            SendStatus.Visibility = Visibility.Collapsed;
            ResendClick.Visibility = Visibility.Visible;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (ControlLoaded)
            {
                return;
            }
            ControlLoaded = true;
            ParseAndBuildDetail();
            ImageDisplay.MaxWidth = MaxWidth * 0.6;
            Avatar.Item = new ChatListItemViewModel
            {
                AvatarType = ChatAvatar.AvatarTypes.QQPrivate,
                GroupName = DisplayName,
                Id = Id
            };
            if (Recalled)
            {
                Recall();
            }
            ChatPage.WindowSizeChanged += ChatPage_WindowSizeChanged;
            ChatPage.MsgRecalled += ChatPage_MsgRecalled;
        }

        private void ChatPage_MsgRecalled(int id)
        {
            if (id == MsgId)
            {
                Recall();
            }
        }

        private void ResendClick_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            UpdateSendStatus(true);
            switch (AvatarType)
            {
                case ChatAvatar.AvatarTypes.QQGroup:
                    if (ChatPage.Instance.CallGroupMsgSend(GroupId, Message) > 0)
                    {
                        UpdateSendStatus(false);
                    }
                    else
                    {
                        SendFail();
                    }
                    break;

                case ChatAvatar.AvatarTypes.QQPrivate:
                    if (ChatPage.Instance.CallPrivateMsgSend(GroupId, Message) > 0)
                    {
                        UpdateSendStatus(false);
                    }
                    else
                    {
                        SendFail();
                    }
                    break;

                case ChatAvatar.AvatarTypes.Fallback:
                default:
                    UpdateSendStatus(false);
                    break;
            }
        }
    }
}