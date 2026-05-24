using System.ComponentModel;
using Another_Mirai_Native.Native;
using Another_Mirai_Native.Model.Enums;

namespace Another_Mirai_Native.WebAPI.Models
{
    [Description("插件信息")]
    public class PluginDto
    {
        [Description("插件授权码，用于 API 路由中标识插件")]
        public int AuthCode { get; set; }

        [Description("是否已启用")]
        public bool Enabled { get; set; }

        [Description("插件标识符")]
        public string PluginId { get; set; } = string.Empty;

        [Description("插件名称")]
        public string PluginName { get; set; } = string.Empty;

        [Description("插件作者")]
        public string Author { get; set; } = string.Empty;

        [Description("插件描述")]
        public string Description { get; set; } = string.Empty;

        [Description("插件版本")]
        public string Version { get; set; } = string.Empty;

        [Description("插件申请的 API 权限列表")]
        public int[] Auth { get; set; } = [];

        [Description("插件类型")]
        public PluginType PluginType { get; set; }

        public static PluginDto CreateFromPlugin(CQPluginProxy plugin)
        {
            return new PluginDto
            {
                AuthCode = plugin.AppInfo.AuthCode,
                Enabled = plugin.Enabled,
                PluginId = plugin.PluginDisplayId,
                PluginName = plugin.PluginName,
                Author = plugin.Author,
                Description = plugin.Description,
                Version = plugin.Version,
                Auth = plugin.AppInfo.auth,
                PluginType = plugin.PluginType
            };
        }
    }
}
