using Another_Mirai_Native.Config;
using Another_Mirai_Native.Model;
using Another_Mirai_Native.Native;
using Fleck;
using Newtonsoft.Json.Linq;

namespace Another_Mirai_Native.WebSocket
{
    public class Server
    {
        public Dictionary<string, InvokeResult> WaitingMessage { get; set; } = new();

        public static Server Instance { get; set; }

        public WebSocketServer WebSocketServer { get; set; }

        public Server()
        {
            Instance = this;
        }

        public void Start()
        {
            WebSocketServer = new WebSocketServer(AppConfig.WebSocketURL);
            WebSocketServer.RestartAfterListenError = true;
            WebSocketServer.Start(Handler);
        }

        private void Handler(IWebSocketConnection connection)
        {
            LogHelper.Info("WebSocket", $"连接已建立, ID={connection.ConnectionInfo.Id}");
            connection.OnClose = () =>
            {
                LogHelper.Info("WebSocket", $"连接已断开, ID={connection.ConnectionInfo.Id}");
                PluginManagerProxy.RemoveProxy(connection.ConnectionInfo.Id);
            };
            connection.OnError = (e) =>
            {
                LogHelper.Info("WebSocket", $"连接已断开, ID={connection.ConnectionInfo.Id}");
                PluginManagerProxy.RemoveProxy(connection.ConnectionInfo.Id);
            };
            connection.OnMessage = (message) =>
            {
                LogHelper.Info("ReceiveFromClient", message);
                Task.Run(() => HandleClientMessage(message, connection));
            };
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

        private void HandleInvokeResult(InvokeResult result, IWebSocketConnection connection)
        {
            switch (result.Type)// 独立处理
            {
                case "PluginInfo":
                    AppInfo appInfo = JObject.FromObject(result.Result).ToObject<AppInfo>();
                    var proxy = PluginManagerProxy.Proxies.FirstOrDefault(x => x.ID == connection.ConnectionInfo.Id);
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
                    PluginManagerProxy.Instance.InvokeEvent(proxy, Model.Enums.PluginEventType.StartUp);
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

        private void HandleInvokeBody(InvokeBody caller, IWebSocketConnection connection)
        {
            if (caller.Function.StartsWith("InvokeCQP"))
            {
                object cqpAPIResult = HandleCQPAPI(caller);
                string message = "InvokeFail";
                connection.Send(new InvokeResult { GUID = caller.GUID, Message = cqpAPIResult == null ? message : "", Result = cqpAPIResult, Type = caller.Function }.ToJson());
            }
        }

        private object HandleCQPAPI(InvokeBody caller)
        {
            try
            {
                string name = caller.Function.Replace("InvokeCQP_", "");
                var plugin = PluginManagerProxy.GetProxy(Convert.ToInt32(caller.Args[0]))
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
    }
}