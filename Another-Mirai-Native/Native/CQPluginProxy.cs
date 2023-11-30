using Another_Mirai_Native.Config;
using Another_Mirai_Native.DB;
using Another_Mirai_Native.Model;
using Another_Mirai_Native.Model.Enums;
using Another_Mirai_Native.WebSocket;
using Fleck;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.IO;

namespace Another_Mirai_Native.Native
{
    public class CQPluginProxy
    {
        public CQPluginProxy()
        {
        }

        public CQPluginProxy(string dllPath)
        {
            PluginPath = dllPath;
        }

        public CQPluginProxy(AppInfo appInfo, IWebSocketConnection connection)
        {
            AppInfo = appInfo;
            Connection = connection;
            ConnectionID = connection.ConnectionInfo.Id;
        }

        public AppInfo AppInfo { get; set; }

        public Guid ConnectionID { get; set; }

        public bool Enabled { get; set; }

        public string PluginName => AppInfo.name;

        public string PluginId => AppInfo.AppId;

        public string PluginPath { get; set; } = "";

        public bool HasConnection { get; set; }

        public IWebSocketConnection Connection { get; set; }

        public Process PluginProcess { get; set; }

        public event Action<CQPluginProxy> OnPluginProcessExited;

        private static int PID => Process.GetCurrentProcess().Id;

        private static List<string> APIAuthWhiteList { get; set; } = new()
        {
            "getImage",
            "getRecordV2",
            "addLog",
            "setFatal",
            "getAppDirectory",
            "getLoginQQ",
            "getLoginNick",
        };

        public InvokeResult Invoke(InvokeBody caller)
        {
            //LogHelper.Debug("服务端发送", caller.ToJson());
            if (HasConnection is false)
            {
                return null;
            }
            Connection.Send(caller.ToJson());
            Server.Instance.WaitingMessage.Add(caller.GUID, new InvokeResult());
            if (RequestWaiter.Wait(caller.GUID, this, AppConfig.PluginInvokeTimeout)
                && Server.Instance.WaitingMessage.TryGetValue(caller.GUID, out InvokeResult result))
            {
                Server.Instance.WaitingMessage.Remove(caller.GUID);
                return result;
            }
            else
            {
                return new InvokeResult() { Message = "Timeout" };
            }
        }

        public bool CheckPluginCanInvoke(string invokeName)
        {
            invokeName = invokeName.Replace("CQ_", "");
            if (APIAuthWhiteList.Any(x => x == invokeName))
            {
                return true;
            }
            invokeName = invokeName.Replace("sendGroupQuoteMsg", "sendGroupMsg");
            if (!Enum.TryParse(invokeName, out PluginAPIType authEnum))
            {
                LogHelper.Error("调用权限检查", $"{invokeName} 无法转换为权限枚举");
                return false;
            }
            return CheckPluginCanInvoke(authEnum);
        }

        public bool CheckPluginCanInvoke(PluginAPIType apiType)
        {
            int id = (int)apiType;
            return AppInfo.auth.Any(x => x == id);
        }

        public bool LoadAppInfo()
        {
            AppInfo = null;
            string appInfoPath = PluginPath.Replace(".dll", ".json");
            if (File.Exists(appInfoPath) is false)
            {
                LogHelper.Error("加载插件", $"{PluginPath} 同名的 json 文件不存在，无法加载插件");
                return false;
            }
            string jsonContent = File.ReadAllText(appInfoPath);
            try
            {
                AppInfo = JsonConvert.DeserializeObject<AppInfo>(jsonContent);
                if (AppInfo == null || string.IsNullOrWhiteSpace(AppInfo.name))
                {
                    LogHelper.Error("加载插件", $"{PluginPath} 的 json 文件格式错误，无法加载插件");
                    return false;
                }
                AppInfo.AuthCode = PluginManagerProxy.MakeAuthCode();
                LogHelper.Info("加载插件", $"{AppInfo.name} 插件信息读取成功");
                return true;
            }
            catch
            {
                LogHelper.Error("加载插件", $"{PluginPath} 的 json 文件格式错误，无法加载插件");
                return false;
            }
        }

        public bool Load()
        {
            if (PluginProcess != null && PluginProcess.HasExited is false)
            {
                LogHelper.Error("加载插件", $"{PluginPath} 进程已启动，请先禁用插件");
                return false;
            }
            if (!StartPluginProcess() || PluginProcess.HasExited)
            {
                LogHelper.Error("加载插件", $"{PluginPath} 进程拉起失败");
                return false;
            }
            if (!WaitClientResponse())
            {
                LogHelper.Error("加载插件", $"{PluginPath} 等待客户端发送插件信息失败");
                KillProcess();
                return false;
            }
            LogHelper.Info("加载插件", $"{PluginName} 插件启动成功");
            return true;
        }

        private bool WaitClientResponse()
        {
            return RequestWaiter.Wait($"ClientStartUp_{PluginProcess.Id}", PluginProcess.Id, AppConfig.LoadTimeout);
        }

        private bool StartPluginProcess()
        {
            string arguments = $"-PID {PID} -AuthCode {AppInfo.AuthCode} -AutoExit {AppConfig.PluginExitWhenCoreExit} -Path {PluginPath} -WS {AppConfig.WebSocketURL}";
            Process? pluginProcess = null;
            var startConfig = new ProcessStartInfo
            {
                Arguments = arguments,
                FileName = $"{AppDomain.CurrentDomain.BaseDirectory}\\{AppDomain.CurrentDomain.FriendlyName}",
                WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory
            };
            if (!AppConfig.DebugMode)
            {
                startConfig.UseShellExecute = false;
                startConfig.CreateNoWindow = true;
                startConfig.RedirectStandardOutput = true;
                pluginProcess = Process.Start(startConfig);
                Task.Run(() =>
                {
                    while (pluginProcess?.StandardOutput != null && !pluginProcess.StandardOutput.EndOfStream)
                    {
                        Console.WriteLine(pluginProcess.StandardOutput.ReadLine());
                    }
                });
            }
            else
            {
                pluginProcess = Process.Start(startConfig);
            }
            pluginProcess.EnableRaisingEvents = true;
            pluginProcess.Exited += PluginProcess_Exited;

            PluginProcess = pluginProcess;
            return PluginProcess != null;
        }

        public void KillProcess()
        {
            PluginProcess?.Kill();
            PluginProcess.WaitForExit();
        }

        private void PluginProcess_Exited(object? sender, EventArgs e)
        {
            OnPluginProcessExited?.Invoke(this);
        }
    }
}