using Another_Mirai_Native.Abstractions.Enums;
using Another_Mirai_Native.Config;
using Another_Mirai_Native.DB;
using Another_Mirai_Native.Model;
using Another_Mirai_Native.WebAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Another_Mirai_Native.WebAPI.Controllers
{
    [ApiController]
    [Route("/api/chat")]
    [Authorize]
    public class ChatController : ControllerBase
    {
        [HttpGet("categories")]
        public async Task<IActionResult> GetChatCategories()
        {
            if (AppConfig.Instance.EnableChat is false)
            {
                return NotFound(ApiResponse.Error(404, "聊天功能未启用"));
            }
            var categories = (await Task.Run(ChatHistoryHelper.GetHistoryCategories)).Select(ChatCategoryDto.CreateFromChatCategoryEntity);
            return Ok(ApiResponse.Ok(categories));
        }

        [HttpGet("history")]
        public async Task<IActionResult> GetChatHistory(ChatHistoryType chatHistoryType, long parentId, int pageIndex = 1, int pageSize = 50)
        {
            if (AppConfig.Instance.EnableChat is false)
            {
                return NotFound(ApiResponse.Error(404, "聊天功能未启用"));
            }
            var history = await Task.Run(() => ChatHistoryHelper.GetHistoriesByPage(parentId, chatHistoryType, pageSize, pageIndex));
            var dto = history.Select(ChatHistoryDto.CreateFromChatHistoryEntity).ToList();
            return Ok(ApiResponse.Ok(dto));
        }

        [HttpGet("message")]
        public async Task<IActionResult> GetChatMessage(ChatHistoryType chatHistoryType, long parentId, int messageId)
        {
            if (AppConfig.Instance.EnableChat is false)
            {
                return NotFound(ApiResponse.Error(404, "聊天功能未启用"));
            }
            var message = await Task.Run(() => ChatHistoryHelper.GetHistoriesByMsgId(parentId, messageId, chatHistoryType));
            if (message == null)
            {
                return NotFound(ApiResponse.Error(404, "未找到对应的聊天消息"));
            }
            var dto = ChatHistoryDto.CreateFromChatHistoryEntity(message);
            return Ok(ApiResponse.Ok(dto));
        }

        [HttpGet("friend-nick")]
        public async Task<IActionResult> GetFriendNick(long qq)
        {
            string nick = await ChatHistoryHelper.GetFriendNick(qq);
            return Ok(ApiResponse.Ok(new { nick }));
        }

        [HttpGet("group-name")]
        public async Task<IActionResult> GetGroupName(long groupId)
        {
            string groupName = await ChatHistoryHelper.GetGroupName(groupId);
            return Ok(ApiResponse.Ok(new { groupName }));
        }

        [HttpGet("group-member-card")]
        public async Task<IActionResult> GetGroupMemberCard(long groupId, long qq)
        {
            string card = await ChatHistoryHelper.GetGroupMemberNick(groupId, qq);
            return Ok(ApiResponse.Ok(new { card }));
        }

        [HttpPost("clear-unread")]
        public async Task<IActionResult> ClearUnread(ChatHistoryType chatHistoryType, long parentId)
        {
            if (AppConfig.Instance.EnableChat is false)
            {
                return NotFound(ApiResponse.Error(404, "聊天功能未启用"));
            }
            await Task.Run(() => ChatHistoryHelper.SetUnreadCount(parentId, chatHistoryType, 0));
            return Ok(ApiResponse.Ok());
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendMessage([FromBody] ChatMessageSendRequest request)
        {
            if (AppConfig.Instance.EnableChat is false)
            {
                return NotFound(ApiResponse.Error(404, "聊天功能未启用"));
            }
            if (request.ParentId <= 0)
            {
                return BadRequest(ApiResponse.Error(400, "发送目标无效"));
            }
            if (string.IsNullOrEmpty(request.Message))
            {
                return BadRequest(ApiResponse.Error(400, "发送消息不得为空"));
            }
            if (request.ChatType != ChatHistoryType.Group && request.ChatType != ChatHistoryType.Private)
            {
                return BadRequest(ApiResponse.Error(400, "发送目标类型无效"));
            }
            var history = new ChatHistory
            {
                Message = request.Message,
                ParentID = request.ParentId,
                SenderID = AppConfig.Instance.CurrentQQ,
                Type = request.ChatType,
                MsgId = 0,
                PluginName = "",
                Time = DateTime.Now,
            };
            int sqlId = ChatHistoryHelper.InsertHistory(history);
            int msgId;
            if (request.ChatType == ChatHistoryType.Group)
            {
                msgId = await CallGroupMsgSendAsync(request.ParentId, request.Message);
            }
            else
            {
                msgId = await CallPrivateMsgSendAsync(request.ParentId, request.Message);
            }
            ChatHistoryHelper.UpdateHistoryMessageId(request.ParentId, sqlId, msgId);
            ChatHistoryHelper.UpdateHistoryCategory(history);
            if (msgId != 0)
            {
                return Ok(ApiResponse.Ok(new { msgId }));
            }
            else
            {
                return BadRequest(ApiResponse.Error(400, "消息发送失败"));
            }
        }

        /// <summary>
        /// 调用发送群消息
        /// </summary>
        /// <param name="groupId">发送的群</param>
        /// <param name="message">发送的消息</param>
        /// <returns>返回的消息ID, 不为0时为成功</returns>
        private static async Task<int> CallGroupMsgSendAsync(long groupId, string message)
        {
            int msgId = 0;
            await Task.Run(() =>
            {
                Stopwatch sw = Stopwatch.StartNew();
                int logId = LogHelper.WriteLog(Model.Enums.LogLevel.InfoSend, "[↑]发送群组消息", $"群:{groupId} 消息:{message}", "处理中...");
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
        private static async Task<int> CallPrivateMsgSendAsync(long qq, string message)
        {
            int msgId = 0;
            await Task.Run(() =>
            {
                Stopwatch sw = Stopwatch.StartNew();
                int logId = LogHelper.WriteLog(Model.Enums.LogLevel.InfoSend, "[↑]发送私聊消息", $"QQ:{qq} 消息:{message}", "处理中...");
                msgId = ProtocolManager.Instance.CurrentProtocol.SendPrivateMessage(qq, message);
                sw.Stop();
                LogHelper.UpdateLogStatus(logId, $"√ {sw.ElapsedMilliseconds / 1000.0:f2} s");
            });
            return msgId;
        }
    }
}
