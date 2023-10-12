using Another_Mirai_Native.Config;
using Another_Mirai_Native.Model;
using Another_Mirai_Native.Model.Enums;
using Another_Mirai_Native.Native;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.ComponentModel;
using System.Net.WebSockets;
using System.Text;

namespace Another_Mirai_Native.WebSocket
{
    public class Client
    {
        public static Client Instance { get; private set; }

        public static bool ExitFlag { get; private set; }

        public ClientWebSocket WebSocketClient { get; private set; }

        public Dictionary<string, InvokeResult> WaitingMessage { get; set; } = new();

        public Client()
        {
            Instance = this;
        }

        public bool Connect(string url)
        {
            Task.Run(() =>
            {
                while (WebSocketClient == null || WebSocketClient.State == WebSocketState.Aborted || WebSocketClient.State == WebSocketState.Closed)
                {
                    try
                    {
                        WebSocketClient = new ClientWebSocket();
                        WebSocketClient.ConnectAsync(new Uri(url), CancellationToken.None).Wait();
                        StartReceiveMessage();
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Error("WebSocket客户端", ex);
                        LogHelper.Error("WebSocket客户端", $"{AppConfig.ReconnectTime} ms后重新连接...");
                        WebSocketClient = null;
                        Thread.Sleep(AppConfig.ReconnectTime);
                    }
                }
            });

            return true;
        }

        public void Send(string message)
        {
            if (WebSocketClient != null && WebSocketClient.State == WebSocketState.Open)
            {
                ArraySegment<byte> buffer = new(Encoding.UTF8.GetBytes(message));
                WebSocketClient.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None).Wait();
            }
        }

        private async void StartReceiveMessage()
        {
            byte[] buffer = new byte[1024 * 4];
            while (true)
            {
                if (WebSocketClient == null || WebSocketClient.State != WebSocketState.Open)
                {
                    break;
                }
                WebSocketReceiveResult result;
                List<byte> data = new();
                do
                {
                    result = await WebSocketClient.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                    data.AddRange(new ArraySegment<byte>(buffer, 0, result.Count));
                }
                while (!result.EndOfMessage);
                string message = Encoding.UTF8.GetString(data.ToArray());
                Task.Run(() => HandleMessage(message));
            }
        }

        private void HandleMessage(string message)
        {
            try
            {
                LogHelper.Info("ReceiveServer", message);
                JObject json = JObject.Parse(message);
                if (json.ContainsKey("Args"))
                {
                    InvokeBody caller = json.ToObject<InvokeBody>();
                    object result = null;
                    if (caller.Function.StartsWith("InvokeEvent"))
                    {
                        result = HandleEvent(caller.Function.Replace("InvokeEvent_", ""), caller.Args);
                        Send(new InvokeResult { GUID = caller.GUID, Type = caller.Function, Result = result }.ToJson());
                    }
                }
                else
                {
                    InvokeResult result = json.ToObject<InvokeResult>();
                    if (WaitingMessage.ContainsKey(result.GUID))
                    {
                        WaitingMessage[result.GUID] = result;
                        WaitingMessage[result.GUID].Success = true;
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("Invoke", ex);
            }
        }

        private int HandleEvent(string function, object[] args)
        {
            PluginEventType eventType = (PluginEventType)Enum.Parse(typeof(PluginEventType), function);
            return PluginManager.LoadedPlugin.CallEvent(eventType, args);
        }
    }
}