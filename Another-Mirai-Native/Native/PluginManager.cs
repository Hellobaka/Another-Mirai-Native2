using Another_Mirai_Native.DB;
using Another_Mirai_Native.Model;
using Another_Mirai_Native.Model.Enums;
using System.Runtime.ExceptionServices;
using System.IO;
using System.Diagnostics;
using Another_Mirai_Native.RPC;
using Another_Mirai_Native.Native.Handler;
using Another_Mirai_Native.Config;

namespace Another_Mirai_Native.Native
{
    public class PluginManager
    {
        public PluginManager()
        {
            Instance = this;
        }

        public static PluginManager Instance { get; private set; }

        public PluginHandlerBase LoadedPlugin { get; private set; }

        public int CallEvent(PluginEventType eventName, object[] args)
        {
            return LoadedPlugin.CallEvent(eventName, args);
        }

        public bool Load(string pluginPath)
        {
            if (!File.Exists(pluginPath))
            {
                LogHelper.Error("加载插件", $"{pluginPath} 文件不存在");
                return false;
            }
            if (AppConfig.Instance.DebugMode && AppConfig.Instance.DebugLazyLoad)
            {
                Console.WriteLine("[-]LazyLoad已开启，请按回车以继续加载");
                Console.ReadLine();
            }
            if (Path.GetFileName(pluginPath).StartsWith("XiaoLiZi_"))
            {
                LoadedPlugin = new Handler.XiaoLiZi.Loader(pluginPath);
            }
            else
            {
                LoadedPlugin = new Handler.CoolQ.Loader(pluginPath);
            }
            var ret = LoadedPlugin.LoadPlugin();
            if (ret)
            {
                ClientManager.Client.ClientStartUp();
            }
            return ret;
        }
    }
}