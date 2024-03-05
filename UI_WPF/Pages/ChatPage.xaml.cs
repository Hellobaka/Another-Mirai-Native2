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
            // TODO: 发送更新左侧列表、修复发送时名片无法正确获取的bug
            // TODO: 实现右键菜单
            // TOOD: 实现图片双击预览
            // TODO: 修复切换左侧列表时重复获取好友列表的bug
            // TODO: URL变为超链接？
            // TODO: 图片收藏功能
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

        public string GetFriendNick(long qq)
        {
            try
            {
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

        private string? AddGroupChatItem(long group, long qq, string msg, DetailItemType itemType, int msgId = 0, Action<string> itemAdded = null)
        {
            ChatDetailItemViewModel item = null;
            if (GroupChatHistory.TryGetValue(group, out var chatHistory))
            {
                if (chatHistory.Count > AppConfig.Instance.MessageCacheSize)
                {
                    chatHistory.RemoveAt(0);
                }
                item = BuildChatDetailItem(msgId, qq, msg, GetGroupMemberNick(group, qq), ChatAvatar.AvatarTypes.QQGroup, itemType);
                chatHistory.Add(item);
            }
            else
            {
                GroupChatHistory.Add(group, new());
                item = BuildChatDetailItem(msgId, qq, msg, GetGroupMemberNick(group, qq), ChatAvatar.AvatarTypes.QQGroup, itemType);
                GroupChatHistory[group].Add(item);
            }
            OnPropertyChanged(nameof(DetailList));
            Dispatcher.BeginInvoke(() => RefreshMessageContainer(false));
            itemAdded?.Invoke(item?.GUID);
            return item?.GUID;
        }

        private string? AddPrivateChatItem(long qq, string msg, DetailItemType itemType, int msgId = 0, Action<string> itemAdded = null)
        {
            ChatDetailItemViewModel item = null;
            if (FriendChatHistory.TryGetValue(qq, out var chatHistory))
            {
                if (chatHistory.Count > AppConfig.Instance.MessageCacheSize)
                {
                    chatHistory.RemoveAt(0);
                }
                item = BuildChatDetailItem(msgId, qq, msg, GetFriendNick(qq), ChatAvatar.AvatarTypes.QQPrivate, itemType);
                chatHistory.Add(item);
            }
            else
            {
                FriendChatHistory.Add(qq, new());
                item = BuildChatDetailItem(msgId, qq, msg, GetFriendNick(qq), ChatAvatar.AvatarTypes.QQPrivate, itemType);
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
                AvatarType = item.AvatarType,
                DisplayName = item.Nick,
                Time = item.Time,
                Id = item.Id,
                GroupId = SelectedItem.Id,
                MsgId = item.MsgId,
                GUID = item.GUID,
                MaxWidth = ActualWidth * 0.6,
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(0, 10, 0, 10)
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
                AvatarType = item.AvatarType,
                DisplayName = item.Nick,
                Time = item.Time,
                Id = item.Id,
                GroupId = SelectedItem.Id,
                MsgId = item.MsgId,
                GUID = item.GUID,
                MaxWidth = ActualWidth * 0.6,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(0, 10, 0, 10)
            };
        }

        private void ChatListDisplay_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = SelectedItem;
            lock (detailListLock)
            {
                if (item != null)
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
            }
            OnPropertyChanged(nameof(DetailList));
            Dispatcher.BeginInvoke(() =>
            {
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

        private void FaceBtn_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedItem != null)
            {
                for (int i = 0; i < 50; i++)
                {
                    PluginManagerProxy_OnGroupMsg(i + 1, SelectedItem.Id, 1145141919, i.ToString());
                }
            }
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
            AddGroupChatItem(group, qq, msg, DetailItemType.Receive);
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
            ReorderChatList();
        }

        private void PluginManagerProxy_OnGroupMsgRecall(int msgId, long groupId, string msg)
        {
            MsgRecalled?.Invoke(msgId);
        }

        private void PluginManagerProxy_OnPrivateMsg(int msgId, long qq, string msg)
        {
            AddPrivateChatItem(qq, msg, DetailItemType.Receive);

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
            ReorderChatList();
        }

        private void PluginManagerProxy_OnPrivateMsgRecall(int msgId, long qq, string msg)
        {
            MsgRecalled?.Invoke(msgId);
        }

        private void RefreshGroupName()
        {
            switch (SelectedItem.AvatarType)
            {
                case ChatAvatar.AvatarTypes.QQGroup:
                    GroupName = GetGroupName(SelectedItem.Id);
                    break;

                case ChatAvatar.AvatarTypes.QQPrivate:
                    GroupName = GetFriendNick(SelectedItem.Id);
                    break;

                default:
                case ChatAvatar.AvatarTypes.Fallback:
                    GroupName = SelectedItem.Id.ToString();
                    break;
            }
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
                var ls = DetailList.Skip(Math.Max(0, DetailList.Count - LoadCount));
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
                if (avatar == ChatAvatar.AvatarTypes.QQPrivate)
                {
                    AddPrivateChatItem(id, sendText, DetailItemType.Send,
                        itemAdded: (guid) =>
                        {
                            UpdateSendStatus(guid, true);
                            if (CallPrivateMsgSend(id, sendText) > 0)
                            {
                                UpdateSendStatus(guid, false);
                            }
                            else
                            {
                                UpdateSendFail(guid);
                            }
                        });
                }
                else
                {
                    AddGroupChatItem(id, AppConfig.Instance.CurrentQQ, sendText, DetailItemType.Send,
                        itemAdded: (guid) =>
                        {
                            UpdateSendStatus(guid, true);
                            if (CallGroupMsgSend(id, sendText) > 0)
                            {
                                UpdateSendStatus(guid, false);
                            }
                            else
                            {
                                UpdateSendFail(guid);
                            }
                        });
                }
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
            foreach (UIElement item in MessageContainer.Children)
            {
                if (item is ChatDetailListItem_Right right && right.GUID == guid)
                {
                    right.SendFail();
                    return;
                }
            }
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