using Another_Mirai_Native.Config;
using Another_Mirai_Native.DB;
using Another_Mirai_Native.Model;
using Another_Mirai_Native.Model.Enums;
using Another_Mirai_Native.UI.Controls;
using Another_Mirai_Native.UI.Pages;
using Another_Mirai_Native.UI.Pages.Helpers;
using Another_Mirai_Native.UI.ViewModel;
using System;
using System.Threading.Tasks;

namespace Another_Mirai_Native.UI.Services
{
    /// <summary>
    /// 消息发送请求
    /// </summary>
    public class SendMessageRequest
    {
        public long TargetId { get; set; }
        public ChatType ChatType { get; set; }
        public ChatAvatar.AvatarTypes AvatarType { get; set; }
        public string Message { get; set; } = "";
        public long SenderId { get; set; }
        public DateTime SendTime { get; set; }
    }

    /// <summary>
    /// 消息发送结果
    /// </summary>
    public class SendResult
    {
        public bool Success { get; set; }
        public int MessageId { get; set; }
        public string? ErrorMessage { get; set; }
        public Exception? Exception { get; set; }

        public static SendResult SuccessResult(int messageId) => new SendResult
        {
            Success = true,
            MessageId = messageId
        };

        public static SendResult FailedResult(string? errorMessage = null) => new SendResult
        {
            Success = false,
            ErrorMessage = errorMessage ?? "发送失败"
        };

        public static SendResult ErrorResult(Exception ex) => new SendResult
        {
            Success = false,
            ErrorMessage = ex.Message,
            Exception = ex
        };
    }

    /// <summary>
    /// 消息发送协调器 - 统一处理消息发送流程
    /// </summary>
    public class MessageSendingCoordinator
    {
        private readonly IMessageService _messageService;
        private readonly MessageContainerManager? _containerManager;

        public MessageSendingCoordinator(
            IMessageService messageService,
            MessageContainerManager? containerManager = null)
        {
            _messageService = messageService ?? throw new ArgumentNullException(nameof(messageService));
            _containerManager = containerManager;
        }

        /// <summary>
        /// 统一的消息发送流程
        /// </summary>
        public async Task<SendResult> SendMessageAsync(SendMessageRequest request)
        {
            if (request.TargetId == 0 || string.IsNullOrEmpty(request.Message))
            {
                return SendResult.FailedResult("目标ID或消息内容为空");
            }

            string? messageGuid = null;
            int sqlId = 0;

            try
            {
                // 1. 保存到数据库
                sqlId = await SaveToDatabase(request);

                // 2. 添加到UI并获取GUID
                messageGuid = await AddToUI(request);

                // 3. 更新发送中状态
                UpdateSendingStatus(messageGuid, true);

                // 4. 调用发送API
                int msgId = await Task.Run(() =>
                    _messageService.SendMessage(request.TargetId, request.ChatType, request.Message));

                if (msgId > 0)
                {
                    // 5. 发送成功：更新UI和数据库
                    UpdateSuccess(messageGuid, msgId);
                    await UpdateDatabaseMessageId(request.TargetId, request.ChatType, sqlId, msgId);
                    return SendResult.SuccessResult(msgId);
                }
                else
                {
                    // 6. 发送失败：标记失败
                    UpdateFailed(messageGuid);
                    return SendResult.FailedResult("API返回失败");
                }
            }
            catch (Exception ex)
            {
                // 7. 异常处理
                if (messageGuid != null)
                {
                    UpdateFailed(messageGuid);
                }
                return SendResult.ErrorResult(ex);
            }
        }

        /// <summary>
        /// 保存消息到数据库
        /// </summary>
        private Task<int> SaveToDatabase(SendMessageRequest request)
        {
            return Task.Run(() =>
            {
                var history = new ChatHistory
                {
                    Message = request.Message,
                    ParentID = request.TargetId,
                    SenderID = request.SenderId,
                    Type = request.ChatType == ChatType.Group ? ChatHistoryType.Group : ChatHistoryType.Private,
                    MsgId = 0,
                    PluginName = "",
                    Time = request.SendTime,
                };
                return ChatHistoryHelper.InsertHistory(history);
            });
        }

        /// <summary>
        /// 添加消息到UI，返回消息GUID
        /// </summary>
        private async Task<string?> AddToUI(SendMessageRequest request)
        {
            string? guid = null;
            var tcs = new TaskCompletionSource<string?>();

            if (request.AvatarType == ChatAvatar.AvatarTypes.QQGroup)
            {
                await ChatPage.Instance.AddGroupChatItem(
                    request.TargetId,
                    request.SenderId,
                    request.Message,
                    DetailItemType.Send,
                    request.SendTime,
                    itemAdded: (g) =>
                    {
                        guid = g;
                        tcs.TrySetResult(g);
                    });
            }
            else
            {
                await ChatPage.Instance.AddPrivateChatItem(
                    request.TargetId,
                    request.SenderId,
                    request.Message,
                    DetailItemType.Send,
                    request.SendTime,
                    itemAdded: (g) =>
                    {
                        guid = g;
                        tcs.TrySetResult(g);
                    });
            }

            // 等待itemAdded回调完成
            await tcs.Task;
            return guid;
        }

        /// <summary>
        /// 更新发送状态
        /// </summary>
        private void UpdateSendingStatus(string? guid, bool isSending)
        {
            if (guid != null)
            {
                _containerManager?.UpdateSendStatus(guid, isSending);
            }
        }

        /// <summary>
        /// 标记发送成功
        /// </summary>
        private void UpdateSuccess(string? guid, int messageId)
        {
            if (guid != null)
            {
                _containerManager?.UpdateSendStatus(guid, false);
                _containerManager?.UpdateMessageId(guid, messageId);
            }
        }

        /// <summary>
        /// 标记发送失败
        /// </summary>
        private void UpdateFailed(string? guid)
        {
            if (guid != null)
            {
                _containerManager?.MarkSendFailed(guid);
            }
        }

        /// <summary>
        /// 更新数据库中的消息ID
        /// </summary>
        private Task UpdateDatabaseMessageId(long targetId, ChatType chatType, int sqlId, int messageId)
        {
            return Task.Run(() =>
            {
                var historyType = chatType == ChatType.Group ? ChatHistoryType.Group : ChatHistoryType.Private;
                ChatHistoryHelper.UpdateHistoryMessageId(targetId, historyType, sqlId, messageId);
            });
        }
    }
}
