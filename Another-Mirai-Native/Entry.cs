using Another_Mirai_Native.Config;
using Another_Mirai_Native.DB;
using Another_Mirai_Native.Model;
using Another_Mirai_Native.Native;
using Another_Mirai_Native.WebSocket;
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
            Console.WriteLine($"Args: {string.Join(" ", args)}");
            // 创建初始文件夹
            CreateInitFolders();
            // 重定向异常
            InitExceptionCapture();
            // 加载配置
            AppConfig.LoadConfig();
            AppConfig.IsCore = args.Length == 0;
            if (AppConfig.UseDatabase && File.Exists(LogHelper.GetLogFilePath()) is false)
            {
                LogHelper.CreateDB();
            }
            if (args.Length == 0)
            {
                // 启动WS服务器
                new Server().Start();
                // 若配置无需UI则自动连接之后加载插件
                if (AppConfig.AutoConnect)
                {
                    if (!new ProtocolManager().Start(AppConfig.AutoProtocol))
                    {
                        return;
                    }
                    if (!new PluginManagerProxy().LoadPlugins())
                    {
                        return;
                    }
                }
            }
            else
            {
                // Console.ReadKey();
                // 解析参数
                ParseArg(args);
                // 监控核心进程
                MonitorCoreProcess(AppConfig.Core_PID);
                // 连接核心服务器
                if (!new Client().Connect(AppConfig.Core_WSURL))
                {
                    return;
                }
                // 加载插件
                if (!new PluginManager().Load(AppConfig.Core_PluginPath))
                {
                    return;
                }
            }
            _quitEvent.WaitOne();
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
            if (AppConfig.Core_AutoExit)
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
                    AppConfig.Core_PID = int.Parse(args[i + 1]);
                }
                if (args[i].ToLower() == "-authcode")
                {
                    AppConfig.Core_AuthCode = int.Parse(args[i + 1]);
                }
                if (args[i].ToLower() == "-autoexit")
                {
                    AppConfig.Core_AutoExit = args[i + 1] == "True";
                }
                if (args[i].ToLower() == "-path")
                {
                    AppConfig.Core_PluginPath = args[i + 1];
                }
                if (args[i].ToLower() == "-ws")
                {
                    AppConfig.Core_WSURL = args[i + 1];
                }
            }
        }
    }
}