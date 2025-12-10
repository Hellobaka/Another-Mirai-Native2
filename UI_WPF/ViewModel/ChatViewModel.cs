using Another_Mirai_Native.Config;
using Another_Mirai_Native.DB;
using Another_Mirai_Native.Model;
using Another_Mirai_Native.Model.Enums;
using Another_Mirai_Native.Native;
using Another_Mirai_Native.UI.Models;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Threading;

namespace Another_Mirai_Native.UI.ViewModel
{
    public class ChatViewModel : INotifyPropertyChanged
    {
        public ChatViewModel()
        {
            CreateRelayCommands();
            Instance = this;
            PluginManagerProxy.OnGroupMsgRecall += PluginManagerProxy_OnGroupMsgRecall;
            PluginManagerProxy.OnPrivateMsgRecall += PluginManagerProxy_OnPrivateMsgRecall;
        }

        public event Action<int>? OnMessageJumpRequested;

        public event Action? OnScrollToBottomRequested;

        public event Action<string>? OnTextAddRequested;

        public event PropertyChangedEventHandler? PropertyChanged;

        public static ChatViewModel Instance { get; set; }

        public bool Avatar_IsAtEnabled { get; set; } = true;

        /// <summary>
        /// 侧边栏列表
        /// </summary>
        public ObservableCollection<ChatListItemViewModel> ChatList { get; set; } = [];

        public string ChatNameDisplay => SelectedChat == null
            ? "" : $"{SelectedChat.GroupName} [{SelectedChat.Id}]";

        /// <summary>
        /// 懒加载当前页数
        /// </summary>
        public int CurrentPageIndex { get; set; }

        public bool EmptyHintVisible => ChatList.Count == 0;

        public bool IsGroupChat => SelectedChat != null && SelectedChat.AvatarType == ChatType.QQGroup;

        public bool LazyLoading { get; set; }

        public bool Message_IsAtEnabled { get; set; }

        /// <summary>
        /// 消息容器列表
        /// </summary>
        public ObservableCollection<MessageViewModel> Messages { get; set; } = [];

        public ChatListItemViewModel? SelectedChat { get; set; }

        public FlowDocument SendText { get; set; } = new();

        public bool ToolBoxEnabled => SelectedChat != null;

        /// <summary>
        /// 初始化加载时的消息数量
        /// </summary>
        private int LoadCount { get; set; } = 15;

