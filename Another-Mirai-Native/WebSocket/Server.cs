using Another_Mirai_Native.Config;
using Another_Mirai_Native.DB;
using Another_Mirai_Native.Model;
using Another_Mirai_Native.Model.Enums;
using Another_Mirai_Native.Native;
using Fleck;
using Newtonsoft.Json.Linq;
using System.Diagnostics;

namespace Another_Mirai_Native.WebSocket
{
    public class Server
    {
        // 务必实现ShowErrorDialog回调，否则错误上抛时将会无限等待
        public Server()
        {
            Instance = this;
            LogHelper.LogAdded -= LogHelper_LogAdded;
            LogHelper.LogAdded += LogHelper_LogAdded;
            LogHelper.LogStatusUpdated -= LogHelper_LogStatusUpdated;
            LogHelper.LogStatusUpdated += LogHelper_LogStatusUpdated;
        }

        public static Server Instance { get; set; }

        public Dictionary<string, InvokeResult> WaitingMessage { get; set; } = new();

        public WebSocketServer WebSocketServer { get; set; }

        public static event Action<InvokeBody> OnShowErrorDialogCalled;

        private List<IWebSocketConnection> WebSocketConnections { get; set; } = new();

        public void Broadcast(InvokeBody invoke)
        {
            try
            {
                foreach (var connection in WebSocketConnections)
                {
                    if (connection.IsAvailable)
                    {
                        connection.Send(invoke.ToJson());
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("广播消息", ex);
            }
        }

        public void Start()
        {
            WebSocketServer = new(AppConfig.WebSocketURL);
            WebSocketServer.RestartAfterListenError = true;
            WebSocketServer.Start(Handler);
        }

        public void ActiveShowErrorDialog(InvokeBody caller)
        {
            OnShowErrorDialogCalled?.Invoke(caller);
        }

        private void HandleClientMessage(string message, IWebSocketConnection connection)
        {
            try
            {
                JObject json = JObject.Parse(message);
                if (json.ContainsKey("Args"))
                {
                    InvokeBody caller = json.ToObject<InvokeBody>();
                    HandleInvokeBody(caller, connection);
                }
                else
                {
                    InvokeResult result = json.ToObject<InvokeResult>();
                    HandleInvokeResult(result, connection);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("处理插件消息", ex);
            }
        }

        private object HandleCoreAPI(InvokeBody caller)
        {
            string name = caller.Function.Replace("InvokeCore_", "");
            try
            {
                switch (name)
                {
                    case "GetAllPlugins":
                        return PluginManagerProxy.Proxies;

                    case "AddLog":
                        LogModel? log = JObject.FromObject(caller.Args[0]).ToObject<LogModel>();
                        if (log == null)
                        {
                            return null;
                        }
                        return LogHelper.WriteLog(log);

                    case "GetCoreVersion":
                        return GetType().Assembly.GetName().Version.ToString();

                    case "GetAppConfig":
                        string configName = caller.Args[0].ToString();
                        var property = typeof(AppConfig).GetProperty(configName);
                        return property == null ? null : property.GetValue(null);

                    case "Restart":
                    case "EnablePlugin":
                    case "DisablePlugin":
                        int authCode = Convert.ToInt32(caller.Args[0]);
                        var pluginProxy = PluginManagerProxy.Proxies.FirstOrDefault(x => x.AppInfo.AuthCode == authCode);
                        if (name == "Restart")
                        {
                            pluginProxy?.KillProcess();
                            Thread.Sleep(100);
                            if (!AppConfig.RestartPluginIfDead)
                            {
                                LogHelper.Info("重启插件", $"{pluginProxy.PluginName} 重启");
                                Process? pluginProcess = PluginManagerProxy.Instance.StartPluginProcess(pluginProxy.AppInfo.PluginPath);
                                if (pluginProcess != null)
                                {
                                    PluginManagerProxy.PluginProcess.Add(pluginProcess.Id, new AppInfo { PluginPath = pluginProxy.AppInfo.PluginPath });
                                }
                            }
                            return 1;
                        }
                        else if (name == "EnablePlugin")
                        {
                            if (pluginProxy.Enabled)
                            {
                                return 1;
                            }
                            PluginManagerProxy.Instance.SetPluginEnabled(pluginProxy, true);
                        }
                        else if (name == "")
                        {
                            if (!pluginProxy.Enabled)
                            {
                                return 1;
                            }
                            PluginManagerProxy.Instance.SetPluginEnabled(pluginProxy, false);
                        }
                        break;
                }
            }
            catch (Exception e)
            {
                LogHelper.Error("调用核心方法", e);
            }
            return null;
        }

        private object HandleCQPAPI(InvokeBody caller)
        {
            try
            {
                string name = caller.Function.Replace("InvokeCQP_", "");
                var plugin = PluginManagerProxy.GetProxyByAuthCode(Convert.ToInt32(caller.Args[0]))
                    ?? throw new Exception("无法获取调用插件实例");
                var impl = new CQPImplementation(plugin);

                return impl.Invoke(name, caller.Args);
            }
            catch (Exception e)
            {
                LogHelper.Error("调用CQP方法", e);
                return null;
            }
        }

        private void HandleInvokeBody(InvokeBody caller, IWebSocketConnection connection)
        {
            object result = null;
            if (caller.Function.StartsWith("InvokeCQP"))
            {
                result = HandleCQPAPI(caller);
            }
            else if (caller.Function.StartsWith("InvokeCore"))
            {
                result = HandleCoreAPI(caller);
            }
            else if (caller.Function == "ShowErrorDialog")
            {
                WaitingMessage.Add(caller.GUID, new InvokeResult());
                OnShowErrorDialogCalled?.Invoke(caller);
                while (!WaitingMessage[caller.GUID].Success)
                {
                    Thread.Sleep(100);
                }
                result = WaitingMessage[caller.GUID];
                WaitingMessage.Remove(caller.GUID);
            }
            string message = "InvokeFail";
            connection.Send(new InvokeResult { GUID = caller.GUID, Message = result == null ? message : "", Result = result, Type = caller.Function }.ToJson());
        }

        private void HandleInvokeResult(InvokeResult result, IWebSocketConnection connection)
        {
            switch (result.Type)// 独立处理
            {
                case "PluginInfo":
                    AppInfo appInfo = JObject.FromObject(result.Result).ToObject<AppInfo>();
                    var proxy = PluginManagerProxy.Proxies.FirstOrDefault(x => x.ConnectionID == connection.ConnectionInfo.Id || x.PluginId == appInfo.AppId);
                    if (proxy == null)
                    {
                        proxy = new CQPluginProxy(appInfo, connection);
                        PluginManagerProxy.AddProxy(proxy);
                    }
                    else
                    {
                        proxy.ConnectionID = connection.ConnectionInfo.Id;
                        proxy.Connection = connection;
                        proxy.AppInfo = appInfo;
                    }
                    PluginManagerProxy.SetProxyConnected(proxy.ConnectionID);
                    if (PluginManagerProxy.PluginProcess.ContainsKey(appInfo.PID))
                    {
                        string path = PluginManagerProxy.PluginProcess[appInfo.PID].PluginPath;
                        PluginManagerProxy.PluginProcess[appInfo.PID] = appInfo;
                        PluginManagerProxy.PluginProcess[appInfo.PID].PluginPath = path;
                    }
                    // LogHelper.Info("HandleClientMessage", $"Load: {appInfo.name}");
                    if (AppConfig.PluginAutoEnable)
                    {
                        PluginManagerProxy.Instance.InvokeEvent(proxy, PluginEventType.Enable);
                        PluginManagerProxy.Instance.InvokeEvent(proxy, PluginEventType.StartUp);
                    }
                    break;

                default:
                    break;
            }
            if (WaitingMessage.ContainsKey(result.GUID))
            {
                WaitingMessage[result.GUID] = result;
                WaitingMessage[result.GUID].Success = true;
            }
        }

        private void Handler(IWebSocketConnection connection)
        {
            LogHelper.Debug("客户端连接", $"连接已建立, ID={connection.ConnectionInfo.Id}");
            connection.OnClose = () =>
            {
                LogHelper.Debug("客户端连接", $"连接已断开, ID={connection.ConnectionInfo.Id}");
                PluginManagerProxy.SetProxyDisconnected(connection.ConnectionInfo.Id);
                WebSocketConnections.Remove(connection);
            };
            connection.OnError = (e) =>
            {
                LogHelper.Debug("客户端连接", $"连接已断开, ID={connection.ConnectionInfo.Id}");
                PluginManagerProxy.SetProxyDisconnected(connection.ConnectionInfo.Id);
                WebSocketConnections.Remove(connection);
            };
            connection.OnMessage = (message) =>
            {
                LogHelper.Debug("收到客户端消息", message);
                Task.Run(() => HandleClientMessage(message, connection));
            };
            // 心跳
            Task.Run(() =>
            {
                Thread.Sleep(100);
                WebSocketConnections.Add(connection);
                while (connection != null && connection.IsAvailable)
                {
                    connection.SendPing(Array.Empty<byte>());
                    Thread.Sleep(AppConfig.HeartBeatInterval);
                }
            });
        }

        private void LogHelper_LogAdded(int logId, LogModel log)
        {
            Broadcast(new InvokeBody { Function = "LogAdded", Args = new object[] { logId, log } });
        }

        private void LogHelper_LogStatusUpdated(int logId, string status)
        {
            Broadcast(new InvokeBody { Function = "LogStatusUpdated", Args = new object[] { logId, status } });
        }
    }
}