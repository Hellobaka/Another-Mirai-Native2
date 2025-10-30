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
            DataContext = this;
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

        public event PropertyChangedEventHandler? PropertyChanged;

        public static ChatPage Instance { get; private set; }

        /// <summary>
        /// 侧边栏列表
        /// </summary>
        public List<ChatListItemViewModel> ChatList { get; set; } = new();

        /// <summary>
        /// 消息容器列表
        /// </summary>
        public List<ChatDetailItemViewModel> DetailList { get; set; } = new();

        /// <summary>
        /// 当前选择项的组名称
        /// </summary>
        public string GroupName { get; set; } = "";

        /// <summary>
        /// 协议API调用锁
        /// </summary>
        private SemaphoreSlim APILock { get; set; } = new(1, 1);

        /// <summary>
        /// At选择器Flyout元素
        /// </summary>
        private ModernWpf.Controls.Flyout AtFlyout { get; set; }

        /// <summary>
        /// At选择器内容
        /// </summary>
        private AtTargetSelector AtTargetSelector { get; set; }

        /// <summary>
        /// 懒加载当前页数
        /// </summary>
        private int CurrentPageIndex { get; set; }

        /// <summary>
        /// 窗体加载完成事件
        /// </summary>
        private bool FormLoaded { get; set; }

        /// <summary>
        /// 好友信息列表缓存
        /// </summary>
        private Dictionary<long, FriendInfo> FriendInfoCache { get; set; } = new();

        /// <summary>
        /// 群信息列表缓存
        /// </summary>
        private Dictionary<long, GroupInfo> GroupInfoCache { get; set; } = new();

        /// <summary>
        /// 群成员信息列表缓存
        /// </summary>
        private Dictionary<long, Dictionary<long, GroupMemberInfo>> GroupMemberCache { get; set; } = new();

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
        /// 左侧聊天列表选中的元素
        /// </summary>
        private ChatListItemViewModel SelectedItem => (ChatListItemViewModel)ChatListDisplay.SelectedItem;

        /// <summary>
        /// 向发送框中添加文本
        /// </summary>
        /// <param name="text">添加的文本</param>
        public void AddTextToSendBox(string text)
        {
            Dispatcher.BeginInvoke(() =>
            {
                TextPointer startPosition = SendText.Document.ContentStart;
                int start = startPosition.GetOffsetToPosition(SendText.CaretPosition);
                SendText.CaretPosition.InsertTextInRun(text);
                SendText.CaretPosition = startPosition.GetPositionAtOffset(start + text.Length + 2);
                SendText.Focus();
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
        /// 从 <see cref="FriendInfoCache"/> 中获取好友昵称
        /// 若缓存中不存在则调用协议API
        /// </summary>
        /// <param name="qq">好友ID</param>
        /// <returns>昵称, 失败时返回QQ号</returns>
        public async Task<string> GetFriendNick(long qq)
        {
            try
            {
                await APILock.WaitAsync();
                if (qq == AppConfig.Instance.CurrentQQ)
                {
                    return AppConfig.Instance.CurrentNickName;
                }
                if (FriendInfoCache.TryGetValue(qq, out var info))
                {
                    if (info == null)
                    {
                        return qq.ToString();
                    }
                    return info.Nick;
                }
                else
                {
                    string r = qq.ToString();
                    await Task.Run(() =>
                    {
                        var ls = ProtocolManager.Instance.CurrentProtocol.GetRawFriendList(false);
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
                    });
                    return r;
                }
            }
            catch
            {
                return qq.ToString();
            }
            finally
            {
                APILock.Release();
            }
        }

        /// <summary>
        /// 从 <see cref="GroupMemberCache"/> 中获取群员名片
        /// 若缓存中不存在则调用协议API
        /// </summary>
        /// <param name="group">群来源</param>
        /// <param name="qq">群员QQ</param>
        /// <returns>群员名片, 若不存在则返回昵称, 若调用失败则返回QQ号</returns>
        public async Task<string> GetGroupMemberNick(long group, long qq)
        {
            try
            {
                await APILock.WaitAsync();

                if (qq == AppConfig.Instance.CurrentQQ)
                {
                    return AppConfig.Instance.CurrentNickName;
                }
                if (GroupMemberCache.TryGetValue(group, out var dict) && dict.TryGetValue(qq, out var info))
                {
                    if (info == null)
                    {
                        return qq.ToString();
                    }
                    return string.IsNullOrEmpty(info.Card) ? info.Nick : info.Card;
                }
                else
                {
                    if (GroupMemberCache.ContainsKey(group) is false)
                    {
                        GroupMemberCache.Add(group, new Dictionary<long, GroupMemberInfo>());
                    }
                    if (GroupMemberCache[group].ContainsKey(qq) is false)
                    {
                        await Task.Run(() =>
                        {
                            var memberInfo = ProtocolManager.Instance.CurrentProtocol.GetRawGroupMemberInfo(group, qq, false);
                            GroupMemberCache[group].Add(qq, memberInfo);
                        });
                    }
                    if (GroupMemberCache[group][qq] == null)
                    {
                        return qq.ToString();
                    }
                    return string.IsNullOrEmpty(GroupMemberCache[group][qq].Card) ? GroupMemberCache[group][qq].Nick : GroupMemberCache[group][qq].Card;
                }
            }
            catch
            {
                return qq.ToString();
            }
            finally
            {
                APILock.Release();
            }
        }

        /// <summary>
        /// 从 <see cref="GroupInfoCache"/> 中获取群名称
        /// 若缓存中不存在则调用协议API
        /// </summary>
        /// <param name="groupId">群号</param>
        /// <returns>群名称, 若不存在则返回群号</returns>
        public async Task<string> GetGroupName(long groupId)
        {
            try
            {
                await APILock.WaitAsync();

                if (GroupInfoCache.TryGetValue(groupId, out var info))
                {
                    if (info == null)
                    {
                        return groupId.ToString();
                    }
                    return info.Name;
                }
                else
                {
                    string r = groupId.ToString();
                    await Task.Run(() =>
                    {
                        GroupInfoCache.Add(groupId, ProtocolManager.Instance.CurrentProtocol.GetRawGroupInfo(groupId, false));
                        if (GroupInfoCache[groupId] == null)
                        {
                            r = groupId.ToString();
                        }
                        r = GroupInfoCache[groupId]?.Name ?? groupId.ToString();
                    });
                    return r;
                }
            }
            catch
            {
                return groupId.ToString();
            }
            finally
            {
                APILock.Release();
            }
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
                // 遍历查询消息ID相同的, 并跳转
                foreach (var control in MessageContainer.Children)
                {
                    if (control is ChatDetailListItem detail
                        && detail.MsgId == msgId)
                    {
                        await Dispatcher.Yield();
                        detail.BringIntoView();
                        break;
                    }
                }
            }
            else
            {
                // 计算相差数量, 进行懒加载并跳转
                int lastId = DetailList.First().SqlId;
                await LazyLoad(lastId - history.ID, msgId);
            }
        }

        /// <summary>
        /// 更新目标消息历史的未读消息数量
        /// </summary>
        /// <param name="model"></param>
        public async void UpdateUnreadCount(ChatListItemViewModel model)
        {
            var item = ChatList.FirstOrDefault(x => x.Id == model.Id && x.AvatarType == model.AvatarType);
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
            ChatDetailItemViewModel item = BuildChatDetailItem(msgId, qq, msg, nick!, ChatAvatar.AvatarTypes.QQGroup, itemType);
            var history = ChatHistoryHelper.GetHistoriesByMsgId(group, msgId, ChatHistoryType.Group);
            AddOrUpdateGroupChatList(group, qq, msg);
            await Dispatcher.BeginInvoke(() =>
            {
                if (SelectedItem?.Id == group)
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
        private void AddItemToMessageContainer(ChatDetailItemViewModel item, bool isRemove)
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
            var item = ChatList.FirstOrDefault(x => x.Id == id && x.AvatarType == ChatAvatar.AvatarTypes.QQGroup);
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
                    ChatList.Add(new ChatListItemViewModel
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
            var item = ChatList.FirstOrDefault(x => x.Id == qq && x.AvatarType == ChatAvatar.AvatarTypes.QQPrivate);
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
                     ChatList.Add(new ChatListItemViewModel
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
            ChatDetailItemViewModel item = BuildChatDetailItem(msgId, sender, msg, nick!, ChatAvatar.AvatarTypes.QQPrivate, itemType);
            var history = ChatHistoryHelper.GetHistoriesByMsgId(qq, msgId, ChatHistoryType.Private);
            ChatHistoryHelper.UpdateHistoryCategory(history);
            AddOrUpdatePrivateChatList(qq, sender, msg);
            await Dispatcher.BeginInvoke(() =>
            {
                if (SelectedItem?.Id == qq)
                {
                    AddItemToMessageContainer(item, true);
                    ScrollToBottom(MessageScrollViewer, sender == AppConfig.Instance.CurrentQQ);
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
                    if (GroupMemberCache.ContainsKey(SelectedItem.Id) is false)
                    {
                        GroupMemberCache.Add(SelectedItem.Id, new Dictionary<long, GroupMemberInfo>());
                    }
                    if (GroupMemberCache.TryGetValue(SelectedItem.Id, out var cache))
                    {
                        if (cache.ContainsKey(item.QQ))
                        {
                            cache[item.QQ] = item;
                        }
                        else
                        {
                            cache.Add(item.QQ, item);
                        }
                    }
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
            StringBuilder stringBuilder = new();
            foreach (Block item in SendText.Document.Blocks)
            {
                // 粘贴的图片
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

        /// <summary>
        /// 左侧列表选中变化
        /// </summary>
        private async void ChatListDisplay_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = SelectedItem;
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

            if (SelectedItem.AvatarType == ChatAvatar.AvatarTypes.QQGroup)
            {
                AtBtn.IsEnabled = true;
            }
            else if (SelectedItem.AvatarType == ChatAvatar.AvatarTypes.QQPrivate)
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
            GroupNameDisplay.Text = $"{SelectedItem.GroupName} [{SelectedItem.Id}]";
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

        private void CleanMessageBtn_Click(object sender, RoutedEventArgs e)
        {
            MessageContainer.Children.Clear();
        }

        private void CleanSendBtn_Click(object sender, RoutedEventArgs e)
        {
            SendText.Document.Blocks.Clear();
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
        /// 消息列表懒加载
        /// </summary>
        /// <param name="count">欲加载的消息数量</param>
        /// <param name="msgId">最终跳转的消息ID, 若为-1则保持当前滚动条</param>
        private async Task LazyLoad(int count, int msgId = -1)
        {
            if (SelectedItem == null)
            {
                return;
            }
            // 加载的页面数量
            int diff = (int)Math.Ceiling(count / (float)UIConfig.Instance.MessageContainerMaxCount);
            // 从数据库取的消息历史
            List<ChatHistory> list = new List<ChatHistory>();
            for (int i = CurrentPageIndex + 1; i <= CurrentPageIndex + diff; i++)
            {
                var ls = ChatHistoryHelper.GetHistoriesByPage(SelectedItem.Id,
                    SelectedItem.AvatarType == ChatAvatar.AvatarTypes.QQPrivate ? ChatHistoryType.Private : ChatHistoryType.Group,
                    count,
                    i);
                list = list.Concat(ls).ToList();
            }
            if (list.Count > 0)
            {
                // 更新页数
                CurrentPageIndex += diff;
            }
            else
            {
                return;
            }
            list.Reverse();
            double distanceToBottom = MessageScrollViewer.ScrollableHeight - MessageScrollViewer.VerticalOffset;
            FrameworkElement? scrollItem = null;
            foreach (var item in list)
            {
                var viewModel = await ParseChatHistoryToViewModel(SelectedItem.AvatarType, item);
                UIElement lastElement = BuildChatDetailItem(viewModel);

                MessageContainer.Children.Insert(0, lastElement);
                DetailList.Insert(0, viewModel);
                if (item.MsgId == msgId)
                {
                    scrollItem = (FrameworkElement)lastElement;
                }
            }
            MessageContainer.UpdateLayout();
            await Dispatcher.Yield();
            if (msgId == -1 || scrollItem == null)
            {
                // 保持滚动条位置
                MessageScrollViewer.ScrollToVerticalOffset(MessageScrollViewer.ScrollableHeight - distanceToBottom);
            }
            else
            {
                scrollItem?.BringIntoView();
            }
        }

        /// <summary>
        /// 加载左侧聊天列表
        /// </summary>
        /// <returns></returns>
        private async Task LoadChatHistory()
        {
            var list = ChatHistoryHelper.GetHistoryCategroies();
            ChatList.Clear();
            foreach (var item in list)
            {
                ChatList.Add(new ChatListItemViewModel
                {
                    AvatarType = item.Type == ChatHistoryType.Private ? ChatAvatar.AvatarTypes.QQPrivate : ChatAvatar.AvatarTypes.QQGroup,
                    Detail = item.Message,
                    GroupName = item.Type == ChatHistoryType.Private ? await GetFriendNick(item.ParentID) : await GetGroupName(item.ParentID),
                    Id = item.ParentID,
                    Time = item.Time,
                    UnreadCount = 0
                });
            }
            EmptyHint.Visibility = ChatList.Count != 0 ? Visibility.Collapsed : Visibility.Visible;
        }

        private void MessageScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (sender is not ScrollViewer scrollViewer || e.VerticalChange == 0)
            {
                return;
            }
            double distanceToBottom = scrollViewer.ScrollableHeight - scrollViewer.VerticalOffset;
            double distanceToTop = scrollViewer.VerticalOffset;

            ScrollBottomContainer.Visibility = distanceToBottom > 100 ? Visibility.Visible : Visibility.Collapsed;
            if (distanceToTop < 50 && distanceToBottom > 100 && !LazyLoading)
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
                if (SelectedItem == null && ChatList.Count > 0)
                {
                    // 当没有内容被选中时 选中第一项
                    await Dispatcher.Yield();
                    ChatListDisplay.SelectedItem = ChatList.First();
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
            return new ChatDetailItemViewModel
            {
                AvatarType = avatarType,
                Content = history.Message,
                DetailItemType = history.Type == ChatHistoryType.Notice ? DetailItemType.Notice : (history.SenderID == AppConfig.Instance.CurrentQQ ? DetailItemType.Send : DetailItemType.Receive),
                Id = history.SenderID,
                MsgId = history.MsgId,
                Nick = (avatarType == ChatAvatar.AvatarTypes.QQPrivate ? await GetFriendNick(history.SenderID) : await GetGroupMemberNick(history.ParentID, history.SenderID))
                     + (string.IsNullOrEmpty(history.PluginName) ? "" : $" [{history.PluginName}]"),
                Recalled = history.Recalled,
                Time = history.Time,
                SqlId = history.ID
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
            if (SelectedItem == null)
            {
                return;
            }
            if (refreshAll)
            {
                await RefreshGroupName();
                MessageContainer.Children.Clear();
                GC.Collect();
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
                if (SelectedItem != null)
                {
                    SelectedItem.UnreadCount = 0;
                }
                ChatList = ChatList.GroupBy(x => x.Id).Select(x => x.First()).OrderByDescending(x => x.Time).ToList();
                OnPropertyChanged(nameof(ChatList));
                EmptyHint.Visibility = ChatList.Count > 0 ? Visibility.Collapsed : Visibility.Visible;
            });
        }

        /// <summary>
        /// 发送文本框响应粘贴事件
        /// </summary>
        private void RichTextboxPasteOverrideAction(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(DataFormats.Bitmap) && e.DataObject.GetData(DataFormats.Bitmap) is BitmapSource image)
            {
                // 粘贴内容为图片 将图片保存进缓存文件夹
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
                if (SendText.Document.Blocks.Count == 0)
                {
                    SendText.Document.Blocks.Add(new Paragraph());
                }
                if (SendText.Document.Blocks.LastBlock is Paragraph lastParagraph)
                {
                    lastParagraph.Inlines.Add(new InlineUIContainer(img));
                }
                e.Handled = true;
                e.CancelCommand();
            }
            else if (e.DataObject.GetDataPresent(DataFormats.UnicodeText)
                && e.DataObject.GetData(DataFormats.UnicodeText) is string text
                && string.IsNullOrEmpty(text) is false)
            {
                // 粘贴内容为文本
                AddTextToSendBox(text);
                e.Handled = true;
                e.CancelCommand();
            }
        }

        /// <summary>
        /// 将滚动容器滚动至底部
        /// 当滚动至底按钮不可见时不滚动
        /// </summary>
        /// <param name="scrollViewer">滚动容器</param>
        /// <param name="forced">true时忽略滚动至底按钮是否可见</param>
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
            // 清理消息内容至数量以下
            if (MessageContainer.Children.Count > UIConfig.Instance.MessageContainerMaxCount
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
            SendText.Document.Blocks.Clear();
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