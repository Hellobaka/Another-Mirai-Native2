using Another_Mirai_Native.Config;
using Another_Mirai_Native.Model;
using Another_Mirai_Native.Model.Enums;
using System.Diagnostics;

namespace Another_Mirai_Native.Native
{
    public class PluginManagerProxy
    {
        public PluginManagerProxy()
        {
            Instance = this;
            StartPluginMonitor();
        }

        public static PluginManagerProxy Instance { get; private set; }

        public static Dictionary<int, string> PluginProcess { get; private set; } = new();

        public static List<CQPluginProxy> Proxies { get; private set; } = new();

        public static void RemoveProxy(Guid id)
        {
            if (Proxies.Any(x => x.ID == id))
            {
                Proxies.Remove(Proxies.First(x => x.ID == id));
            }
        }

        public bool LoadPlugins()
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

        public InvokeResult Invoke(CQPluginProxy proxy, string function, params object[] args)
        {
            string guid = Guid.NewGuid().ToString();
            var r = proxy.Invoke(new InvokeBody { GUID = guid, Function = function, Args = args });
            if (!r.Success)
            {
                // Invoke Fail
                LogHelper.Error("InvokeFail", $"Function: {function}, Message: {r.Message}");
            }
            return r;
        }

        public int InvokeEvent(CQPluginProxy proxy, PluginEventType eventType, params object[] args)
        {
            var r = Invoke(proxy, $"InvokeEvent_{eventType}", args);
            return !r.Success ? 0 : (int)r.Result;
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