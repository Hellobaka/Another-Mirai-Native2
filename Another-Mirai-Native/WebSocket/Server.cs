using Another_Mirai_Native.Config;
using Another_Mirai_Native.Model;
using Another_Mirai_Native.Model.Enums;
using Another_Mirai_Native.Native;
using Fleck;
using Newtonsoft.Json;
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
                HandleClientMessage(message, connection);
            };
        }

        private void HandleClientMessage(string message, IWebSocketConnection connection)
        {
            try
            {
                LogHelper.Info("ReceiveClient", message);
                InvokeResult result = JsonConvert.DeserializeObject<InvokeResult>(message);
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
                        Thread.Sleep(10000);
                        PluginManagerProxy.Instance.InvokeEvent(PluginManagerProxy.Proxies.First(), PluginEventType.StartUp);
                        PluginManagerProxy.Instance.InvokeEvent(PluginManagerProxy.Proxies.First(), PluginEventType.Enable);
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
            catch (Exception ex)
            {
                LogHelper.Error("WebSocket处理消息", ex);
            }
        }
    }
}