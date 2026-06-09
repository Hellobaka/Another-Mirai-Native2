using System.ComponentModel;

namespace Another_Mirai_Native.WebAPI.Models
{
    [Description("登录 / Token 刷新响应")]
    public class LoginResponseData
    {
        [Description("JWT Token 字符串")]
        public string Token { get; set; } = string.Empty;

        [Description("Token 过期时间（UTC）")]
        public DateTime ExpiresAt { get; set; }
    }
}
