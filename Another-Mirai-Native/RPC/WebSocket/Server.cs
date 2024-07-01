using Another_Mirai_Native.Config;
using Another_Mirai_Native.DB;
using Another_Mirai_Native.Model;
using Another_Mirai_Native.Model.Enums;
using Another_Mirai_Native.Native;
using Another_Mirai_Native.RPC.Interface;
using Fleck;
using Newtonsoft.Json.Linq;

namespace Another_Mirai_Native.RPC.WebSocket
{
    public class Server : ServerBase
    {
        public WebSocketServer WebSocketServer { get; set; }

        private Dictionary<int, IWebSocketConnection> WebSocketConnections { get; set; } = new();

        public override bool SetConnectionConfig()
        {
            return !string.IsNullOrEmpty(AppConfig.Instance.WebSocketURL);
        }

        public override bool Start()
        {
            try
            {
                WebSocketServer = new(AppConfig.Instance.WebSocketURL)
                {
                    RestartAfterListenError = true
                };
                WebSocketServer.Start(Handler);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public override bool Stop()
        {
            try
            {
                WebSocketServer.Dispose();
                WebSocketServer = null;
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void HandleClientMessage(string message, IWebSocketConnection connection)
        {
            try
            {
                JObject json = JObject.Parse(message);
                if (json.ContainsKey("Args"))
                {
                    InvokeBody? caller = json.ToObject<InvokeBody>();
                    HandleInvokeBody(caller, connection);
                }
                else
                {
                    InvokeResult? result = json.ToObject<InvokeResult>();
                    HandleInvokeResult(result, connection);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("处理插件消息", ex);
            }
        }

        public override void SendMessage(object connection, string message)
        {
            if (connection is IWebSocketConnection client)
            {
                client.Send(message);
            }
        }

        private void Handler(IWebSocketConnection connection)
        {
            LogHelper.Debug("客户端连接", $"连接已建立, ID={connection.ConnectionInfo.Id}");
            connection.OnClose = () =>
            {
                LogHelper.Debug("客户端连接", $"连接已断开, ID={connection.ConnectionInfo.Id}");
                var id = WebSocketConnections.FirstOrDefault(x => x.Value.ConnectionInfo.Id == connection.ConnectionInfo.Id);
                if (id.Value != null)
                {
                    var proxy = PluginManagerProxy.GetProxyByAuthCode(id.Key);
                    if (proxy != null)
                    {
                        ClientDisconnect(proxy);
                    }
                    WebSocketConnections.Remove(id.Key);
                }
                RequestWaiter.ResetSignalByConnectionID(connection.ConnectionInfo.Id.ToString());
            };
            connection.OnMessage = (message) =>
            {
                LogHelper.Debug("收到客户端消息", message);
                Task.Run(() => HandleClientMessage(message, connection));
            };
        }
    }
}