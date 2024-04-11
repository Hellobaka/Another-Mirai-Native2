using Another_Mirai_Native.DB;
using Another_Mirai_Native.Model;
using Another_Mirai_Native.UI.Pages;
using Another_Mirai_Native.UI.ViewModel;
using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

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

        public bool ControlLoaded { get; set; }

        public DetailItemType DetailItemType { get; set; }

        public string DisplayName { get; set; }

        public string GUID { get; set; }

        public long Id { get; set; }

        public string Message { get; set; } = "";

        public int MsgId { get; set; }

        public long ParentId { get; set; }

        public ChatAvatar.AvatarTypes ParentType { get; set; } = ChatAvatar.AvatarTypes.Fallback;

        public bool Recalled { get; set; }

        public DateTime Time { get; set; }

        private Paragraph CurrentParagraph => DetailContainer.Document.Blocks.LastBlock as Paragraph;

        public async void ParseAndBuildDetail()
        {
            Regex regex = new("(\\[CQ:.*?,.*?\\])");
            var cqCodeCaptures = regex.Matches(Message).Cast<Match>().Select(m => m.Value).ToList();

            var ls = CQCode.Parse(Message);
            int imageCount = ls.Count(x => x.IsImageCQCode);
            int recordCount = ls.Count(x => x.IsRecordCQCode);

            var s = regex.Split(Message).ToList();
            s.RemoveAll(string.IsNullOrEmpty);
            double minWidth = 0;
            foreach (var item in s)
            {
                if (cqCodeCaptures.Contains(item))
                {
                    var cqcode = CQCode.Parse(item).FirstOrDefault();
                    if (cqcode == null)
                    {
                        Expander expander = new()
                        {
                            Header = "CQ 码",
                            Margin = new Thickness(10),
                            Content = ChatDetailListItem_Common.BuildTextElement(item)
                        };
                        CurrentParagraph.Inlines.Add(new InlineUIContainer(expander));
                        minWidth = Math.Max(minWidth, expander.Width);
                        continue;
                    }
                    if (cqcode.Function == Model.Enums.CQCodeType.Image)
                    {
                        if (imageCount == 1 && s.Count == 1)
                        {
                            ImageBorder.Visibility = Visibility.Visible;
                            DetailBorder.Visibility = Visibility.Collapsed;
                            ImageDisplay.Children.Add(ChatDetailListItem_Common.BuildImageElement(cqcode, MaxWidth * 0.5));
                        }
                        else
                        {
                            DetailContainer.Document.Blocks.Add(new Paragraph());
                            CurrentParagraph.Inlines.Add(new InlineUIContainer(ChatDetailListItem_Common.BuildImageElement(cqcode, MaxWidth * 0.5)));
                            DetailContainer.Document.Blocks.Add(new Paragraph());
                        }
                        minWidth = Math.Max(minWidth, 150);
                    }
                    else if (cqcode.Function == Model.Enums.CQCodeType.At
                        && cqcode.Items.TryGetValue("qq", out string qq) && long.TryParse(qq, out long id))
                    {
                        string nick = ParentType == ChatAvatar.AvatarTypes.QQGroup
                                ? await ChatPage.Instance.GetGroupMemberNick(ParentId, id)
                                    : await ChatPage.Instance.GetFriendNick(id);
                        var hyperlink = new Hyperlink(new Run($" @{nick} "))
                        {
                            NavigateUri = new Uri("https://www.google.com"),
                            Tag = $" @{nick} "
                        };
                        hyperlink.Background = Brushes.SeaGreen;
                        hyperlink.Foreground = Brushes.White;
                        hyperlink.TextDecorations = null;
                        hyperlink.RequestNavigate += (_, e) =>
                        {
                            e.Handled = true;
                            ChatPage.Instance.AddTextToSendBox(cqcode.ToSendString());
                        };
                        CurrentParagraph.Inlines.Add(hyperlink);
                    }
                    else if (cqcode.Function == Model.Enums.CQCodeType.Face
                        && int.TryParse(cqcode.Items["id"], out int faceId))
                    {
                        Image? faceElement = ChatDetailListItem_Common.BuildFaceElement(faceId, true);
                        if (faceElement != null)
                        {
                            CurrentParagraph.Inlines.Add(faceElement);
                            minWidth += faceElement.Width;
                        }
                        else
                        {
                            Expander expander = new()
                            {
                                Header = "CQ 码",
                                Margin = new Thickness(10),
                                Content = ChatDetailListItem_Common.BuildTextElement(item)
                            };
                            CurrentParagraph.Inlines.Add(new InlineUIContainer(expander));
                            minWidth = Math.Max(minWidth, expander.Width);
                        }
                    }
                    else if (cqcode.Function == Model.Enums.CQCodeType.Reply
                         && int.TryParse(cqcode.Items["id"], out int replyId))
                    {
                        var messageItem = ChatHistoryHelper.GetHistoriesByMsgId(ParentId, replyId, ParentType == ChatAvatar.AvatarTypes.QQGroup ? ChatHistoryType.Group : ChatHistoryType.Private);
                        if (messageItem == null)
                        {
                            Expander expander = new()
                            {
                                Header = "CQ 码",
                                Margin = new Thickness(10),
                                Content = ChatDetailListItem_Common.BuildTextElement(item)
                            };
                            CurrentParagraph.Inlines.Add(new InlineUIContainer(expander));
                            minWidth = Math.Max(minWidth, expander.Width);
                        }
                        else
                        {
                            string nick = ParentType == ChatAvatar.AvatarTypes.QQGroup ?
                                await ChatPage.Instance.GetGroupMemberNick(ParentId, Id) :
                                await ChatPage.Instance.GetFriendNick(Id);
                            var reply = ChatDetailListItem_Common.BuildReplyElement(nick, messageItem.Message, () =>
                            {
                                ChatPage.Instance.JumpToReplyItem(messageItem.MsgId);
                            });
                            CurrentParagraph.Inlines.Add(reply);
                            reply.UpdateLayout();
                            minWidth = Math.Max(minWidth, reply.ActualWidth);
                            DetailContainer.Document.Blocks.Add(new Paragraph());
                        }
                    }
                    else
                    {
                        Expander expander = new()
                        {
                            Header = "CQ 码",
                            Margin = new Thickness(10),
                            Content = ChatDetailListItem_Common.BuildTextElement(cqcode.ToSendString())
                        };
                        CurrentParagraph.Inlines.Add(new InlineUIContainer(expander));
                        minWidth = Math.Max(minWidth, expander.Width);
                    }
                }
                else
                {
                    ChatDetailListItem_Common.AddTextToRichTextBox(CurrentParagraph, item);
                }
            }
            ChatDetailListItem_Common.SetElementNoSelectEffect(DetailContainer);
            DetailContainer.ContextMenu = ChatDetailListItem_Common.BuildDetailContextMenu();
            ChangeContainerWidth(minWidth);
            TimeDisplay.ToolTip = Time.ToString("G");
            NameDisplay.ToolTip = $"{DisplayName} [{Id}]";
            
            var lastParagraph = DetailContainer.Document.Blocks.Count > 0 && DetailContainer.Document.Blocks.LastBlock is Paragraph p
                && p.Inlines.Count == 0;
            // 文本垂直居中
            var text = new TextRange(DetailContainer.Document.ContentStart, DetailContainer.Document.ContentEnd);
            text.ApplyPropertyValue(Inline.BaselineAlignmentProperty, BaselineAlignment.Center);
            // 若末尾段落内容为空则移除
            if (lastParagraph)
            {
                DetailContainer.Document.Blocks.Remove(DetailContainer.Document.Blocks.LastBlock);
            }
        }

        private void ChangeContainerWidth(double minWidth)
        {
            double pixelsPerDip = VisualTreeHelper.GetDpi(DetailContainer).PixelsPerDip;
            double width = minWidth;
            foreach (Paragraph paragraph in DetailContainer.Document.Blocks.Cast<Paragraph>())
            {
                double currentWidth = 0;
                foreach (Inline inline in paragraph.Inlines)
                {
                    if (inline is InlineUIContainer ui && ui.Child is Expander expander)
                    {
                        currentWidth += expander.ActualWidth;
                    }
                    else if (inline is InlineUIContainer ui2 && ui2.Child is Image image)
                    {
                        currentWidth += image.Width;
                    }
                    else if (inline is Run || inline is Hyperlink)
                    {
                        string text = "";
                        if (inline is Run run)
                        {
                            text = run.Text;
                        }
                        else if (inline is Hyperlink link)
                        {
                            text = link.Tag.ToString();
                        }
                        var formattedText = new FormattedText(
                            text,
                            CultureInfo.CurrentCulture,
                            FlowDirection.LeftToRight,
                            new Typeface(DetailContainer.FontFamily, DetailContainer.FontStyle, DetailContainer.FontWeight, DetailContainer.FontStretch),
                            DetailContainer.FontSize,
                            Brushes.Black,
                            new NumberSubstitution(),
                            TextFormattingMode.Display, pixelsPerDip);
                        currentWidth += formattedText.Width;
                    }
                }
                width = Math.Max(currentWidth, width);
            }

            DetailContainer.Width = width + 5 + DetailContainer.Padding.Left + DetailContainer.Padding.Right;
        }

        public void Dispose()
        {
            ChatPage.WindowSizeChanged -= ChatPage_WindowSizeChanged;
            ChatPage.MsgRecalled -= ChatPage_MsgRecalled;
        }

        public void Recall()
        {
            Dispatcher.BeginInvoke(() =>
            {
                RecallDisplay.Visibility = Visibility.Visible;
            });
        }

        public void SendFail()
        {
            SendStatus.Visibility = Visibility.Collapsed;
            ResendClick.Visibility = Visibility.Visible;
        }

        public void UpdateSendStatus(bool sending)
        {
            SendStatus.Visibility = sending ? Visibility.Visible : Visibility.Collapsed;
            ResendClick.Visibility = Visibility.Collapsed;
        }

        public void ContextMenu_Repeat(object sender, EventArgs e)
        {
            Task.Run(() => ChatPage.Instance.ExecuteSendMessage(ParentId, ParentType, Message));
        }

        public void ContextMenu_Recall(object sender, EventArgs e)
        {
            if (MsgId > 0)
            {
                ProtocolManager.Instance.CurrentProtocol.DeleteMsg(MsgId);
            }
        }

        public void ContextMenu_At(object sender, EventArgs e)
        {
            ChatPage.Instance.AddTextToSendBox($"[CQ:at,qq={Id}]");
        }

        public void ContextMenu_Reply(object sender, EventArgs e)
        {
            ChatPage.Instance.AddTextToSendBox($"[CQ:reply,id={MsgId}]");
        }

        public void ContextMenu_CopyMessage(object sender, EventArgs e)
        {
            Clipboard.SetText(Message);
        }

        public void ContextMenu_CopyId(object sender, EventArgs e)
        {
            Clipboard.SetText(Id.ToString());
        }

        public void ContextMenu_CopyNick(object sender, EventArgs e)
        {
            Clipboard.SetText(DisplayName);
        }

        private void ChatPage_MsgRecalled(int id)
        {
            if (id == MsgId)
            {
                Recall();
            }
        }

        private void ChatPage_WindowSizeChanged(SizeChangedEventArgs e)
        {
            MaxWidth = e.NewSize.Width * 0.6;
            ImageBorder.MaxWidth = MaxWidth;
            DetailContainer.MaxWidth = MaxWidth * 0.8;
        }

        private void ResendClick_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            UpdateSendStatus(true);
            switch (ParentType)
            {
                case ChatAvatar.AvatarTypes.QQGroup:
                    if (ChatPage.Instance.CallGroupMsgSend(ParentId, Message) > 0)
                    {
                        UpdateSendStatus(false);
                    }
                    else
                    {
                        SendFail();
                    }
                    break;

                case ChatAvatar.AvatarTypes.QQPrivate:
                    if (ChatPage.Instance.CallPrivateMsgSend(ParentId, Message) > 0)
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

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (ControlLoaded)
            {
                return;
            }
            ControlLoaded = true;
            ParseAndBuildDetail();
            ImageDisplay.MaxWidth = MaxWidth * 0.6;
            DetailContainer.MaxWidth = MaxWidth * 0.8;
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

            DetailBorder.ContextMenu = ChatDetailListItem_Common.BuildDetailContextMenu();
            ImageBorder.ContextMenu = DetailBorder.ContextMenu;
            Avatar.ContextMenu = ChatDetailListItem_Common.BuildAvatarContextMenu();
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            Dispose();
        }
    }
}