        /// <summary>
        /// 将历史转换为消息模型
        /// </summary>
        /// <param name="avatarType">消息来源</param>
        /// <param name="history">聊天历史</param>
        /// <returns>消息模型</returns>
        public static async Task<MessageViewModel> ParseChatHistoryToViewModel(ChatType avatarType, ChatHistory history)
        {
            return new MessageViewModel
            {
                AvatarType = avatarType,
                Content = history.Message,
                DetailItemType = history.Type == ChatHistoryType.Notice ? DetailItemType.Notice : (history.SenderID == AppConfig.Instance.CurrentQQ ? DetailItemType.Send : DetailItemType.Receive),
                Id = history.SenderID,
                ParentId = history.ParentID,
                MsgId = history.MsgId,
                Nick = (avatarType == ChatType.QQPrivate ? await Caches.GetFriendNick(history.SenderID) : await Caches.GetGroupMemberNick(history.ParentID, history.SenderID))
                     + (string.IsNullOrEmpty(history.PluginName) ? "" : $" [{history.PluginName}]"),
                Recalled = history.Recalled,
                Time = history.Time,
                SqlId = history.ID
            };
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
        public async Task AddGroupChatItem(long group, long qq, string msg, DetailItemType itemType, DateTime time, int msgId = 0, CQPluginProxy? plugin = null)
        {
            string nick = await Caches.GetGroupMemberNick(group, qq) ?? qq.ToString();
            if (nick != null && plugin != null)
            {
                nick = $"{nick} [{plugin.PluginName}]";
            }
            MessageViewModel item = new()
            {
                AvatarType = ChatType.QQGroup,
                Content = msg,
                DetailItemType = itemType,
                Id = qq,
                Nick = nick!,
                MsgId = msgId,
                Time = DateTime.Now,
            };

            var history = ChatHistoryHelper.GetHistoriesByMsgId(group, msgId, ChatHistoryType.Group);
            ChatHistoryHelper.UpdateHistoryCategory(history);
            await AddOrUpdateGroupChatList(group, qq, msg);
            if (SelectedChat?.Id == group)
            {
                // TODO: 考虑是否需要限制数量
                Messages.Add(item);
                // TODO: 可能没那么必要
                ScrollToBottom();
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
        public async Task AddPrivateChatItem(long qq, long sender, string msg, DetailItemType itemType, DateTime time, int msgId = 0, Action<string>? itemAdded = null, CQPluginProxy? plugin = null)
        {
            string nick = await Caches.GetFriendNick(sender);
            if (nick != null && plugin != null)
            {
                nick = $"{nick} [{plugin.PluginName}]";
            }
            MessageViewModel item = new()
            {
                AvatarType = ChatType.QQPrivate,
                Content = msg,
                DetailItemType = itemType,
                Id = qq,
                Nick = nick!,
                MsgId = msgId,
                Time = DateTime.Now,
            };
            var history = ChatHistoryHelper.GetHistoriesByMsgId(qq, msgId, ChatHistoryType.Private);
            ChatHistoryHelper.UpdateHistoryCategory(history);
            await AddOrUpdatePrivateChatList(qq, sender, msg);
            if (SelectedChat?.Id == qq)
            {
                // TODO: 考虑是否需要限制数量
                Messages.Add(item);
                // TODO: 可能没那么必要
                ScrollToBottom();
            }
        }

        /// <summary>
        /// 向发送框中添加文本
        /// </summary>
        /// <param name="text">添加的文本</param>
        public void AddTextToSendBox(string text)
        {
            OnTextAddRequested?.Invoke(text);
        }

        /// <summary>
        /// 调用发送群消息
        /// </summary>
        /// <param name="groupId">发送的群</param>
        /// <param name="message">发送的消息</param>
        /// <returns>返回的消息ID, 不为0时为成功</returns>
        public async Task<int> CallGroupMsgSendAsync(long groupId, string message)
        {
            int msgId = 0;
            await Task.Run(() =>
            {
                Stopwatch sw = Stopwatch.StartNew();
                int logId = LogHelper.WriteLog(LogLevel.InfoSend, "[↑]发送群组消息", $"群:{groupId} 消息:{message}", "处理中...");
                msgId = ProtocolManager.Instance.CurrentProtocol.SendGroupMessage(groupId, message);
                sw.Stop();
                LogHelper.UpdateLogStatus(logId, $"√ {sw.ElapsedMilliseconds / 1000.0:f2} s");
            });
            return msgId;
        }

        /// <summary>
        /// 调用发送好友消息
        /// </summary>
        /// <param name="qq">发送的好友</param>
        /// <param name="message">发送的消息</param>
        /// <returns>返回的消息ID, 不为0时为成功</returns>
        public async Task<int> CallPrivateMsgSendAsync(long qq, string message)
        {
            int msgId = 0;
            await Task.Run(() =>
            {
                Stopwatch sw = Stopwatch.StartNew();
                int logId = LogHelper.WriteLog(LogLevel.InfoSend, "[↑]发送私聊消息", $"QQ:{qq} 消息:{message}", "处理中...");
                int msgId = ProtocolManager.Instance.CurrentProtocol.SendPrivateMessage(qq, message);
                sw.Stop();
                LogHelper.UpdateLogStatus(logId, $"√ {sw.ElapsedMilliseconds / 1000.0:f2} s");
            });
            return msgId;
        }

        public async Task ExecuteSendMessageAsync(long id, ChatType chatType, string message)
        {
            if (id == 0 || string.IsNullOrEmpty(message))
            {
                return;
            }
            // 插入历史数据库
            var history = new ChatHistory
            {
                Message = message,
                ParentID = id,
                SenderID = AppConfig.Instance.CurrentQQ,
                Type = chatType == ChatType.QQGroup ? ChatHistoryType.Group : ChatHistoryType.Private,
                MsgId = 0,
                PluginName = "",
                Time = DateTime.Now,
            };
            int sqlId = ChatHistoryHelper.InsertHistory(history);

            // 创建UI模型
            Task<int> sendTask;
            string nick;
            if (chatType == ChatType.QQGroup)
            {
                sendTask = CallGroupMsgSendAsync(id, message);
                nick = await Caches.GetGroupMemberNick(id, AppConfig.Instance.CurrentQQ);
                await AddOrUpdateGroupChatList(id, AppConfig.Instance.CurrentQQ, message);
            }
            else
            {
                sendTask = CallPrivateMsgSendAsync(id, message);
                nick = await Caches.GetFriendNick(AppConfig.Instance.CurrentQQ);
                await AddOrUpdatePrivateChatList(id, AppConfig.Instance.CurrentQQ, message);
            }
            var messageViewModel = new MessageViewModel
            {
                AvatarType = chatType,
                Content = message,
                DetailItemType = DetailItemType.Send,
                Id = AppConfig.Instance.CurrentQQ,
                Nick = nick,
                MsgId = 0,
                Time = DateTime.Now,
            };
            Messages.Add(messageViewModel);

            // 等待消息发送函数结果，更新消息发送状态与消息ID
            messageViewModel.MsgId = await sendTask;
            messageViewModel.MessageStatus = messageViewModel.MsgId != 0 ? MessageStatus.Sent : MessageStatus.SendFailed;

            // 更新数据库中的消息ID
            ChatHistoryHelper.UpdateHistoryMessageId(id, chatType == ChatType.QQGroup ? ChatHistoryType.Group : ChatHistoryType.Private
                , sqlId, messageViewModel.MsgId);
            ChatHistoryHelper.UpdateHistoryCategory(history);
            ScrollToBottom();
        }

        public void InvokePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #region Commands

        public RelayCommand Command_ToolAudio { get; set; }

        public RelayCommand Command_ToolClear { get; set; }

        public RelayCommand Command_ToolPicture { get; set; }

        public RelayCommand Command_ToolSendText { get; set; }

        #endregion Commands

        public void JumpToMessage(int msgId)
        {
            OnMessageJumpRequested?.Invoke(msgId);
        }

        public async Task LoadChatList()
        {
            var list = ChatHistoryHelper.GetHistoryCategories();
            ChatList.Clear();
            foreach (var item in list)
            {
                ChatList.Add(new ChatListItemViewModel
                {
                    AvatarType = item.Type == ChatHistoryType.Private ? ChatType.QQPrivate : ChatType.QQGroup,
                    Detail = item.Message,
                    GroupName = item.Type == ChatHistoryType.Private ? await Caches.GetFriendNick(item.ParentID) : await Caches.GetGroupName(item.ParentID),
                    Id = item.ParentID,
                    Time = item.Time,
                    UnreadCount = 0
                });
            }
            await ReorderChatList();
        }

        public async void OnSelectedChatChanged()
        {
            Messages = [];
            if (SelectedChat == null)
            {
                return;
            }
            var histories = SelectedChat.AvatarType == ChatType.QQGroup
                ? await ChatHistoryHelper.GetHistoriesByPageAsync(SelectedChat.Id, ChatHistoryType.Group, LoadCount, 1)
                : await ChatHistoryHelper.GetHistoriesByPageAsync(SelectedChat.Id, ChatHistoryType.Private, LoadCount, 1);

            foreach (var item in histories)
            {
                Messages.Add(await ParseChatHistoryToViewModel(SelectedChat.AvatarType, item));
            }
            CurrentPageIndex = 1;
            SelectedChat.UnreadCount = 0;
            await Dispatcher.Yield();
            ScrollToBottom();
        }

        public void ScrollToBottom()
        {
            OnScrollToBottomRequested?.Invoke();
        }

        public void ToolAudio(object? parameter)
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

        public void ToolClear(object? parameter)
        {
            Messages.Clear();
        }

        public void ToolPicture(object? parameter)
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

        public async Task ToolSendText(object? parameter)
        {
            if (SelectedChat == null)
            {
                return;
            }
            string sendText = BuildTextFromRichTextBox();
            ChatType avatar = SelectedChat.AvatarType;
            long id = SelectedChat.Id;
            await ExecuteSendMessageAsync(id, avatar, sendText);
            // 清空发送框
            SendText.Blocks.Clear();
        }

        /// <summary>
        /// 添加或更新左侧聊天列表群内容
        /// </summary>
        /// <param name="id">群ID</param>
        /// <param name="qq">发送者ID</param>
        /// <param name="msg">消息</param>
        private async Task AddOrUpdateGroupChatList(long group, long qq, string msg)
        {
            msg = msg.Replace("\r", "").Replace("\n", "");
            var item = ChatList.FirstOrDefault(x => x.Id == group && x.AvatarType == ChatType.QQGroup);
            if (item != null) // 消息已存在, 更新
            {
                item.GroupName = await Caches.GetGroupName(group);
                item.Detail = $"{await Caches.GetGroupMemberNick(group, qq)}: {msg}";
                item.Time = DateTime.Now;
                if (SelectedChat != item)
                {
                    item.UnreadCount++;
                }
                else
                {
                    item.UnreadCount = 0;
                }
            }
            else
            {
                item = new ChatListItemViewModel
                {
                    AvatarType = ChatType.QQGroup,
                    Detail = $"{await Caches.GetGroupMemberNick(group, qq)}: {msg}",
                    GroupName = await Caches.GetGroupName(group),
                    Id = group,
                    Time = DateTime.Now,
                    UnreadCount = 1
                };
                ChatList.Add(item);
            }
            await ReorderChatList();
        }

        /// <summary>
        /// 添加或更新左侧聊天列表好友内容
        /// </summary>
        /// <param name="qq">好友ID</param>
        /// <param name="sender">发送者ID</param>
        /// <param name="msg">消息</param>
        private async Task AddOrUpdatePrivateChatList(long qq, long sender, string msg)
        {
            msg = msg.Replace("\r", "").Replace("\n", "");
            var item = ChatList.FirstOrDefault(x => x.Id == qq && x.AvatarType == ChatType.QQPrivate);
            if (item != null) // 消息已存在, 更新
            {
                item.GroupName = await Caches.GetFriendNick(qq);
                item.Detail = msg;
                item.Time = DateTime.Now;
                item.UnreadCount++;
            }
            else
            {
                item = new ChatListItemViewModel
                {
                    AvatarType = ChatType.QQPrivate,
                    Detail = msg,
                    GroupName = await Caches.GetFriendNick(qq),
                    Id = sender,
                    Time = DateTime.Now,
                    UnreadCount = 1
                };
                ChatList.Add(item);
            }
            await ReorderChatList();
        }

        /// <summary>
        /// 发送消息转CQ码
        /// </summary>
        /// <returns>处理后的CQ码消息</returns>
        private string BuildTextFromRichTextBox()
        {
            StringBuilder stringBuilder = new();
            foreach (Block item in SendText.Blocks)
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

        private void CreateRelayCommands()
        {
            var fields = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.PropertyType == typeof(RelayCommand) && p.Name.StartsWith("Command_"));

            foreach (var prop in fields)
            {
                string methodName = prop.Name.Replace("Command_", "");
                var methodInfo = GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

                if (methodInfo != null)
                {
                    // 创建委托
                    Action<object> action = (obj) => methodInfo.Invoke(this, [obj]);
                    var cmd = new RelayCommand(action);
                    prop.SetValue(this, cmd);
                }
                else
                {
                    // 没有找到方法时
                    throw new Exception($"方法 {methodName} 未定义!");
                }
            }
        }

        private void PluginManagerProxy_OnGroupMsgRecall(int msgId, long groupId, string msg)
        {
            if (SelectedChat != null
                && SelectedChat.AvatarType == ChatType.QQGroup
                && SelectedChat.Id == groupId)
            {
                var item = Messages.FirstOrDefault(x => x.MsgId == msgId);
                if (item != null)
                {
                    item.Recalled = true;
                }
            }
        }

        private void PluginManagerProxy_OnPrivateMsgRecall(int msgId, long qq, string msg)
        {
            if (SelectedChat != null
                && SelectedChat.AvatarType == ChatType.QQPrivate
                && SelectedChat.Id == qq)
            {
                var item = Messages.FirstOrDefault(x => x.MsgId == msgId);
                if (item != null)
                {
                    item.Recalled = true;
                }
            }
        }

        private async Task ReorderChatList()
        {
            ChatList = ChatList.OrderByDescending(x => x.Time).ToObservableCollection();
        }
    }
}