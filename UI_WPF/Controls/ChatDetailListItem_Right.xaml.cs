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

        public void ParseAndBuildDetail()
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
                                ? ChatPage.Instance.GetGroupMemberNick(ParentId, id)
                                    : ChatPage.Instance.GetFriendNick(id);
                        var hyperlink = new Hyperlink(new Run($"@{nick}"))
                        {
                            NavigateUri = new Uri("https://www.google.com"),
                        };
                        hyperlink.SetResourceReference(Hyperlink.ForegroundProperty, "TextFillColorPrimaryBrush");

                        hyperlink.RequestNavigate += (_, e) =>
                        {
                            e.Handled = true;
                            ChatPage.Instance.AddTextToSendBox(cqcode.ToSendString());
                        };
                        CurrentParagraph.Inlines.Add(hyperlink);
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
                    ChatDetailListItem_Common.AddTextToRichTextBox(DetailContainer, item);
                }
            }
            ChatDetailListItem_Common.SetElementNoSelectEffect(DetailContainer);
            DetailContainer.ContextMenu = ChatDetailListItem_Common.BuildDetailContextMenu();
            ChangeContainerWidth(minWidth);
            TimeDisplay.ToolTip = Time.ToString("G");
            NameDisplay.ToolTip = $"{DisplayName} [{Id}]";
        }

        private void ChangeContainerWidth(double minWidth)
        {
            TextRange documentRange = new TextRange(DetailContainer.Document.ContentStart, DetailContainer.Document.ContentEnd);
            string text = documentRange.Text;
            double pixelsPerDip = VisualTreeHelper.GetDpi(DetailContainer).PixelsPerDip;

            var formattedText = new FormattedText(
                    text,
                    CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    new Typeface(DetailContainer.FontFamily, DetailContainer.FontStyle, DetailContainer.FontWeight, DetailContainer.FontStretch),
                    DetailContainer.FontSize,
                    Brushes.Black,
                    new NumberSubstitution(),
                    TextFormattingMode.Display, pixelsPerDip);

            DetailContainer.Width = formattedText.Width + 30 + minWidth;
            if (minWidth > 0)
            {
                DetailContainer.Padding = new Thickness(10, 10, 10, 0);
            }
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
            DetailContainer.MaxWidth = ImageDisplay.MaxWidth;
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
            DetailContainer.MaxWidth = ImageDisplay.MaxWidth;
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