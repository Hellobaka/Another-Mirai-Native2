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

        public override int? InvokeEvents(CQPluginProxy target, PluginEventType eventType, params object[] args)
        {
            if (target == null
                || target.HasConnection is false
                || WebSocketConnections.TryGetValue(target.PluginProcess.Id, out var connection) is false)
            {
                return null;
            }
            string guid = Guid.NewGuid().ToString();
            connection.Send(new InvokeBody { GUID = guid, Function = $"InvokeEvent_{eventType}", Args = args }.ToJson());
            WaitingMessage.Add(guid, new InvokeResult());

            if (RequestWaiter.Wait(guid, target, AppConfig.Instance.PluginInvokeTimeout, out _)
                    && WaitingMessage.TryGetValue(guid, out InvokeResult result))
            {
                WaitingMessage.Remove(guid);
                if (result.Success && int.TryParse(result.Result.ToString(), out int r))
                {
                    return r;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
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
                LogHelper.Error("处理插件消息", ex);
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
                        return GetAllPlugins();

                    case "AddLog":
                        return AddLog(JObject.FromObject(caller.Args[0]).ToObject<LogModel>());

                    case "GetCoreVersion":
                        return GetCoreVersion();

                    case "GetAppConfig":
                        return GetAppConfig(caller.Args[0].ToString());

                    case "Restart":
                        return RestartPlugin(Convert.ToInt32(caller.Args[0]));

                    case "EnablePlugin":
                        return EnablePlugin(Convert.ToInt32(caller.Args[0]));

                    case "DisablePlugin":
                        return DisablePlugin(Convert.ToInt32(caller.Args[0]));
                }
            }
            catch (Exception e)
            {
                LogHelper.Error("调用核心方法", e);
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
                LogHelper.Error("调用CQP方法", e);
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
            else if (caller.Function == "ShowErrorDialog")
            {
                ShowErrorDialog(caller.GUID, Convert.ToInt32(caller.Args[0]), caller.Args[1].ToString(), caller.Args[2].ToString(), Convert.ToBoolean(caller.Args[3]));
                result = 1;
            }
            else if (caller.Function == "HeartBeat")
            {
                connection.Send(new InvokeBody() { Function = "HeartBeat" }.ToJson());
            }
            string message = "InvokeFail";
            connection.Send(new InvokeResult { GUID = caller.GUID, Message = result == null ? message : "", Result = result, Type = caller.Function }.ToJson());
        }

        private void HandleInvokeResult(InvokeResult result, IWebSocketConnection connection)
        {
            string command = result.Type.Split('_').First();
            string id = result.Type.Split('_').Last();
            switch (command)
            {
                case "ClientStartUp":
                    WebSocketConnections.Add(Convert.ToInt32(id), connection);
                    ClientStartup(Convert.ToInt32(id), result.Result.ToString());
                    break;

                default:
                    break;
            }
            if (WaitingMessage.ContainsKey(result.GUID))
            {
                WaitingMessage[result.GUID] = result;
                WaitingMessage[result.GUID].Success = true;
                RequestWaiter.TriggerByKey(result.GUID);
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