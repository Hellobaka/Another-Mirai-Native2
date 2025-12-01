using Another_Mirai_Native.DB;
using Another_Mirai_Native.Model;
using Another_Mirai_Native.Model.Enums;
using Another_Mirai_Native.UI.Models;
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

namespace Another_Mirai_Native.UI.Controls.Chat
{
    /// <summary>
    /// MessageItem.xaml 的交互逻辑
    /// 统一处理 Left(Receive)、Right(Send)、Center(Notice) 三种消息类型
    /// </summary>
    public partial class MessageItem : UserControl
    {
        public MessageItem()
        {
            InitializeComponent();
        }

        #region 属性定义

        public bool ControlLoaded { get; set; }

        public MessageViewModel ViewModel => (MessageViewModel)DataContext;

        /// <summary>
        /// 最后一个段落（用于添加内容）
        /// </summary>
        private Paragraph CurrentParagraph => DetailContainer.Document.Blocks.LastBlock as Paragraph;

        /// <summary>
        /// 回复元素引用
        /// </summary>
        private Border ReplyElement { get; set; }

        #endregion

        #region 公开方法

        /// <summary>
        /// 转换消息为可显示内容（解析 CQ 码）
        /// </summary>
        public async void ParseAndBuildDetail()
        {
            if (CurrentParagraph == null)
            {
                return;
            }

            // 系统消息（Notice）不解析 CQ 码，直接显示
            if (ViewModel.DetailItemType == DetailItemType.Notice)
            {
                MessageItem_Common.AddTextToRichTextBox(CurrentParagraph, ViewModel.Content);
                MessageItem_Common.SetElementNoSelectEffect(DetailContainer);
                TimeDisplay.ToolTip = ViewModel.Time.ToString("G");
                return;
            }

            // 拆分 CQ 码
            Regex regex = new("(\\[CQ:.*?,.*?\\])");
            var cqCodeCaptures = regex.Matches(ViewModel.Content).Cast<Match>().Select(m => m.Value).ToList();

            var ls = CQCode.Parse(ViewModel.Content);
            int imageCount = ls.Count(x => x.IsImageCQCode);

            var s = regex.Split(ViewModel.Content).ToList();
            s.RemoveAll(string.IsNullOrEmpty);
            double minWidth = 0;

            // 处理消息内容
            foreach (var item in s)
            {
                if (cqCodeCaptures.Contains(item))
                {
                    minWidth = await ProcessCQCode(item, imageCount, s.Count, minWidth);
                }
                else
                {
                    // 纯文本消息
                    MessageItem_Common.AddTextToRichTextBox(CurrentParagraph, item);
                }
            }

            // 删除点击效果
            MessageItem_Common.SetElementNoSelectEffect(DetailContainer);

            // 计算容器宽度
            ChangeContainerWidth(minWidth);

            // 设置工具提示
            TimeDisplay.ToolTip = ViewModel.Time.ToString("G");
            NameDisplay.ToolTip = $"{ViewModel.Nick} [{ViewModel.Id}]";

            // 移除末尾空段落
            var lastParagraph = DetailContainer.Document.Blocks.Count > 0 && DetailContainer.Document.Blocks.LastBlock is Paragraph p
                && p.Inlines.Count == 0;

            // 文本垂直居中
            var text = new TextRange(DetailContainer.Document.ContentStart, DetailContainer.Document.ContentEnd);
            text.ApplyPropertyValue(Inline.BaselineAlignmentProperty, BaselineAlignment.Center);

            if (lastParagraph)
            {
                DetailContainer.Document.Blocks.Remove(DetailContainer.Document.Blocks.LastBlock);
            }
        }

        /// <summary>
        /// 显示消息已撤回
        /// </summary>
        public void Recall()
        {
            Dispatcher.BeginInvoke(() =>
            {
                RecallDisplay.Visibility = Visibility.Visible;
            });
        }

