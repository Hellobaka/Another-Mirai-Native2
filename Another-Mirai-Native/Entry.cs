﻿using Another_Mirai_Native.Config;
using Another_Mirai_Native.DB;
using Another_Mirai_Native.Model.Enums;
using Another_Mirai_Native.Native;
using Another_Mirai_Native.RPC;
using Another_Mirai_Native.RPC.Interface;
using System.Diagnostics;
using System.Text;

namespace Another_Mirai_Native
{
    public class Entry
    {
        private static readonly ManualResetEvent _quitEvent = new(false);

        private static Thread UIThread { get; set; }

        private static NotifyIcon NotifyIcon { get; set; }

        private static ToolStripMenuItem TaskBarMenuParent { get; set; }

        private static IntPtr ConsoleHandle { get; set; }

        public static event Action ServerStarted;

        // 定义启动参数:
        // 无参时作为框架主体启动
        // -PID 核心进程PID
        // -AutoExit 核心进程退出时主动退出
        // -Path 欲加载的插件路径
        // -WS 核心WS路径
        public static void Main(string[] args)
        {
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
                if (!AppConfig.Instance.AutoConnect)
                {
                    Console.WriteLine();
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

        private static void BuildTaskBar()
        {
            if (UIThread == null)
            {
                UIThread = new Thread(() =>
                {
                    ConsoleHandle = WinNative.GetConsoleWindow();
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    WinNative.SetProcessDPIAware();

                    NotifyIcon = new NotifyIcon();
                    NotifyIcon.Icon = new Icon(new MemoryStream(Convert.FromBase64String(Resources.TaskBarIconResources.IconBase64)));
                    var menu = new ContextMenuStrip();
                    NotifyIcon.ContextMenuStrip = menu;

                    menu.Items.Add(new ToolStripMenuItem { Text = $"{AppConfig.Instance.CurrentNickName}({AppConfig.Instance.CurrentQQ})" });
                    menu.Items.Add("-");
                    menu.Items.Add(new ToolStripMenuItem { Text = $"框架版本: {ServerManager.Server.GetCoreVersion()}" });
                    menu.Items.Add("-");
                    TaskBarMenuParent = new ToolStripMenuItem() { Text = "应用" };
                    menu.Items.Add(TaskBarMenuParent);
                    menu.Items.Add("-");
                    ToolStripMenuItem reloadItem = new() { Text = "重载插件" };
                    reloadItem.Click += async (a, b) =>
                    {
                        await Task.Run(() =>
                        {
                            if (MessageBox.Show("确定要重载插件吗？", "嗯？", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                            {
                                PluginManagerProxy.Instance.ReloadAllPlugins();
                            }
                        });
                    };
                    menu.Items.Add(reloadItem);
                    ToolStripMenuItem exitItem = new() { Text = "退出" };
                    exitItem.Click += async (a, b) =>
                    {
                        await Task.Run(() =>
                        {
                            if (MessageBox.Show("确定要退出框架吗？", "嗯？", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                            {
                                Environment.Exit(0);
                            }
                        });
                    };
                    menu.Items.Add(exitItem);

                    NotifyIcon.Text = $"{AppConfig.Instance.CurrentNickName}({AppConfig.Instance.CurrentQQ})\n已启用 {PluginManagerProxy.Proxies.Count(x => x.Enabled)} 个插件";
                    NotifyIcon.Visible = true;
                    NotifyIcon.DoubleClick += (_, _) => WinNative.SetForegroundWindow(ConsoleHandle);
                    RebuildTaskBarMenu();
                    PluginManagerProxy.OnPluginEnableChanged -= (_) => Invoke(RebuildTaskBarMenu);
                    PluginManagerProxy.OnPluginEnableChanged += (_) => Invoke(RebuildTaskBarMenu);
                    Application.Run();
                });
                UIThread.SetApartmentState(ApartmentState.STA);
                UIThread.Start();
            }
        }

        private static void RebuildTaskBarMenu()
        {
            NotifyIcon.Text = $"{AppConfig.Instance.CurrentNickName}({AppConfig.Instance.CurrentQQ})\n已启用 {PluginManagerProxy.Proxies.Count(x => x.Enabled)} 个插件";
            TaskBarMenuParent.DropDownItems.Clear();
            foreach (var item in PluginManagerProxy.Proxies.OrderBy(x => x.PluginName))
            {
                ToolStripMenuItem menuItem = new() { Text = $"{item.PluginName}", Tag = item };
                ToolStripMenuItem enableItem = new() { Text = item.Enabled ? "√ 启用" : "启用", Tag = item };
                ToolStripMenuItem disableItem = new() { Text = !item.Enabled ? "√ 禁用" : "禁用", Tag = item };
                enableItem.Click += async (sender, b) =>
                {
                    await Task.Run(() =>
                    {
                        if (sender is not ToolStripMenuItem selectItem || selectItem.Tag is not CQPluginProxy plugin || plugin.Enabled)
                        {
                            return;
                        }

                        if (PluginManagerProxy.Instance.SetPluginEnabled(plugin, true))
                        {
                            Invoke(() =>
                            {
                                enableItem.Text = "√ 启用";
                                disableItem.Text = "禁用";
                            });

                            AppConfig.Instance.AutoEnablePlugin.Add(plugin.PluginName);
                            AppConfig.Instance.AutoEnablePlugin = AppConfig.Instance.AutoEnablePlugin.Distinct().ToList();
                            AppConfig.Instance.SetConfig("AutoEnablePlugins", AppConfig.Instance.AutoEnablePlugin);
                        }
                        else
                        {
                            MessageBox.Show("插件启用失败", "啊嘞？", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    });
                };
                disableItem.Click += async (sender, b) =>
                {
                    await Task.Run(() =>
                    {
                        if (sender is not ToolStripMenuItem selectItem || selectItem.Tag is not CQPluginProxy plugin || !plugin.Enabled)
                        {
                            return;
                        }

                        if (PluginManagerProxy.Instance.SetPluginEnabled(plugin, false))
                        {
                            Invoke(() =>
                            {
                                enableItem.Text = "启用";
                                disableItem.Text = "√ 禁用";
                            });

                            AppConfig.Instance.AutoEnablePlugin.Remove(plugin.PluginName);
                            AppConfig.Instance.AutoEnablePlugin = AppConfig.Instance.AutoEnablePlugin.Distinct().ToList();
                            AppConfig.Instance.SetConfig("AutoEnablePlugins", AppConfig.Instance.AutoEnablePlugin);
                        }
                        else
                        {
                            MessageBox.Show("插件禁用失败", "啊嘞？", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    });
                };
                menuItem.DropDownItems.Add(enableItem);
                menuItem.DropDownItems.Add(disableItem);
                if (item.AppInfo.menu.Length > 0)
                {
                    menuItem.DropDownItems.Add("-");
                }
                foreach (var subMenu in item.AppInfo.menu)
                {
                    ToolStripMenuItem subMenuItem = new() { Text = subMenu.name, Tag = item };
                    subMenuItem.Click += async (sender, b) =>
                    {
                        await Task.Run(() =>
                         {
                             if (sender is not ToolStripMenuItem selectItem || selectItem.Tag is not CQPluginProxy plugin)
                             {
                                 return;
                             }

                             if (plugin.Enabled is false)
                             {
                                 MessageBox.Show("当前插件未启用，无法调用窗口事件", "嗯哼", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                 return;
                             }
                             PluginManagerProxy.Instance.InvokeEvent(plugin, PluginEventType.Menu, subMenu.function);
                         });
                    };
                    menuItem.DropDownItems.Add(subMenuItem);
                }

                TaskBarMenuParent.DropDownItems.Add(menuItem);
            }
        }

        private static void Invoke(Action action)
        {
            if (NotifyIcon.ContextMenuStrip != null && NotifyIcon.ContextMenuStrip.InvokeRequired)
            {
                NotifyIcon.ContextMenuStrip.BeginInvoke(action);
            }
            else
            {
                action.Invoke();
            }
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