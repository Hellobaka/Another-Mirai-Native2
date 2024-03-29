﻿using Another_Mirai_Native.Config;
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
using System.Threading.Tasks;
using System.Timers;
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
        private object detailListLock = new object();

        public ChatPage()
        {
            InitializeComponent();
            DataContext = this;
            Instance = this;
            // TODO: 图片收藏功能
            // TODO: 消息引用显示
            // TODO: 实现功能按钮
        }

        public static event Action<int> MsgRecalled;

        public static event Action<SizeChangedEventArgs> WindowSizeChanged;

        public event PropertyChangedEventHandler PropertyChanged;

        public static ChatPage Instance { get; private set; }

        public List<ChatListItemViewModel> ChatList { get; set; } = new();

        public List<ChatDetailItemViewModel> DetailList { get; set; } = new();

        public string GroupName { get; set; } = "";

        private int CurrentPageIndex { get; set; }

        private bool FormLoaded { get; set; }

        private Dictionary<long, FriendInfo> FriendInfoCache { get; set; } = new();

        private Dictionary<long, GroupInfo> GroupInfoCache { get; set; } = new();

        private Dictionary<long, Dictionary<long, GroupMemberInfo>> GroupMemberCache { get; set; } = new();

        private DispatcherTimer LazyLoadDebounceTimer { get; set; }

        private bool LazyLoading { get; set; }

        private int LoadCount { get; set; } = 15;

        private ChatListItemViewModel SelectedItem => (ChatListItemViewModel)ChatListDisplay.SelectedItem;

        public void AddTextToSendBox(string text)
        {
            Dispatcher.BeginInvoke(() =>
            {
                TextPointer startPosition = SendText.Document.ContentStart;
                int start = startPosition.GetOffsetToPosition(SendText.CaretPosition);
                SendText.CaretPosition.InsertTextInRun(text);
                SendText.CaretPosition = startPosition.GetPositionAtOffset(start + text.Length);
                SendText.Focus();
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
            string nick = GetGroupMemberNick(group, qq);
            if (plugin != null)
            {
                nick = $"{nick} [{plugin.PluginName}]";
            }
            ChatDetailItemViewModel item = BuildChatDetailItem(msgId, qq, msg, nick, ChatAvatar.AvatarTypes.QQGroup, itemType);
            var history = new ChatHistory
            {
                Message = msg,
                ParentID = group,
                SenderID = qq,
                Type = itemType == DetailItemType.Notice ? ChatHistoryType.Notice : ChatHistoryType.Group,
                MsgId = msgId,
            };
            ChatHistoryHelper.InsertHistory(history);
            history.Message = $"{GetGroupMemberNick(group, qq)}: {msg}";
            ChatHistoryHelper.UpdateHistoryCategory(history);
            AddOrUpdateGroupChatList(group, qq, msg);
            Dispatcher.BeginInvoke(() =>
            {
                if (SelectedItem?.Id == group)
                {
                    AddItemToMessageContainer(item, true);
                    ScrollToBottom(MessageScrollViewer, true);
                }
            });
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
            ReorderChatList();
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
            ReorderChatList();
        }

        private string? AddPrivateChatItem(long qq, string msg, DetailItemType itemType, int msgId = 0, Action<string> itemAdded = null, CQPluginProxy plugin = null)
        {
            string nick = GetFriendNick(qq);
            if (plugin != null)
            {
                nick = $"{nick} [{plugin.PluginName}]";
            }
            ChatDetailItemViewModel item = BuildChatDetailItem(msgId, qq, msg, nick, ChatAvatar.AvatarTypes.QQPrivate, itemType);
            var history = new ChatHistory
            {
                Message = msg,
                ParentID = qq,
                SenderID = qq,
                Type = itemType == DetailItemType.Notice ? ChatHistoryType.Notice : ChatHistoryType.Private,
                MsgId = msgId,
            };
            ChatHistoryHelper.InsertHistory(history);
            history.Message = $"{GetFriendNick(qq)}: {msg}";
            ChatHistoryHelper.UpdateHistoryCategory(history);
            AddOrUpdatePrivateChatList(qq, msg);
            Dispatcher.BeginInvoke(() =>
            {
                if (SelectedItem?.Id == qq)
                {
                    AddItemToMessageContainer(item, true);
                    ScrollToBottom(MessageScrollViewer, true);
                }
            });
            itemAdded?.Invoke(item?.GUID);
            return item?.GUID;
        }

        private void AtBtn_Click(object sender, RoutedEventArgs e)
        {
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
            if (filePath.StartsWith(audioPath))
            {
                filePath = filePath.Replace(audioPath, "");
            }
            else
            {
                string fileName = Path.GetFileName(filePath);
                File.Copy(filePath, Path.Combine(audioPath, fileName), true);
                filePath = fileName;
            }
            AddTextToSendBox(CQCode.CQCode_Record(filePath).ToSendString());
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

        private string BuildTextFromRichTextBox()
        {
            StringBuilder stringBuilder = new();
            foreach (Block item in SendText.Document.Blocks)
            {
                if (item is BlockUIContainer blockImgContainer && blockImgContainer.Child is Image blockImg)
                {
                    stringBuilder.Append(blockImg.Tag?.ToString());
                    continue;
                }
                if (item is not Paragraph paragraph)
                {
                    continue;
                }
                foreach (Inline inline in paragraph.Inlines)
                {
                    if (inline is InlineUIContainer uiContainer && uiContainer.Child is Image inlineImage)
                    {
                        stringBuilder.Append(inlineImage.Tag?.ToString());
                    }
                    else
                    {
                        stringBuilder.Append(new TextRange(inline.ContentStart, inline.ContentEnd).Text);
                    }
                }
            }
            return stringBuilder.ToString();
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
                List<ChatHistory> history = new();
                if (item.AvatarType == ChatAvatar.AvatarTypes.QQPrivate)
                {
                    history = ChatHistoryHelper.GetHistoriesByPage(item.Id, ChatHistoryType.Private, 15, 1);
                }
                else
                {
                    history = ChatHistoryHelper.GetHistoriesByPage(item.Id, ChatHistoryType.Group, 15, 1);
                }
                DetailList.Clear();
                history.ForEach(x =>
                {
                    DetailList.Add(ParseChatHistoryToViewModel(item.AvatarType, x));
                });
                CurrentPageIndex = 1;
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

        private void CleanMessageBtn_Click(object sender, RoutedEventArgs e)
        {
            MessageContainer.Children.Clear();
        }

        private void CleanSendBtn_Click(object sender, RoutedEventArgs e)
        {
            SendText.Document.Blocks.Clear();
        }

        private void CQPImplementation_OnGroupMessageSend(int msgId, long group, string msg, CQPluginProxy plugin)
        {
            AddOrUpdateGroupChatList(group, AppConfig.Instance.CurrentQQ, msg);
            AddGroupChatItem(group, AppConfig.Instance.CurrentQQ, msg, DetailItemType.Send, msgId, plugin: plugin);
        }

        private void CQPImplementation_OnPrivateMessageSend(int msgId, long qq, string msg, CQPluginProxy plugin)
        {
            AddOrUpdatePrivateChatList(qq, msg);
            AddPrivateChatItem(qq, msg, DetailItemType.Send, msgId, plugin: plugin);
        }

        private void FaceBtn_Click(object sender, RoutedEventArgs e)
        {
        }

        private async void LazyLoad()
        {
            if (SelectedItem == null)
            {
                return;
            }
            lock (detailListLock)
            {
                var list = ChatHistoryHelper.GetHistoriesByPage(SelectedItem.Id, SelectedItem.AvatarType == ChatAvatar.AvatarTypes.QQPrivate ? ChatHistoryType.Private : ChatHistoryType.Group, UIConfig.Instance.MessageContainerMaxCount, CurrentPageIndex + 1);
                if (list.Count > 0)
                {
                    CurrentPageIndex++;
                }
                else
                {
                    return;
                }
                list.Reverse();
                FrameworkElement firstItem = (FrameworkElement)MessageContainer.Children[0];
                foreach (var item in list)
                {
                    UIElement lastElement;
                    if (item.SenderID == AppConfig.Instance.CurrentQQ)
                    {
                        lastElement = BuildRightBlock(ParseChatHistoryToViewModel(SelectedItem.AvatarType, item));
                    }
                    else if (item.Type == ChatHistoryType.Notice)
                    {
                        lastElement = BuildMiddleBlock(ParseChatHistoryToViewModel(SelectedItem.AvatarType, item));
                    }
                    else
                    {
                        lastElement = BuildLeftBlock(ParseChatHistoryToViewModel(SelectedItem.AvatarType, item));
                    }
                    MessageContainer.Children.Insert(0, lastElement);
                }
                MessageContainer.UpdateLayout();
                firstItem.BringIntoView();
            }
        }

        private void LoadChatHistory()
        {
            var list = ChatHistoryHelper.GetHistoryCategroies();
            ChatList.Clear();
            foreach (var item in list)
            {
                ChatList.Add(new ChatListItemViewModel
                {
                    AvatarType = item.Type == ChatHistoryType.Private ? ChatAvatar.AvatarTypes.QQPrivate : ChatAvatar.AvatarTypes.QQGroup,
                    Detail = item.Message,
                    GroupName = GetGroupName(item.ParentID),
                    Id = item.ParentID,
                    Time = item.Time,
                    UnreadCount = 0
                });
            }
            EmptyHint.Visibility = ChatList.Count != 0 ? Visibility.Collapsed : Visibility.Visible;
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
            if (distanceToTop < 50 && distanceToBottom > 100 && !LazyLoading)
            {
                if (LazyLoadDebounceTimer == null)
                {
                    LazyLoadDebounceTimer = new DispatcherTimer
                    {
                        Interval = TimeSpan.FromMilliseconds(500),
                    };
                    LazyLoadDebounceTimer.Tick += (_, _) =>
                    {
                        LazyLoadDebounceTimer.Stop();
                        Dispatcher.BeginInvoke(LazyLoad);
                        Dispatcher.BeginInvoke(() => LazyLoading = false);
                    };
                    LazyLoading = true;
                    LazyLoadDebounceTimer.Start();
                }
                else
                {
                    LazyLoadDebounceTimer.Stop();
                }
                LazyLoading = true;
                LazyLoadDebounceTimer.Start();
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (UIConfig.Instance.ChatEnabled is false)
            {
                DisableDisplay.Visibility = Visibility.Visible;
                MainContent.Visibility = Visibility.Collapsed;
                return;
            }
            DisableDisplay.Visibility = Visibility.Collapsed;
            MainContent.Visibility = Visibility.Visible;

            if (FormLoaded)
            {
                return;
            }
            FormLoaded = true;
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

            DataObject.AddPastingHandler(SendText, RichTextboxPasteOverrideAction);
            LoadChatHistory();
            ReorderChatList();
        }

        private ChatDetailItemViewModel ParseChatHistoryToViewModel(ChatAvatar.AvatarTypes avatarType, ChatHistory history)
        {
            return new ChatDetailItemViewModel
            {
                AvatarType = avatarType,
                Content = history.Message,
                DetailItemType = history.Type == ChatHistoryType.Notice ? DetailItemType.Notice : (history.SenderID == AppConfig.Instance.CurrentQQ ? DetailItemType.Send : DetailItemType.Receive),
                Id = history.SenderID,
                MsgId = history.MsgId,
                Nick = avatarType == ChatAvatar.AvatarTypes.QQPrivate ? GetFriendNick(history.SenderID) : GetGroupMemberNick(history.ParentID, history.SenderID),
                Recalled = history.Recalled,
                Time = history.Time,
            };
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
                    filePath = filePath.Replace(picPath, "");
                }
                else
                {
                    string fileName = Path.GetFileName(filePath);
                    File.Copy(filePath, Path.Combine(picPath, fileName), true);
                    filePath = fileName;
                }
                AddTextToSendBox(CQCode.CQCode_Image(filePath).ToSendString());
            }
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
        }

        private void PluginManagerProxy_OnGroupMsgRecall(int msgId, long groupId, string msg)
        {
            ChatHistoryHelper.UpdateHistoryRecall(groupId, msgId, ChatHistoryType.Group, true);
            MsgRecalled?.Invoke(msgId);
        }

        private void PluginManagerProxy_OnPrivateMsg(int msgId, long qq, string msg)
        {
            AddPrivateChatItem(qq, msg, DetailItemType.Receive, msgId);
            AddOrUpdatePrivateChatList(qq, msg);
        }

        private void PluginManagerProxy_OnPrivateMsgRecall(int msgId, long qq, string msg)
        {
            ChatHistoryHelper.UpdateHistoryRecall(qq, msgId, ChatHistoryType.Private, true);
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
                GC.Collect();
            }

            lock (detailListLock)
            {
                var ls = DetailList.Skip(Math.Max(0, DetailList.Count - LoadCount)).ToList();
                foreach (var item in ls)
                {
                    if (!CheckMessageContainerHasItem(item.GUID))
                    {
                        AddItemToMessageContainer(item, isRemove: true);
                    }
                }
            }
            ScrollToBottom(MessageScrollViewer);
        }

        private void AddItemToMessageContainer(ChatDetailItemViewModel item, bool isRemove)
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

            if (isRemove && MessageContainer.Children.Count > UIConfig.Instance.MessageContainerMaxCount
                && MessageScrollViewer.VerticalOffset > 100)// 数量超过30，且滚动条不在懒加载区
            {
                do
                {
                    MessageContainer.Children.RemoveAt(0);
                } while (MessageContainer.Children.Count > UIConfig.Instance.MessageContainerMaxCount);
            }
        }

        private void ReorderChatList()
        {
            Dispatcher.BeginInvoke(() =>
            {
                if (SelectedItem != null)
                {
                    SelectedItem.UnreadCount = 0;
                }
                ChatList = ChatList.GroupBy(x => x.Id).Select(x => x.First()).OrderByDescending(x => x.Time).ToList();
                OnPropertyChanged(nameof(ChatList));

                EmptyHint.Visibility = ChatList.Count > 0 ? Visibility.Collapsed : Visibility.Visible;
            });
        }

        private void RichTextboxPasteOverrideAction(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(DataFormats.Bitmap) && e.DataObject.GetData(DataFormats.Bitmap) is BitmapSource image)
            {
                string cacheImagePath = Path.Combine("data", "image", "cached");
                using MemoryStream memoryStream = new();
                BitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(image));
                encoder.Save(memoryStream);
                var buffer = memoryStream.ToArray();
                string md5 = buffer.MD5();

                File.WriteAllBytes(Path.Combine(cacheImagePath, md5 + ".png"), buffer);
                Image img = new()
                {
                    Source = image,
                    Width = image.Width,
                    Height = image.Height,
                    Tag = $"[CQ:image,file=cached\\{md5}.png]"
                };
                (SendText.Document.Blocks.LastBlock as Paragraph).Inlines.Add(new InlineUIContainer(img));
                e.Handled = true;
                e.CancelCommand();
            }
            else if (e.DataObject.GetDataPresent(DataFormats.Text)
                && e.DataObject.GetData(DataFormats.Text) is string text
                && string.IsNullOrEmpty(text) is false)
            {
                AddTextToSendBox(text);
                e.Handled = true;
                e.CancelCommand();
            }
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
            string sendText = BuildTextFromRichTextBox();
            ChatAvatar.AvatarTypes avatar = SelectedItem.AvatarType;
            long id = SelectedItem.Id;
            Task.Run(() =>
            {
                ExecuteSendMessage(id, avatar, sendText);
            });
            SendText.Document.Blocks.Clear();
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