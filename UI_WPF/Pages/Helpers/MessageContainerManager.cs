using Another_Mirai_Native.Config;
using Another_Mirai_Native.UI.Controls;
using Another_Mirai_Native.UI.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Another_Mirai_Native.UI.Pages.Helpers
{
    /// <summary>
    /// 管理消息容器的添加、删除、滚动
    /// </summary>
    public class MessageContainerManager
    {
        private readonly StackPanel _messageContainer;
        private readonly ScrollViewer _scrollViewer;
        private readonly FrameworkElement _scrollBottomContainer;
        private readonly List<ChatDetailItemViewModel> _detailList;
        private readonly Func<ChatDetailItemViewModel, UIElement> _buildMessageItem;
        private readonly Dispatcher _dispatcher;
        private readonly int _maxMessageCount;

        /// <summary>
        /// 当前消息列表
        /// </summary>
        public List<ChatDetailItemViewModel> DetailList => _detailList;

        public MessageContainerManager(
            StackPanel messageContainer,
            ScrollViewer scrollViewer,
            FrameworkElement scrollBottomContainer,
            List<ChatDetailItemViewModel> detailList,
            Func<ChatDetailItemViewModel, UIElement> buildMessageItem,
            Dispatcher dispatcher,
            int maxMessageCount = 30)
        {
            _messageContainer = messageContainer ?? throw new ArgumentNullException(nameof(messageContainer));
            _scrollViewer = scrollViewer ?? throw new ArgumentNullException(nameof(scrollViewer));
            _scrollBottomContainer = scrollBottomContainer ?? throw new ArgumentNullException(nameof(scrollBottomContainer));
            _detailList = detailList ?? throw new ArgumentNullException(nameof(detailList));
            _buildMessageItem = buildMessageItem ?? throw new ArgumentNullException(nameof(buildMessageItem));
            _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
            _maxMessageCount = maxMessageCount;

            // 订阅滚动事件，更新"滚动到底部"按钮可见性
            _scrollViewer.ScrollChanged += OnScrollChanged;
        }

        /// <summary>
        /// 添加单条消息到容器
        /// </summary>
        /// <param name="item">消息ViewModel</param>
        /// <param name="autoClean">是否自动清理旧消息</param>
        public void AddMessage(ChatDetailItemViewModel item, bool autoClean = true)
        {
            var uiElement = _buildMessageItem(item);
            _messageContainer.Children.Add(uiElement);
            _detailList.Add(item);

            if (autoClean && ShouldCleanOldMessages())
            {
                RemoveOldMessages(_maxMessageCount);
            }
        }

        /// <summary>
        /// 批量添加消息到容器
        /// </summary>
        /// <param name="items">消息ViewModels列表</param>
        /// <param name="insertAtBeginning">true=插入到前面（懒加载），false=添加到后面（新消息）</param>
        public void AddMessages(List<ChatDetailItemViewModel> items, bool insertAtBeginning = false)
        {
            if (insertAtBeginning)
            {
                // 懒加载：在前面插入
                for (int i = items.Count - 1; i >= 0; i--)
                {
                    var uiElement = _buildMessageItem(items[i]);
                    _messageContainer.Children.Insert(0, uiElement);
                    _detailList.Insert(0, items[i]);
                }
            }
            else
            {
                // 新消息：添加到后面
                foreach (var item in items)
                {
                    var uiElement = _buildMessageItem(item);
                    _messageContainer.Children.Add(uiElement);
                    _detailList.Add(item);
                }
            }
        }

        /// <summary>
        /// 清空所有消息
        /// </summary>
        public void ClearMessages()
        {
            _messageContainer.Children.Clear();
            _detailList.Clear();
            GC.Collect();
        }

        /// <summary>
        /// 滚动到底部
        /// </summary>
        /// <param name="forced">true时忽略按钮状态，强制滚动</param>
        public void ScrollToBottom(bool forced = false)
        {
            if (!forced && _scrollBottomContainer.Visibility == Visibility.Visible)
            {
                // 如果"滚动到底部"按钮可见，说明用户可能在浏览历史消息，不自动滚动
                return;
            }
            _scrollViewer.ScrollToBottom();
        }

        /// <summary>
        /// 检查容器中是否已有指定GUID的消息
        /// </summary>
        public bool HasMessage(string guid)
        {
            foreach (UIElement item in _messageContainer.Children)
            {
                if (item is ChatDetailListItem detail && detail.GUID == guid)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 移除旧消息，保持消息数量在指定范围内
        /// </summary>
        /// <param name="targetCount">目标消息数量</param>
        public void RemoveOldMessages(int targetCount)
        {
            while (_messageContainer.Children.Count > targetCount)
            {
                _messageContainer.Children.RemoveAt(0);
                _detailList.RemoveAt(0);
            }
        }

        /// <summary>
        /// 通过消息ID查找并滚动到指定消息
        /// </summary>
        public void ScrollToMessage(int msgId)
        {
            foreach (var control in _messageContainer.Children)
            {
                if (control is ChatDetailListItem detail && detail.MsgId == msgId)
                {
                    detail.BringIntoView();
                    break;
                }
            }
        }

        /// <summary>
        /// 通过GUID查找并更新消息发送状态
        /// </summary>
        public void UpdateSendStatus(string? guid, bool isSending)
        {
            if (string.IsNullOrEmpty(guid))
            {
                return;
            }

            _dispatcher.BeginInvoke(() =>
            {
                foreach (UIElement item in _messageContainer.Children)
                {
                    if (item is ChatDetailListItem detail && detail.GUID == guid)
                    {
                        detail.UpdateSendStatus(isSending);
                        return;
                    }
                }
            });
        }

        /// <summary>
        /// 通过GUID标记消息发送失败
        /// </summary>
        public void MarkSendFailed(string? guid)
        {
            if (string.IsNullOrEmpty(guid))
            {
                return;
            }

            _dispatcher.BeginInvoke(() =>
            {
                foreach (UIElement item in _messageContainer.Children)
                {
                    if (item is ChatDetailListItem detail && detail.GUID == guid)
                    {
                        detail.SendFail();
                        return;
                    }
                }
            });
        }

        /// <summary>
        /// 通过GUID更新消息ID
        /// </summary>
        public void UpdateMessageId(string? guid, int msgId)
        {
            if (string.IsNullOrEmpty(guid))
            {
                return;
            }

            _dispatcher.BeginInvoke(() =>
            {
                foreach (UIElement item in _messageContainer.Children)
                {
                    if (item is ChatDetailListItem detail && detail.GUID == guid)
                    {
                        detail.UpdateMessageId(msgId);
                        return;
                    }
                }
            });
        }

        /// <summary>
        /// 判断是否应该清理旧消息
        /// </summary>
        private bool ShouldCleanOldMessages()
        {
            return _messageContainer.Children.Count > _maxMessageCount
                   && _scrollViewer.VerticalOffset > 100; // 滚动条不在懒加载区
        }

        /// <summary>
        /// 处理滚动事件，更新"滚动到底部"按钮可见性
        /// </summary>
        private void OnScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (e.VerticalChange == 0)
            {
                return;
            }

            double distanceToBottom = _scrollViewer.ScrollableHeight - _scrollViewer.VerticalOffset;
            _scrollBottomContainer.Visibility = distanceToBottom > 100 ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// 刷新消息容器，加载指定数量的最新消息
        /// </summary>
        /// <param name="loadCount">加载的消息数量</param>
        public async Task RefreshAsync(int loadCount)
        {
            ClearMessages();

            var itemsToLoad = _detailList.Skip(Math.Max(0, _detailList.Count - loadCount)).ToList();
            foreach (var item in itemsToLoad)
            {
                if (!HasMessage(item.GUID))
                {
                    AddMessage(item, autoClean: true);
                }
            }

            await Dispatcher.Yield();
            ScrollToBottom();
        }

        /// <summary>
        /// 清理资源
        /// </summary>
        public void Dispose()
        {
            _scrollViewer.ScrollChanged -= OnScrollChanged;
        }
    }
}
