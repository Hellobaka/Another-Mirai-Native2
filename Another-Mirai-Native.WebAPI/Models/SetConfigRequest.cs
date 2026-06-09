using System.ComponentModel;
using System.Text.Json.Nodes;

namespace Another_Mirai_Native.WebAPI.Models
{
    [Description("修改配置请求")]
    public class SetConfigRequest
    {
        [Description("配置项键名")]
        public string Key { get; set; } = string.Empty;

        [Description("新值（类型自动适配）")]
        public JsonNode? Value { get; set; }
    }
}
