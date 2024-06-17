using Another_Mirai_Native.Config;
using Another_Mirai_Native.DB;
using Another_Mirai_Native.Model;
using Another_Mirai_Native.Model.Enums;
using Another_Mirai_Native.Native;
using Another_Mirai_Native.RPC.Interface;
using Another_Mirai_Native.RPC.WebSocket;
using Newtonsoft.Json.Linq;
using System.Net.WebSockets;

namespace Another_Mirai_Native.RPC.WebSocket
{
    public class Client : ClientBase
    {
        public WebSocketClient WebSocketClient { get; set; }

        private string ConnectUrl { get; set; } = AppConfig.Instance.Core_WSURL;

        private int ReconnectCount { get; set; }

        public override void AddLog(LogModel model)
        {
            InvokeCQPFuntcion("InvokeCore_AddLog", false, model);
        }

        public override void ClientStartUp()
        {
            Send(new InvokeResult() { Type = $"ClientStartUp_{PID}", Result = PluginManager.LoadedPlugin.AppInfo.AppId }.ToJson());
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
            HeartBeatLostCount = 0;
            return WebSocketClient.ReadyState == WebSocketState.Open;
        }

        private void WebSocketClient_OnError(Exception exc)
        {
            var color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            LogHelper.LocalDebug("Websocket_Error", $"Connection Error: {exc.Message} {exc.StackTrace}");
            Console.ForegroundColor = color;
        }

        public override object InvokeCQPFuntcion(string function, bool waiting, params object[] args)
        {
            string guid = Guid.NewGuid().ToString();
            if (function.StartsWith("CQ_"))
            {
                function = "InvokeCQP_" + function;
            }
            Send(new InvokeBody { GUID = guid, Function = function, Args = args }.ToJson());
            if (!waiting)
            {
                return null;
            }
            WaitingMessage.Add(guid, new InvokeResult());
            if (RequestWaiter.Wait(guid, WebSocketClient, AppConfig.Instance.PluginInvokeTimeout, out _) && WaitingMessage.ContainsKey(guid))
            {
                var result = WaitingMessage[guid];
                WaitingMessage.Remove(guid);
                if (result.Success)
                {
                    return result.Result;
                }
                else
                {
                    LogHelper.Error("调用失败", $"GUID={guid}, msg={result.Message}");
                    return null;
                }
            }
            else
            {
                LogHelper.Error("调用超时", "Timeout");
                return null;
            }
        }

        public override void SendHeartBeat()
        {
            Send(new InvokeBody()
            {
                Function = "HeartBeat"
            }.ToJson());
        }

        public override bool SetConnectionConfig()
        {
            return !string.IsNullOrEmpty(ConnectUrl);
        }

        public override void ShowErrorDialog(string guid, string title, string content, bool canIgnore)
        {
            Send(new InvokeBody
            {
                GUID = guid,
                Args = new object[]
                {
                    AppConfig.Instance.Core_AuthCode,
                    title,
                    content ?? "",
                    canIgnore
                },
                Function = "ShowErrorDialog"
            }.ToJson());
            WaitingMessage.Add(guid, new InvokeResult());
        }

        private void HandleMessage(string message)
        {
            try
            {
                LogHelper.LocalDebug("Websocket_Message", message);
                JObject json = JObject.Parse(message);
                if (json.ContainsKey("Args"))
                {
                    InvokeBody caller = json.ToObject<InvokeBody>();
                    object result = null;
                    if (caller.Function.StartsWith("InvokeEvent"))
                    {
                        PluginEventType eventType = (PluginEventType)Enum.Parse(typeof(PluginEventType), caller.Function.Replace("InvokeEvent_", ""));
                        result = InvokeEvent(eventType, caller.Args);
                        Send(new InvokeResult { GUID = caller.GUID, Type = caller.Function, Result = result }.ToJson());
                    }
                    else if (caller.Function == "KillProcess")
                    {
                        Send(new InvokeResult { GUID = caller.GUID, Type = caller.Function, Result = 1 }.ToJson());
                        KillProcess();
                    }
                    else if (caller.Function == "HeartBeat")
                    {
                        HeartBeatLostCount = 0;
                    }
                }
                else
                {
                    InvokeResult result = json.ToObject<InvokeResult>();
                    if (WaitingMessage.ContainsKey(result.GUID))
                    {
                        WaitingMessage[result.GUID] = result;
                        WaitingMessage[result.GUID].Success = true;
                        RequestWaiter.TriggerByKey(result.GUID);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("处理服务器消息", ex);
            }
        }

        private void Send(string message)
        {
            if (WebSocketClient != null && WebSocketClient.ReadyState == WebSocketState.Open)
            {
                LogHelper.LocalDebug("Websocket_Send", message);
                WebSocketClient.Send(message);
                LogHelper.LocalDebug("Websocket_Send", "Send Ok.");
            }
        }

        private void WebSocketClient_OnClose()
        {
            LogHelper.LocalDebug("Websocket_Close", "Connection Lost...");

            ReconnectCount++;
            LogHelper.Error("与服务器连接断开", $"{AppConfig.Instance.ReconnectTime} ms后重新连接...");
            RequestWaiter.ResetSignalByWebSocket(WebSocketClient);
            Thread.Sleep(AppConfig.Instance.ReconnectTime);
            Connect();
        }

        private void WebSocketClient_OnMessage(string message)
        {
            new Thread(() => HandleMessage(message)).Start();
        }
    }
}