using Another_Mirai_Native.Abstractions.Enums;
using Another_Mirai_Native.Config;
using Another_Mirai_Native.DB;
using Another_Mirai_Native.Model;
using Another_Mirai_Native.Model.Enums;
using Another_Mirai_Native.Native;
using Newtonsoft.Json.Linq;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;

namespace Another_Mirai_Native.RPC.Interface
{
    public class ServerBase : IServer
    {
        public event Action<string, int, string, string, bool> OnShowErrorDialogCalled;

        public bool ShowErrorDialogEventHasSubscribed => OnShowErrorDialogCalled?.GetInvocationList().Length > 0;

        public ConcurrentDictionary<string, InvokeResult> WaitingMessage { get; set; } = new();

        /// <summary>
        /// PID/Object
        /// </summary>
        internal ConcurrentDictionary<int, object> Connections { get; set; } = new();

        public void ActiveShowErrorDialog(string guid, int authCode, string title, string content, bool canIgnore)
        {
            OnShowErrorDialogCalled?.Invoke(guid, authCode, title, content, canIgnore);
        }

        public int AddLog(LogModel? log)
        {
            return log == null ? -1 : LogHelper.WriteLog(log);
        }

        public void ClientStartup(int pid, string? appId)
        {
            var proxy = PluginManagerProxy.Proxies.FirstOrDefault(x => x.PluginProcess != null && x.PluginProcess.Id == pid);
            if (proxy != null && !string.IsNullOrEmpty(appId))
            {
                proxy.AppInfo.AppId = appId!;
            }
            else
            {
                LogHelper.Error("插件客户端启动", "无效AppID或对应的插件");
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

        public object? GetAppConfig(string? key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return null;
            }
            var property = typeof(AppConfig).GetProperty(key);
            return property?.GetValue(null);
        }

        public string GetCoreVersion() => GetType().Assembly.GetName().Version?.ToString() ?? "未知版本";

        public object? HandleCoreAPI(InvokeBody? caller)
        {
            if (caller == null)
            {
                return null;
            }

            // 异常由上层处理，和正常返回 null 区分开。
            string name = caller.Function.Replace("InvokeCore_", "");
            switch (name)
            {
                case "GetAllPlugins":
                    return GetAllPlugins();

                case "AddLog":
                    return AddLog(JObject.FromObject(caller.Args[0]).ToObject<LogModel>() ?? null);

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
            return null;
        }

        public object? HandleCQPAPI(InvokeBody? caller)
        {
            if (caller == null)
            {
                return null;
            }

            string name = caller.Function.Replace("InvokeCQP_", "");
            var plugin = PluginManagerProxy.GetProxyByAuthCode(Convert.ToInt32(caller.Args[0]))
                ?? throw new Exception("无法获取调用插件实例");
            var impl = new CQPImplementation(plugin);

            return impl.Invoke(name, caller.Args);
        }

        public void HandleInvokeBody(InvokeBody? caller, object connection)
        {
            if (caller == null)
            {
                return;
            }
            if (caller.Function == "HeartBeat")
            {
                // TODO: 管理心跳
                SendMessage(connection, new InvokeBody() { Function = "HeartBeat" }.ToJson());
                return;
            }

            InvokeResult result = new()
            {
                GUID = caller.GUID,
                Type = caller.Function,
                Success = true,
            };
            try
            {
                if (caller.Function.StartsWith("InvokeCQP"))
                {
                    result.Result = HandleCQPAPI(caller);
                }
                else if (caller.Function.StartsWith("InvokeCore"))
                {
                    result.Result = HandleCoreAPI(caller);
                }
                else if (caller.Function == "ShowErrorDialog")
                {
                    ShowErrorDialog(caller.GUID, Convert.ToInt32(caller.Args[0]), caller.Args[1].ToString(), caller.Args[2].ToString(), Convert.ToBoolean(caller.Args[3]));
                    result.Result = 1;
                }
            }
            catch (Exception e)
            {
                // 于正常返回 null 区分开，否则调用方无法判断
                result.Success = false;
                result.Message = e.Message;
                LogHelper.Error("处理调用", e);
            }

            SendMessage(connection, result.ToJson());
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
                    Connections.AddOrUpdate(Convert.ToInt32(pid), connection, (key, oldValue) => connection);
                    ClientStartup(Convert.ToInt32(pid), result.Result?.ToString());
                    break;

                case "UpdateConnection":
                    int _pid = Convert.ToInt32(pid);
                    if (Connections.TryGetValue(_pid, out var oldConnection))
                    {
                        DropConnection(oldConnection);
                        Connections[_pid] = connection;
                        LogHelper.Debug("连接重置", $"PID={_pid} 连接已更新");
                    }
                    break;

                default:
                    break;
            }

            RequestWaiter.TriggerByKey(result.GUID, result);
        }

        public int? InvokeCQPAPI(CQPImplementation cqp, string functionName, params object[] args)
        {
            try
            {
                return (int?)cqp.Invoke(functionName, args);
            }
            catch (Exception e)
            {
                LogHelper.Error("调用CQP函数", $"{functionName}。{e.Message}\n{e.StackTrace}");
                return null;
            }
        }

        public int? InvokeEvents(CQPluginProxy target, PluginEventType eventType, params object[] args)
        {
            if (target == null
                || target.PluginProcess == null
                || target.HasConnection is false
                || Connections.TryGetValue(target.PluginProcess.Id, out var connection) is false)
            {
                return null;
            }
            Stopwatch stopwatch = Stopwatch.StartNew();
            string guid = Guid.NewGuid().ToString();
            LogHelper.Debug($"InvokeEvent_{eventType}", $"GUID = {guid} 插件 = {target.PluginName}");

            if (RequestWaiter.Wait(guid, target, AppConfig.Instance.PluginInvokeTimeout, () =>
                    {
                        SendMessage(connection, new InvokeBody { GUID = guid, Function = $"InvokeEvent_{eventType}", Args = args }.ToJson());
                    }, out object? obj) && obj is InvokeResult result)
            {
                LogHelper.Debug($"InvokeEvent_{eventType}", $"结束 GUID = {guid} 插件 = {target.PluginName}");
                if (int.TryParse(result.Result?.ToString(), out int r))
                {
                    if (AppConfig.Instance.DebugMode)
                    {
                        LogHelper.WriteLog(LogLevel.Debug, logOrigin:"AMN框架", "插件耗时", $"{target.PluginName} 处理 {eventType} 事件结果：成功，耗时 {stopwatch.ElapsedMilliseconds} ms");
                    }
                    return r;
                }
                else
                {
                    if (AppConfig.Instance.DebugMode)
                    {
                        LogHelper.WriteLog(LogLevel.Debug, logOrigin: "AMN框架", "插件耗时", $"{target.PluginName} 处理 {eventType} 事件结果：失败，耗时 {stopwatch.ElapsedMilliseconds} ms");
                    }
                    LogHelper.Error("响应超时", $"插件 {target.PluginName} 响应 {eventType} 事件返回值无效");
                    return null;
                }
            }
            else
            {
                if (AppConfig.Instance.DebugMode)
                {
                    LogHelper.WriteLog(LogLevel.Debug, logOrigin: "AMN框架", "插件耗时", $"{target.PluginName} 处理 {eventType} 事件耗时 {stopwatch.ElapsedMilliseconds} ms");
                }
                LogHelper.Debug("等待失败", $"GUID={guid}");
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

        public void ShowErrorDialog(string guid, int authCode, string? title, string? content, bool canIgnore)
        {
            WaitingMessage.AddOrUpdate(guid, new InvokeResult(), (key, oldValue) => new InvokeResult());
            OnShowErrorDialogCalled?.Invoke(guid, authCode, title ?? "", content ?? "", canIgnore);
            // 只有 UI 订阅了事件才会有人 TriggerByKey；没订阅时等待会永远不返回，
            // 把处理这条消息的线程永久挂住。
            if (ShowErrorDialogEventHasSubscribed)
            {
                RequestWaiter.Wait(guid, -1, null, out _);
            }
            WaitingMessage.TryRemove(guid, out _);
        }

        public void NotifyCurrentQQChanged(long currentQQ, string nickName)
        {
            string message = new InvokeBody() { Function = "CurrentQQChanged", Args = [currentQQ, nickName] }.ToJson();
            // 必须传真正的连接对象：Connections 的枚举项是 KeyValuePair，
            // 传输层 SendMessage 里的 `is NamedPipe` / `is IWebSocketConnection` 会直接不匹配，导致一条都发不出去。
            foreach (var connection in Connections.Values)
            {
                SendMessage(connection, message);
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

        public virtual void DropConnection(object connection)
        {
        }

        #endregion Virtual
    }
}