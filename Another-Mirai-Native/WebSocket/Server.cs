using Another_Mirai_Native.Config;
using Another_Mirai_Native.DB;
using Another_Mirai_Native.Model;
using Another_Mirai_Native.Model.Enums;
using Another_Mirai_Native.Native;
using Fleck;
using Newtonsoft.Json.Linq;

namespace Another_Mirai_Native.WebSocket
{
    public class Server
    {
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

        private List<IWebSocketConnection> WebSocketConnections { get; set; } = new();

        public void Broadcast(InvokeBody invoke)
        {
            foreach (var connection in WebSocketConnections)
            {
                connection.Send(invoke.ToJson());
            }
        }

        public void Start()
        {
            WebSocketServer = new(AppConfig.WebSocketURL);
            WebSocketServer.RestartAfterListenError = true;
            WebSocketServer.Start(Handler);
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
                LogHelper.Error("WebSocket处理消息", ex);
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
                        LogHelper.WriteLog(log);
                        return 1;

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
                        var pluginProcess = PluginManagerProxy.Proxies.FirstOrDefault(x => x.AppInfo.AuthCode == authCode);
                        if (name == "Restart")
                        {
                            pluginProcess?.KillProcess();
                            return 1;
                        }
                        else if (name == "EnablePlugin")
                        {
                            if (pluginProcess.Enabled)
                            {
                                return 1;
                            }
                            PluginManagerProxy.Instance.InvokeEvent(pluginProcess, PluginEventType.Enable);
                            PluginManagerProxy.Instance.InvokeEvent(pluginProcess, PluginEventType.StartUp);
                            pluginProcess.Enabled = true;
                        }
                        else if (name == "")
                        {
                            if (!pluginProcess.Enabled)
                            {
                                return 1;
                            }
                            PluginManagerProxy.Instance.InvokeEvent(pluginProcess, PluginEventType.Disable);
                            PluginManagerProxy.Instance.InvokeEvent(pluginProcess, PluginEventType.Exit);
                            pluginProcess.Enabled = false;
                        }
                        break;
                }
            }
            catch (Exception e)
            {
                LogHelper.Error("HandleCoreAPI", e);
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
                LogHelper.Error("HandleCQPAPI", e);
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
            string message = "InvokeFail";
            connection.Send(new InvokeResult { GUID = caller.GUID, Message = result == null ? message : "", Result = result, Type = caller.Function }.ToJson());
        }

        private void HandleInvokeResult(InvokeResult result, IWebSocketConnection connection)
        {
            switch (result.Type)// 独立处理
            {
                case "PluginInfo":
                    AppInfo appInfo = JObject.FromObject(result.Result).ToObject<AppInfo>();
                    var proxy = PluginManagerProxy.Proxies.FirstOrDefault(x => x.ConnectionID == connection.ConnectionInfo.Id);
                    if (proxy == null)
                    {
                        proxy = new CQPluginProxy(appInfo, connection);
                        PluginManagerProxy.Proxies.Add(proxy);
                    }
                    else
                    {
                        proxy.AppInfo = appInfo;
                    }
                    LogHelper.Info("HandleClientMessage", $"Load: {appInfo.name}");
                    // TODO: Delete
                    PluginManagerProxy.Instance.InvokeEvent(proxy, PluginEventType.StartUp);
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
            WebSocketConnections.Add(connection);
            LogHelper.Info("WebSocket", $"连接已建立, ID={connection.ConnectionInfo.Id}");
            connection.OnClose = () =>
            {
                LogHelper.Info("WebSocket", $"连接已断开, ID={connection.ConnectionInfo.Id}");
                PluginManagerProxy.RemoveProxy(connection.ConnectionInfo.Id);
                WebSocketConnections.Remove(connection);
            };
            connection.OnError = (e) =>
            {
                LogHelper.Info("WebSocket", $"连接已断开, ID={connection.ConnectionInfo.Id}");
                PluginManagerProxy.RemoveProxy(connection.ConnectionInfo.Id);
                WebSocketConnections.Remove(connection);
            };
            connection.OnMessage = (message) =>
            {
                LogHelper.Info("ReceiveFromClient", message);
                Task.Run(() => HandleClientMessage(message, connection));
            };
            Task.Run(() =>
            {
                while (connection != null && connection.IsAvailable)
                {
                    Thread.Sleep(AppConfig.HeartBeatInterval);
                    connection.SendPing(Array.Empty<byte>());
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