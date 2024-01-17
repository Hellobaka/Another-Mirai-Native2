using Another_Mirai_Native.Config;
using Another_Mirai_Native.DB;
using Another_Mirai_Native.Native;
using Another_Mirai_Native.RPC;
using Another_Mirai_Native.RPC.Interface;
using System.Diagnostics;
using System.IO;

namespace Another_Mirai_Native
{
    public class Entry
    {
        private static readonly ManualResetEvent _quitEvent = new(false);

        // 定义启动参数:
        // 无参时作为框架主体启动
        // -PID 核心进程PID
        // -AutoExit 核心进程退出时主动退出
        // -Path 欲加载的插件路径
        // -WS 核心WS路径
        public static void Main(string[] args)
        {
            Console.CancelKeyPress += (sender, eArgs) =>
            {
                _quitEvent.Set();
                eArgs.Cancel = true;
            };
            // 创建初始文件夹
            CreateInitFolders();
            // 重定向异常
            InitExceptionCapture();
            PrintSystemInfo();
            // 加载配置
            AppConfig.Instance.IsCore = args.Length == 0;
            if (AppConfig.Instance.DebugMode && !AppConfig.Instance.IsCore)
            {
                Console.WriteLine($"Args: {string.Join(" ", args)}");
            }

            if (AppConfig.Instance.UseDatabase && File.Exists(LogHelper.GetLogFilePath()) is false)
            {
                LogHelper.CreateDB();
            }
            if (args.Length == 0)
            {
                // 启动服务器
                ServerManager serverManager = new();
                if (serverManager.Build(AppConfig.Instance.ServerType) is false)
                {
                    LogHelper.Debug("初始化", "构建服务器失败");
                    return;
                }
                if (ServerManager.Server.SetConnectionConfig() is false)
                {
                    LogHelper.Debug("初始化", "初始化连接参数失败，请检查配置内容");
                    return;
                }
                if (!ServerManager.Server.Start())
                {
                    LogHelper.Debug("初始化", "构建服务器失败");
                    return;
                }
                // 若配置无需UI则自动连接之后加载插件
                if (AppConfig.Instance.AutoConnect)
                {
                    if (!new ProtocolManager().Start(AppConfig.Instance.AutoProtocol))
                    {
                        return;
                    }
                    LogHelper.Info("加载插件", $"配置中启动启用插件为 {AppConfig.Instance.AutoEnablePlugin.Count} 个，开始加载...");
                    if (!new PluginManagerProxy().LoadPlugins())
                    {
                        return;
                    }
                    int count = 0;
                    foreach (var item in PluginManagerProxy.Proxies)
                    {
                        if (AppConfig.Instance.AutoEnablePlugin.Contains(item.PluginName))
                        {
                            item.Load();
                            UpdateConsoleTitle($"Another-Mirai-Native2 控制台版本-核心 加载了 {++count} 个插件");
                        }
                    }
                }
            }
            else
            {
                UpdateConsoleTitle("Another-Mirai-Native2 控制台版本-插件");
                // 解析参数
                ParseArg(args);
                // 监控核心进程
                MonitorCoreProcess(AppConfig.Instance.Core_PID);
                // 连接核心服务器
                ClientManager clientManager = new();
                if (!clientManager.Build(AppConfig.Instance.ServerType))
                {
                    LogHelper.Debug("初始化", "构建客户端失败");
                    return;
                }
                if (ClientManager.Client.SetConnectionConfig() is false)
                {
                    LogHelper.Debug("初始化", "初始化连接参数失败，请检查配置内容");
                    return;
                }
                if (!ClientManager.Client.Connect())
                {
                    LogHelper.Debug("初始化", "连接服务器失败");
                    return;
                }
                // 加载插件
                if (!new PluginManager().Load(AppConfig.Instance.Core_PluginPath))
                {
                    return;
                }
                UpdateConsoleTitle($"[{ClientBase.PID}]Another-Mirai-Native2 控制台版本-插件 [{PluginManager.LoadedPlugin.Name}]");
            }
            _quitEvent.WaitOne();
        }

        private static void PrintSystemInfo()
        {
            UpdateConsoleTitle("Another-Mirai-Native2 控制台版本");
            Console.WriteLine($"框架版本: {AppConfig.Instance.GetType().Assembly.GetName().Version}");
        }

        private static void UpdateConsoleTitle(string title)
        {
            if (Console.LargestWindowWidth > 0)
            {
                Console.Title = title;
            }
        }

        public static void InitExceptionCapture()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception ex)
            {
                Helper.ShowErrorDialog(ex, false);
            }
        }

        public static void CreateInitFolders()
        {
            Directory.CreateDirectory(@"data\app");
            Directory.CreateDirectory(@"data\plugins");
            Directory.CreateDirectory(@"data\image");
            Directory.CreateDirectory(@"data\record");
            Directory.CreateDirectory(@"logs");
            Directory.CreateDirectory("protocols");
        }

        public static void MonitorCoreProcess(int pid)
        {
            if (AppConfig.Instance.Core_AutoExit)
            {
                Task.Run(() =>
                {
                    try
                    {
                        var core = Process.GetProcessById(pid);
                        core.WaitForExit();
                        Environment.Exit(0);
                    }
                    catch
                    {
                        Environment.Exit(0);
                    }
                });
            }
        }

        private static void ParseArg(string[] args)
        {
            for (int i = 0; i < args.Length; i += 2)
            {
                if (args[i].ToLower() == "-pid")
                {
                    AppConfig.Instance.Core_PID = int.Parse(args[i + 1]);
                }
                if (args[i].ToLower() == "-authcode")
                {
                    AppConfig.Instance.Core_AuthCode = int.Parse(args[i + 1]);
                }
                if (args[i].ToLower() == "-autoexit")
                {
                    AppConfig.Instance.Core_AutoExit = args[i + 1] == "True";
                }
                if (args[i].ToLower() == "-path")
                {
                    AppConfig.Instance.Core_PluginPath = args[i + 1];
                }
                if (args[i].ToLower() == "-ws")
                {
                    AppConfig.Instance.Core_WSURL = args[i + 1];
                }
            }
        }
    }
}