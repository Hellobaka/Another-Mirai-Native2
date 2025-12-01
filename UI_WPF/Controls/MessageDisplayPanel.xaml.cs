using Another_Mirai_Native.UI.ViewModel;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Another_Mirai_Native.UI.Controls
{
    /// <summary>
    /// MessageDisplayPanel.xaml 的交互逻辑
    /// 消息显示区域控件，包含消息容器、滚动视图和滚动到底部按钮
    /// </summary>
    public partial class MessageDisplayPanel : UserControl
    {
        private bool _isAtBottom = true;
        private Func<ChatDetailItemViewModel, UIElement>? _messageBuilder;

        public MessageDisplayPanel()
        {
            InitializeComponent();
        }

        #region 依赖属性

        /// <summary>
        /// 消息容器的最大消息数量
        /// </summary>
        public int MaxMessageCount
        {
            get => (int)GetValue(MaxMessageCountProperty);
            set => SetValue(MaxMessageCountProperty, value);
        }

        public static readonly DependencyProperty MaxMessageCountProperty =
            DependencyProperty.Register(nameof(MaxMessageCount), typeof(int), typeof(MessageDisplayPanel), new PropertyMetadata(100));

        #endregion

        #region 事件

        /// <summary>
        /// 滚动到底部按钮点击事件
        /// </summary>
        public event EventHandler? ScrollToBottomClicked;

        /// <summary>
        /// 滚动到顶部触发懒加载事件
        /// </summary>
        public event EventHandler? LazyLoadTriggered;

        /// <summary>
        /// 滚动事件
        /// </summary>
        public event EventHandler<ScrollChangedEventArgs>? Scrolled;

        #endregion

        #region 事件处理

        private void MessageScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            // 检查是否在底部
            _isAtBottom = MessageScrollViewer.VerticalOffset >= MessageScrollViewer.ScrollableHeight - 10;
            
            // 更新滚动到底部按钮的可见性
            UpdateScrollToBottomVisibility();
            
            // 检查是否在顶部触发懒加载
            if (MessageScrollViewer.VerticalOffset < 10 && MessageContainer.Children.Count > 0)
            {
                LazyLoadTriggered?.Invoke(this, EventArgs.Empty);
            }
            
            Scrolled?.Invoke(this, e);
        }

        private void ScrollToBottomBtn_Click(object sender, RoutedEventArgs e)
        {
            ScrollToBottom(true);
            ScrollToBottomClicked?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region 公共方法

        /// <summary>
        /// 设置消息构建器函数
        /// </summary>
        /// <param name="builder">将ChatDetailItemViewModel转换为UIElement的函数</param>
        public void SetMessageBuilder(Func<ChatDetailItemViewModel, UIElement> builder)
        {
            _messageBuilder = builder;
        }

        /// <summary>
        /// 添加消息
        /// </summary>
        /// <param name="item">消息视图模型</param>
        /// <param name="autoClean">是否自动清理旧消息</param>
        public void AddMessage(ChatDetailItemViewModel item, bool autoClean = true)
        {
            if (_messageBuilder == null)
            {
                throw new InvalidOperationException("Message builder is not set. Call SetMessageBuilder first.");
            }

            var element = _messageBuilder(item);
            MessageContainer.Children.Add(element);

            if (autoClean)
            {
                RemoveOldMessages(MaxMessageCount);
            }
        }

        /// <summary>
        /// 在开头插入消息（用于懒加载历史消息）
        /// </summary>
        /// <param name="items">消息列表</param>
        public void InsertMessagesAtBeginning(IEnumerable<ChatDetailItemViewModel> items)
        {
            if (_messageBuilder == null)
            {
                throw new InvalidOperationException("Message builder is not set. Call SetMessageBuilder first.");
            }

            int insertIndex = 0;
            foreach (var item in items)
            {
                var element = _messageBuilder(item);
                MessageContainer.Children.Insert(insertIndex++, element);
            }
        }

        /// <summary>
        /// 清空所有消息
        /// </summary>
        public void ClearMessages()
        {
            MessageContainer.Children.Clear();
        }

        /// <summary>
        /// 滚动到底部
        /// </summary>
        /// <param name="forced">是否强制滚动</param>
        public void ScrollToBottom(bool forced = false)
        {
            if (forced || _isAtBottom)
            {
                Dispatcher.BeginInvoke(() =>
                {
                    MessageScrollViewer.ScrollToEnd();
                }, DispatcherPriority.Loaded);
            }
        }

        /// <summary>
        /// 滚动到指定消息
        /// </summary>
        /// <param name="msgId">消息ID</param>
        public void ScrollToMessage(int msgId)
        {
            foreach (UIElement child in MessageContainer.Children)
            {
                if (child is ChatDetailListItem chatItem && chatItem.MsgId == msgId)
                {
                    chatItem.BringIntoView();
                    break;
                }
            }
        }

        /// <summary>
        /// 检查消息是否已存在
        /// </summary>
        /// <param name="guid">消息GUID</param>
        /// <returns>是否存在</returns>
        public bool HasMessage(string guid)
        {
            foreach (UIElement child in MessageContainer.Children)
            {
                if (child is ChatDetailListItem chatItem && chatItem.GUID == guid)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 清理旧消息，保持消息数量在maxCount以下
        /// </summary>
        /// <param name="maxCount">最大消息数量</param>
        public void RemoveOldMessages(int maxCount)
        {
            while (MessageContainer.Children.Count > maxCount)
            {
                MessageContainer.Children.RemoveAt(0);
            }
        }

        /// <summary>
        /// 更新消息发送状态
        /// </summary>
        /// <param name="guid">消息GUID</param>
        /// <param name="isSending">是否正在发送</param>
        public void UpdateSendStatus(string? guid, bool isSending)
        {
            if (string.IsNullOrEmpty(guid)) return;
            
            foreach (UIElement child in MessageContainer.Children)
            {
                if (child is ChatDetailListItem chatItem && chatItem.GUID == guid)
                {
                    chatItem.UpdateSendStatus(isSending);
                    break;
                }
            }
        }

        /// <summary>
        /// 标记消息发送失败
        /// </summary>
        /// <param name="guid">消息GUID</param>
        public void MarkSendFailed(string? guid)
        {
            if (string.IsNullOrEmpty(guid)) return;
            
            foreach (UIElement child in MessageContainer.Children)
            {
                if (child is ChatDetailListItem chatItem && chatItem.GUID == guid)
                {
                    chatItem.SendFail();
                    break;
                }
            }
        }

        /// <summary>
        /// 更新消息ID
        /// </summary>
        /// <param name="guid">消息GUID</param>
        /// <param name="msgId">新的消息ID</param>
        public void UpdateMessageId(string? guid, int msgId)
        {
            if (string.IsNullOrEmpty(guid)) return;
            
            foreach (UIElement child in MessageContainer.Children)
            {
                if (child is ChatDetailListItem chatItem && chatItem.GUID == guid)
                {
                    chatItem.MsgId = msgId;
                    break;
                }
            }
        }

        /// <summary>
        /// 获取内部的ScrollViewer控件
        /// </summary>
        /// <returns>ScrollViewer控件</returns>
        public ScrollViewer GetScrollViewer()
        {
            return MessageScrollViewer;
        }

        /// <summary>
        /// 获取内部的消息容器
        /// </summary>
        /// <returns>消息容器StackPanel</returns>
        public StackPanel GetMessageContainer()
        {
            return MessageContainer;
        }

        /// <summary>
        /// 获取滚动到底部按钮容器
        /// </summary>
        /// <returns>滚动到底部按钮容器Grid</returns>
        public Grid GetScrollBottomContainer()
        {
            return ScrollBottomContainer;
        }

        #endregion

        #region 私有方法

        private void UpdateScrollToBottomVisibility()
        {
            ScrollBottomContainer.Visibility = _isAtBottom ? Visibility.Collapsed : Visibility.Visible;
        }

        #endregion
    }
}
