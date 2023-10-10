using Another_Mirai_Native.Config;
using Fleck;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Another_Mirai_Native.WebSocket
{
    public class Server
    {
        private Dictionary<string, IWebSocketConnection> Connections { get; set; } = new();

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
            if (!Connections.ContainsKey(connection.ConnectionInfo.Id.ToString()))
            {
                Connections.Add(connection.ConnectionInfo.Id.ToString(), connection);
            }
            connection.OnClose = () =>
            {
                LogHelper.Info("WebSocket", $"连接已断开, ID={connection.ConnectionInfo.Id}");
                Connections.Remove(connection.ConnectionInfo.Id.ToString());
            };
            connection.OnError = (e) =>
            {
                LogHelper.Info("WebSocket", $"连接已断开, ID={connection.ConnectionInfo.Id}");
                Connections.Remove(connection.ConnectionInfo.Id.ToString());
            };
            connection.OnMessage = (message) =>
            {
                HandleClientMessage(message, connection.ConnectionInfo);
            };
        }

        private void HandleClientMessage(string message, IWebSocketConnectionInfo connectionInfo)
        {
            try
            {
                string type = JObject.Parse(message)["Type"].ToString();
                switch (type)
                {
                    case "PluginInfo":
                        break;

                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("WebSocket处理消息", ex);
            }
        }
    }
}