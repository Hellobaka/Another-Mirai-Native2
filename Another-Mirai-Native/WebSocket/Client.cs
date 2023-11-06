using Another_Mirai_Native.Config;
using Another_Mirai_Native.DB;
using Another_Mirai_Native.Model;
using Another_Mirai_Native.Model.Enums;
using Another_Mirai_Native.Native;
using Newtonsoft.Json.Linq;
using System.Security.Policy;
using System.Text;

namespace Another_Mirai_Native.WebSocket
{
    public class Client
    {
        public static Client Instance { get; private set; }

        public static bool ExitFlag { get; private set; }

        public WebSocketSharp.WebSocket WebSocketClient { get; private set; }

        public Dictionary<string, InvokeResult> WaitingMessage { get; set; } = new();

        public int ReconnectCount { get; private set; }

        public Client()
        {
            Instance = this;
        }

        public bool Connect(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                LogHelper.Error("连接服务端", "参数无效");
                return false;
            }

            WebSocketClient = new(url);
            WebSocketClient.OnClose += WebSocketClient_OnClose;
            WebSocketClient.OnMessage += WebSocketClient_OnMessage;
            WebSocketClient.Connect();
            LogHelper.Debug("连接服务端", "连接成功");
            return WebSocketClient.ReadyState == WebSocketSharp.WebSocketState.Open;
        }

        private void WebSocketClient_OnMessage(object? sender, WebSocketSharp.MessageEventArgs e)
        {
            new Thread(() => HandleMessage(e.Data)).Start();
        }

        private void WebSocketClient_OnClose(object? sender, WebSocketSharp.CloseEventArgs e)
        {
            ReconnectCount++;
            LogHelper.Error("与服务器连接断开", $"{AppConfig.ReconnectTime} ms后重新连接...");
            Thread.Sleep(AppConfig.ReconnectTime);
            Connect(AppConfig.Core_WSURL);
        }

        public void Send(string message)
        {
            if (WebSocketClient != null && WebSocketClient.ReadyState == WebSocketSharp.WebSocketState.Open)
            {
                // LogHelper.Debug("向服务端发送", message);
                WebSocketClient.Send(message);
            }
        }

        private void HandleMessage(string message)
        {
            try
            {
                //LogHelper.Debug("来自服务端的消息", message);
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
                    else if (caller.Function == "KillProcess")
                    {
                        Send(new InvokeResult { GUID = caller.GUID, Type = caller.Function, Result = 1 }.ToJson());
                        Environment.Exit(0);
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
                LogHelper.Error("处理服务器消息", ex);
            }
        }

        public InvokeResult Invoke(string function, bool waiting, params object[] args)
        {
            string guid = Guid.NewGuid().ToString();

            Send(new InvokeBody { GUID = guid, Function = function, Args = args }.ToJson());
            if (!waiting)
            {
                return null;
            }
            WaitingMessage.Add(guid, new InvokeResult());
            for (int i = 0; i < AppConfig.PluginInvokeTimeout / 10; i++)
            {
                if (WaitingMessage.ContainsKey(guid) && WaitingMessage[guid].Success)
                {
                    var result = WaitingMessage[guid];
                    WaitingMessage.Remove(guid);
                    return result;
                }
                Thread.Sleep(10);
            }
            LogHelper.Error("调用超时", "Timeout");
            return new InvokeResult() { Message = "Timeout" };
        }

        private int HandleEvent(string function, object[] args)
        {
            PluginEventType eventType = (PluginEventType)Enum.Parse(typeof(PluginEventType), function);
            return PluginManager.Instance.CallEvent(eventType, args);
        }
    }
}