using Another_Mirai_Native.Model;
using Another_Mirai_Native.WebSocket;

namespace Another_Mirai_Native.Native
{
    public class PluginManager
    {
        public PluginManager()
        {
            Instance = this;
        }

        public static PluginManager Instance { get; private set; }

        public static CQPlugin LoadedPlugin { get; private set; }

        public bool Load(string pluginPath)
        {
            if (!File.Exists(pluginPath))
            {
                LogHelper.Error("加载插件", $"{pluginPath} 文件不存在");
                return false;
            }
            CQPlugin plugin = new(pluginPath);
            var ret = plugin.Load();
            if (ret)
            {
                LoadedPlugin = plugin;
                Client.Instance.Send(new InvokeResult() { Type = "PluginInfo", Result = LoadedPlugin.AppInfo }.ToJson());
            }
            return ret;
        }
    }
}