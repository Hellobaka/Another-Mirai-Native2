using Another_Mirai_Native.DB;
using Another_Mirai_Native.Model.Enums;
using Another_Mirai_Native.WebAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

namespace Another_Mirai_Native.WebAPI.Controllers
{
    [ApiController]
    [Route("/api/cache")]
    [Authorize]
    public class CachedFileController : ControllerBase
    {
        [HttpGet("{type}/{file}")]
        [EndpointSummary("获取缓存文件")]
        [EndpointDescription("若 file 含 '.' 则按文件名直接返回文件；否则按哈希查询缓存后重定向到文件路径")]
        [ProducesResponseType(StatusCodes.Status302Found)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public IActionResult GetFileOrCache(
            [Description("文件类型：image / record / video")] string type,
            [Description("文件名（含扩展名）或缓存哈希值")] string file)
        {
            if (!Enum.TryParse(type, true, out CachedFileType fileType))
                return BadRequest(ApiResponse.Error(400, "无效的文件类型"));

            if (file.Contains('.'))
                return Redirect($"/external/{fileType}/{Uri.EscapeDataString(file)}");

            var cache = CachedFile.GetCachedFileByHash(fileType, file);
            if (cache == null)
                return NotFound(ApiResponse.Error(404, "找不到此哈希对应的缓存文件；可能未缓存或已删除"));

            return Redirect($"/external/{fileType}/cached/{Uri.EscapeDataString(cache.FileName)}");
        }
    }
}