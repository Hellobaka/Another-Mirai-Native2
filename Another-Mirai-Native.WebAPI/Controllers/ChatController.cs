using Another_Mirai_Native.Abstractions.Enums;
using Another_Mirai_Native.Abstractions.Models.MessageItem;
using Another_Mirai_Native.Config;
using Another_Mirai_Native.DB;
using Another_Mirai_Native.Model;
using Another_Mirai_Native.Native;
using Another_Mirai_Native.WebAPI.Models;
using Azure.Core;
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
            if (WebUIConfig.Instance.EnableChat is false)
            {
                return NotFound(ApiResponse.Error(404, "聊天功能未启用"));
            }
            var categories = (await Task.Run(ChatHistoryHelper.GetHistoryCategories)).Select(ChatCategoryDto.CreateFromChatCategoryEntity);
            return Ok(ApiResponse.Ok(categories));
        }

        [HttpGet("history")]
        public async Task<IActionResult> GetChatHistory(ChatHistoryType chatHistoryType, long parentId, int pageIndex = 1, int pageSize = 50)
        {
            if (WebUIConfig.Instance.EnableChat is false)
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
            if (WebUIConfig.Instance.EnableChat is false)
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
            if (WebUIConfig.Instance.EnableChat is false)
            {
                return NotFound(ApiResponse.Error(404, "聊天功能未启用"));
            }
            await Task.Run(() => ChatHistoryHelper.SetUnreadCount(parentId, chatHistoryType, 0));
            return Ok(ApiResponse.Ok());
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendMessage([FromBody] ChatMessageSendRequest request)
        {
            if (WebUIConfig.Instance.EnableChat is false)
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

        [HttpGet("message-chain")]
        public IActionResult ConvertToMessageChain(string message)
        {
            if (WebUIConfig.Instance.EnableChat is false)
            {
                return NotFound(ApiResponse.Error(404, "聊天功能未启用"));
            }
            return Ok(ApiResponse.Ok(new { Message = message.ToMessageChain() }));
        }

        [HttpPost("recall")]
        public async Task<IActionResult> RecallMessage(ChatHistoryType chatHistoryType, long parentId, int messageId)
        {
            if (WebUIConfig.Instance.EnableChat is false)
            {
                return NotFound(ApiResponse.Error(404, "聊天功能未启用"));
            }
            if (chatHistoryType != ChatHistoryType.Group && chatHistoryType != ChatHistoryType.Private)
            {
                return BadRequest(ApiResponse.Error(400, "发送目标类型无效"));
            }

            var message = await Task.Run(() => ChatHistoryHelper.GetHistoriesByMsgId(parentId, messageId, chatHistoryType));
            if (message == null)
            {
                return NotFound(ApiResponse.Error(404, "未找到对应的聊天消息"));
            }

            bool success = (await Task.Run(() => ProtocolManager.Instance.CurrentProtocol.DeleteMsg(messageId))) == 1;
            if (success)
            {
                message.Recalled = true;
                ChatHistoryHelper.UpdateHistory(message);
                if (chatHistoryType == ChatHistoryType.Group)
                {
                    PluginManagerProxy.Instance.Event_OnGroupMsgRecall(messageId, parentId, message.Message);
                }
                else
                {
                    PluginManagerProxy.Instance.Event_OnPrivateMsgRecall(messageId, parentId, message.Message);
                }
                return Ok(ApiResponse.Ok());
            }
            else
            {
                return BadRequest(ApiResponse.Error(400, "撤回消息失败"));
            }
        }

        [HttpGet("collected")]
        public async Task<IActionResult> GetCollectedPictures()
        {
            if (WebUIConfig.Instance.EnableChat is false)
            {
                return NotFound(ApiResponse.Error(404, "聊天功能未启用"));
            }
            var dir = Path.Combine(Helper.GetCacheDirectoryByCachedFileType(Model.Enums.CachedFileType.Image, false), "collected");
            if (Directory.Exists(dir))
            {
                string[] extensions = { ".png", ".jpg", ".jpeg", ".gif" };
                var files = Directory.GetFiles(dir).Where(f => extensions.Any(ext => string.Equals(Path.GetExtension(f), ext, StringComparison.OrdinalIgnoreCase))).Select(Path.GetFileName).ToArray();
                return Ok(ApiResponse.Ok(files));
            }
            else
            {
                return Ok(ApiResponse.Ok(Array.Empty<string>()));
            }
        }

        [HttpPost("collect")]
        public async Task<IActionResult> CollectPicture(string file)
        {
            if (WebUIConfig.Instance.EnableChat is false)
            {
                return NotFound(ApiResponse.Error(404, "聊天功能未启用"));
            }
            try
            {
                var collectPath = Path.Combine(Helper.GetCacheDirectoryByCachedFileType(Model.Enums.CachedFileType.Image, false), "collected");
                Directory.CreateDirectory(collectPath);
                string cachePath = Helper.GetCacheDirectoryByCachedFileType(Model.Enums.CachedFileType.Image);
                var img = CachedFile.GetCachedFileByHash(Model.Enums.CachedFileType.Image, file);
                string filePath = Path.Combine(cachePath, img?.FileName ?? "");
                if (img == null || !System.IO.File.Exists(filePath))
                {
                    return NotFound(ApiResponse.Error(404, "找不到此哈希对应的缓存文件；可能未缓存或已删除"));
                }

                System.IO.File.Copy(filePath, Path.Combine(collectPath, img.FileName));
                return Ok(ApiResponse.Ok(img.FileName));
            }
            catch (Exception e)
            {
                return StatusCode(500, ApiResponse.Error(500, $"由于服务器内部错误，收藏图片失败: {e.Message}"));
            }
        }

        [HttpPost("upload-picture")]
        public async Task<IActionResult> UploadPicture(IFormFile file)
        {
            if (WebUIConfig.Instance.EnableChat is false)
            {
                return NotFound(ApiResponse.Error(404, "聊天功能未启用"));
            }
            if (file == null || file.Length == 0)
            {
                return BadRequest(ApiResponse.Error(400, "请选择要上传的图片文件"));
            }
            var allowedExtensions = new[] { ".png", ".jpg", ".jpeg", ".gif" };
            var extension = Path.GetExtension(file.FileName).ToLower();
            if (!allowedExtensions.Contains(extension))
            {
                return BadRequest(ApiResponse.Error(400, "仅支持 PNG、JPG、JPEG 和 GIF 格式的图片文件"));
            }
            try
            {
                var cachePath = Helper.GetCacheDirectoryByCachedFileType(Model.Enums.CachedFileType.Image);
                Directory.CreateDirectory(cachePath);
                var fileName = $"{Guid.NewGuid()}{extension}";
                var filePath = Path.Combine(cachePath, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                return Ok(ApiResponse.Ok(new { item = new Image(filePath: "cached\\" + fileName, isFlash: false, isEmoji: false) }));
            }
            catch (Exception e)
            {
                return StatusCode(500, ApiResponse.Error(500, $"由于服务器内部错误，上传图片失败: {e.Message}"));
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
