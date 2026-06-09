using Another_Mirai_Native.WebAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel;
using System.Text;

namespace Another_Mirai_Native.WebAPI.Controllers
{
    [ApiController]
    [Route("/api/auth")]
    [Authorize]
    public class AuthController(ILogger<AuthController> logger) : ControllerBase
    {
        private readonly ILogger<AuthController> _logger = logger;

        public static string CurrentPassword => WebAPIConfig.Instance.Password.PadRight(32, '~');

        [HttpPost("login")]
        [AllowAnonymous]
        [EnableRateLimiting("login")]
        [EndpointSummary("密码登录")]
        [EndpointDescription("使用 WebUI 配置文件中的密码验证身份，成功则返回 7 天有效的 JWT Token")]
        [ProducesResponseType(typeof(ApiResponse<LoginResponseData>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
        public IActionResult Login([Description("包含密码字段的请求体")][FromBody] LoginRequest request)
        {
            _logger.LogInformation("登录请求来自 {IP}", HttpContext.Connection.RemoteIpAddress);
            if (request.Password == WebAPIConfig.Instance.Password)
            {
                _logger.LogInformation("登录成功");
                return Ok(ApiResponse.Ok(CreateLoginResponse()));
            }
            _logger.LogWarning("登录失败：密码错误，尝试密码「{Password}」，来源 IP: {IP}", request.Password, HttpContext.Connection.RemoteIpAddress);
            return Unauthorized(ApiResponse.Error(401, "密码错误"));
        }

        [HttpGet("refresh")]
        [EndpointSummary("刷新 Token")]
        [EndpointDescription("验证当前 Token 有效后，重新签发新的 JWT Token 延长有效期")]
        [ProducesResponseType(typeof(ApiResponse<LoginResponseData>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
        public IActionResult Refresh()
        {
            _logger.LogInformation("刷新 Token");
            return Ok(ApiResponse.Ok(CreateLoginResponse()));
        }

        private static LoginResponseData CreateLoginResponse()
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(CurrentPassword));
            var handler = new JsonWebTokenHandler();
            var expiresAt = DateTime.UtcNow.AddDays(7);
            var jwt = handler.CreateToken(new SecurityTokenDescriptor
            {
                Expires = expiresAt,
                SigningCredentials = new(key, "HS256")
            });
            return new LoginResponseData
            {
                Token = jwt,
                ExpiresAt = expiresAt
            };
        }
    }
}