using Another_Mirai_Native.DB;
using Another_Mirai_Native.Model;
using Another_Mirai_Native.Model.Enums;
using Another_Mirai_Native.UI.Models;
using Another_Mirai_Native.UI.Pages;
using Another_Mirai_Native.UI.ViewModel;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace Another_Mirai_Native.UI.Controls.Chat
{
    /// <summary>
    /// MessageItem.xaml 的交互逻辑
    /// </summary>
    public partial class MessageItem : UserControl
    {
        private static readonly Regex SegmentRegex = new("(\\[CQ:.*?,.*?\\])", RegexOptions.Compiled);

        public MessageItem()
        {
            InitializeComponent();
        }

        #region 属性定义

        public bool ControlLoaded { get; set; }

        public MessageViewModel ViewModel => (MessageViewModel)DataContext;

        private Brush DefaultTextBrush => (Brush)FindResource("SystemControlForegroundBaseHighBrush");

        #endregion 属性定义

        #region 公开方法

        /// <summary>
        /// 转换消息为可显示内容（解析 CQ 码）
        /// </summary>
        public async void ParseAndBuildDetail()
        {
            MessageContent.Inlines.Clear();
            DetailBorder.Visibility = Visibility.Visible;
            ImageBorder.Visibility = Visibility.Collapsed;
            ImageDisplay.Children.Clear();
            ViewModel.Content = ViewModel.Content.Trim();
            if (ViewModel.DetailItemType == DetailItemType.Notice)
            {
                AddTextSegment(ViewModel.Content);
                return;
            }

            var cqCodeCaptures = SegmentRegex.Matches(ViewModel.Content).Cast<Match>().Select(m => m.Value).ToList();
            var parsedCodes = CQCode.Parse(ViewModel.Content);
            int imageCount = parsedCodes.Count(x => x.IsImageCQCode);

            var segments = SegmentRegex.Split(ViewModel.Content).Where(s => string.IsNullOrEmpty(s) is false).ToList();
            foreach (var item in segments)
            {
                if (cqCodeCaptures.Contains(item))
                {
                    await ProcessCQCode(item, imageCount, segments.Count);
                }
                else
                {
                    AddTextSegment(item);
                }
            }

            NameDisplay.ToolTip = $"{ViewModel.Nick} [{ViewModel.Id}]";
        }

        /// <summary>
        /// 清理资源
        /// </summary>
        public void Dispose()
        {
            ChatPage.WindowSizeChanged -= ChatPage_WindowSizeChanged;
        }

        #endregion 公开方法

        #region 私有方法

        private async Task ProcessCQCode(string item, int imageCount, int splitCount)
        {
            var cqcode = CQCode.Parse(item).FirstOrDefault();
            if (cqcode == null)
            {
                AddExpanderForCQCode(item);
                return;
            }

            switch (cqcode.Function)
            {
                case CQCodeType.Image:
                    ProcessImageCQCode(cqcode, imageCount, splitCount);
                    break;

                case CQCodeType.At:
                    await ProcessAtCQCode(cqcode);
                    break;

                case CQCodeType.Face:
                    ProcessFaceCQCode(cqcode, item);
                    break;

                case CQCodeType.Reply:
                    await ProcessReplyCQCode(cqcode, item);
                    break;

                default:
                    AddExpanderForCQCode(cqcode.ToSendString());
                    break;
            }
        }

        private void ProcessImageCQCode(CQCode cqcode, int imageCount, int splitCount)
        {
            if (imageCount == 1 && splitCount == 1)
            {
                ImageBorder.Visibility = Visibility.Visible;
                DetailBorder.Visibility = Visibility.Collapsed;
                ImageDisplay.Children.Add(new ChatImageDisplay
                {
                    CQCode = cqcode,
                    MaxImageWidth = MaxWidth * 0.5,
                    MaxImageHeight = MessageItem_Common.ImageMaxHeight
                });
                return;
            }

            var imageElement = new ChatImageDisplay
            {
                CQCode = cqcode,
                MaxImageWidth = MaxWidth * 0.5,
                MaxImageHeight = MessageItem_Common.ImageMaxHeight
            };
            if (MessageContent.Inlines.Count > 0 && !(MessageContent.Inlines.LastInline is LineBreak))
            {
                MessageContent.Inlines.Add(new LineBreak());
            }
            MessageContent.Inlines.Add(new InlineUIContainer(imageElement) { BaselineAlignment = BaselineAlignment.Bottom });
            MessageContent.Inlines.Add(new LineBreak());
        }

        private async Task ProcessAtCQCode(CQCode cqcode)
        {
            if (!cqcode.Items.TryGetValue("qq", out string? qq) || !long.TryParse(qq, out long id))
            {
                AddExpanderForCQCode(cqcode.ToSendString());
                return;
            }

            string nick = ViewModel.AvatarType == ChatType.QQGroup
                ? await ChatHistoryHelper.GetGroupMemberNick(ViewModel.ParentId, id)
                : await ChatHistoryHelper.GetFriendNick(id);

            var element = CreateHyperlinkElement(nick, cqcode);
            MessageContent.Inlines.Add(new InlineUIContainer(element) { BaselineAlignment = BaselineAlignment.Center });
        }

        private void ProcessFaceCQCode(CQCode cqcode, string item)
        {
            if (!int.TryParse(cqcode.Items["id"], out int faceId))
            {
                AddExpanderForCQCode(item);
                return;
            }

            var faceDisplay = new ChatFaceDisplay { FaceId = faceId };
            if (faceDisplay.HasFace)
            {
                faceDisplay.Margin = new Thickness(1, 0, 1, 0);
                MessageContent.Inlines.Add(new InlineUIContainer(faceDisplay) { BaselineAlignment = BaselineAlignment.Center });
            }
            else
            {
                AddExpanderForCQCode(item);
            }
        }

        private async Task ProcessReplyCQCode(CQCode cqcode, string item)
        {
            if (!int.TryParse(cqcode.Items["id"], out int replyId))
            {
                AddExpanderForCQCode(item);
                return;
            }

            var messageItem = ChatHistoryHelper.GetHistoriesByMsgId(ViewModel.ParentId, replyId,
                ViewModel.AvatarType == ChatType.QQGroup ? ChatHistoryType.Group : ChatHistoryType.Private);

            if (messageItem == null)
            {
                AddExpanderForCQCode(item);
                return;
            }

            string nick = ViewModel.AvatarType == ChatType.QQGroup
                ? await ChatHistoryHelper.GetGroupMemberNick(ViewModel.ParentId, messageItem.SenderID)
                : await ChatHistoryHelper.GetFriendNick(messageItem.SenderID);

            var reply = new ChatReplyDisplay
            {
                Nick = nick,
                Msg = messageItem.Message,
                JumpAction = () =>
                {
                    ChatViewModel.Instance.JumpToMessage(messageItem.MsgId);
                },
                Margin = new Thickness(0, 0, 0, 5)
            };

            if (MessageContent.Inlines.Count > 0 && !(MessageContent.Inlines.LastInline is LineBreak))
            {
                MessageContent.Inlines.Add(new LineBreak());
            }
            MessageContent.Inlines.Add(new InlineUIContainer(reply) { BaselineAlignment = BaselineAlignment.Bottom });
            MessageContent.Inlines.Add(new LineBreak());
        }

        private void AddTextSegment(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return;
            }

            string[] lines = text.Replace("\r", string.Empty).Split('\n');
            for (int i = 0; i < lines.Length; i++)
            {
                if (i > 0)
                {
                    MessageContent.Inlines.Add(new LineBreak());
                }

                string line = lines[i];
                if (line.Length == 0)
                {
                    // line = " "; // 空行不需要特殊处理，LineBreak 已经足够
                }
                else
                {
                    var run = new Run(line) { Foreground = DefaultTextBrush };
                    MessageContent.Inlines.Add(run);
                }
            }
        }

        private UIElement CreateHyperlinkElement(string nick, CQCode cqcode)
        {
            var text = ViewModel.DetailItemType == DetailItemType.Send ? $" @{nick} " : $"@{nick} ";
            var border = new Border
            {
                CornerRadius = new CornerRadius(4),
                Padding = new Thickness(4, 2, 4, 2),
                Cursor = Cursors.Hand,
                Margin = new Thickness(1, 0, 1, 0)
            };

            var textBlock = new TextBlock
            {
                Text = text,
                TextWrapping = TextWrapping.NoWrap,
                Foreground = ViewModel.DetailItemType == DetailItemType.Send
                    ? Brushes.White
                    : new SolidColorBrush(Color.FromRgb(199, 37, 38))
            };

            border.Background = ViewModel.DetailItemType == DetailItemType.Send
                ? Brushes.SeaGreen
                : new SolidColorBrush(Color.FromRgb(249, 242, 244));

            border.Child = textBlock;
            border.MouseLeftButtonDown += (_, e) =>
            {
                e.Handled = true;
                ChatViewModel.Instance.AddTextToSendBox(cqcode.ToSendString());
            };
            return border;
        }

        private void AddExpanderForCQCode(string cqCodeString)
        {
            Expander expander = new()
            {
                Header = "CQ 码",
                Margin = new Thickness(10, 0, 10, 0),
                Content = new SelectableTextBlock() { Text = cqCodeString, Margin = new Thickness(10) }
            };
            MessageContent.Inlines.Add(new InlineUIContainer(expander) { BaselineAlignment = BaselineAlignment.Center });
        }

        private void ChatPage_WindowSizeChanged(SizeChangedEventArgs e)
        {
            MaxWidth = e.NewSize.Width * 0.6;
            ImageBorder.MaxWidth = MaxWidth;
            MessageContent.MaxWidth = MaxWidth * 0.8;
        }

        #endregion 私有方法

        #region 事件处理

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (ControlLoaded)
            {
                return;
            }
            ControlLoaded = true;

            AvatarRight.Visibility = ViewModel.DetailItemType == DetailItemType.Send ? Visibility.Visible : Visibility.Collapsed;
            AvatarLeft.Visibility = ViewModel.DetailItemType == DetailItemType.Receive ? Visibility.Visible : Visibility.Collapsed;
            NameDisplay.Visibility = ViewModel.DetailItemType == DetailItemType.Notice ? Visibility.Collapsed : Visibility.Visible;
            TimeDisplay.Visibility = ViewModel.DetailItemType == DetailItemType.Notice ? Visibility.Collapsed : Visibility.Visible;
            var avatarItem = new ChatListItemViewModel
            {
                AvatarType = ChatType.QQPrivate,
                GroupName = ViewModel.Nick,
                Id = ViewModel.Id
            };
            AvatarRight.Item = avatarItem;
            AvatarLeft.Item = avatarItem;

            ParseAndBuildDetail();
            ImageDisplay.MaxWidth = MaxWidth * 0.6;
            MessageContent.MaxWidth = MaxWidth * 0.8;

            ChatPage.WindowSizeChanged += ChatPage_WindowSizeChanged;
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            Dispose();
        }

        private async void ResendClick_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ViewModel.MessageStatus = MessageStatus.Sending;
            Task<int>? sendTask = null;
            switch (ViewModel.AvatarType)
            {
                case ChatType.QQGroup:
                    sendTask = ChatViewModel.Instance.CallGroupMsgSendAsync(ViewModel.ParentId, ViewModel.Content);
                    break;

                case ChatType.QQPrivate:
                    sendTask = ChatViewModel.Instance.CallPrivateMsgSendAsync(ViewModel.ParentId, ViewModel.Content);
                    break;

                case ChatType.Fallback:
                default:
                    break;
            }
            if (sendTask != null)
            {
                ViewModel.MsgId = await sendTask;
                ViewModel.MessageStatus = ViewModel.MsgId != 0 ? MessageStatus.Sent : MessageStatus.SendFailed;
                ChatHistoryHelper.UpdateHistoryMessageId(ViewModel.ParentId, ViewModel.SqlId, ViewModel.MsgId);
            }
            else
            {
                ViewModel.MessageStatus = MessageStatus.SendFailed;
            }
        }

        #endregion 事件处理
    }
}