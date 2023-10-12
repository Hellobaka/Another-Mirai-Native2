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
                object result = 0;
                var methodInfo = typeof(CQPImplementation).GetMethods().FirstOrDefault(x => x.Name == name);
                if (methodInfo == null)
                {
                    return null;
                }
                var argumentList = methodInfo.GetParameters();
                if (caller.Args.Length != argumentList.Length)
                {
                    return null;
                }
                object[] args = new object[argumentList.Length];
                for (int i = 0; i < caller.Args.Length; i++)
                {
                    switch (argumentList[i].ParameterType.Name)
                    {
                        case "Int64":
                            args[i] = Convert.ToInt64(caller.Args[i]);
                            break;

                        case "Int32":
                            args[i] = Convert.ToInt32(caller.Args[i]);
                            break;

                        case "String":
                            args[i] = caller.Args[i].ToString();
                            break;

                        case "Boolean":
                            args[i] = Convert.ToBoolean(caller.Args[i]);
                            break;
                    }
                }
                return methodInfo.Invoke(null, args);
            }
            catch (Exception e)
            {
                LogHelper.Error("HandleCQPAPI", e);
                return null;
            }
        }
    }
}