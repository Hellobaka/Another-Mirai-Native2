using System.ComponentModel;

namespace Another_Mirai_Native.WebAPI.Models
{
    [Description("单个配置项")]
    public class GetConfigResponseItem
    {
        [Description("配置项显示名称")]
        public string Title { get; set; } = string.Empty;

        [Description("配置项说明")]
        public string Description { get; set; } = string.Empty;

        [Description("值的数据类型")]
        public string Type => Value?.GetType().Name ?? "unknown";

        [Description("当前值")]
        public object? Value { get; set; }
    }
}
