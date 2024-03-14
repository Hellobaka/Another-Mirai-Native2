using Another_Mirai_Native.Config;
using Another_Mirai_Native.DB;
using Another_Mirai_Native.Model;
using Another_Mirai_Native.Model.Enums;
using Another_Mirai_Native.Native;
using Another_Mirai_Native.UI.Controls;
using Another_Mirai_Native.UI.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Another_Mirai_Native.UI.Pages
{
    /// <summary>
    /// ChatPage.xaml 的交互逻辑
    /// </summary>
    public partial class ChatPage : Page, INotifyPropertyChanged
    {
        private object detailListLock = new object();

        public ChatPage()
        {
            InitializeComponent();
            DataContext = this;
            Instance = this;
            // TODO: 图片收藏功能
            // TODO: 消息引用显示
            // TODO: 解决表情如何显示
            // TODO: 实现功能按钮
            // TODO: 解决同一页面长时间挂机时内存溢出问题
            // TODO: 解决部分图片URL不规则，无法通过CQImg ID与URL对应的问题
            // TODO: 消息记录持久化，缓解内存压力
            // TODO: 添加清空按钮
        }

        public static event Action<int> MsgRecalled;

        public static event Action<SizeChangedEventArgs> WindowSizeChanged;

        public event PropertyChangedEventHandler PropertyChanged;

        public static ChatPage Instance { get; private set; }

        public List<ChatListItemViewModel> ChatList { get; set; } = new();

        public List<ChatDetailItemViewModel> DetailList { get; set; } = new();

        public string GroupName { get; set; } = "";

        private bool FormLoaded { get; set; }

        private Dictionary<long, List<ChatDetailItemViewModel>> FriendChatHistory { get; set; } = new();

        private Dictionary<long, FriendInfo> FriendInfoCache { get; set; } = new();

        private Dictionary<long, List<ChatDetailItemViewModel>> GroupChatHistory { get; set; } = new();

        private Dictionary<long, GroupInfo> GroupInfoCache { get; set; } = new();

        private Dictionary<long, Dictionary<long, GroupMemberInfo>> GroupMemberCache { get; set; } = new();

        private string LastMessageGUID { get; set; } = "";

        private Timer LazyLoadDebounceTimer { get; set; }

        private int LoadCount { get; set; } = 15;

        private ChatListItemViewModel SelectedItem => (ChatListItemViewModel)ChatListDisplay.SelectedItem;

        public void AddTextToSendBox(string text)
        {
            Dispatcher.BeginInvoke(() =>
            {
                SendText.Text += text;
            });
        }

        public int CallGroupMsgSend(long groupId, string message)
        {
            Stopwatch sw = Stopwatch.StartNew();
            int logId = LogHelper.WriteLog(LogLevel.InfoSend, "[↑]发送群组消息", $"群:{groupId} 消息:{message}", "处理中...");
            int msgId = ProtocolManager.Instance.CurrentProtocol.SendGroupMessage(groupId, message);
            sw.Stop();
            LogHelper.UpdateLogStatus(logId, $"√ {sw.ElapsedMilliseconds / 1000.0:f2} s");
            return msgId;
        }

        public int CallPrivateMsgSend(long qq, string message)
        {
            Stopwatch sw = Stopwatch.StartNew();
            int logId = LogHelper.WriteLog(LogLevel.InfoSend, "[↑]发送私聊消息", $"QQ:{qq} 消息:{message}", "处理中...");
            int msgId = ProtocolManager.Instance.CurrentProtocol.SendPrivateMessage(qq, message);
            sw.Stop();
            LogHelper.UpdateLogStatus(logId, $"√ {sw.ElapsedMilliseconds / 1000.0:f2} s");
            return msgId;
        }

        public void ExecuteSendMessage(long id, ChatAvatar.AvatarTypes avatar, string message)
        {
            if (id == 0 || string.IsNullOrEmpty(message))
            {
                return;
            }
            if (avatar == ChatAvatar.AvatarTypes.QQGroup)
            {
                AddGroupChatItem(id, AppConfig.Instance.CurrentQQ, message, DetailItemType.Send,
                    itemAdded: (guid) =>
                    {
                        UpdateSendStatus(guid, true);
                        if (CallGroupMsgSend(id, message) > 0)
                        {
                            UpdateSendStatus(guid, false);
                        }
                        else
                        {
                            UpdateSendFail(guid);
                        }
                    }
                );
            }
            else
            {
                AddPrivateChatItem(id, message, DetailItemType.Send,
                    itemAdded: (guid) =>
                    {
                        UpdateSendStatus(guid, true);
                        if (CallPrivateMsgSend(id, message) > 0)
                        {
                            UpdateSendStatus(guid, false);
                        }
                        else
                        {
                            UpdateSendFail(guid);
                        }
                    });
            }
        }

        public string GetFriendNick(long qq)
        {
            try
            {
                if (qq == AppConfig.Instance.CurrentQQ)
                {
                    return AppConfig.Instance.CurrentNickName;
                }
                if (FriendInfoCache.TryGetValue(qq, out var info))
                {
                    return info.Nick;
                }
                else
                {
                    var ls = ProtocolManager.Instance.CurrentProtocol.GetRawFriendList(false);
                    string r = qq.ToString();
                    foreach (var item in ls)
                    {
                        if (FriendInfoCache.ContainsKey(item.QQ))
                        {
                            FriendInfoCache[item.QQ] = item;
                        }
                        else
                        {
                            FriendInfoCache.Add(item.QQ, item);
                        }
                        if (item.QQ == qq)
                        {
                            r = item.Nick;
                        }
                    }
                    return r;
                }
            }
            catch
            {
                return qq.ToString();
            }
        }

        public string GetGroupMemberNick(long group, long qq)
        {
            try
            {
                if (qq == AppConfig.Instance.CurrentQQ)
                {
                    return AppConfig.Instance.CurrentNickName;
                }
                if (GroupMemberCache.TryGetValue(group, out var dict) && dict.TryGetValue(qq, out var info))
                {
                    return info.Nick;
                }
                else
                {
                    if (GroupMemberCache.ContainsKey(group) is false)
                    {
                        GroupMemberCache.Add(group, new Dictionary<long, GroupMemberInfo>());
                    }
                    if (GroupMemberCache[group].ContainsKey(qq) is false)
                    {
                        var memberInfo = ProtocolManager.Instance.CurrentProtocol.GetRawGroupMemberInfo(group, qq, false);
                        GroupMemberCache[group].Add(qq, memberInfo);
                    }
                    return GroupMemberCache[group][qq].Nick;
                }
            }
            catch
            {
                return qq.ToString();
            }
        }

        public string GetGroupName(long groupId)
        {
            try
            {
                if (GroupInfoCache.TryGetValue(groupId, out var info))
                {
                    return info.Name;
                }
                else
                {
                    GroupInfoCache.Add(groupId, ProtocolManager.Instance.CurrentProtocol.GetRawGroupInfo(groupId, false));
                    return GroupInfoCache[groupId].Name;
                }
            }
            catch
            {
                return groupId.ToString();
            }
        }

        public void UpdateUnreadCount(ChatListItemViewModel model)
        {
            var item = ChatList.FirstOrDefault(x => x.Id == model.Id && x.AvatarType == model.AvatarType);
            if (item != null)
            {
                item.UnreadCount = model.UnreadCount;
                ReorderChatList();
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private string? AddGroupChatItem(long group, long qq, string msg, DetailItemType itemType, int msgId = 0, Action<string> itemAdded = null, CQPluginProxy plugin = null)
        {
            ChatDetailItemViewModel item = null;
            string nick = GetGroupMemberNick(group, qq);
            if (plugin != null)
            {
                nick = $"{nick} [{plugin.PluginName}]";
            }
            if (GroupChatHistory.TryGetValue(group, out var chatHistory))
            {
                if (chatHistory.Count > AppConfig.Instance.MessageCacheSize)
                {
                    chatHistory.RemoveAt(0);
                }
                item = BuildChatDetailItem(msgId, qq, msg, nick, ChatAvatar.AvatarTypes.QQGroup, itemType);
                chatHistory.Add(item);
            }
            else
            {
                GroupChatHistory.Add(group, new());
                item = BuildChatDetailItem(msgId, qq, msg, nick, ChatAvatar.AvatarTypes.QQGroup, itemType);
                GroupChatHistory[group].Add(item);
            }
            OnPropertyChanged(nameof(DetailList));
            Dispatcher.BeginInvoke(() => RefreshMessageContainer(false));
            itemAdded?.Invoke(item?.GUID);
            return item?.GUID;
        }

        private void AddOrUpdateGroupChatList(long group, long qq, string msg)
        {
            var item = ChatList.FirstOrDefault(x => x.Id == group && x.AvatarType == ChatAvatar.AvatarTypes.QQGroup);
            if (item != null)
            {
                item.GroupName = GetGroupName(group);
                item.Detail = $"{GetGroupMemberNick(group, qq)}: {msg}";
                item.Time = DateTime.Now;
                item.UnreadCount++;
            }
            else
            {
                Dispatcher.BeginInvoke(() =>
                {
                    ChatList.Add(new ChatListItemViewModel
                    {
                        AvatarType = ChatAvatar.AvatarTypes.QQGroup,
                        Detail = $"{GetGroupMemberNick(group, qq)}: {msg}",
                        GroupName = GetGroupName(group),
                        Id = group,
                        Time = DateTime.Now,
                        UnreadCount = 1
                    });
                });
            }
        }

        private void AddOrUpdatePrivateChatList(long qq, string msg)
        {
            var item = ChatList.FirstOrDefault(x => x.Id == qq && x.AvatarType == ChatAvatar.AvatarTypes.QQPrivate);
            if (item != null)
            {
                item.GroupName = GetFriendNick(qq);
                item.Detail = msg;
                item.Time = DateTime.Now;
                item.UnreadCount++;
            }
            else
            {
                Dispatcher.BeginInvoke(() =>
                {
                    ChatList.Add(new ChatListItemViewModel
                    {
                        AvatarType = ChatAvatar.AvatarTypes.QQPrivate,
                        Detail = msg,
                        GroupName = GetFriendNick(qq),
                        Id = qq,
                        Time = DateTime.Now,
                        UnreadCount = 1
                    });
                });
            }
        }

        private string? AddPrivateChatItem(long qq, string msg, DetailItemType itemType, int msgId = 0, Action<string> itemAdded = null, CQPluginProxy plugin = null)
        {
            ChatDetailItemViewModel item = null;
            string nick = GetFriendNick(qq);
            if (plugin != null)
            {
                nick = $"{nick} [{plugin.PluginName}]";
            }
            if (FriendChatHistory.TryGetValue(qq, out var chatHistory))
            {
                if (chatHistory.Count > AppConfig.Instance.MessageCacheSize)
                {
                    chatHistory.RemoveAt(0);
                }
                item = BuildChatDetailItem(msgId, qq, msg, nick, ChatAvatar.AvatarTypes.QQPrivate, itemType);
                chatHistory.Add(item);
            }
            else
            {
                FriendChatHistory.Add(qq, new());
                item = BuildChatDetailItem(msgId, qq, msg, nick, ChatAvatar.AvatarTypes.QQPrivate, itemType);
                FriendChatHistory[qq].Add(item);
            }
            OnPropertyChanged(nameof(DetailList));
            Dispatcher.BeginInvoke(() =>
            {
                RefreshMessageContainer(false);
                itemAdded?.Invoke(item?.GUID);
            });
            return item?.GUID;
        }

        private void AtBtn_Click(object sender, RoutedEventArgs e)
        {
        }

        private void AudioBtn_Click(object sender, RoutedEventArgs e)
        {
        }

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

        private UIElement BuildLeftBlock(ChatDetailItemViewModel item)
        {
            return new ChatDetailListItem_Left()
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
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(0, 10, 0, 10),
                Recalled = item.Recalled,
            };
        }

        private UIElement BuildMiddleBlock(ChatDetailItemViewModel item)
        {
            return new ChatDetailListItem_Center()
            {
                Message = item.Content,
                DetailItemType = item.DetailItemType,
                GUID = item.GUID,
                MaxWidth = ActualWidth * 0.6,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 10, 0, 10)
            };
        }

        private UIElement BuildRightBlock(ChatDetailItemViewModel item)
        {
            return new ChatDetailListItem_Right()
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
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(0, 10, 0, 10),
                Recalled = item.Recalled,
            };
        }

        private void ChatListDisplay_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = SelectedItem;
            if (item == null)
            {
                return;
            }
            lock (detailListLock)
            {
                if (item.AvatarType == ChatAvatar.AvatarTypes.QQPrivate)
                {
                    if (FriendChatHistory.TryGetValue(item.Id, out var msg))
                    {
                        DetailList = msg;
                    }
                    else
                    {
                        FriendChatHistory.Add(item.Id, new());
                        DetailList = FriendChatHistory[item.Id];
                    }
                }
                else
                {
                    if (GroupChatHistory.TryGetValue(item.Id, out var msg))
                    {
                        DetailList = msg;
                    }
                    else
                    {
                        GroupChatHistory.Add(item.Id, new());
                        DetailList = GroupChatHistory[item.Id];
                    }
                }
            }
            OnPropertyChanged(nameof(DetailList));
            Dispatcher.BeginInvoke(() =>
            {
                item.UnreadCount = 0;
                UpdateUnreadCount(item);
                RefreshMessageContainer(true);
            });
        }

        private bool CheckMessageContainerHasItem(string guid)
        {
            foreach (UIElement item in MessageContainer.Children)
            {
                if (item is ChatDetailListItem_Center center && center.GUID == guid)
                {
                    return true;
                }
                else if (item is ChatDetailListItem_Right right && right.GUID == guid)
                {
                    return true;
                }
                else if (item is ChatDetailListItem_Left left && left.GUID == guid)
                {
                    return true;
                }
            }
            return false;
        }

        private void CQPImplementation_OnGroupMessageSend(int msgId, long group, string msg, CQPluginProxy plugin)
        {
            AddOrUpdateGroupChatList(group, AppConfig.Instance.CurrentQQ, msg);
            AddGroupChatItem(group, AppConfig.Instance.CurrentQQ, msg, DetailItemType.Send, msgId, plugin: plugin);
            ReorderChatList();
        }

        private void CQPImplementation_OnPrivateMessageSend(int msgId, long qq, string msg, CQPluginProxy plugin)
        {
            AddOrUpdatePrivateChatList(qq, msg);
            AddPrivateChatItem(qq, msg, DetailItemType.Send, msgId, plugin: plugin);
            ReorderChatList();
        }

        private void FaceBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void LazyLoad()
        {
            if (string.IsNullOrWhiteSpace(LastMessageGUID))
            {
                return;
            }
            int index = -1;
            for (int i = 0; i < DetailList.Count; i++)
            {
                var item = DetailList[i];
                if (item.GUID == LastMessageGUID)
                {
                    index = i;
                    break;
                }
            }
            IEnumerable<ChatDetailItemViewModel> ls;
            if (index <= 0)
            {
                return;
            }
            else
            {
                ls = index > 15 ? DetailList.Skip(Math.Max(0, index - LoadCount)).Take(LoadCount) : DetailList.Take(LoadCount);
            }
            if (ls == null || !ls.Any())
            {
                return;
            }
            LastMessageGUID = ls.First().GUID;

            UIElement lastElement;
            Dispatcher.BeginInvoke(() =>
            {
                FrameworkElement firstItem = (FrameworkElement)MessageContainer.Children[0];
                foreach (var item in ls.Reverse())
                {
                    if (!CheckMessageContainerHasItem(item.GUID))
                    {
                        lastElement = item.DetailItemType switch
                        {
                            DetailItemType.Notice => BuildMiddleBlock(item),
                            DetailItemType.Receive => BuildLeftBlock(item),
                            _ => BuildRightBlock(item),
                        };
                        MessageContainer.Children.Insert(0, lastElement);
                    }
                }

                firstItem.BringIntoView();
            });
        }

        private void MessageScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (sender is not ScrollViewer scrollViewer)
            {
                return;
            }
            double distanceToBottom = scrollViewer.ScrollableHeight - scrollViewer.VerticalOffset;
            double distanceToTop = scrollViewer.VerticalOffset;

            ScrollBottomContainer.Visibility = distanceToBottom > 100 ? Visibility.Visible : Visibility.Collapsed;

            if (distanceToTop < 100)
            {
                if (LazyLoadDebounceTimer == null)
                {
                    LazyLoadDebounceTimer = new Timer
                    {
                        Interval = 200,
                    };
                    LazyLoadDebounceTimer.Elapsed += (_, _) =>
                    {
                        LazyLoadDebounceTimer.Stop();
                        LazyLoad();
                    };
                    LazyLoadDebounceTimer.Start();
                }
                else
                {
                    LazyLoadDebounceTimer.Stop();
                    LazyLoadDebounceTimer.Start();
                }
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (FormLoaded)
            {
                return;
            }
            FormLoaded = true;
            if (UIConfig.Instance.ChatEnabled is false)
            {
                return;
            }
            SizeChanged += (_, e) => WindowSizeChanged?.Invoke(e);

            PluginManagerProxy.OnGroupBan += PluginManagerProxy_OnGroupBan;
            PluginManagerProxy.OnGroupAdded += PluginManagerProxy_OnGroupAdded;
            PluginManagerProxy.OnGroupMsg += PluginManagerProxy_OnGroupMsg;
            PluginManagerProxy.OnGroupLeft += PluginManagerProxy_OnGroupLeft;
            PluginManagerProxy.OnAdminChanged += PluginManagerProxy_OnAdminChanged;
            PluginManagerProxy.OnFriendAdded += PluginManagerProxy_OnFriendAdded;
            PluginManagerProxy.OnPrivateMsg += PluginManagerProxy_OnPrivateMsg;
            PluginManagerProxy.OnGroupMsgRecall += PluginManagerProxy_OnGroupMsgRecall;
            PluginManagerProxy.OnPrivateMsgRecall += PluginManagerProxy_OnPrivateMsgRecall;

            CQPImplementation.OnPrivateMessageSend += CQPImplementation_OnPrivateMessageSend;
            CQPImplementation.OnGroupMessageSend += CQPImplementation_OnGroupMessageSend;
        }

        private void PictureBtn_Click(object sender, RoutedEventArgs e)
        {
        }

        private void PluginManagerProxy_OnAdminChanged(long group, long qq, QQGroupMemberType type)
        {
            if (GroupMemberCache.TryGetValue(group, out var dict) && dict.TryGetValue(qq, out var info))
            {
                if (info.MemberType != QQGroupMemberType.Creator)
                {
                    info.MemberType = type;
                }
                else
                {
                    // 群主 未定义操作
                }
            }
        }

        private void PluginManagerProxy_OnFriendAdded(long qq)
        {
            GetFriendNick(qq);
            // 额外实现
        }

        private void PluginManagerProxy_OnGroupAdded(long group, long qq)
        {
            if (GroupMemberCache.TryGetValue(group, out var dict) && dict.ContainsKey(qq))
            {
                AddGroupChatItem(group, qq, $"{GetGroupMemberNick(group, qq)} 加入了本群", DetailItemType.Notice);
            }
        }

        private void PluginManagerProxy_OnGroupBan(long group, long qq, long operatedQQ, long time)
        {
            if (GroupMemberCache.TryGetValue(group, out var dict) && dict.ContainsKey(qq))
            {
                AddGroupChatItem(group, qq, $"{GetGroupMemberNick(group, qq)} 禁言了 {GetGroupMemberNick(group, operatedQQ)} {time}秒", DetailItemType.Notice);
            }
        }

        private void PluginManagerProxy_OnGroupLeft(long group, long qq)
        {
            if (GroupMemberCache.TryGetValue(group, out var dict) && dict.ContainsKey(qq))
            {
                dict.Remove(qq);
                AddGroupChatItem(group, qq, $"{GetGroupMemberNick(group, qq)}离开了群", DetailItemType.Notice);
            }
        }

        private void PluginManagerProxy_OnGroupMsg(int msgId, long group, long qq, string msg)
        {
            AddGroupChatItem(group, qq, msg, DetailItemType.Receive, msgId);
            AddOrUpdateGroupChatList(group, qq, msg);
            ReorderChatList();
        }

        private void PluginManagerProxy_OnGroupMsgRecall(int msgId, long groupId, string msg)
        {
            if (GroupChatHistory.TryGetValue(groupId, out var list))
            {
                var item = list.FirstOrDefault(x => x.MsgId == msgId);
                if (item != null)
                {
                    item.Recalled = true;
                }
            }
            MsgRecalled?.Invoke(msgId);
        }

        private void PluginManagerProxy_OnPrivateMsg(int msgId, long qq, string msg)
        {
            AddPrivateChatItem(qq, msg, DetailItemType.Receive, msgId);

            AddOrUpdatePrivateChatList(qq, msg);
            ReorderChatList();
        }

        private void PluginManagerProxy_OnPrivateMsgRecall(int msgId, long qq, string msg)
        {
            if (FriendChatHistory.TryGetValue(qq, out var list))
            {
                var item = list.FirstOrDefault(x => x.MsgId == msgId);
                if (item != null)
                {
                    item.Recalled = true;
                }
            }
            MsgRecalled?.Invoke(msgId);
        }

        private void RefreshGroupName()
        {
            GroupName = SelectedItem.AvatarType switch
            {
                ChatAvatar.AvatarTypes.QQGroup => GetGroupName(SelectedItem.Id),
                ChatAvatar.AvatarTypes.QQPrivate => GetFriendNick(SelectedItem.Id),
                _ => SelectedItem.Id.ToString(),
            };
            OnPropertyChanged(nameof(GroupName));
        }

        private void RefreshMessageContainer(bool refreshAll)
        {
            if (SelectedItem == null)
            {
                return;
            }
            if (refreshAll)
            {
                RefreshGroupName();
                MessageContainer.Children.Clear();
                LastMessageGUID = "";
                GC.Collect();
            }

            lock (detailListLock)
            {
                var ls = DetailList.Skip(Math.Max(0, DetailList.Count - LoadCount)).ToList();
                if (string.IsNullOrEmpty(LastMessageGUID))
                {
                    LastMessageGUID = ls.First().GUID;
                }
                foreach (var item in ls)
                {
                    if (!CheckMessageContainerHasItem(item.GUID))
                    {
                        switch (item.DetailItemType)
                        {
                            case DetailItemType.Notice:
                                MessageContainer.Children.Add(BuildMiddleBlock(item));
                                break;

                            case DetailItemType.Receive:
                                MessageContainer.Children.Add(BuildLeftBlock(item));
                                break;

                            default:
                            case DetailItemType.Send:
                                MessageContainer.Children.Add(BuildRightBlock(item));
                                break;
                        }
                    }
                }
            }
            ScrollToBottom(MessageScrollViewer);
        }

        private void ReorderChatList()
        {
            Dispatcher.BeginInvoke(() =>
            {
                if (SelectedItem != null)
                {
                    SelectedItem.UnreadCount = 0;
                }
                ChatList = ChatList.Distinct().ToList();
                ChatList = ChatList.OrderByDescending(x => x.Time).ToList();
                OnPropertyChanged(nameof(ChatList));

                EmptyHint.Visibility = ChatList.Count > 0 ? Visibility.Collapsed : Visibility.Visible;
            });
        }

        private void ScrollToBottom(ScrollViewer scrollViewer, bool forced = false)
        {
            if (!forced && ScrollBottomContainer.Visibility == Visibility.Visible)
            {
                return;
            }
            scrollViewer.ScrollToBottom();
        }

        private void ScrollToBottomBtn_Click(object sender, RoutedEventArgs e)
        {
            ScrollToBottom(MessageScrollViewer, true);
        }

        private void SendBtn_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedItem == null)
            {
                return;
            }
            string sendText = SendText.Text;
            ChatAvatar.AvatarTypes avatar = SelectedItem.AvatarType;
            long id = SelectedItem.Id;
            Task.Run(() =>
            {
                ExecuteSendMessage(id, avatar, sendText);
            });
            SendText.Text = "";
        }

        private void SendText_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter && (Keyboard.Modifiers & ModifierKeys.Shift) != ModifierKeys.Shift)
            {
                e.Handled = true;
                SendBtn_Click(sender, e);
            }
        }

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
                    if (item is ChatDetailListItem_Right right && right.GUID == guid)
                    {
                        right.SendFail();
                        return;
                    }
                }
            });
        }

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
                    if (item is ChatDetailListItem_Right right && right.GUID == guid)
                    {
                        right.UpdateSendStatus(enable);
                        return;
                    }
                }
            });
        }
    }
}