        /// <summary>
        /// 显示发送失败（仅 Send 类型）
        /// </summary>
        public void SendFail()
        {
            SendStatus.Visibility = Visibility.Collapsed;
            ResendClick.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// 更新消息ID（仅 Send 类型）
        /// </summary>
        public void UpdateMessageId(int msgId)
        {
            ViewModel.MsgId = msgId;
        }

        /// <summary>
        /// 更新发送状态（仅 Send 类型）
        /// </summary>
        public void UpdateSendStatus(bool sending)
        {
            SendStatus.Visibility = sending ? Visibility.Visible : Visibility.Collapsed;
            ResendClick.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// 清理资源
        /// </summary>
        public void Dispose()
        {
            ChatPage.WindowSizeChanged -= ChatPage_WindowSizeChanged;
            ChatPage.MsgRecalled -= ChatPage_MsgRecalled;
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 处理单个 CQ 码
        /// </summary>
        private async Task<double> ProcessCQCode(string item, int imageCount, int splitCount, double minWidth)
        {
            var cqcode = CQCode.Parse(item).FirstOrDefault();
            if (cqcode == null)
            {
                return AddExpanderForCQCode(item, minWidth);
            }

            minWidth = cqcode.Function switch
            {
                CQCodeType.Image => ProcessImageCQCode(cqcode, imageCount, splitCount, minWidth),
                CQCodeType.At => await ProcessAtCQCode(cqcode, minWidth),
                CQCodeType.Face => ProcessFaceCQCode(cqcode, item, minWidth),
                CQCodeType.Reply => await ProcessReplyCQCode(cqcode, item, minWidth),
                _ => AddExpanderForCQCode(cqcode.ToSendString(), minWidth),
            };
            return minWidth;
        }

        /// <summary>
        /// 处理图片 CQ 码
        /// </summary>
        private double ProcessImageCQCode(CQCode cqcode, int imageCount, int splitCount, double minWidth)
        {
            if (imageCount == 1 && splitCount == 1)
            {
                // 仅图片消息
                ImageBorder.Visibility = Visibility.Visible;
                DetailBorder.Visibility = Visibility.Collapsed;
                ImageDisplay.Children.Add(MessageItem_Common.BuildImageElement(cqcode, MaxWidth * 0.5));
            }
            else
            {
                // 混合消息
                DetailContainer.Document.Blocks.Add(new Paragraph());
                CurrentParagraph.Inlines.Add(new InlineUIContainer(MessageItem_Common.BuildImageElement(cqcode, MaxWidth * 0.5)));
                DetailContainer.Document.Blocks.Add(new Paragraph());
            }
            return Math.Max(minWidth, 150);
        }

        /// <summary>
        /// 处理 At CQ 码
        /// </summary>
        private async Task<double> ProcessAtCQCode(CQCode cqcode, double minWidth)
        {
            if (!cqcode.Items.TryGetValue("qq", out string? qq) || !long.TryParse(qq, out long id))
            {
                return AddExpanderForCQCode(cqcode.ToSendString(), minWidth);
            }

            string nick = ViewModel.AvatarType == AvatarTypes.QQGroup
                    ? await ChatPage.Instance.GetGroupMemberNick(ViewModel.ParentId, id)
                    : await ChatPage.Instance.GetFriendNick(id);

            var hyperlink = BuildAtHyperlink(nick, cqcode);
            CurrentParagraph.Inlines.Add(hyperlink);
            return minWidth;
        }

        /// <summary>
        /// 构建 At 超链接（根据消息类型调整样式）
        /// </summary>
        private Hyperlink BuildAtHyperlink(string nick, CQCode cqcode)
        {
            string text = ViewModel.DetailItemType == DetailItemType.Send ? $" @{nick} " : $"@{nick} ";
            var hyperlink = new Hyperlink(new Run(text))
            {
                NavigateUri = new Uri("https://www.google.com"),
                Tag = text
            };

            // 根据消息类型选择样式
            if (ViewModel.DetailItemType == DetailItemType.Send)
            {
                // Right 样式：海洋绿
                hyperlink.Background = Brushes.SeaGreen;
                hyperlink.Foreground = Brushes.White;
            }
            else
            {
                // Left 样式：粉红色
                hyperlink.Background = new SolidColorBrush(Color.FromRgb(249, 242, 244));
                hyperlink.Foreground = new SolidColorBrush(Color.FromRgb(199, 37, 38));
            }

            hyperlink.TextDecorations = null;

            hyperlink.RequestNavigate += (_, e) =>
            {
                e.Handled = true;
                ChatViewModel.Instance.AddTextToSendBox(cqcode.ToSendString());
            };

            return hyperlink;
        }

        /// <summary>
        /// 处理表情 CQ 码
        /// </summary>
        private double ProcessFaceCQCode(CQCode cqcode, string item, double minWidth)
        {
            if (!int.TryParse(cqcode.Items["id"], out int faceId))
            {
                return AddExpanderForCQCode(item, minWidth);
            }

            Image? faceElement = MessageItem_Common.BuildFaceElement(faceId, true);
            if (faceElement != null)
            {
                CurrentParagraph.Inlines.Add(faceElement);
                return minWidth + faceElement.Width;
            }
            else
            {
                return AddExpanderForCQCode(item, minWidth);
            }
        }

        /// <summary>
        /// 处理回复 CQ 码
        /// </summary>
        private async Task<double> ProcessReplyCQCode(CQCode cqcode, string item, double minWidth)
        {
            if (!int.TryParse(cqcode.Items["id"], out int replyId))
            {
                return AddExpanderForCQCode(item, minWidth);
            }

            var messageItem = ChatHistoryHelper.GetHistoriesByMsgId(ViewModel.ParentId, replyId,
                ViewModel.AvatarType == AvatarTypes.QQGroup ? ChatHistoryType.Group : ChatHistoryType.Private);

            if (messageItem == null)
            {
                return AddExpanderForCQCode(item, minWidth);
            }

            string nick = ViewModel.AvatarType == AvatarTypes.QQGroup ?
                await ChatPage.Instance.GetGroupMemberNick(ViewModel.ParentId, messageItem.SenderID) :
                await ChatPage.Instance.GetFriendNick(messageItem.SenderID);

            var reply = MessageItem_Common.BuildReplyElement(nick, ViewModel.Content, () =>
            {
                ChatPage.Instance.JumpToReplyItem(messageItem.MsgId);
            });

            CurrentParagraph.Inlines.Add(reply);
            ReplyElement = reply;
            reply.UpdateLayout();
            minWidth = Math.Max(minWidth, reply.ActualWidth);
            DetailContainer.Document.Blocks.Add(new Paragraph());
            return minWidth;
        }

        /// <summary>
        /// 添加无法识别的 CQ 码展示器
        /// </summary>
        private double AddExpanderForCQCode(string cqCodeString, double minWidth)
        {
            Expander expander = new()
            {
                Header = "CQ 码",
                Margin = new Thickness(10),
                Content = MessageItem_Common.BuildTextElement(cqCodeString)
            };
            CurrentParagraph.Inlines.Add(new InlineUIContainer(expander));
            return Math.Max(minWidth, expander.Width);
        }

        /// <summary>
        /// 根据内容计算消息容器的宽度
        /// </summary>
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
                            text = link.Tag?.ToString() ?? "";
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

            DetailContainer.Width = width + 10 + DetailContainer.Padding.Left + DetailContainer.Padding.Right;
            if (ReplyElement != null)
            {
                ReplyElement.Width = width;
            }
        }

        /// <summary>
        /// 处理消息撤回事件
        /// </summary>
        private void ChatPage_MsgRecalled(int msgId)
        {
            if (msgId == ViewModel.MsgId)
            {
                Recall();
            }
        }

        /// <summary>
        /// 处理窗体尺寸变化
        /// </summary>
        private void ChatPage_WindowSizeChanged(SizeChangedEventArgs e)
        {
            MaxWidth = e.NewSize.Width * 0.6;
            ImageBorder.MaxWidth = MaxWidth;
            DetailContainer.MaxWidth = MaxWidth * 0.8;
        }

        #endregion

        #region 事件处理

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (ControlLoaded)
            {
                return;
            }
            ControlLoaded = true;

            // 根据消息类型显示对应的头像
            if (ViewModel.DetailItemType == DetailItemType.Send)
            {
                AvatarRight.Visibility = Visibility.Visible;
                AvatarRight.Item = new ChatListItemViewModel
                {
                    AvatarType = AvatarTypes.QQPrivate,
                    GroupName = ViewModel.Nick,
                };
            }
            else if (ViewModel.DetailItemType == DetailItemType.Receive)
            {
                AvatarLeft.Visibility = Visibility.Visible;
                AvatarLeft.Item = new ChatListItemViewModel
                {
                    AvatarType = AvatarTypes.QQPrivate,
                    GroupName = ViewModel.Nick,
                };
            }

            // 系统消息不显示头像
            // DetailItemType == DetailItemType.Notice 时不设置头像

            ParseAndBuildDetail();
            ImageDisplay.MaxWidth = MaxWidth * 0.6;
            DetailContainer.MaxWidth = MaxWidth * 0.8;

            if (ViewModel.Recalled)
            {
                Recall();
            }

            // 订阅事件
            ChatPage.WindowSizeChanged += ChatPage_WindowSizeChanged;
            ChatPage.MsgRecalled += ChatPage_MsgRecalled;
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            Dispose();
        }

        /// <summary>
        /// 重新发送按钮点击事件（仅 Send 类型）
        /// </summary>
        private void ResendClick_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            UpdateSendStatus(true);
            switch (ViewModel.AvatarType)
            {
                case AvatarTypes.QQGroup:
                    if (ChatPage.Instance.CallGroupMsgSend(ViewModel.ParentId, ViewModel.Content) > 0)
                    {
                        UpdateSendStatus(false);
                    }
                    else
                    {
                        SendFail();
                    }
                    break;

                case AvatarTypes.QQPrivate:
                    if (ChatPage.Instance.CallPrivateMsgSend(ViewModel.ParentId, ViewModel.Content) > 0)
                    {
                        UpdateSendStatus(false);
                    }
                    else
                    {
                        SendFail();
                    }
                    break;

                case AvatarTypes.Fallback:
                default:
                    UpdateSendStatus(false);
                    break;
            }
        }

        #endregion
    }
}
