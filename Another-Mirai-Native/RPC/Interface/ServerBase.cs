using Another_Mirai_Native.Config;
using Another_Mirai_Native.DB;
using Another_Mirai_Native.Model;
using Another_Mirai_Native.Model.Enums;
using Another_Mirai_Native.Native;
using Fleck;
using Newtonsoft.Json.Linq;
using System.Reflection;

namespace Another_Mirai_Native.RPC.Interface
{
    public class ServerBase : IServer
    {
        public event Action<string, int, string, string, bool> OnShowErrorDialogCalled;

        public bool ShowErrorDialogEventHasSubscribed => OnShowErrorDialogCalled?.GetInvocationList().Length > 0;

        public Dictionary<string, InvokeResult> WaitingMessage { get; set; } = new();

        /// <summary>
        /// PID/Object
        /// </summary>
        private Dictionary<int, object> Connections { get; set; } = new();

        public void ActiveShowErrorDialog(string guid, int authCode, string title, string content, bool canIgnore)
        {
            OnShowErrorDialogCalled?.Invoke(guid, authCode, title, content, canIgnore);
        }

        public int? AddLog(LogModel log)
        {
            return log == null ? null : LogHelper.WriteLog(log);
        }

        public void ClientStartup(int pid, string appId)
        {
            var proxy = PluginManagerProxy.Proxies.FirstOrDefault(x => x.PluginProcess != null && x.PluginProcess.Id == pid);
            if (proxy != null && !string.IsNullOrEmpty(appId))
            {
                proxy.AppInfo.AppId = appId;
            }
            else
            {
                return;
            }
            PluginManagerProxy.SetProxyConnected(proxy);
            RequestWaiter.TriggerByKey($"ClientStartUp_{pid}");
        }

        public bool DisablePlugin(int authCode)
        {
            var plugin = PluginManagerProxy.GetProxyByAuthCode(authCode);
            if (plugin == null)
            {
                return false;
            }
            PluginManagerProxy.Instance.SetPluginEnabled(plugin, false);
            return true;
        }

        public bool EnablePlugin(int authCode)
        {
            var plugin = PluginManagerProxy.GetProxyByAuthCode(authCode);
            if (plugin == null)
            {
                return false;
            }
            PluginManagerProxy.Instance.SetPluginEnabled(plugin, true);
            return true;
        }

        public List<CQPluginProxy> GetAllPlugins() => PluginManagerProxy.Proxies;

        public object GetAppConfig(string key)
        {
            var property = typeof(AppConfig).GetProperty(key);
            return property == null ? null : property.GetValue(null);
        }

        public string GetCoreVersion() => GetType().Assembly.GetName().Version.ToString();

        public object? HandleCoreAPI(InvokeBody? caller)
        {
            if (caller == null)
            {
                return null;
            }

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

        public object? HandleCQPAPI(InvokeBody? caller)
        {
            if (caller == null)
            {
                return null;
            }

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

        public void HandleInvokeBody(InvokeBody? caller, object connection)
        {
            if (caller == null)
            {
                return;
            }
            object? result = null;
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
                // TODO: 管理心跳
                SendMessage(connection, new InvokeBody() { Function = "HeartBeat" }.ToJson());
                return;
            }
            string message = "InvokeFail";
            SendMessage(connection, new InvokeResult { GUID = caller.GUID, Message = result == null ? message : "", Result = result, Type = caller.Function }.ToJson());
        }

        public void HandleInvokeResult(InvokeResult? result, object connection)
        {
            if (result == null)
            {
                return;
            }

            string command = result.Type.Split('_').First();
            string pid = result.Type.Split('_').Last();
            switch (command)
            {
                case "ClientStartUp":
                    Connections.Add(Convert.ToInt32(pid), connection);
                    ClientStartup(Convert.ToInt32(pid), result.Result.ToString());
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

        public int? InvokeCQPAPI(CQPImplementation cqp, string functionName, params object[] args)
        {
            try
            {
                return (int)cqp.Invoke(functionName, args);
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public int? InvokeEvents(CQPluginProxy target, PluginEventType eventType, params object[] args)
        {
            if (target == null
                            || target.HasConnection is false
                            || Connections.TryGetValue(target.PluginProcess.Id, out var connection) is false)
            {
                return null;
            }
            string guid = Guid.NewGuid().ToString();
            SendMessage(connection, new InvokeBody { GUID = guid, Function = $"InvokeEvent_{eventType}", Args = args }.ToJson());
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

        public bool RestartPlugin(int authCode)
        {
            var plugin = PluginManagerProxy.GetProxyByAuthCode(authCode);
            if (plugin == null)
            {
                return false;
            }
            PluginManagerProxy.Instance.ReloadPlugin(plugin);
            return true;
        }

        public void ShowErrorDialog(string guid, int authCode, string title, string content, bool canIgnore)
        {
            WaitingMessage.Add(guid, new InvokeResult());
            OnShowErrorDialogCalled?.Invoke(guid, authCode, title, content, canIgnore);
            RequestWaiter.Wait(guid, -1, out _);
            WaitingMessage.Remove(guid);
        }

        public void NotifyCurrentQQChanged(long currentQQ, string nickName)
        {
            foreach (var item in Connections)
            {
                SendMessage(item, new InvokeBody() { Function = "CurrentQQChanged", Args = [currentQQ, nickName] }.ToJson());
            }
        }

        #region Virtual

        public virtual void ClientDisconnect(CQPluginProxy plugin)
        {
            PluginManagerProxy.SetProxyDisconnected(plugin);
        }

        public virtual void SendMessage(object connection, string message)
        {
        }

        public virtual bool SetConnectionConfig()
        {
            return false;
        }

        public virtual bool Start()
        {
            return false;
        }

        public virtual bool Stop()
        {
            return false;
        }

        #endregion Virtual
    }
}