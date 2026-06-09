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
    public class CachedFileController(ILogger<CachedFileController> logger) : ControllerBase
    {
        private readonly ILogger<CachedFileController> _logger = logger;

        private string? Token
        {
            get
            {
                var queryToken = HttpContext.Request.Query["access_token"].FirstOrDefault();
                if (!string.IsNullOrEmpty(queryToken)) return queryToken;

                var authHeader = HttpContext.Request.Headers.Authorization.FirstOrDefault();
                if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
                    return authHeader["Bearer ".Length..];

                return null;
            }
        }

        private string AppendAccessToken(string url)
        {
            var token = Token;
            return token != null ? $"{url}?access_token={Uri.EscapeDataString(token)}" : url;
        }

        [HttpGet("{type}/{*file}")]
        [EndpointSummary("获取缓存文件")]
        [EndpointDescription("若 file 含 '.' 则按文件名直接返回文件；否则按哈希查询缓存后重定向到文件路径")]
        [ProducesResponseType(StatusCodes.Status302Found)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public IActionResult GetFileOrCache(
            [Description("文件类型：image / record / video")] string type,
            [Description("文件名（含扩展名）或缓存哈希值")] string file)
        {
            _logger.LogInformation("获取缓存文件: Type={Type}, File={File}", type, file);
            if (!Enum.TryParse(type, true, out CachedFileType fileType))
            {
                _logger.LogWarning("获取缓存文件失败：无效的文件类型 Type={Type}", type);
                return BadRequest(ApiResponse.Error(400, "无效的文件类型"));
            }

            if (file.Contains('.'))
            {
                _logger.LogInformation("按文件名重定向: /external/{FileType}/{File}", fileType, file);
                return Redirect(AppendAccessToken($"/external/{fileType}/{EscapePath(file)}"));
            }

            var cache = CachedFile.GetCachedFileByHash(fileType, file);
            if (cache == null)
            {
                _logger.LogInformation("获取缓存文件失败：哈希对应的缓存文件不存在 Hash={Hash}", file);
                return NotFound(ApiResponse.Error(404, "找不到此哈希对应的缓存文件；可能未缓存或已删除"));
            }

            _logger.LogInformation("按哈希重定向: Hash={Hash}, FileName={FileName}", file, cache.FileName);
            return Redirect(AppendAccessToken($"/external/{fileType}/cached/{EscapePath(cache.FileName)}"));
        }

        private static string EscapePath(string path)
        {
            return string.Join("/", path.Split('/').Select(Uri.EscapeDataString));
        }
    }
}