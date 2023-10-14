using Another_Mirai_Native.Config;
using Another_Mirai_Native.Model;
using Another_Mirai_Native.Model.Enums;
using System.Data;
using System.Diagnostics;
using System.Drawing;

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
            if (Proxies.Any(x => x.ConnectionID == id))
            {
                Proxies.Remove(Proxies.First(x => x.ConnectionID == id));
            }
        }

        public static CQPluginProxy GetProxyByAuthCode(int authCode)
        {
            return Proxies.FirstOrDefault(x => x.AppInfo.AuthCode == authCode);
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

        public InvokeResult Invoke(CQPluginProxy target, string function, params object[] args)
        {
            string guid = Guid.NewGuid().ToString();
            var r = target.Invoke(new InvokeBody { GUID = guid, Function = function, Args = args });
            if (!r.Success)
            {
                // Invoke Fail
                LogHelper.Error("InvokeFail", $"Function: {function}, Message: {r.Message}");
            }
            return r;
        }

        public int InvokeEvent(CQPluginProxy target, PluginEventType eventType, params object[] args)
        {
            var r = Invoke(target, $"InvokeEvent_{eventType}", args);
            return !r.Success ? 0 : Convert.ToInt32(r.Result);
        }

        public int Event_OnPrivateMsg(CQPluginProxy target, int subType, int msgId, long fromQQ, string msg, int font)
        {
            return InvokeEvent(target, PluginEventType.PrivateMsg, subType, msgId, fromQQ, msg, font);
        }

        public int Event_OnGroupMsg(CQPluginProxy target, int subType, int msgId, long fromGroup, long fromQQ, string fromAnonymous, string msg, int font)
        {
            return InvokeEvent(target, PluginEventType.GroupMsg, subType, msgId, fromGroup, fromQQ, fromAnonymous, msg, font);
        }

        public int Event_OnDiscussMsg(CQPluginProxy target, int subType, int msgId, long fromNative, long fromQQ, string msg, int font)
        {
            return InvokeEvent(target, PluginEventType.DiscussMsg, subType, msgId, fromNative, fromQQ, msg, font);
        }

        public int Event_OnUpload(CQPluginProxy target, int subType, int sendTime, long fromGroup, long fromQQ, string file)
        {
            return InvokeEvent(target, PluginEventType.Upload, subType, sendTime, fromGroup, fromQQ, file);
        }

        public int Event_OnAdminChange(CQPluginProxy target, int subType, int sendTime, long fromGroup, long beingOperateQQ)
        {
            return InvokeEvent(target, PluginEventType.AdminChange, subType, sendTime, fromGroup, beingOperateQQ);
        }

        public int Event_OnGroupMemberDecrease(CQPluginProxy target, int subType, int sendTime, long fromGroup, long fromQQ, long beingOperateQQ)
        {
            return InvokeEvent(target, PluginEventType.GroupMemberDecrease, subType, sendTime, fromGroup, fromQQ, beingOperateQQ);
        }

        public int Event_OnGroupMemberIncrease(CQPluginProxy target, int subType, int sendTime, long fromGroup, long fromQQ, long beingOperateQQ)
        {
            return InvokeEvent(target, PluginEventType.GroupMemberIncrease, subType, sendTime, fromGroup, fromQQ, beingOperateQQ);
        }

        public int Event_OnGroupBan(CQPluginProxy target, int subType, int sendTime, long fromGroup, long fromQQ, long beingOperateQQ, long duration)
        {
            return InvokeEvent(target, PluginEventType.GroupBan, subType, sendTime, fromGroup, fromQQ, beingOperateQQ, duration);
        }

        public int Event_OnFriendAdded(CQPluginProxy target, int subType, int sendTime, long fromQQ)
        {
            return InvokeEvent(target, PluginEventType.FriendAdded, subType, sendTime, fromQQ);
        }

        public int Event_OnFriendAddRequest(CQPluginProxy target, int subType, int sendTime, long fromQQ, string msg, string responseFlag)
        {
            return InvokeEvent(target, PluginEventType.FriendRequest, subType, sendTime, fromQQ, msg, responseFlag);
        }

        public int Event_OnGroupAddRequest(CQPluginProxy target, int subType, int sendTime, long fromGroup, long fromQQ, string msg, string responseFlag)
        {
            return InvokeEvent(target, PluginEventType.GroupAddRequest, subType, sendTime, fromGroup, fromQQ, msg, responseFlag);
        }

        public int Event_OnStartUp(CQPluginProxy target)
        {
            return InvokeEvent(target, PluginEventType.StartUp);
        }

        public int Event_OnExit(CQPluginProxy target)
        {
            return InvokeEvent(target, PluginEventType.Exit);
        }

        public int Event_OnEnable(CQPluginProxy target)
        {
            return InvokeEvent(target, PluginEventType.Enable);
        }

        public int Event_OnDisable(CQPluginProxy target)
        {
            return InvokeEvent(target, PluginEventType.Disable);
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