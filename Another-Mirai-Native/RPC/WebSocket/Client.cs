using Another_Mirai_Native.Config;
using Another_Mirai_Native.DB;
using Another_Mirai_Native.Model;
using Another_Mirai_Native.Native;
using Another_Mirai_Native.RPC.Interface;
using System.Net.WebSockets;

namespace Another_Mirai_Native.RPC.WebSocket
{
    public class Client : ClientBase
    {
        public WebSocketClient WebSocketClient { get; set; }

        private string ConnectUrl { get; set; } = AppConfig.Instance.Core_WSURL;

        private int ReconnectCount { get; set; }

        public override void ClientStartUp()
        {
            Send(new InvokeResult() { Type = $"ClientStartUp_{PID}", Result = PluginManager.LoadedPlugin.AppInfo?.AppId }.ToJson());
        }

        public override void Close()
        {
            WebSocketClient?.Close();
        }

        public override bool Connect()
        {
            WebSocketClient = new(ConnectUrl);
            WebSocketClient.OnClose += WebSocketClient_OnClose;
            WebSocketClient.OnMessage += WebSocketClient_OnMessage;
            WebSocketClient.OnError += WebSocketClient_OnError;
            WebSocketClient.Connect();
            LogHelper.Debug("连接服务端", "连接成功");
            LogHelper.LocalDebug("Websocket_Connect", "Connection Ok.");
            Connection = WebSocketClient;
            HeartBeatLostCount = 0;
            return WebSocketClient.ReadyState == WebSocketState.Open;
        }

        public override void Send(string message)
        {
            if (WebSocketClient != null && WebSocketClient.ReadyState == WebSocketState.Open)
            {
                LogHelper.LocalDebug("Websocket_Send", message);
                WebSocketClient.Send(message);
                LogHelper.LocalDebug("Websocket_Send", "Send Ok.");
            }
        }

        public override bool SetConnectionConfig()
        {
            return !string.IsNullOrEmpty(ConnectUrl);
        }

        private void WebSocketClient_OnClose()
        {
            LogHelper.LocalDebug("Websocket_Close", $"Connection Lost... Reconnect in {AppConfig.Instance.ReconnectTime} ms");

            ReconnectCount++;
            RequestWaiter.ResetSignalByConnection(WebSocketClient);
            Thread.Sleep(AppConfig.Instance.ReconnectTime);
            if (Connect())
            {
                UpdateConnection();
            }
        }

        private void WebSocketClient_OnError(Exception exc)
        {
            var color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            LogHelper.LocalDebug("Websocket_Error", $"Connection Error: {exc.Message} {exc.StackTrace}");
            Console.ForegroundColor = color;
        }

        private void WebSocketClient_OnMessage(string message)
        {
            new Thread(() => HandleMessage(message)).Start();
        }
    }
}