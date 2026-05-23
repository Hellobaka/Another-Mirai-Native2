using System.ComponentModel;

namespace Another_Mirai_Native.WebAPI.Models
{
    [Description("登录请求")]
    public class LoginRequest
    {
        [Description("WebUI 管理面板密码")]
        public string Password { get; set; } = string.Empty;
    }
}
