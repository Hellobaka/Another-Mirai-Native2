using Another_Mirai_Native.Config;
using Another_Mirai_Native.DB;
using Another_Mirai_Native.Native;
using Another_Mirai_Native.RPC;
using Another_Mirai_Native.RPC.Interface;
using System.Diagnostics;
using System.Text;

namespace Another_Mirai_Native
{
    public partial class Entry
    {
        private static readonly ManualResetEvent _quitEvent = new(false);

        public static event Action ServerStarted;

        // 定义启动参数:
        // 无参时作为框架主体启动
        // -PID 核心进程PID
        // -AutoExit 核心进程退出时主动退出
        // -Path 欲加载的插件路径
        // -WS 核心WS路径
        public static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;

            if (args.Length == 0)
            {
                Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
            }
            if (Directory.Exists("wwwroot"))
            {
                Helper.CreateDirectoryLink(@"wwwroot\image", @"data\image");
            }

            AppConfig.Instance.StartTime = DateTime.Now;

#if NET5_0_OR_GREATER
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
#endif

            // 创建初始文件夹
            CreateInitFolders();
            // 重定向异常
            InitExceptionCapture();
            PrintSystemInfo();
            ListenConsoleExit();
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
                    LogHelper.Error("初始化", "构建服务器失败");
                    return;
                }
                if (ServerManager.Server.SetConnectionConfig() is false)
                {
                    LogHelper.Error("初始化", "初始化连接参数失败，请检查配置内容");
                    return;
                }
                if (!ServerManager.Server.Start())
                {
                    LogHelper.Error("初始化", "构建服务器失败");
                    return;
                }
                ProtocolManager protocolManager = new();
                SetQRCodeAction(protocolManager);
                if (!AppConfig.Instance.AutoConnect)
                {
                    Console.WriteLine();
                    Console.WriteLine("[-]可用协议列表：");
                    foreach(var item in ProtocolManager.Protocols)
                    {
                        Console.WriteLine(item.Name);
                    }
                    Console.WriteLine("[-]当前配置不会自动连接协议，修改请前往 conf/Config.json 配置文件中，修改 AutoConnect 配置为 true。");
                    Console.WriteLine("[-]请输入 connect 以进行手动连接。");
                    while (true)
                    {
                        if (Console.ReadLine()?.ToLower() == "connect")
                        {
                            break;
                        }
                    }
                }
                // 若配置无需UI则自动连接之后加载插件
                if (!protocolManager.Start(AppConfig.Instance.AutoProtocol))
                {
                    return;
                }
                LogHelper.Info("加载插件", $"配置中启动启用插件为 {AppConfig.Instance.AutoEnablePlugin.Count} 个，开始加载...");
                if (!new PluginManagerProxy().LoadPlugins())
                {
                    return;
                }
                int count = 0;
                if (AppConfig.Instance.ShowTaskBar)
                {
                    BuildTaskBar();
                }
                foreach (var item in PluginManagerProxy.Proxies)
                {
                    if (AppConfig.Instance.AutoEnablePlugin.Contains(item.PluginName))
                    {
                        if (item.Load() && PluginManagerProxy.Instance.SetPluginEnabled(item, true))
                        {
                            LogHelper.Info("加载插件", $"{item.PluginName} 启动完成");
                            UpdateConsoleTitle($"Another-Mirai-Native2 加载了 {++count} 个插件");
                        }
                    }
                }
                LogHelper.Info("加载插件", $"插件启动完成，开始处理事件");
                PluginManagerProxy.Instance.OnPluginLoaded();
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
                    LogHelper.Error("初始化", "构建客户端失败");
                    return;
                }
                if (ClientManager.Client.SetConnectionConfig() is false)
                {
                    LogHelper.Error("初始化", "初始化连接参数失败，请检查配置内容");
                    return;
                }
                if (!ClientManager.Client.Connect())
                {
                    LogHelper.Error("初始化", "连接服务器失败");
                    return;
                }
                // 加载插件
                if (!new PluginManager().Load(AppConfig.Instance.Core_PluginPath))
                {
                    return;
                }
                UpdateConsoleTitle($"[{ClientBase.PID}] [{PluginManager.LoadedPlugin.PluginName}]");
            }
            ChatHistoryHelper.Initialize();
            ServerStarted?.Invoke();
            _quitEvent.WaitOne();
        }

        private static void ListenConsoleExit()
        {
            var listen = new WinNative.ConsoleEventDelegate((e) =>
            {
                if (e == WinNative.CTRL_CLOSE_EVENT || e == WinNative.CTRL_C_EVENT)
                {
                    Console.WriteLine("Exiting...");
                    _quitEvent.Set();
                    if (AppConfig.Instance.ShowTaskBar)
                    {
                        Invoke(() => Environment.Exit(0));
                    }
                }
                return true;
            });
            WinNative.SetConsoleCtrlHandler(listen, true);
        }

        private static void PrintSystemInfo()
        {
            UpdateConsoleTitle("Another-Mirai-Native2 控制台版本");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Another-Mirai-Native2 控制台版本 [https://github.com/Hellobaka/Another-Mirai-Native2]");
            Console.WriteLine($"· 框架版本: {AppConfig.Instance.GetType().Assembly.GetName().Version}");
            Console.WriteLine($"· 调试模式: {AppConfig.Instance.DebugMode}");
            Console.WriteLine($"· 服务类型: {AppConfig.Instance.ServerType}");
            Console.WriteLine($"· 连接协议: {AppConfig.Instance.AutoProtocol}");
            Console.WriteLine();
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
            string basePath = AppDomain.CurrentDomain.BaseDirectory;
            Directory.CreateDirectory(Path.Combine(basePath, "data", "app"));
            Directory.CreateDirectory(Path.Combine(basePath, "data", "plugins"));
            Directory.CreateDirectory(Path.Combine(basePath, "data", "image"));
            Directory.CreateDirectory(Path.Combine(basePath, "data", "record"));
            Directory.CreateDirectory(Path.Combine(basePath, "logs"));
            Directory.CreateDirectory(Path.Combine(basePath, "protocols"));
            Directory.CreateDirectory(Path.Combine(basePath, "loaders"));
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
                    AppConfig.Instance.Core_PluginPath = args[i + 1].Replace("\"", "");
                }
                if (args[i].ToLower() == "-ws")
                {
                    AppConfig.Instance.Core_WSURL = args[i + 1];
                }
                if (args[i].ToLower() == "-qq")
                {
                    AppConfig.Instance.CurrentQQ = long.Parse(args[i + 1]);
                }
            }
        }
    }
}