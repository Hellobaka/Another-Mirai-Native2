using Another_Mirai_Native.WebAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Another_Mirai_Native.WebAPI.Controllers
{
    [ApiController]
    [Route("/api/auth")]
    [Authorize]
    public class AuthController : ControllerBase
    {
        [HttpPost("login")]
        [AllowAnonymous]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            if (request.Password == WebUIConfig.Instance.Password)
            {
                (string jwt, DateTime expiresAt) = CreateJwtToken();
                return Ok(ApiResponse.Ok(new
                {
                    Token = jwt,
                    ExpiresAt = expiresAt
                }));
            }
            return Unauthorized(ApiResponse.Error(401, "密码错误"));
        }

        [HttpGet("refresh")]
        public IActionResult Refresh()
        {
            (string jwt, DateTime expiresAt) = CreateJwtToken();
            return Ok(ApiResponse.Ok(new
            {
                Token = jwt,
                ExpiresAt = expiresAt
            }));
        }

        private (string, DateTime) CreateJwtToken()
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(WebUIConfig.Instance.Password));
            var handler = new JsonWebTokenHandler();
            var expiresAt = DateTime.UtcNow.AddDays(7);
            var jwt = handler.CreateToken(new SecurityTokenDescriptor
            {
                Expires = expiresAt,
                SigningCredentials = new(key, "HS256")
            });
            return (jwt, expiresAt);
        }
    }
}
