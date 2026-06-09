using Another_Mirai_Native.Abstractions.Enums;
using Another_Mirai_Native.Abstractions.Models.MessageItem;
using Another_Mirai_Native.Config;
using Another_Mirai_Native.DB;
using Another_Mirai_Native.Model;
using Another_Mirai_Native.Native;
using Another_Mirai_Native.WebAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.Diagnostics;

namespace Another_Mirai_Native.WebAPI.Controllers
{
    [ApiController]
    [Route("/api/chat")]
    [Authorize]
    public class ChatController(ILogger<ChatController> logger) : ControllerBase
    {
        private readonly ILogger<ChatController> _logger = logger;
        [HttpGet("categories")]
        [EndpointSummary("获取聊天会话列表")]
        [EndpointDescription("获取所有聊天会话分类，按最后一条消息降序排列。message 字段为已解析的消息链，time 已转为 DateTime")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetChatCategories()
        {
            _logger.LogInformation("获取聊天会话列表");
            if (WebAPIConfig.Instance.EnableChat is false)
            {
                _logger.LogWarning("获取聊天会话列表失败：聊天功能未启用");
                return NotFound(ApiResponse.Error(404, "聊天功能未启用"));
            }
            var categories = (await Task.Run(ChatHistoryHelper.GetHistoryCategories)).Select(ChatCategoryDto.CreateFromChatCategoryEntity);
            _logger.LogInformation("获取聊天会话列表成功，共 {Count} 条", categories.Count());
            return Ok(ApiResponse.Ok(categories));
        }

        [HttpGet("history")]
        [EndpointSummary("分页查询聊天记录")]
        [EndpointDescription("按会话分页获取聊天记录")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetChatHistory(
            [Description("聊天类型：0=群聊，1=私聊")] ChatHistoryType chatHistoryType,
            [Description("群号或好友 QQ")] long parentId,
            [Description("页码，从 1 开始")] int pageIndex = 1,
            [Description("每页条数")] int pageSize = 50)
        {
            _logger.LogInformation("查询聊天记录: Type={Type}, ParentId={ParentId}, Page={Page}, Size={Size}", chatHistoryType, parentId, pageIndex, pageSize);
            if (WebAPIConfig.Instance.EnableChat is false)
            {
                _logger.LogWarning("查询聊天记录失败：聊天功能未启用");
                return NotFound(ApiResponse.Error(404, "聊天功能未启用"));
            }
            var history = await Task.Run(() => ChatHistoryHelper.GetHistoriesByPage(parentId, chatHistoryType, pageSize, pageIndex));
            var dto = history.Select(ChatHistoryDto.CreateFromChatHistoryEntity).ToList();
            _logger.LogInformation("查询聊天记录成功，返回 {Count} 条", dto.Count);
            return Ok(ApiResponse.Ok(dto));
        }

        [HttpGet("message")]
        [EndpointSummary("查询单条消息")]
        [EndpointDescription("按消息 ID 查询单条聊天记录")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetChatMessage(
            [Description("聊天类型：0=群聊，1=私聊")] ChatHistoryType chatHistoryType,
            [Description("群号或好友 QQ")] long parentId,
            [Description("消息 ID")] int messageId)
        {
            _logger.LogInformation("查询单条消息: Type={Type}, ParentId={ParentId}, MessageId={MessageId}", chatHistoryType, parentId, messageId);
            if (WebAPIConfig.Instance.EnableChat is false)
            {
                _logger.LogWarning("查询单条消息失败：聊天功能未启用");
                return NotFound(ApiResponse.Error(404, "聊天功能未启用"));
            }
            var message = await Task.Run(() => ChatHistoryHelper.GetHistoriesByMsgId(parentId, messageId, chatHistoryType));
            if (message == null)
            {
                _logger.LogInformation("查询单条消息失败：消息不存在 Type={Type}, ParentId={ParentId}, MessageId={MessageId}", chatHistoryType, parentId, messageId);
                return NotFound(ApiResponse.Error(404, "未找到对应的聊天消息"));
            }
            var dto = ChatHistoryDto.CreateFromChatHistoryEntity(message);
            _logger.LogInformation("查询单条消息成功");
            return Ok(ApiResponse.Ok(dto));
        }

        [HttpGet("friend-nick")]
        [EndpointSummary("获取好友昵称")]
        [EndpointDescription("根据 QQ 号查询好友昵称")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetFriendNick([Description("好友 QQ 号")] long qq)
        {
            _logger.LogInformation("获取好友昵称: QQ={QQ}", qq);
            string nick = await ChatHistoryHelper.GetFriendNick(qq);
            _logger.LogInformation("获取好友昵称成功: QQ={QQ}, Nick={Nick}", qq, nick);
            return Ok(ApiResponse.Ok(new { nick }));
        }

        [HttpGet("group-name")]
        [EndpointSummary("获取群名称")]
        [EndpointDescription("根据群号查询群名称")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetGroupName([Description("群号")] long groupId)
        {
            _logger.LogInformation("获取群名称: GroupId={GroupId}", groupId);
            string groupName = await ChatHistoryHelper.GetGroupName(groupId);
            _logger.LogInformation("获取群名称成功: GroupId={GroupId}, Name={Name}", groupId, groupName);
            return Ok(ApiResponse.Ok(new { groupName }));
        }

        [HttpGet("group-member-card")]
        [EndpointSummary("获取群成员名片")]
        [EndpointDescription("根据群号和 QQ 号查询群成员名片")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetGroupMemberCard(
            [Description("群号")] long groupId,
            [Description("群成员 QQ 号")] long qq)
        {
            _logger.LogInformation("获取群成员名片: GroupId={GroupId}, QQ={QQ}", groupId, qq);
            string card = await ChatHistoryHelper.GetGroupMemberNick(groupId, qq);
            _logger.LogInformation("获取群成员名片成功: GroupId={GroupId}, QQ={QQ}, Card={Card}", groupId, qq, card);
            return Ok(ApiResponse.Ok(new { card }));
        }

        [HttpPost("clear-unread")]
        [EndpointSummary("清除未读计数")]
        [EndpointDescription("将指定会话的未读消息计数清零")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ClearUnread(
            [Description("聊天类型：0=群聊，1=私聊")] ChatHistoryType chatHistoryType,
            [Description("群号或好友 QQ")] long parentId)
        {
            _logger.LogInformation("清除未读计数: Type={Type}, ParentId={ParentId}", chatHistoryType, parentId);
            if (WebAPIConfig.Instance.EnableChat is false)
            {
                _logger.LogWarning("清除未读计数失败：聊天功能未启用");
                return NotFound(ApiResponse.Error(404, "聊天功能未启用"));
            }
            await Task.Run(() => ChatHistoryHelper.SetUnreadCount(parentId, chatHistoryType, 0));
            _logger.LogInformation("清除未读计数成功: Type={Type}, ParentId={ParentId}", chatHistoryType, parentId);
            return Ok(ApiResponse.Ok());
        }

        [HttpPost("send")]
        [EndpointSummary("发送消息")]
        [EndpointDescription("发送群聊或私聊消息，content 使用原始 CQ 码格式。成功返回消息 ID")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> SendMessage(
            [Description("消息发送请求体")] [FromBody] ChatMessageSendRequest request)
        {
            _logger.LogInformation("发送消息: Type={Type}, ParentId={ParentId}, MessageLength={Length}", request.ChatType, request.ParentId, request.Message?.Length ?? 0);
            if (WebAPIConfig.Instance.EnableChat is false)
            {
                _logger.LogWarning("发送消息失败：聊天功能未启用");
                return NotFound(ApiResponse.Error(404, "聊天功能未启用"));
            }
            if (request.ParentId <= 0)
            {
                _logger.LogWarning("发送消息失败：发送目标无效 ParentId={ParentId}", request.ParentId);
                return BadRequest(ApiResponse.Error(400, "发送目标无效"));
            }
            if (string.IsNullOrEmpty(request.Message))
            {
                _logger.LogWarning("发送消息失败：消息内容为空");
                return BadRequest(ApiResponse.Error(400, "发送消息不得为空"));
            }
            if (request.ChatType != ChatHistoryType.Group && request.ChatType != ChatHistoryType.Private)
            {
                _logger.LogWarning("发送消息失败：发送目标类型无效 Type={Type}", request.ChatType);
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
                _logger.LogInformation("消息发送成功: Type={Type}, ParentId={ParentId}, MsgId={MsgId}", request.ChatType, request.ParentId, msgId);
                return Ok(ApiResponse.Ok(new { msgId }));
            }
            else
            {
                _logger.LogError("消息发送失败: Type={Type}, ParentId={ParentId}, 协议层返回 MsgId=0", request.ChatType, request.ParentId);
                return BadRequest(ApiResponse.Error(400, "消息发送失败"));
            }
        }

        [HttpGet("message-chain")]
        [EndpointSummary("CQ 码转消息链")]
        [EndpointDescription("将原始 CQ 码字符串解析为结构化的 MessageItemBase 数组，用于发送前预览")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        public IActionResult ConvertToMessageChain(
            [Description("原始 CQ 码字符串")] string message)
        {
            _logger.LogInformation("CQ 码转消息链: MessageLength={Length}", message?.Length ?? 0);
            if (WebAPIConfig.Instance.EnableChat is false)
            {
                _logger.LogWarning("CQ 码转消息链失败：聊天功能未启用");
                return NotFound(ApiResponse.Error(404, "聊天功能未启用"));
            }
            if (string.IsNullOrEmpty(message))
            {
                _logger.LogWarning("CQ 码转消息链失败：输入消息为空");
                return BadRequest(ApiResponse.Error(400, "输入消息不得为空"));
            }
            var chain = message.ToMessageChain();
            _logger.LogInformation("CQ 码转消息链成功，共 {Count} 个片段", chain.Length);
            return Ok(ApiResponse.Ok(new { Message = chain }));
        }

        [HttpPost("recall")]
        [EndpointSummary("撤回消息")]
        [EndpointDescription("撤回指定会话中的指定消息，成功后将消息标记为已撤回并触发插件事件")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RecallMessage(
            [Description("聊天类型：0=群聊，1=私聊")] ChatHistoryType chatHistoryType,
            [Description("群号或好友 QQ")] long parentId,
            [Description("消息 ID")] int messageId)
        {
            _logger.LogInformation("撤回消息: Type={Type}, ParentId={ParentId}, MessageId={MessageId}", chatHistoryType, parentId, messageId);
            if (WebAPIConfig.Instance.EnableChat is false)
            {
                _logger.LogWarning("撤回消息失败：聊天功能未启用");
                return NotFound(ApiResponse.Error(404, "聊天功能未启用"));
            }
            if (chatHistoryType != ChatHistoryType.Group && chatHistoryType != ChatHistoryType.Private)
            {
                _logger.LogWarning("撤回消息失败：发送目标类型无效 Type={Type}", chatHistoryType);
                return BadRequest(ApiResponse.Error(400, "发送目标类型无效"));
            }

            var message = await Task.Run(() => ChatHistoryHelper.GetHistoriesByMsgId(parentId, messageId, chatHistoryType));
            if (message == null)
            {
                _logger.LogWarning("撤回消息失败：消息不存在 Type={Type}, ParentId={ParentId}, MessageId={MessageId}", chatHistoryType, parentId, messageId);
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
                _logger.LogInformation("撤回消息成功: Type={Type}, ParentId={ParentId}, MessageId={MessageId}", chatHistoryType, parentId, messageId);
                return Ok(ApiResponse.Ok());
            }
            else
            {
                _logger.LogError("撤回消息失败：协议层返回失败 Type={Type}, ParentId={ParentId}, MessageId={MessageId}", chatHistoryType, parentId, messageId);
                return BadRequest(ApiResponse.Error(400, "撤回消息失败"));
            }
        }

        [HttpGet("collected")]
        [EndpointSummary("获取收藏图片列表")]
        [EndpointDescription("获取已收藏图片的文件名列表")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCollectedPictures()
        {
            _logger.LogInformation("获取收藏图片列表");
            if (WebAPIConfig.Instance.EnableChat is false)
            {
                _logger.LogWarning("获取收藏图片列表失败：聊天功能未启用");
                return NotFound(ApiResponse.Error(404, "聊天功能未启用"));
            }
            var dir = Path.Combine(Helper.GetCacheDirectoryByCachedFileType(Model.Enums.CachedFileType.Image, false), "collected");
            if (Directory.Exists(dir))
            {
                string[] extensions = { ".png", ".jpg", ".jpeg", ".gif" };
                var files = Directory.GetFiles(dir).Where(f => extensions.Any(ext => string.Equals(Path.GetExtension(f), ext, StringComparison.OrdinalIgnoreCase))).Select(Path.GetFileName).ToArray();
                _logger.LogInformation("获取收藏图片列表成功，共 {Count} 张", files.Length);
                return Ok(ApiResponse.Ok(files));
            }
            else
            {
                _logger.LogInformation("收藏目录不存在，返回空列表");
                return Ok(ApiResponse.Ok(Array.Empty<string>()));
            }
        }

        [HttpPost("collect")]
        [EndpointSummary("收藏图片")]
        [EndpointDescription("将已缓存的图片按哈希复制到收藏目录")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CollectPicture(
            [Description("图片缓存哈希")] string file)
        {
            _logger.LogInformation("收藏图片: Hash={Hash}", file);
            if (WebAPIConfig.Instance.EnableChat is false)
            {
                _logger.LogWarning("收藏图片失败：聊天功能未启用");
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
                    _logger.LogWarning("收藏图片失败：缓存文件不存在 Hash={Hash}", file);
                    return NotFound(ApiResponse.Error(404, "找不到此哈希对应的缓存文件；可能未缓存或已删除"));
                }

                System.IO.File.Copy(filePath, Path.Combine(collectPath, img.FileName));
                _logger.LogInformation("收藏图片成功: Hash={Hash}, FileName={FileName}", file, img.FileName);
                return Ok(ApiResponse.Ok(img.FileName));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "收藏图片异常: Hash={Hash}", file);
                return StatusCode(500, ApiResponse.Error(500, $"由于服务器内部错误，收藏图片失败: {e.Message}"));
            }
        }

        [HttpPost("upload-picture")]
        [EndpointSummary("上传图片")]
        [EndpointDescription("上传图片文件到 data/image 目录，返回构造好的 Image 消息片段。仅允许 PNG/JPG/JPEG/GIF 格式")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UploadPicture(
            [Description("图片文件（multipart/form-data）")] IFormFile file)
        {
            _logger.LogInformation("上传图片: FileName={FileName}, Size={Size}", file?.FileName, file?.Length);
            if (WebAPIConfig.Instance.EnableChat is false)
            {
                _logger.LogWarning("上传图片失败：聊天功能未启用");
                return NotFound(ApiResponse.Error(404, "聊天功能未启用"));
            }
            if (file == null || file.Length == 0)
            {
                _logger.LogWarning("上传图片失败：文件为空");
                return BadRequest(ApiResponse.Error(400, "请选择要上传的图片文件"));
            }
            var allowedExtensions = new[] { ".png", ".jpg", ".jpeg", ".gif" };
            var extension = Path.GetExtension(file.FileName).ToLower();
            if (!allowedExtensions.Contains(extension))
            {
                _logger.LogWarning("上传图片失败：格式不支持 FileName={FileName}, Extension={Extension}", file.FileName, extension);
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

                _logger.LogInformation("上传图片成功: Original={Original}, Saved={Saved}", file.FileName, fileName);
                return Ok(ApiResponse.Ok(new { item = new Image(filePath: "cached\\" + fileName, isFlash: false, isEmoji: false) }));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "上传图片异常: FileName={FileName}", file.FileName);
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
