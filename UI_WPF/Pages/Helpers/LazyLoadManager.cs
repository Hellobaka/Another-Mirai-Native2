using Another_Mirai_Native.Config;
using Another_Mirai_Native.DB;
using Another_Mirai_Native.Model;
using Another_Mirai_Native.Model.Enums;
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
    /// 管理消息列表的懒加载逻辑
    /// </summary>
    public class LazyLoadManager
    {
        private readonly ScrollViewer _scrollViewer;
        private readonly Func<ChatListItemViewModel?> _getSelectedItem;
        private readonly Func<ChatAvatar.AvatarTypes, ChatHistory, Task<ChatDetailItemViewModel>> _parseHistory;
        private readonly Action<List<ChatDetailItemViewModel>, bool> _addItemsToContainer;
        private readonly Dispatcher _dispatcher;
        
        private DispatcherTimer? _debounceTimer;
        private bool _isLoading;
        private int _currentPageIndex;
        private readonly int _loadCount;

        /// <summary>
        /// 当前页数索引
        /// </summary>
        public int CurrentPageIndex
        {
            get => _currentPageIndex;
            set => _currentPageIndex = value;
        }

        /// <summary>
        /// 是否正在加载中
        /// </summary>
        public bool IsLoading => _isLoading;

        public LazyLoadManager(
            ScrollViewer scrollViewer,
            Func<ChatListItemViewModel?> getSelectedItem,
            Func<ChatAvatar.AvatarTypes, ChatHistory, Task<ChatDetailItemViewModel>> parseHistory,
            Action<List<ChatDetailItemViewModel>, bool> addItemsToContainer,
            Dispatcher dispatcher,
            int loadCount = 15)
        {
            _scrollViewer = scrollViewer ?? throw new ArgumentNullException(nameof(scrollViewer));
            _getSelectedItem = getSelectedItem ?? throw new ArgumentNullException(nameof(getSelectedItem));
            _parseHistory = parseHistory ?? throw new ArgumentNullException(nameof(parseHistory));
            _addItemsToContainer = addItemsToContainer ?? throw new ArgumentNullException(nameof(addItemsToContainer));
            _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
            _loadCount = loadCount;
            _currentPageIndex = 1;

            _scrollViewer.ScrollChanged += OnScrollChanged;
        }

        /// <summary>
        /// 启用懒加载
        /// </summary>
        public void Enable()
        {
            _scrollViewer.ScrollChanged += OnScrollChanged;
        }

        /// <summary>
        /// 禁用懒加载
        /// </summary>
        public void Disable()
        {
            _scrollViewer.ScrollChanged -= OnScrollChanged;
            _debounceTimer?.Stop();
        }

        /// <summary>
        /// 重置页数
        /// </summary>
        public void Reset()
        {
            _currentPageIndex = 1;
            _isLoading = false;
            _debounceTimer?.Stop();
        }

        /// <summary>
        /// 处理滚动事件
        /// </summary>
        private void OnScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (e.VerticalChange == 0)
            {
                return;
            }

            double distanceToTop = _scrollViewer.VerticalOffset;
            double distanceToBottom = _scrollViewer.ScrollableHeight - _scrollViewer.VerticalOffset;

            // 滚动条距顶部50像素以内 且 距离底部100像素以上
            if (distanceToTop < 50 && distanceToBottom > 100 && !_isLoading)
            {
                TriggerLazyLoadWithDebounce();
            }
        }

        /// <summary>
        /// 触发懒加载（带防抖）
        /// </summary>
        private void TriggerLazyLoadWithDebounce()
        {
            if (_debounceTimer == null)
            {
                _debounceTimer = new DispatcherTimer
                {
                    Interval = TimeSpan.FromMilliseconds(300),
                };
                _debounceTimer.Tick += async (_, _) =>
                {
                    _debounceTimer.Stop();
                    await LoadMoreMessagesAsync(UIConfig.Instance.MessageContainerMaxCount);
                    _isLoading = false;
                };
            }
            else
            {
                _debounceTimer.Stop();
            }
            
            _isLoading = true;
            _debounceTimer.Start();
        }

        /// <summary>
        /// 加载更多消息
        /// </summary>
        /// <param name="count">欲加载的消息数量</param>
        /// <param name="targetMsgId">最终跳转的消息ID, 若为-1则保持当前滚动条</param>
        public async Task LoadMoreMessagesAsync(int count, int targetMsgId = -1)
        {
            var selectedItem = _getSelectedItem();
            if (selectedItem == null)
            {
                return;
            }

            // 加载的页面数量
            int pageCount = (int)Math.Ceiling(count / (float)UIConfig.Instance.MessageContainerMaxCount);
            
            // 从数据库取的消息历史
            List<ChatHistory> historyList = new();
            for (int i = _currentPageIndex + 1; i <= _currentPageIndex + pageCount; i++)
            {
                var pageHistories = ChatHistoryHelper.GetHistoriesByPage(
                    selectedItem.Id,
                    selectedItem.AvatarType == ChatAvatar.AvatarTypes.QQPrivate ? ChatHistoryType.Private : ChatHistoryType.Group,
                    count,
                    i);
                historyList = historyList.Concat(pageHistories).ToList();
            }

            if (historyList.Count > 0)
            {
                // 更新页数
                _currentPageIndex += pageCount;
            }
            else
            {
                return;
            }

            // 反转列表，让旧消息在前
            historyList.Reverse();

            // 记录滚动位置
            double distanceToBottom = _scrollViewer.ScrollableHeight - _scrollViewer.VerticalOffset;

            // 解析历史记录并构建ViewModels
            List<ChatDetailItemViewModel> viewModels = new();
            foreach (var history in historyList)
            {
                var viewModel = await _parseHistory(selectedItem.AvatarType, history);
                viewModels.Add(viewModel);
            }

            // 添加到容器（在前面插入）
            _addItemsToContainer(viewModels, true);

            // 等待UI更新
            await Dispatcher.Yield();

            // 恢复或调整滚动位置
            if (targetMsgId == -1)
            {
                // 保持滚动条位置
                _scrollViewer.ScrollToVerticalOffset(_scrollViewer.ScrollableHeight - distanceToBottom);
            }
            else
            {
                // 滚动到目标消息（需要外部实现）
                // 这个功能由MessageContainerManager.ScrollToMessage实现
            }
        }

        /// <summary>
        /// 清理资源
        /// </summary>
        public void Dispose()
        {
            Disable();
            _debounceTimer = null;
        }
    }
}
