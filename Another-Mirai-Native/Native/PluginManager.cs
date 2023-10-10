using Another_Mirai_Native.Config;
using Another_Mirai_Native.WebSocket;
using System.Diagnostics;
using System.Security.Cryptography;

namespace Another_Mirai_Native.Native
{
    public class PluginManager
    {
        public PluginManager()
        {
            Instance = this;
            StartPluginMonitor();
        }

        public static PluginManager Instance { get; private set; }

        public static Dictionary<int, string> PluginProcess { get; private set; } = new();

        public static CQPlugin Plugins { get; private set; }

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
                Plugins = plugin;
                Client.Instance.Send(new { Type = "LoadPluginComplete", Content = plugin.AppInfo }.ToJson());
            }
            return ret;
        }

        public bool LoadAndStart()
        {
            int pid = Process.GetCurrentProcess().Id;
            foreach (var item in Directory.GetFiles(@"data\plugins", "*.dll"))
            {
                Process? pluginProcess = StartPluginProcess(pid, item);
                if (pluginProcess != null)
                {
                    PluginProcess.Add(pluginProcess.Id, item);
                }
            }
            return true;
        }

        private static Process? StartPluginProcess(int pid, string item)
        {
            string arguments = $"-PID {pid} -AutoExit {AppConfig.PluginExitWhenCoreExit} -Path {item} -WS {AppConfig.WebSocketURL}";
            Process? pluginProcess = Process.Start(new ProcessStartInfo
            {
                Arguments = arguments,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                FileName = $"{AppDomain.CurrentDomain.BaseDirectory}\\{AppDomain.CurrentDomain.FriendlyName}",
                WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory
            });
            Task.Run(() =>
            {
                while (pluginProcess?.StandardOutput != null && !pluginProcess.StandardOutput.EndOfStream)
                {
                    Console.WriteLine(pluginProcess.StandardOutput.ReadLine());
                }
            });
            return pluginProcess;
        }

        private void StartPluginMonitor()
        {
            try
            {
                var monitor = Task.Run(() =>
                {
                    while (true)
                    {
                        for (int i = 0; i < PluginProcess.Count; i++)
                        {
                            var plugin = PluginProcess.ElementAt(i);
                            try
                            {
                                _ = Process.GetProcessById(plugin.Key);
                            }
                            catch
                            {
                                LogHelper.Info("StartPluginMonitor", $"{plugin.Value} 进程不存在");
                                PluginProcess.Remove(plugin.Key);
                                if (AppConfig.RestartPluginIfDead)
                                {
                                    LogHelper.Info("StartPluginMonitor", $"{plugin.Value} 重启");
                                    Process? pluginProcess = StartPluginProcess(Process.GetCurrentProcess().Id, plugin.Value);
                                    if (pluginProcess != null)
                                    {
                                        PluginProcess.Add(pluginProcess.Id, plugin.Value);
                                    }
                                }
                            }
                        }
                        Thread.Sleep(1000);
                    }
                });
            }
            catch (AggregateException ex)
            {
                foreach (var item in ex.InnerExceptions)
                {
                    LogHelper.Error("StartPluginMonitor", ex);
                }
            }
        }
    }
}