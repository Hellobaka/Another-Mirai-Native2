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
                Console.WriteLine("[-]LazyLoad已开启，等待调试器中");
                if (Debugger.Launch())
                {
                    Console.WriteLine("[+]成功启动调试器");
                }
                else
                {
                    Console.WriteLine("[-]未成功启动调试器，请按回车以继续加载");
                    Console.ReadLine();
                }
            }
            string fileName = Path.GetFileName(pluginPath);
            if (fileName.StartsWith("XiaoLiZi_"))
            {
                LoadedPlugin = new Handler.XiaoLiZi.Loader(pluginPath);
            }
            else if (fileName.StartsWith("Native_"))
            {
                LoadedPlugin = new Handler.CSharp.Loader(pluginPath);
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