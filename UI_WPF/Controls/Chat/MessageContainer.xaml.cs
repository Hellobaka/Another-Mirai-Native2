using Another_Mirai_Native.DB;
using Another_Mirai_Native.Model;
using Another_Mirai_Native.UI.Models;
using Another_Mirai_Native.UI.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Another_Mirai_Native.UI.Controls.Chat
{
    /// <summary>
    /// MessageContainer.xaml 的交互逻辑
    /// </summary>
    public partial class MessageContainer : UserControl
    {
        public MessageContainer()
        {
            InitializeComponent();
        }

        public ChatViewModel ViewModel => (ChatViewModel)DataContext;

        private DispatcherTimer LazyLoadDebounceTimer { get; set; }

        private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (sender is not ScrollViewer scrollViewer || e.VerticalChange == 0)
            {
                return;
            }
            double distanceToBottom = scrollViewer.ScrollableHeight - scrollViewer.VerticalOffset;
            double distanceToTop = scrollViewer.VerticalOffset;

            ScrollBottomContainer.Visibility = distanceToBottom > 100 ? Visibility.Visible : Visibility.Collapsed;
            if (distanceToTop < 50 && distanceToBottom > 100 && !ViewModel.LazyLoading)
            {
                // 滚动条距顶部50像素以内 且 距离底部100像素以上
                // 懒加载防抖
                if (LazyLoadDebounceTimer == null)
                {
                    LazyLoadDebounceTimer = new DispatcherTimer
                    {
                        Interval = TimeSpan.FromMilliseconds(300),
                    };
                    LazyLoadDebounceTimer.Tick += (_, _) =>
                    {
                        LazyLoadDebounceTimer.Stop();
                        Dispatcher.BeginInvoke(async () => await LazyLoad(UIConfig.Instance.MessageContainerMaxCount));
                        Dispatcher.BeginInvoke(() => ViewModel.LazyLoading = false);
                    };
                    ViewModel.LazyLoading = true;
                    LazyLoadDebounceTimer.Start();
                }
                else
                {
                    LazyLoadDebounceTimer.Stop();
                }
                ViewModel.LazyLoading = true;
                LazyLoadDebounceTimer.Start();
            }
        }

        private void ScrollToBottomBtn_Click(object sender, RoutedEventArgs e)
        {
            // 滚动到最底部
            MessageScrollViewer.ScrollToBottom();

            // 如果消息数量超过上限且当前滚动条不在懒加载区域（距顶部较远），则删除最旧的消息以限制数量
            try
            {
                if (ViewModel?.Messages != null
                    && ViewModel.Messages.Count > UIConfig.Instance.MessageContainerMaxCount
                    && MessageScrollViewer.VerticalOffset > 100)
                {
                    while (ViewModel.Messages.Count > UIConfig.Instance.MessageContainerMaxCount)
                    {
                        ViewModel.Messages.RemoveAt(0);
                    }

                    // 重置当前页索引为1，保证后续懒加载行为正确
                    ViewModel.CurrentPageIndex = 1;

                    // 更新布局并确保仍在底部
                    MessageScrollViewer.UpdateLayout();
                    MessageScrollViewer.ScrollToBottom();
                }
            }
            catch { }
        }

        /// <summary>
        /// 消息列表懒加载
        /// </summary>
        /// <param name="count">欲加载的消息数量</param>
        /// <param name="msgId">最终跳转的消息ID, 若为-1则保持当前滚动条</param>
        private async Task LazyLoad(int count, int msgId = -1)
        {
            if (ViewModel.SelectedChat == null)
            {
                return;
            }

            int pageSize = UIConfig.Instance.MessageContainerMaxCount;
            //需要加载的页数
            int pagesToLoad = (count + pageSize - 1) / pageSize;

            // 从数据库取的消息历史
            List<ChatHistory> list = [];
            for (int i = ViewModel.CurrentPageIndex + 1; i <= ViewModel.CurrentPageIndex + pagesToLoad; i++)
            {
                var ls = await ChatHistoryHelper.GetHistoriesByPageAsync(ViewModel.SelectedChat.Id,
                    ViewModel.SelectedChat.AvatarType == ChatType.QQPrivate ? ChatHistoryType.Private : ChatHistoryType.Group,
                    count,
                    i);
                if (ls != null && ls.Count > 0)
                {
                    list.AddRange(ls);
                }
            }

            if (list.Count == 0)
            {
                return;
            }

            // 更新页数
            ViewModel.CurrentPageIndex += pagesToLoad;

            list.Reverse();

            double distanceToBottom = MessageScrollViewer.ScrollableHeight - MessageScrollViewer.VerticalOffset;
            MessageViewModel? scrollItem = null;

            foreach (var item in list)
            {
                var viewModel = await ChatViewModel.ParseChatHistoryToViewModel(ViewModel.SelectedChat.AvatarType, item);

                // 确保在 UI线程更新集合
                if (!Dispatcher.CheckAccess())
                {
                    await Dispatcher.InvokeAsync(() => ViewModel.Messages.Insert(0, viewModel));
                }
                else
                {
                    ViewModel.Messages.Insert(0, viewModel);
                }

                if (item.MsgId == msgId)
                {
                    scrollItem = viewModel;
                }
            }

            MessageScrollViewer.UpdateLayout();
            await Dispatcher.Yield();

            if (msgId == -1 || scrollItem == null)
            {
                // 保持滚动条位置
                MessageScrollViewer.ScrollToVerticalOffset(MessageScrollViewer.ScrollableHeight - distanceToBottom);
            }
            else
            {
                var container = MessageItemsControl.ItemContainerGenerator.ContainerFromItem(scrollItem) as FrameworkElement;
                container?.BringIntoView();
            }
        }

        private void ViewModel_OnScrollToBottomRequested()
        {
            ScrollToBottomBtn_Click(null, null);
        }

        private async void ViewModel_OnMessageJumpRequested(int msgId)
        {
            if (ViewModel.SelectedChat == null)
            {
                return;
            }
            var history = ChatHistoryHelper.GetHistoriesByMsgId(ViewModel.SelectedChat.Id, msgId,
                                                                ViewModel.SelectedChat.AvatarType == ChatType.QQGroup ? ChatHistoryType.Group : ChatHistoryType.Private);
            if (history == null)
            {
                return;
            }
            // 当前消息列表中已经有此消息
            var item = ViewModel.Messages.FirstOrDefault(x => x.MsgId == msgId);
            if (item != null)
            {
                var container = MessageItemsControl.ItemContainerGenerator.ContainerFromItem(item) as FrameworkElement;
                container?.BringIntoView();
            }
            else
            {
                // 计算相差数量, 进行懒加载并跳转
                int lastId = ViewModel.Messages.First().SqlId;
                await LazyLoad(lastId - history.ID, msgId);
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            ViewModel.OnMessageJumpRequested += ViewModel_OnMessageJumpRequested;
            ViewModel.OnScrollToBottomRequested += ViewModel_OnScrollToBottomRequested;
        }
    }
}
