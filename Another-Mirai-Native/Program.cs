using System.Diagnostics;

namespace Another_Mirai_Native
{
    internal class Program
    {
        // 定义启动参数:
        // 无参时作为框架主体启动
        // -PID 核心进程PID
        // -AutoExit 核心进程退出时主动退出
        // -Path 欲加载的插件路径
        // -WS 核心WS路径
        private static void Main(string[] args)
        {
            // 加载配置
            AppConfig.LoadConfig();
            if (args.Length == 0)
            {
                // 启动WS服务器
                WebSocketServer.Start();
                // 若配置无需UI则自动连接之后加载插件
                if (AppConfig.AutoConnect)
                {
                    if (!new ProtocolManager().Start(AppConfig.AutoProtocol))
                    {
                        return;
                    }
                    if (!new PluginManager().LoadAndEnable())
                    {
                        return;
                    }
                }
            }
            else
            {
                // 解析参数
                ParseArg(args);
                // 监控核心进程
                MonitorCoreProcess(AppConfig.Core_PID);
                // 连接核心服务器
                if (!new WebSocketClient().Connect(AppConfig.Core_WSURL))
                {
                    return;
                }
                // 加载插件
                if (!new PluginManager().LoadAndEnable(AppConfig.Core_PluginPath))
                {
                    return;
                }
            }
            while (true)
            {
                string command = Console.ReadLine();
                switch (command.ToLower())
                {
                }
            }
        }

        private static void MonitorCoreProcess(int pid)
        {
            if (AppConfig.Core_AutoExit)
            {
                while (!WebSocketClient.ExitFlag)
                {
                    try
                    {
                        _ = Process.GetProcessById(pid);
                    }
                    catch
                    {
                        Environment.Exit(0);
                    }
                    Thread.Sleep(1000);
                }
            }
        }

        private static void ParseArg(string[] args)
        {
            for (int i = 0; i < args.Length; i += 2)
            {
                if (args[i].ToLower() == "-pid")
                {
                    AppConfig.Core_PID = short.Parse(args[i + 1]);
                }
                if (args[i].ToLower() == "-autoexit")
                {
                    AppConfig.Core_AutoExit = args[i + 1] == "1";
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