using Another_Mirai_Native.WebAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel;
using System.Text;

namespace Another_Mirai_Native.WebAPI.Controllers
{
    [ApiController]
    [Route("/api/auth")]
    [Authorize]
    public class AuthController : ControllerBase
    {
        public static string CurrentPassword => WebUIConfig.Instance.Password.PadRight(32, '~');

        [HttpPost("login")]
        [AllowAnonymous]
        [EndpointSummary("密码登录")]
        [EndpointDescription("使用 WebUI 配置文件中的密码验证身份，成功则返回 7 天有效的 JWT Token")]
        [ProducesResponseType(typeof(ApiResponse<LoginResponseData>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
        public IActionResult Login([Description("包含密码字段的请求体")][FromBody] LoginRequest request)
        {
            if (request.Password == WebUIConfig.Instance.Password)
            {
                return Ok(ApiResponse.Ok(CreateLoginResponse()));
            }
            return Unauthorized(ApiResponse.Error(401, "密码错误"));
        }

        [HttpGet("refresh")]
        [EndpointSummary("刷新 Token")]
        [EndpointDescription("验证当前 Token 有效后，重新签发新的 JWT Token 延长有效期")]
        [ProducesResponseType(typeof(ApiResponse<LoginResponseData>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
        public IActionResult Refresh()
        {
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