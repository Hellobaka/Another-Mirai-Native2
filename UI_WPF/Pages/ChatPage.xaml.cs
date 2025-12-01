using Another_Mirai_Native.Config;
using Another_Mirai_Native.DB;
using Another_Mirai_Native.Model;
using Another_Mirai_Native.Model.Enums;
using Another_Mirai_Native.Native;
using Another_Mirai_Native.UI.Controls;
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
    public partial class ChatPage : Page, INotifyPropertyChanged
    {
        public ChatPage()
        {
            InitializeComponent();
            ViewModel = new ChatViewModel();
            DataContext = ViewModel;
            Instance = this;
            Page_Loaded(null, null);
        }

        public ChatViewModel ViewModel { get; set; }

        /// <summary>
        /// 框架撤回消息事件, 各个消息块均订阅此事件
        /// </summary>
        public static event Action<int> MsgRecalled;

        /// <summary>
        /// 主窗体尺寸变化事件, 各个消息块均订阅此事件
        /// </summary>

        public static event Action<SizeChangedEventArgs> WindowSizeChanged;

        public event PropertyChangedEventHandler? PropertyChanged;

        public static ChatPage Instance { get; private set; }

        /// <summary>
        /// 当前选择项的组名称
        /// </summary>
        public string GroupName { get; set; } = "";

        /// <summary>
        /// 窗体加载完成事件
        /// </summary>
        private bool FormLoaded { get; set; }

        /// <summary>
        /// 懒加载防抖时钟
        /// </summary>
        private DispatcherTimer LazyLoadDebounceTimer { get; set; }

        /// <summary>
        /// 是否在懒加载中
        /// </summary>
        private bool LazyLoading { get; set; }

        /// <summary>
        /// 初始化加载时的消息数量
        /// </summary>
        private int LoadCount { get; set; } = 15;


        /// <summary>
        /// 调用发送群消息
        /// </summary>
        /// <param name="groupId">发送的群</param>
        /// <param name="message">发送的消息</param>
        /// <returns>返回的消息ID, 不为0时为成功</returns>
        public int CallGroupMsgSend(long groupId, string message)
        {
            Stopwatch sw = Stopwatch.StartNew();
            int logId = LogHelper.WriteLog(LogLevel.InfoSend, "[↑]发送群组消息", $"群:{groupId} 消息:{message}", "处理中...");
            int msgId = ProtocolManager.Instance.CurrentProtocol.SendGroupMessage(groupId, message);
            sw.Stop();
            LogHelper.UpdateLogStatus(logId, $"√ {sw.ElapsedMilliseconds / 1000.0:f2} s");
            return msgId;
        }

        /// <summary>
        /// 调用发送好友消息
        /// </summary>
        /// <param name="qq">发送的好友</param>
        /// <param name="message">发送的消息</param>
        /// <returns>返回的消息ID, 不为0时为成功</returns>
        public int CallPrivateMsgSend(long qq, string message)
        {
            Stopwatch sw = Stopwatch.StartNew();
            int logId = LogHelper.WriteLog(LogLevel.InfoSend, "[↑]发送私聊消息", $"QQ:{qq} 消息:{message}", "处理中...");
            int msgId = ProtocolManager.Instance.CurrentProtocol.SendPrivateMessage(qq, message);
            sw.Stop();
            LogHelper.UpdateLogStatus(logId, $"√ {sw.ElapsedMilliseconds / 1000.0:f2} s");
            return msgId;
        }

        /// <summary>
        /// 添加消息块并调用发送
        /// </summary>
        /// <param name="id">发送对象</param>
        /// <param name="avatar">发送对象的类型</param>
        /// <param name="message">发送的消息</param>
        public async void ExecuteSendMessage(long id, ChatAvatar.AvatarTypes avatar, string message)
        {
            if (id == 0 || string.IsNullOrEmpty(message))
            {
                return;
            }
            int sendMsgId = 0, sqlId = 0;
            ManualResetEvent sendSignal = new(false);
            var history = new ChatHistory
            {
                Message = message,
                ParentID = id,
                SenderID = AppConfig.Instance.CurrentQQ,
                Type = avatar == ChatAvatar.AvatarTypes.QQGroup ? ChatHistoryType.Group : ChatHistoryType.Private,
                MsgId = 0,
                PluginName = "",
                Time = DateTime.Now,
            };
            sqlId = ChatHistoryHelper.InsertHistory(history);

            if (avatar == ChatAvatar.AvatarTypes.QQGroup)
            {
                await AddGroupChatItem(id, AppConfig.Instance.CurrentQQ, message, DetailItemType.Send, DateTime.Now,
                    itemAdded: (guid) =>
                    {
                        UpdateSendStatus(guid, true);
                        int msgId = CallGroupMsgSend(id, message);
                        if (msgId != 0)
                        {
                            sendMsgId = msgId;
                            UpdateSendStatus(guid, false);
                            UpdateMessageId(guid, sendMsgId);
                        }
                        else
                        {
                            UpdateSendFail(guid);
                        }
                        sendSignal.Set();
                    }
                );
            }
            else
            {
                await AddPrivateChatItem(id, AppConfig.Instance.CurrentQQ, message, DetailItemType.Send, DateTime.Now,
                     itemAdded: (guid) =>
                     {
                         UpdateSendStatus(guid, true);
                         int msgId = CallPrivateMsgSend(id, message);
                         if (msgId != 0)
                         {
                             sendMsgId = msgId;
                             UpdateSendStatus(guid, false);
                             UpdateMessageId(guid, sendMsgId);
                         }
                         else
                         {
                             UpdateSendFail(guid);
                         }
                         sendSignal.Set();
                     });
            }
            sendSignal.WaitOne();
            if (sendMsgId != 0)
            {
                ChatHistoryHelper.UpdateHistoryMessageId(id, avatar == ChatAvatar.AvatarTypes.QQGroup ? ChatHistoryType.Group : ChatHistoryType.Private
                    , sqlId, sendMsgId);
            }
        }


        /// <summary>
        /// 更新目标消息历史的未读消息数量
        /// </summary>
        /// <param name="model"></param>
        public async void UpdateUnreadCount(ChatListItemViewModel model)
        {
            var item = ViewModel.ChatList.FirstOrDefault(x => x.Id == model.Id && x.AvatarType == model.AvatarType);
            if (item != null)
            {
                item.UnreadCount = model.UnreadCount;
                await ReorderChatList();
            }
        }

        /// <summary>
        /// MVVM
        /// </summary>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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
        private async Task<int> AddGroupChatItem(long group, long qq, string msg, DetailItemType itemType, DateTime time, int msgId = 0, Action<string>? itemAdded = null, CQPluginProxy? plugin = null)
        {
            string nick = await GetGroupMemberNick(group, qq);
            if (nick != null && plugin != null)
            {
                nick = $"{nick} [{plugin.PluginName}]";
            }
            MessageViewModel item = BuildChatDetailItem(msgId, qq, msg, nick!, ChatAvatar.AvatarTypes.QQGroup, itemType);
            var history = ChatHistoryHelper.GetHistoriesByMsgId(group, msgId, ChatHistoryType.Group);
            AddOrUpdateGroupChatList(group, qq, msg);
            await Dispatcher.BeginInvoke(() =>
            {
                if (ViewModel.SelectedChat?.Id == group)
                {
                    AddItemToMessageContainer(item, true);
                    ScrollToBottom(MessageScrollViewer, qq == AppConfig.Instance.CurrentQQ);
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
        private void AddItemToMessageContainer(MessageViewModel item, bool isRemove)
        {
            MessageContainer.Children.Add(BuildChatDetailItem(item));
            DetailList.Add(item);

            if (isRemove && MessageContainer.Children.Count > UIConfig.Instance.MessageContainerMaxCount
                && MessageScrollViewer.VerticalOffset > 100)// 数量超过30，且滚动条不在懒加载区
            {
                do
                {
                    MessageContainer.Children.RemoveAt(0);
                    DetailList.RemoveAt(0);
                    CurrentPageIndex = 1;
                } while (MessageContainer.Children.Count > UIConfig.Instance.MessageContainerMaxCount);
            }
        }

        /// <summary>
        /// 添加或更新左侧聊天列表群内容
        /// </summary>
        /// <param name="id">群ID</param>
        /// <param name="qq">发送者ID</param>
        /// <param name="msg">消息</param>
        private async void AddOrUpdateGroupChatList(long id, long qq, string msg)
        {
            msg = msg.Replace("\r", "").Replace("\n", "");
            var item = ViewModel.ChatList.FirstOrDefault(x => x.Id == id && x.AvatarType == ChatAvatar.AvatarTypes.QQGroup);
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
                    ViewModel.ChatList.Add(new ChatListItemViewModel
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
            msg = msg.Replace("\r", "").Replace("\n", "");
            var item = ViewModel.ChatList.FirstOrDefault(x => x.Id == qq && x.AvatarType == ChatAvatar.AvatarTypes.QQPrivate);
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
                     ViewModel.ChatList.Add(new ChatListItemViewModel
                     {
                         AvatarType = ChatAvatar.AvatarTypes.QQPrivate,
                         Detail = msg,
                         GroupName = await GetFriendNick(qq),
                         Id = sender,
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
        private async Task<int> AddPrivateChatItem(long qq, long sender, string msg, DetailItemType itemType, DateTime time, int msgId = 0, Action<string>? itemAdded = null, CQPluginProxy? plugin = null)
        {
            string nick = await GetFriendNick(sender);
            if (nick != null && plugin != null)
            {
                nick = $"{nick} [{plugin.PluginName}]";
            }
            MessageViewModel item = BuildChatDetailItem(msgId, sender, msg, nick!, ChatAvatar.AvatarTypes.QQPrivate, itemType);
            var history = ChatHistoryHelper.GetHistoriesByMsgId(qq, msgId, ChatHistoryType.Private);
            ChatHistoryHelper.UpdateHistoryCategory(history);
            AddOrUpdatePrivateChatList(qq, sender, msg);
            await Dispatcher.BeginInvoke(() =>
            {
                if (ViewModel.SelectedChat?.Id == qq)
                {
                    AddItemToMessageContainer(item, true);
                    ScrollToBottom(MessageScrollViewer, sender == AppConfig.Instance.CurrentQQ);
                }
            });
            itemAdded?.Invoke(item.GUID);
            return history?.ID ?? 0;
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
        private MessageViewModel BuildChatDetailItem(int msgId, long qq, string msg, string nick, ChatAvatar.AvatarTypes avatarType, DetailItemType itemType)
        {
            return new MessageViewModel
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
        private UIElement BuildChatDetailItem(MessageViewModel item)
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
                ParentId = ViewModel.SelectedChat.Id,
                MsgId = item.MsgId,
                GUID = item.GUID,
                MaxWidth = ActualWidth * 0.6,
                HorizontalAlignment = alignment,
                Margin = new Thickness(0, 10, 0, 10),
                Recalled = item.Recalled,
            };
        }

        /// <summary>
        /// 左侧列表选中变化
        /// </summary>
        private async void ChatListDisplay_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = ViewModel.SelectedChat;
            if (item == null)
            {
                return;
            }
            FaceBtn.IsEnabled = true;
            PictureBtn.IsEnabled = true;
            AudioBtn.IsEnabled = true;
            CleanMessageBtn.IsEnabled = true;
            SendText.IsEnabled = true;
            CleanSendBtn.IsEnabled = true;
            SendBtn.IsEnabled = true;

            if (ViewModel.SelectedChat.AvatarType == ChatAvatar.AvatarTypes.QQGroup)
            {
                AtBtn.IsEnabled = true;
            }
            else if (ViewModel.SelectedChat.AvatarType == ChatAvatar.AvatarTypes.QQPrivate)
            {
                // 私聊时禁用At按钮
                AtBtn.IsEnabled = false;
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
            CurrentPageIndex = 1;
            OnPropertyChanged(nameof(DetailList));
            item.UnreadCount = 0;
            GroupNameDisplay.Text = $"{ViewModel.SelectedChat.GroupName} [{ViewModel.SelectedChat.Id}]";
            UpdateUnreadCount(item);
            await RefreshMessageContainer(true);
        }

        /// <summary>
        /// 检测GUID是否已存在于消息容器中
        /// </summary>
        private bool CheckMessageContainerHasItem(string guid)
        {
            foreach (UIElement item in MessageContainer.Children)
            {
                if (item is ChatDetailListItem detail && detail.GUID == guid)
                {
                    return true;
                }
            }
            return false;
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
        /// 加载左侧聊天列表
        /// </summary>
        /// <returns></returns>
        private async Task LoadChatHistory()
        {
            var list = ChatHistoryHelper.GetHistoryCategroies();
            ViewModel.ChatList.Clear();
            foreach (var item in list)
            {
                ViewModel.ChatList.Add(new ChatListItemViewModel
                {
                    AvatarType = item.Type == ChatHistoryType.Private ? ChatAvatar.AvatarTypes.QQPrivate : ChatAvatar.AvatarTypes.QQGroup,
                    Detail = item.Message,
                    GroupName = item.Type == ChatHistoryType.Private ? await GetFriendNick(item.ParentID) : await GetGroupName(item.ParentID),
                    Id = item.ParentID,
                    Time = item.Time,
                    UnreadCount = 0
                });
            }
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
                if (ViewModel.SelectedChat == null && ViewModel.ChatList.Count > 0)
                {
                    // 当没有内容被选中时 选中第一项
                    await Dispatcher.Yield();
                    ViewModel.SelectedChat = ViewModel.ChatList.First();
                }
                return;
            }
            FormLoaded = true;
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

            await LoadChatHistory();
            await ReorderChatList();
        }

        private void PictureBtn_Click(object sender, RoutedEventArgs e)
        {
        }

        private async void PluginManagerProxy_OnGroupAdded(long group, long qq)
        {
            if (GroupMemberCache.TryGetValue(group, out var dict) && dict.ContainsKey(qq))
            {
                await AddGroupChatItem(group, qq, $"{await GetGroupMemberNick(group, qq)} 加入了本群", DetailItemType.Notice, DateTime.Now);
            }
        }

        private async void PluginManagerProxy_OnGroupBan(long group, long qq, long operatedQQ, long time)
        {
            if (GroupMemberCache.TryGetValue(group, out var dict) && dict.ContainsKey(qq))
            {
                await AddGroupChatItem(group, qq, $"{await GetGroupMemberNick(group, qq)} 禁言了 {await GetGroupMemberNick(group, operatedQQ)} {time}秒", DetailItemType.Notice, DateTime.Now);
            }
        }

        private async void PluginManagerProxy_OnGroupLeft(long group, long qq)
        {
            if (GroupMemberCache.TryGetValue(group, out var dict) && dict.ContainsKey(qq))
            {
                await AddGroupChatItem(group, AppConfig.Instance.CurrentQQ, $"{await GetGroupMemberNick(group, qq)} 离开了群", DetailItemType.Notice, DateTime.Now);
                dict.Remove(qq);
            }
            else
            {
                await AddGroupChatItem(group, AppConfig.Instance.CurrentQQ, $"{qq} 离开了群", DetailItemType.Notice, DateTime.Now);
            }
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
            GroupName = ViewModel.SelectedChat.AvatarType switch
            {
                ChatAvatar.AvatarTypes.QQGroup => await GetGroupName(ViewModel.SelectedChat.Id),
                ChatAvatar.AvatarTypes.QQPrivate => await GetFriendNick(ViewModel.SelectedChat.Id),
                _ => ViewModel.SelectedChat.Id.ToString(),
            };
            OnPropertyChanged(nameof(GroupName));
        }

        /// <summary>
        /// 更新或刷新消息容器
        /// </summary>
        /// <param name="refreshAll">是否清空后加载</param>
        private async Task RefreshMessageContainer(bool refreshAll)
        {
            if (ViewModel.SelectedChat == null)
            {
                return;
            }
            if (refreshAll)
            {
                await RefreshGroupName();
                MessageContainer.Children.Clear();
            }

            var ls = DetailList.Skip(Math.Max(0, DetailList.Count - LoadCount)).ToList();
            foreach (var item in ls)
            {
                if (!CheckMessageContainerHasItem(item.GUID))
                {
                    AddItemToMessageContainer(item, isRemove: true);
                }
            }
            ScrollToBottom(MessageScrollViewer);
        }

        /// <summary>
        /// 按时间重新排序左侧聊天列表
        /// </summary>
        private async Task ReorderChatList()
        {
            await Dispatcher.BeginInvoke(() =>
            {
                if (ViewModel.SelectedChat != null)
                {
                    ViewModel.SelectedChat.UnreadCount = 0;
                }
                //ViewModel.ChatList = ViewModel.ChatList.GroupBy(x => x.Id).Select(x => x.First()).OrderByDescending(x => x.Time).ToList();
                OnPropertyChanged(nameof(ViewModel.ChatList));
            });
        }

        /// <summary>
        /// 消息发送失败 显示重新发送按钮
        /// </summary>
        /// <param name="guid">目标GUID</param>
        private void UpdateSendFail(string? guid)
        {
            if (string.IsNullOrEmpty(guid))
            {
                return;
            }
            Dispatcher.BeginInvoke(() =>
            {
                foreach (UIElement item in MessageContainer.Children)
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
        /// 更新消息发送状态
        /// </summary>
        /// <param name="guid">消息GUID</param>
        /// <param name="enable">正在发送</param>
        private void UpdateSendStatus(string? guid, bool enable)
        {
            if (string.IsNullOrEmpty(guid))
            {
                return;
            }
            Dispatcher.BeginInvoke(() =>
            {
                foreach (UIElement item in MessageContainer.Children)
                {
                    if (item is ChatDetailListItem detail && detail.GUID == guid)
                    {
                        detail.UpdateSendStatus(enable);
                        return;
                    }
                }
            });
        }

        /// <summary>
        /// 更新消息ID
        /// </summary>
        /// <param name="guid">消息GUID</param>
        /// <param name="msgId">消息ID</param>
        private void UpdateMessageId(string? guid, int msgId)
        {
            if (string.IsNullOrEmpty(guid))
            {
                return;
            }
            Dispatcher.BeginInvoke(() =>
            {
                foreach (UIElement item in MessageContainer.Children)
                {
                    if (item is ChatDetailListItem detail && detail.GUID == guid)
                    {
                        detail.UpdateMessageId(msgId);
                        return;
                    }
                }
            });
        }
    }
}