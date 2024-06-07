using Another_Mirai_Native.Native;

namespace Another_Mirai_Native.BlazorUI.Models
{
    public class CQPluginProxyWrapper
    {
        public CQPluginProxy TargetPlugin { get; set; }

        public bool Selected { get; set; }

        public CQPluginProxyWrapper(CQPluginProxy proxy)
        {
            TargetPlugin = proxy;
            if (string.IsNullOrEmpty(TargetPlugin.PluginId))
            {
                TargetPlugin.AppInfo.AppId = "启用插件以查看 AppId";
            }
        }
    }
}
