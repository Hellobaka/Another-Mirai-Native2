using Another_Mirai_Native.Config;
using Another_Mirai_Native.DB;
using Another_Mirai_Native.Model;
using Another_Mirai_Native.Model.Enums;
using Another_Mirai_Native.Native;
using Another_Mirai_Native.UI.Controls;
using Another_Mirai_Native.UI.Pages.Helpers;
using Another_Mirai_Native.UI.Services;
using Another_Mirai_Native.UI.ViewModel;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Another_Mirai_Native.UI.Pages
{
    /// <summary>
    /// ChatPage.xaml 的交互逻辑
    /// </summary>
    public partial class ChatPage : Page, IDisposable
    {
        // 服务实例
        private readonly ICacheService _cacheService;
        private readonly IMessageService _messageService;
        private readonly IChatListService _chatListService;
        private MessageSendingCoordinator _messageSendingCoordinator;

        // 辅助管理器
        private LazyLoadManager? _lazyLoadManager;
        private MessageContainerManager? _messageContainerManager;

        // ViewModel
        private ChatPageViewModel? _viewModel;
        
        // 标记是否已释放资源
        private bool _disposed;

        public ChatPage()
        {
            // 初始化服务
            _cacheService = new CacheService();
            _messageService = new MessageService(_cacheService);
            _chatListService = new ChatListService(_cacheService);

            InitializeComponent();
            
            // 初始化ViewModel
            _viewModel = new ChatPageViewModel();
            DataContext = _viewModel;
            
            // 初始化消息发送协调器（在MessageContainerManager初始化后会重新设置）
            _messageSendingCoordinator = new MessageSendingCoordinator(_messageService);
            
            // 订阅ViewModel事件
            _viewModel.SelectedChatItemChanged += ViewModel_SelectedChatItemChanged;
            _viewModel.SendMessageRequested += ViewModel_SendMessageRequested;
            _viewModel.ClearMessageRequested += ViewModel_ClearMessageRequested;
            _viewModel.ClearSendBoxRequested += ViewModel_ClearSendBoxRequested;
            _viewModel.ScrollToBottomRequested += ViewModel_ScrollToBottomRequested;
            
            // 订阅Unloaded事件以清理资源
            Unloaded += ChatPage_Unloaded;
            
            Instance = this;
            Page_Loaded(null, null);
        }

        /// <summary>
        /// 框架撤回消息事件, 各个消息块均订阅此事件
        /// </summary>
        public static event Action<int> MsgRecalled;

        /// <summary>
        /// 主窗体尺寸变化事件, 各个消息块均订阅此事件
        /// </summary>

        public static event Action<SizeChangedEventArgs> WindowSizeChanged;

        public static ChatPage Instance { get; private set; }

        /// <summary>
        /// 侧边栏列表（通过ViewModel访问）
        /// </summary>
        public List<ChatListItemViewModel> ChatList => _viewModel?.ChatList.ToList() ?? new List<ChatListItemViewModel>();

        /// <summary>
        /// 消息容器列表
        /// </summary>
        public List<ChatDetailItemViewModel> DetailList { get; set; } = new();

        /// <summary>
        /// 当前选择项的组名称（通过ViewModel访问）
        /// </summary>
        public string GroupName
        {
            get => _viewModel?.GroupName ?? "";
            set
            {
                if (_viewModel != null)
                {
                    _viewModel.GroupName = value;
                }
            }
        }

        /// <summary>
        /// At选择器Flyout元素
        /// </summary>
        private ModernWpf.Controls.Flyout AtFlyout { get; set; }

        /// <summary>
        /// At选择器内容
        /// </summary>
        private AtTargetSelector AtTargetSelector { get; set; }

        /// <summary>
        /// 窗体加载完成事件
        /// </summary>
        private bool FormLoaded { get; set; }

        /// <summary>
        /// 初始化加载时的消息数量
        /// </summary>
        private int LoadCount { get; set; } = 15;

        /// <summary>
        /// 左侧聊天列表选中的元素
        /// </summary>
        private ChatListItemViewModel? SelectedItem => _viewModel?.SelectedChatItem;

        /// <summary>
        /// ViewModel事件处理：选中项改变
        /// </summary>
        private async void ViewModel_SelectedChatItemChanged(object? sender, ChatListItemViewModel? item)
        {
            if (item == null)
            {
                return;
            }

            // 重新加载消息内容
            List<ChatHistory> history = new();
            if (item.AvatarType == ChatAvatar.AvatarTypes.QQPrivate)
            {
                history = ChatHistoryHelper.GetHistoriesByPage(item.Id, ChatHistoryType.Private, LoadCount, 1);
            }
            else
            {
                history = ChatHistoryHelper.GetHistoriesByPage(item.Id, ChatHistoryType.Group, LoadCount, 1);
            }
            DetailList.Clear();
            foreach (var x in history)
            {
                DetailList.Add(await ParseChatHistoryToViewModel(item.AvatarType, x));
            }
            if (_lazyLoadManager != null)
            {
                _lazyLoadManager.CurrentPageIndex = 1;
            }
            
            // 更新显示名称
            GroupNameDisplay.Text = $"{item.GroupName} [{item.Id}]";
            GroupName = item.GroupName;
            
            // 重置懒加载管理器
            _lazyLoadManager?.Reset();
            
            await RefreshMessageContainer(true);
        }

        /// <summary>
        /// ViewModel事件处理：发送消息请求
        /// </summary>
        private void ViewModel_SendMessageRequested(object? sender, SendMessageEventArgs e)
        {
            string sendText = BuildTextFromRichTextBox();
            Task.Run(() =>
            {
                ExecuteSendMessage(e.TargetId, 
                    e.ChatType == ChatType.Group ? ChatAvatar.AvatarTypes.QQGroup : ChatAvatar.AvatarTypes.QQPrivate, 
                    sendText);
            });
            // 清空发送框
            RichTextBoxHelper.Clear(SendText);
        }

        /// <summary>
        /// ViewModel事件处理：清空消息容器
        /// </summary>
        private void ViewModel_ClearMessageRequested(object? sender, EventArgs e)
        {
            _messageContainerManager?.ClearMessages();
        }

        /// <summary>
        /// ViewModel事件处理：清空发送框
        /// </summary>
        private void ViewModel_ClearSendBoxRequested(object? sender, EventArgs e)
        {
            RichTextBoxHelper.Clear(SendText);
        }

        /// <summary>
        /// ViewModel事件处理：滚动到底部
        /// </summary>
        private void ViewModel_ScrollToBottomRequested(object? sender, EventArgs e)
        {
            _messageContainerManager?.ScrollToBottom(true);
            _messageContainerManager?.RemoveOldMessages(UIConfig.Instance.MessageContainerMaxCount);
            _lazyLoadManager?.Reset();
        }

        /// <summary>
        /// 向发送框中添加文本
        /// </summary>
        /// <param name="text">添加的文本</param>
        public void AddTextToSendBox(string text)
        {
            Dispatcher.BeginInvoke(() =>
            {
                RichTextBoxHelper.InsertText(SendText, text);
            });
        }

        /// <summary>
        /// 调用发送群消息
        /// </summary>
        /// <param name="groupId">发送的群</param>
        /// <param name="message">发送的消息</param>
        /// <returns>返回的消息ID, 不为0时为成功</returns>
        public int CallGroupMsgSend(long groupId, string message)
        {
            return _messageService.SendMessage(groupId, ChatType.Group, message);
        }

        /// <summary>
        /// 调用发送好友消息
        /// </summary>
        /// <param name="qq">发送的好友</param>
        /// <param name="message">发送的消息</param>
        /// <returns>返回的消息ID, 不为0时为成功</returns>
        public int CallPrivateMsgSend(long qq, string message)
        {
            return _messageService.SendMessage(qq, ChatType.Private, message);
        }

        /// <summary>
        /// 添加消息块并调用发送
        /// </summary>
        /// <param name="id">发送对象</param>
        /// <param name="avatar">发送对象的类型</param>
        /// <param name="message">发送的消息</param>
        public async void ExecuteSendMessage(long id, ChatAvatar.AvatarTypes avatar, string message)
        {
            var request = new SendMessageRequest
            {
                TargetId = id,
                ChatType = avatar == ChatAvatar.AvatarTypes.QQGroup ? ChatType.Group : ChatType.Private,
                AvatarType = avatar,
                Message = message,
                SenderId = AppConfig.Instance.CurrentQQ,
                SendTime = DateTime.Now
            };

            var result = await _messageSendingCoordinator.SendMessageAsync(request);
            
            if (!result.Success && result.Exception != null)
            {
                // 可以在这里添加日志记录或用户提示
                System.Diagnostics.Debug.WriteLine($"消息发送失败: {result.ErrorMessage}");
            }
        }

        /// <summary>
        /// 从缓存中获取好友昵称
        /// 若缓存中不存在则调用协议API
        /// </summary>
        /// <param name="qq">好友ID</param>
        /// <returns>昵称, 失败时返回QQ号</returns>
        public async Task<string> GetFriendNick(long qq)
        {
            return await _cacheService.GetFriendNickAsync(qq);
        }

        /// <summary>
        /// 从缓存中获取群员名片
        /// 若缓存中不存在则调用协议API
        /// </summary>
        /// <param name="group">群来源</param>
        /// <param name="qq">群员QQ</param>
        /// <returns>群员名片, 若不存在则返回昵称, 若调用失败则返回QQ号</returns>
        public async Task<string> GetGroupMemberNick(long group, long qq)
        {
            return await _cacheService.GetGroupMemberNickAsync(group, qq);
        }

        /// <summary>
        /// 从缓存中获取群名称
        /// 若缓存中不存在则调用协议API
        /// </summary>
        /// <param name="groupId">群号</param>
        /// <returns>群名称, 若不存在则返回群号</returns>
        public async Task<string> GetGroupName(long groupId)
        {
            return await _cacheService.GetGroupNameAsync(groupId);
        }

        /// <summary>
        /// 跳转至消息ID目标的消息
        /// </summary>
        /// <param name="msgId">消息ID</param>
        public async void JumpToReplyItem(int msgId)
        {
            var history = ChatHistoryHelper.GetHistoriesByMsgId(SelectedItem.Id, msgId, SelectedItem.AvatarType == ChatAvatar.AvatarTypes.QQGroup ? ChatHistoryType.Group : ChatHistoryType.Private);
            if (history == null) // 查询失败
            {
                return;
            }
            // 当前消息列表中已经有此消息
            var item = DetailList.FirstOrDefault(x => x.MsgId == msgId);
            if (item != null)
            {
                // 滚动到该消息
                _messageContainerManager?.ScrollToMessage(msgId);
            }
            else
            {
                // 计算相差数量, 进行懒加载并跳转
                int lastId = DetailList.First().SqlId;
                if (_lazyLoadManager != null)
                {
                    await _lazyLoadManager.LoadMoreMessagesAsync(lastId - history.ID, msgId);
                }
            }
        }

        /// <summary>
        /// 更新目标消息历史的未读消息数量
        /// </summary>
        /// <param name="model"></param>
        public async void UpdateUnreadCount(ChatListItemViewModel model)
        {
            var item = _viewModel?.ChatList.FirstOrDefault(x => x.Id == model.Id && x.AvatarType == model.AvatarType);
            if (item != null)
            {
                item.UnreadCount = model.UnreadCount;
                await ReorderChatList();
            }
        }

        /// <summary>
        /// MVVM属性变化通知（保留用于向后兼容）
        /// </summary>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            // 由ViewModel处理，这里保留空实现用于向后兼容
        }

        /// <summary>
        /// 消息持久化 若消息刚好为当前群则向容器构建消息并滚动至底
        /// </summary>
        /// <param name="group">群号</param>
        /// <param name="qq">发送者ID</param>
        /// <param name="msg">消息</param>
        /// <param name="itemType">消息位置</param>
        /// <param name="msgId">消息ID</param>
        /// <param name="itemAdded">消息添加后的回调</param>
        /// <param name="plugin">发送来源插件</param>
        /// <returns>消息持久化的ID</returns>
        internal async Task<int> AddGroupChatItem(long group, long qq, string msg, DetailItemType itemType, DateTime time, int msgId = 0, Action<string>? itemAdded = null, CQPluginProxy? plugin = null)
        {
            string nick = await GetGroupMemberNick(group, qq);
            if (nick != null && plugin != null)
            {
                nick = $"{nick} [{plugin.PluginName}]";
            }
            ChatDetailItemViewModel item = BuildChatDetailItem(msgId, qq, msg, nick!, ChatAvatar.AvatarTypes.QQGroup, itemType);
            var history = ChatHistoryHelper.GetHistoriesByMsgId(group, msgId, ChatHistoryType.Group);
            AddOrUpdateGroupChatList(group, qq, msg);
            await Dispatcher.BeginInvoke(() =>
            {
                if (SelectedItem?.Id == group)
                {
                    AddItemToMessageContainer(item, true);
                    _messageContainerManager?.ScrollToBottom(qq == AppConfig.Instance.CurrentQQ);
                }
            });
            itemAdded?.Invoke(item.GUID);

            return history?.ID ?? 0;
        }

        /// <summary>
        /// 向消息容器构建消息块
        /// </summary>
        /// <param name="item">消息模型</param>
        /// <param name="isRemove">是否需要清理消息至最少</param>
        private void AddItemToMessageContainer(ChatDetailItemViewModel item, bool isRemove)
        {
            _messageContainerManager?.AddMessage(item, isRemove);
        }

        /// <summary>
        /// 添加或更新左侧聊天列表群内容
        /// </summary>
        /// <param name="id">群ID</param>
        /// <param name="qq">发送者ID</param>
        /// <param name="msg">消息</param>
        private async void AddOrUpdateGroupChatList(long id, long qq, string msg)
        {
            if (_viewModel == null) return;
            
            msg = msg.Replace("\r", "").Replace("\n", "");
            var item = _viewModel.ChatList.FirstOrDefault(x => x.Id == id && x.AvatarType == ChatAvatar.AvatarTypes.QQGroup);
            if (item != null) // 消息已存在, 更新
            {
                item.GroupName = await GetGroupName(id);
                item.Detail = $"{await GetGroupMemberNick(id, qq)}: {msg}";
                item.Time = DateTime.Now;
                item.UnreadCount++;
                await ReorderChatList();
            }
            else
            {
                await Dispatcher.BeginInvoke(async () =>
                {
                    _viewModel.ChatList.Add(new ChatListItemViewModel
                    {
                        AvatarType = ChatAvatar.AvatarTypes.QQGroup,
                        Detail = $"{await GetGroupMemberNick(id, qq)}: {msg}",
                        GroupName = await GetGroupName(id),
                        Id = id,
                        Time = DateTime.Now,
                        UnreadCount = 1
                    });
                    await ReorderChatList();
                });
            }
        }

        /// <summary>
        /// 添加或更新左侧聊天列表好友内容
        /// </summary>
        /// <param name="qq">好友ID</param>
        /// <param name="sender">发送者ID</param>
        /// <param name="msg">消息</param>
        private async void AddOrUpdatePrivateChatList(long qq, long sender, string msg)
        {
            if (_viewModel == null) return;
            
            msg = msg.Replace("\r", "").Replace("\n", "");
            var item = _viewModel.ChatList.FirstOrDefault(x => x.Id == qq && x.AvatarType == ChatAvatar.AvatarTypes.QQPrivate);
            if (item != null)
            {
                item.GroupName = await GetFriendNick(qq);
                item.Detail = msg;
                item.Time = DateTime.Now;
                item.UnreadCount++;
                await ReorderChatList();
            }
            else
            {
                await Dispatcher.BeginInvoke(async () =>
                 {
                     _viewModel.ChatList.Add(new ChatListItemViewModel
                     {
                         AvatarType = ChatAvatar.AvatarTypes.QQPrivate,
                         Detail = msg,
                         GroupName = await GetFriendNick(qq),
                         Id = qq,  // 修复: 应该是qq而不是sender
                         Time = DateTime.Now,
                         UnreadCount = 1
                     });
                     await ReorderChatList();
                 });
            }
        }

        /// <summary>
        /// 消息持久化 若消息刚好为当前好友则向容器构建消息并滚动至底
        /// </summary>
        /// <param name="qq">好友ID</param>
        /// <param name="sender">发送者ID</param>
        /// <param name="msg">消息内容</param>
        /// <param name="itemType">消息位置</param>
        /// <param name="msgId">消息ID</param>
        /// <param name="itemAdded">持久化后的回调</param>
        /// <param name="plugin">消息来源的插件</param>
        /// <returns>持久化后的ID</returns>
        internal async Task<int> AddPrivateChatItem(long qq, long sender, string msg, DetailItemType itemType, DateTime time, int msgId = 0, Action<string>? itemAdded = null, CQPluginProxy? plugin = null)
        {
            string nick = await GetFriendNick(sender);
            if (nick != null && plugin != null)
            {
                nick = $"{nick} [{plugin.PluginName}]";
            }
            ChatDetailItemViewModel item = BuildChatDetailItem(msgId, sender, msg, nick!, ChatAvatar.AvatarTypes.QQPrivate, itemType);
            var history = ChatHistoryHelper.GetHistoriesByMsgId(qq, msgId, ChatHistoryType.Private);
            ChatHistoryHelper.UpdateHistoryCategory(history);
            AddOrUpdatePrivateChatList(qq, sender, msg);
            await Dispatcher.BeginInvoke(() =>
            {
                if (SelectedItem?.Id == qq)
                {
                    AddItemToMessageContainer(item, true);
                    _messageContainerManager?.ScrollToBottom(sender == AppConfig.Instance.CurrentQQ);
                }
            });
            itemAdded?.Invoke(item.GUID);
            return history?.ID ?? 0;
        }

        private void AtBtn_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedItem == null)
            {
                return;
            }
            List<ChatListItemViewModel> list = new();
            var rawList = ProtocolManager.Instance.CurrentProtocol.GetRawGroupMemberList(SelectedItem.Id);
            // 构建群成员列表
            if (rawList != null)
            {
                foreach (var item in rawList)
                {
                    if (item == null)
                    {
                        continue;
                    }
                    list.Add(new ChatListItemViewModel
                    {
                        Id = item.QQ,
                        GroupName = string.IsNullOrEmpty(item.Card) ? item.Nick : item.Card,
                        AvatarType = ChatAvatar.AvatarTypes.QQPrivate
                    });
                    // 缓存由CacheService管理，不需要手动更新
                }
            }
            // 显示Flyout
            AtTargetSelector = new(list);
            AtTargetSelector.ItemSelected -= AtTargetSelector_ItemSelected;
            AtTargetSelector.ItemSelected += AtTargetSelector_ItemSelected;
            AtFlyout = new ModernWpf.Controls.Flyout
            {
                Content = AtTargetSelector,
                Placement = ModernWpf.Controls.Primitives.FlyoutPlacementMode.TopEdgeAlignedLeft
            };
            AtFlyout.ShowAt(AtBtn);
        }

        /// <summary>
        /// At选择器的项目被选中
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AtTargetSelector_ItemSelected(object? sender, EventArgs e)
        {
            AddTextToSendBox(AtTargetSelector.SelectedCQCode);
            AtFlyout.Hide();
        }

        private void AudioBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new()
            {
                AddExtension = true,
                CheckFileExists = true,
                CheckPathExists = true,
                Multiselect = false,
                Filter = "音频文件|*.wav;*.mp3;*.flac;*.amr;*.m4a|所有文件|*.*",
                Title = "请选择要发送的音频"
            };
            if (openFileDialog.ShowDialog() is false)
            {
                return;
            }
            string filePath = openFileDialog.FileName;
            string audioPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"data\record\cached");
            Directory.CreateDirectory(audioPath);
            // 选择的文件在缓存文件夹中
            if (filePath.StartsWith(audioPath))
            {
                filePath = filePath.Replace(audioPath, "");
            }
            else
            {
                string fileName = Path.GetFileName(filePath);
                // 复制文件到缓存文件夹中
                File.Copy(filePath, Path.Combine(audioPath, fileName), true);
                filePath = @$"cached\\{fileName}";
            }
            AddTextToSendBox(CQCode.CQCode_Record(filePath).ToSendString());
        }

        /// <summary>
        /// 构建聊天模型
        /// </summary>
        /// <param name="msgId">消息ID</param>
        /// <param name="qq">来源QQ</param>
        /// <param name="msg">消息内容</param>
        /// <param name="nick">昵称</param>
        /// <param name="avatarType">消息来源</param>
        /// <param name="itemType">消息位置</param>
        private ChatDetailItemViewModel BuildChatDetailItem(int msgId, long qq, string msg, string nick, ChatAvatar.AvatarTypes avatarType, DetailItemType itemType)
        {
            return new ChatDetailItemViewModel
            {
                AvatarType = avatarType,
                Content = msg,
                DetailItemType = itemType,
                Id = qq,
                Nick = nick,
                MsgId = msgId,
                Time = DateTime.Now,
            };
        }

        /// <summary>
        /// 构建统一的消息块（支持 Send/Receive/Notice 三种类型）
        /// </summary>
        /// <param name="item">消息模型</param>
        /// <returns>消息块元素</returns>
        private UIElement BuildChatDetailItem(ChatDetailItemViewModel item)
        {
            HorizontalAlignment alignment = item.DetailItemType switch
            {
                DetailItemType.Send => HorizontalAlignment.Right,
                DetailItemType.Notice => HorizontalAlignment.Center,
                _ => HorizontalAlignment.Left  // Receive
            };

            return new ChatDetailListItem()
            {
                Message = item.Content,
                DetailItemType = item.DetailItemType,
                ParentType = item.AvatarType,
                DisplayName = item.Nick,
                Time = item.Time,
                Id = item.Id,
                ParentId = SelectedItem.Id,
                MsgId = item.MsgId,
                GUID = item.GUID,
                MaxWidth = ActualWidth * 0.6,
                HorizontalAlignment = alignment,
                Margin = new Thickness(0, 10, 0, 10),
                Recalled = item.Recalled,
            };
        }

        /// <summary>
        /// 发送消息转CQ码
        /// </summary>
        /// <returns>处理后的CQ码消息</returns>
        private string BuildTextFromRichTextBox()
        {
            return RichTextBoxHelper.ConvertToCQCode(SendText);
        }

        /// <summary>
        /// 左侧列表选中变化（绑定到ViewModel的SelectedChatItem）
        /// </summary>
        private void ChatListDisplay_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_viewModel == null) return;
            
            var item = ChatListDisplay.SelectedItem as ChatListItemViewModel;
            if (item != null)
            {
                // 更新ViewModel的选中项，这会触发ViewModel_SelectedChatItemChanged事件
                _viewModel.SelectedChatItem = item;
            }
        }

        /// <summary>
        /// 检测GUID是否已存在于消息容器中
        /// </summary>
        private bool CheckMessageContainerHasItem(string guid)
        {
            return _messageContainerManager?.HasMessage(guid) ?? false;
        }

        private void CleanMessageBtn_Click(object sender, RoutedEventArgs e)
        {
            _messageContainerManager?.ClearMessages();
        }

        private void CleanSendBtn_Click(object sender, RoutedEventArgs e)
        {
            RichTextBoxHelper.Clear(SendText);
        }

        /// <summary>
        /// CQP事件_群消息发送
        /// </summary>
        private async void CQPImplementation_OnGroupMessageSend(int msgId, long group, string msg, CQPluginProxy plugin)
        {
            // AddOrUpdateGroupChatList(group, AppConfig.Instance.CurrentQQ, msg);
            await AddGroupChatItem(group, AppConfig.Instance.CurrentQQ, msg, DetailItemType.Send, DateTime.Now, msgId, plugin: plugin);
        }

        private async void CQPImplementation_OnPrivateMessageSend(int msgId, long qq, string msg, CQPluginProxy plugin)
        {
            // AddOrUpdatePrivateChatList(qq, AppConfig.Instance.CurrentQQ, msg);
            await AddPrivateChatItem(qq, AppConfig.Instance.CurrentQQ, msg, DetailItemType.Send, DateTime.Now, msgId, plugin: plugin);
        }

        private void FaceImageSelector_ImageSelected(object sender, EventArgs e)
        {
            AddTextToSendBox(FaceImageSelector.SelectedImageCQCode);
            FaceImageFlyout.Hide();
        }

        /// <summary>
        /// 初始化辅助管理器
        /// </summary>
        private void InitializeManagers()
        {
            // 初始化消息容器管理器
            _messageContainerManager = new MessageContainerManager(
                MessageContainer,
                MessageScrollViewer,
                ScrollBottomContainer,
                DetailList,
                BuildChatDetailItem,
                Dispatcher,
                UIConfig.Instance.MessageContainerMaxCount
            );

            // 初始化懒加载管理器
            _lazyLoadManager = new LazyLoadManager(
                MessageScrollViewer,
                () => SelectedItem,
                async (avatarType, history) => await ParseChatHistoryToViewModel(avatarType, history),
                (items, insertAtBeginning) =>
                {
                    if (_messageContainerManager != null)
                    {
                        _messageContainerManager.AddMessages(items, insertAtBeginning);
                    }
                },
                Dispatcher,
                LoadCount
            );
            
            // 重新初始化MessageSendingCoordinator，传入MessageContainerManager
            _messageSendingCoordinator = new MessageSendingCoordinator(_messageService, _messageContainerManager);
        }

        /// <summary>
        /// 加载左侧聊天列表
        /// </summary>
        /// <returns></returns>
        private async Task LoadChatHistory()
        {
            if (_viewModel == null) return;
            
            var list = ChatHistoryHelper.GetHistoryCategroies();
            _viewModel.ChatList.Clear();
            foreach (var item in list)
            {
                _viewModel.ChatList.Add(new ChatListItemViewModel
                {
                    AvatarType = item.Type == ChatHistoryType.Private ? ChatAvatar.AvatarTypes.QQPrivate : ChatAvatar.AvatarTypes.QQGroup,
                    Detail = item.Message,
                    GroupName = item.Type == ChatHistoryType.Private ? await GetFriendNick(item.ParentID) : await GetGroupName(item.ParentID),
                    Id = item.ParentID,
                    Time = item.Time,
                    UnreadCount = 0
                });
            }
            EmptyHint.Visibility = _viewModel.ChatList.Count != 0 ? Visibility.Collapsed : Visibility.Visible;
        }

        private async void Page_Loaded(object? sender, RoutedEventArgs? e)
        {
            if (AppConfig.Instance.EnableChat is false)
            {
                DisableDisplay.Visibility = Visibility.Visible;
                MainContent.Visibility = Visibility.Collapsed;
                return;
            }
            DisableDisplay.Visibility = Visibility.Collapsed;
            MainContent.Visibility = Visibility.Visible;

            if (FormLoaded)
            {
                if (SelectedItem == null && _viewModel != null && _viewModel.ChatList.Count > 0)
                {
                    // 当没有内容被选中时 选中第一项
                    await Dispatcher.Yield();
                    ChatListDisplay.SelectedItem = _viewModel.ChatList.First();
                }
                return;
            }
            FormLoaded = true;
            
            // 初始化辅助管理器
            InitializeManagers();
            
            SizeChanged += (_, e) => WindowSizeChanged?.Invoke(e);

            PluginManagerProxy.OnGroupBan += PluginManagerProxy_OnGroupBan;
            PluginManagerProxy.OnGroupAdded += PluginManagerProxy_OnGroupAdded;
            PluginManagerProxy.OnGroupMsg += PluginManagerProxy_OnGroupMsg;
            PluginManagerProxy.OnGroupLeft += PluginManagerProxy_OnGroupLeft;
            PluginManagerProxy.OnPrivateMsg += PluginManagerProxy_OnPrivateMsg;
            PluginManagerProxy.OnGroupMsgRecall += PluginManagerProxy_OnGroupMsgRecall;
            PluginManagerProxy.OnPrivateMsgRecall += PluginManagerProxy_OnPrivateMsgRecall;

            CQPImplementation.OnPrivateMessageSend += CQPImplementation_OnPrivateMessageSend;
            CQPImplementation.OnGroupMessageSend += CQPImplementation_OnGroupMessageSend;

            DataObject.AddPastingHandler(SendText, RichTextboxPasteOverrideAction);
            await LoadChatHistory();
            await ReorderChatList();
        }

        /// <summary>
        /// 将历史转换为消息模型
        /// </summary>
        /// <param name="avatarType">消息来源</param>
        /// <param name="history">聊天历史</param>
        /// <returns>消息模型</returns>
        private async Task<ChatDetailItemViewModel> ParseChatHistoryToViewModel(ChatAvatar.AvatarTypes avatarType, ChatHistory history)
        {
            return await _messageService.ParseHistoryAsync(history, avatarType);
        }

        private void PictureBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new()
            {
                AddExtension = true,
                CheckFileExists = true,
                CheckPathExists = true,
                Filter = "图片文件|*.jpg;*.jpeg;*.png;*.bmp;*.webp|所有文件|*.*",
                Title = "请选择要发送的图片"
            };
            if (openFileDialog.ShowDialog() is false)
            {
                return;
            }
            foreach (var file in openFileDialog.FileNames)
            {
                string filePath = file;
                string picPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"data\image\cached");
                Directory.CreateDirectory(picPath);
                if (filePath.StartsWith(picPath))
                {
                    // 选中图片已经存在于缓存文件夹
                    filePath = filePath.Replace(picPath, "");
                }
                else
                {
                    // 复制至缓存文件夹
                    string fileName = Path.GetFileName(filePath);
                    File.Copy(filePath, Path.Combine(picPath, fileName), true);
                    filePath = @$"cached\\{fileName}";
                }
                AddTextToSendBox(CQCode.CQCode_Image(filePath).ToSendString());
            }
        }

        private async void PluginManagerProxy_OnGroupAdded(long group, long qq)
        {
            await AddGroupChatItem(group, qq, $"{await GetGroupMemberNick(group, qq)} 加入了本群", DetailItemType.Notice, DateTime.Now);
        }

        private async void PluginManagerProxy_OnGroupBan(long group, long qq, long operatedQQ, long time)
        {
            await AddGroupChatItem(group, qq, $"{await GetGroupMemberNick(group, qq)} 禁言了 {await GetGroupMemberNick(group, operatedQQ)} {time}秒", DetailItemType.Notice, DateTime.Now);
        }

        private async void PluginManagerProxy_OnGroupLeft(long group, long qq)
        {
            await AddGroupChatItem(group, AppConfig.Instance.CurrentQQ, $"{await GetGroupMemberNick(group, qq)} 离开了群", DetailItemType.Notice, DateTime.Now);
        }

        private async void PluginManagerProxy_OnGroupMsg(int msgId, long group, long qq, string msg, DateTime time)
        {
            await AddGroupChatItem(group, qq, msg, DetailItemType.Receive, time, msgId);
            AddOrUpdateGroupChatList(group, qq, msg);
        }

        private void PluginManagerProxy_OnGroupMsgRecall(int msgId, long groupId, string msg)
        {
            MsgRecalled?.Invoke(msgId);
        }

        private async void PluginManagerProxy_OnPrivateMsg(int msgId, long qq, string msg, DateTime time)
        {
            await AddPrivateChatItem(qq, qq, msg, DetailItemType.Receive, time, msgId);
            AddOrUpdatePrivateChatList(qq, qq, msg);
        }

        private void PluginManagerProxy_OnPrivateMsgRecall(int msgId, long qq, string msg)
        {
            MsgRecalled?.Invoke(msgId);
        }

        /// <summary>
        /// 更新顶部显示的群名称
        /// </summary>
        private async Task RefreshGroupName()
        {
            GroupName = SelectedItem.AvatarType switch
            {
                ChatAvatar.AvatarTypes.QQGroup => await GetGroupName(SelectedItem.Id),
                ChatAvatar.AvatarTypes.QQPrivate => await GetFriendNick(SelectedItem.Id),
                _ => SelectedItem.Id.ToString(),
            };
            OnPropertyChanged(nameof(GroupName));
        }

        /// <summary>
        /// 更新或刷新消息容器
        /// </summary>
        /// <param name="refreshAll">是否清空后加载</param>
        private async Task RefreshMessageContainer(bool refreshAll)
        {
            if (SelectedItem == null || _messageContainerManager == null)
            {
                return;
            }
            if (refreshAll)
            {
                await RefreshGroupName();
                _messageContainerManager.ClearMessages();
            }

            var ls = DetailList.Skip(Math.Max(0, DetailList.Count - LoadCount)).ToList();
            foreach (var item in ls)
            {
                if (!CheckMessageContainerHasItem(item.GUID))
                {
                    AddItemToMessageContainer(item, isRemove: true);
                }
            }
            _messageContainerManager.ScrollToBottom();
        }

        /// <summary>
        /// 按时间重新排序左侧聊天列表
        /// </summary>
        private async Task ReorderChatList()
        {
            if (_viewModel == null) return;
            
            await Dispatcher.BeginInvoke(() =>
            {
                if (SelectedItem != null)
                {
                    SelectedItem.UnreadCount = 0;
                }
                var sortedList = _viewModel.ChatList.GroupBy(x => x.Id).Select(x => x.First()).OrderByDescending(x => x.Time).ToList();
                _viewModel.ChatList.Clear();
                foreach (var item in sortedList)
                {
                    _viewModel.ChatList.Add(item);
                }
                EmptyHint.Visibility = _viewModel.ChatList.Count > 0 ? Visibility.Collapsed : Visibility.Visible;
            });
        }

        /// <summary>
        /// 发送文本框响应粘贴事件
        /// </summary>
        private void RichTextboxPasteOverrideAction(object sender, DataObjectPastingEventArgs e)
        {
            RichTextBoxHelper.HandlePaste(e, SendText);
        }

        private void ScrollToBottomBtn_Click(object sender, RoutedEventArgs e)
        {
            _messageContainerManager?.ScrollToBottom(true);
            // 清理消息内容至数量以下
            _messageContainerManager?.RemoveOldMessages(UIConfig.Instance.MessageContainerMaxCount);
            _lazyLoadManager?.Reset();
        }

        private void SendBtn_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedItem == null)
            {
                return;
            }
            string sendText = BuildTextFromRichTextBox();
            ChatAvatar.AvatarTypes avatar = SelectedItem.AvatarType;
            long id = SelectedItem.Id;
            Task.Run(() =>
            {
                ExecuteSendMessage(id, avatar, sendText);
            });
            // 清空发送框
            RichTextBoxHelper.Clear(SendText);
        }

        private void SendText_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter && (Keyboard.Modifiers & ModifierKeys.Shift) != ModifierKeys.Shift)
            {
                e.Handled = true;
                SendBtn_Click(sender, e);
            }
            else if (e.Key == Key.D2 && (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
            {
                // 触发@
                e.Handled = true;
                AtBtn_Click(sender, e);
            }
        }

        /// <summary>
        /// 消息发送失败 显示重新发送按钮
        /// </summary>
        /// <param name="guid">目标GUID</param>
        private void UpdateSendFail(string? guid)
        {
            _messageContainerManager?.MarkSendFailed(guid);
        }

        /// <summary>
        /// 更新消息发送状态
        /// </summary>
        /// <param name="guid">消息GUID</param>
        /// <param name="enable">正在发送</param>
        private void UpdateSendStatus(string? guid, bool enable)
        {
            _messageContainerManager?.UpdateSendStatus(guid, enable);
        }

        /// <summary>
        /// 更新消息ID
        /// </summary>
        /// <param name="guid">消息GUID</param>
        /// <param name="msgId">消息ID</param>
        private void UpdateMessageId(string? guid, int msgId)
        {
            _messageContainerManager?.UpdateMessageId(guid, msgId);
        }

        /// <summary>
        /// 页面卸载事件处理，取消所有事件订阅以防止内存泄漏
        /// </summary>
        private void ChatPage_Unloaded(object sender, RoutedEventArgs e)
        {
            Dispose();
        }

        /// <summary>
        /// 释放资源，取消所有事件订阅
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            // 取消ViewModel事件订阅
            if (_viewModel != null)
            {
                _viewModel.SelectedChatItemChanged -= ViewModel_SelectedChatItemChanged;
                _viewModel.SendMessageRequested -= ViewModel_SendMessageRequested;
                _viewModel.ClearMessageRequested -= ViewModel_ClearMessageRequested;
                _viewModel.ClearSendBoxRequested -= ViewModel_ClearSendBoxRequested;
                _viewModel.ScrollToBottomRequested -= ViewModel_ScrollToBottomRequested;
            }

            // 取消PluginManagerProxy事件订阅
            PluginManagerProxy.OnGroupBan -= PluginManagerProxy_OnGroupBan;
            PluginManagerProxy.OnGroupAdded -= PluginManagerProxy_OnGroupAdded;
            PluginManagerProxy.OnGroupMsg -= PluginManagerProxy_OnGroupMsg;
            PluginManagerProxy.OnGroupLeft -= PluginManagerProxy_OnGroupLeft;
            PluginManagerProxy.OnPrivateMsg -= PluginManagerProxy_OnPrivateMsg;
            PluginManagerProxy.OnGroupMsgRecall -= PluginManagerProxy_OnGroupMsgRecall;
            PluginManagerProxy.OnPrivateMsgRecall -= PluginManagerProxy_OnPrivateMsgRecall;

            // 取消CQPImplementation事件订阅
            CQPImplementation.OnPrivateMessageSend -= CQPImplementation_OnPrivateMessageSend;
            CQPImplementation.OnGroupMessageSend -= CQPImplementation_OnGroupMessageSend;

            // 取消粘贴事件订阅
            if (SendText != null)
            {
                DataObject.RemovePastingHandler(SendText, RichTextboxPasteOverrideAction);
            }

            // 释放辅助管理器
            _lazyLoadManager?.Dispose();
            _messageContainerManager?.Dispose();

            // 取消Unloaded事件订阅
            Unloaded -= ChatPage_Unloaded;

            _disposed = true;
        }
    }
}