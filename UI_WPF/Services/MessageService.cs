using Another_Mirai_Native.Config;
using Another_Mirai_Native.DB;
using Another_Mirai_Native.Model;
using Another_Mirai_Native.Model.Enums;
using Another_Mirai_Native.Native;
using Another_Mirai_Native.UI.Controls;
using Another_Mirai_Native.UI.ViewModel;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Another_Mirai_Native.UI.Services
{
    /// <summary>
    /// 消息服务实现
    /// </summary>
    public class MessageService : IMessageService
    {
        private readonly ICacheService _cacheService;

        public MessageService(ICacheService cacheService)
        {
            _cacheService = cacheService;
        }

        /// <summary>
        /// 发送消息（统一处理群消息和私聊消息）
        /// </summary>
        public int SendMessage(long targetId, ChatType chatType, string message)
        {
            Stopwatch sw = Stopwatch.StartNew();
            string logPrefix = chatType == ChatType.Group ? "发送群组消息" : "发送私聊消息";
            string logDetail = chatType == ChatType.Group 
                ? $"群:{targetId} 消息:{message}" 
                : $"QQ:{targetId} 消息:{message}";
            
            int logId = LogHelper.WriteLog(LogLevel.InfoSend, $"[↑]{logPrefix}", logDetail, "处理中...");
            
            int msgId = chatType == ChatType.Group
                ? ProtocolManager.Instance.CurrentProtocol.SendGroupMessage(targetId, message)
                : ProtocolManager.Instance.CurrentProtocol.SendPrivateMessage(targetId, message);
            
            sw.Stop();
            LogHelper.UpdateLogStatus(logId, $"√ {sw.ElapsedMilliseconds / 1000.0:f2} s");
            
            return msgId;
        }

        /// <summary>
        /// 添加聊天消息项
        /// </summary>
        public async Task<int> AddChatItemAsync(AddChatItemParameters parameters)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            // 获取发送者昵称
            string nick = parameters.ChatType == ChatType.Group
                ? await _cacheService.GetGroupMemberNickAsync(parameters.TargetId, parameters.SenderId)
                : await _cacheService.GetFriendNickAsync(parameters.SenderId);

            // 添加插件名称后缀
            if (!string.IsNullOrEmpty(parameters.PluginName))
            {
                nick = $"{nick} [{parameters.PluginName}]";
            }

            // 构建ViewModel
            ChatDetailItemViewModel item = new ChatDetailItemViewModel
            {
                AvatarType = parameters.ChatType == ChatType.Group 
                    ? ChatAvatar.AvatarTypes.QQGroup 
                    : ChatAvatar.AvatarTypes.QQPrivate,
                Content = parameters.Message,
                DetailItemType = parameters.ItemType,
                Id = parameters.SenderId,
                Nick = nick,
                MsgId = parameters.MsgId,
                Time = parameters.Time,
            };

            // 触发回调（UI层处理）
            parameters.ItemAddedCallback?.Invoke(item.GUID);

            // 查询历史记录ID
            var history = ChatHistoryHelper.GetHistoriesByMsgId(
                parameters.TargetId, 
                parameters.MsgId, 
                parameters.ChatType == ChatType.Group ? ChatHistoryType.Group : ChatHistoryType.Private);

            return history?.ID ?? 0;
        }

        /// <summary>
        /// 将历史记录转换为ViewModel
        /// </summary>
        public async Task<ChatDetailItemViewModel> ParseHistoryAsync(ChatHistory history, ChatAvatar.AvatarTypes avatarType)
        {
            if (history == null)
            {
                throw new ArgumentNullException(nameof(history));
            }

            // 获取昵称
            string nick = avatarType == ChatAvatar.AvatarTypes.QQPrivate
                ? await _cacheService.GetFriendNickAsync(history.SenderID)
                : await _cacheService.GetGroupMemberNickAsync(history.ParentID, history.SenderID);

            // 添加插件名称后缀
            if (!string.IsNullOrEmpty(history.PluginName))
            {
                nick = $"{nick} [{history.PluginName}]";
            }

            // 确定消息类型
            DetailItemType itemType;
            if (history.Type == ChatHistoryType.Notice)
            {
                itemType = DetailItemType.Notice;
            }
            else if (history.SenderID == AppConfig.Instance.CurrentQQ)
            {
                itemType = DetailItemType.Send;
            }
            else
            {
                itemType = DetailItemType.Receive;
            }

            return new ChatDetailItemViewModel
            {
                AvatarType = avatarType,
                Content = history.Message,
                DetailItemType = itemType,
                Id = history.SenderID,
                MsgId = history.MsgId,
                Nick = nick,
                Recalled = history.Recalled,
                Time = history.Time,
                SqlId = history.ID
            };
        }

        /// <summary>
        /// 执行完整的发送消息流程
        /// </summary>
        public async Task<int> ExecuteSendMessageAsync(SendMessageParameters parameters)
        {
            if (parameters == null || parameters.TargetId == 0 || string.IsNullOrEmpty(parameters.Message))
            {
                return 0;
            }

            int sendMsgId = 0;
            ManualResetEvent sendSignal = new(false);

            // 创建历史记录
            var history = new ChatHistory
            {
                Message = parameters.Message,
                ParentID = parameters.TargetId,
                SenderID = parameters.SenderId,
                Type = parameters.ChatType == ChatType.Group ? ChatHistoryType.Group : ChatHistoryType.Private,
                MsgId = 0,
                PluginName = parameters.PluginName ?? "",
                Time = parameters.Time,
            };
            
            int sqlId = ChatHistoryHelper.InsertHistory(history);

            // 添加聊天项（UI层会通过回调处理发送）
            await AddChatItemAsync(new AddChatItemParameters
            {
                TargetId = parameters.TargetId,
                SenderId = parameters.SenderId,
                Message = parameters.Message,
                ChatType = parameters.ChatType,
                ItemType = DetailItemType.Send,
                Time = parameters.Time,
                MsgId = 0,
                PluginName = parameters.PluginName,
                ItemAddedCallback = (guid) =>
                {
                    // 实际发送消息
                    int msgId = SendMessage(parameters.TargetId, parameters.ChatType, parameters.Message);
                    if (msgId != 0)
                    {
                        sendMsgId = msgId;
                    }
                    sendSignal.Set();
                }
            });

            sendSignal.WaitOne();

            // 更新历史记录的消息ID
            if (sendMsgId != 0)
            {
                ChatHistoryHelper.UpdateHistoryMessageId(
                    parameters.TargetId,
                    parameters.ChatType == ChatType.Group ? ChatHistoryType.Group : ChatHistoryType.Private,
                    sqlId,
                    sendMsgId);
            }

            return sendMsgId;
        }
    }
}
