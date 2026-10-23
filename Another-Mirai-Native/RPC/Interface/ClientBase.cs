using Another_Mirai_Native.Abstractions.Enums;
using Another_Mirai_Native.Config;
using Another_Mirai_Native.DB;
using Another_Mirai_Native.Model;
using Another_Mirai_Native.Model.Enums;
using Another_Mirai_Native.Native;
using Newtonsoft.Json.Linq;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Another_Mirai_Native.RPC.Interface
{
    public class ClientBase : IClient
    {
        public event Action<bool> OnReceiveHeartBeat;

        public static int PID => Process.GetCurrentProcess().Id;

        public object Connection { get; set; }

        public int HeartBeatLostCount { get; set; }

        public ConcurrentDictionary<string, InvokeResult> WaitingMessage { get; set; } = new();

        public void HandleMessage(string message)
        {
            try
            {
                LogHelper.LocalDebug("Client_Message", message);
                JObject json = JObject.Parse(message);
                if (json.ContainsKey("Args"))
                {
                    InvokeBody? caller = json.ToObject<InvokeBody>();
                    if (caller == null)
                    {
                        return;
                    }
                    object? result = null;
                    if (caller.Function.StartsWith("InvokeEvent"))
                    {
                        PluginEventType eventType = (PluginEventType)Enum.Parse(typeof(PluginEventType), caller.Function.Replace("InvokeEvent_", ""));
                        result = InvokeEvent(eventType, caller.Args);
                        Send(new InvokeResult { GUID = caller.GUID, Type = caller.Function, Result = result, Success = true }.ToJson());
                    }
                    else if (caller.Function == "KillProcess")
                    {
                        Send(new InvokeResult { GUID = caller.GUID, Type = caller.Function, Result = 1, Success = true }.ToJson());
                        KillProcess();
                    }
                    else if (caller.Function == "HeartBeat")
                    {
                        HeartBeatLostCount = 0;
                        OnReceiveHeartBeat?.Invoke(true);
                    }
                    else if (caller.Function == "CurrentQQChanged")
                    {
                        if (caller.Args != null 
                            && caller.Args.Length == 2
                            && long.TryParse(caller.Args[0].ToString(), out long v)
                            && !string.IsNullOrEmpty(caller.Args[1]?.ToString()))
                        {
                            AppConfig.Instance.CurrentQQ = v;
                            AppConfig.Instance.CurrentNickName = caller.Args[1]?.ToString() ?? "";
                        }
                    }
                }
                else
                {
                    InvokeResult? result = json.ToObject<InvokeResult>();
                    if (result == null)
                    {
                        return;
                    }
                    if (WaitingMessage.ContainsKey(result.GUID))
                    {
                        // 不要在这里强行把 Success 置为 true：
                        // 成功/失败由服务端在结果里给出，否则"结果就是 null"会被当成失败。
                        WaitingMessage[result.GUID] = result;
                        RequestWaiter.TriggerByKey(result.GUID);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("处理服务器消息", ex);
            }
        }

        public int InvokeEvent(PluginEventType eventType, object[] args)
        {
            return PluginManager.Instance.CallEvent(eventType, args);
        }

        public void KillProcess()
        {
            Environment.Exit(0);
        }

        #region Virtual

        public virtual void AddLog(LogModel model)
        {
            InvokeCQPFunction("InvokeCore_AddLog", false, model);
        }

        public virtual void ClientStartUp()
        {
            Send(new InvokeResult() { Type = $"ClientStartUp_{PID}", Result = PluginManager.Instance.LoadedPlugin.AppInfo?.AppId }.ToJson());
        }

        public virtual void UpdateConnection()
        {
            Send(new InvokeResult() { Type = $"UpdateConnection_{PID}" }.ToJson());
        }

        public virtual void Close()
        {
        }

        public virtual bool Connect()
        {
            return false;
        }

        /// <summary>
        /// 重连。默认是"关掉再连"，传输层可覆盖它以加入去重/退避。
        /// </summary>
        public virtual void Reconnect()
        {
            Close();
            Connect();
        }

        public virtual object? InvokeCQPFunction(string function, bool waiting, params object[] args)
        {
            string guid = Guid.NewGuid().ToString();
            if (function.StartsWith("CQ_"))
            {
                function = "InvokeCQP_" + function;
            }
            WaitingMessage[guid] = new InvokeResult();
            if (!waiting)
            {
                Send(new InvokeBody { GUID = guid, Function = function, Args = args }.ToJson());
                WaitingMessage.TryRemove(guid, out _);
                return null;
            }
            if (RequestWaiter.Wait(guid, Connection, AppConfig.Instance.PluginInvokeTimeout,
                () =>
                {
                    Send(new InvokeBody { GUID = guid, Function = function, Args = args }.ToJson());
                }, out _) && WaitingMessage.TryRemove(guid, out InvokeResult? value))
            {
                if (value.Success)
                {
                    return value.Result;
                }
                LogHelper.Error("调用失败", $"GUID={guid}, msg={value.Message}");
                return null;
            }
            else
            {
                WaitingMessage.TryRemove(guid, out _);
                LogHelper.Error("调用超时", "Timeout");
                return null;
            }
        }

        public virtual async Task<object?> InvokeCQPFunctionAsync(string function, bool waiting, params object[] args)
        {
            string guid = Guid.NewGuid().ToString();
            if (function.StartsWith("CQ_"))
            {
                function = "InvokeCQP_" + function;
            }
            WaitingMessage[guid] = new InvokeResult();
            if (!waiting)
            {
                Send(new InvokeBody { GUID = guid, Function = function, Args = args }.ToJson());
                WaitingMessage.TryRemove(guid, out _);
                return null;
            }
            if (await Task.Run(() => RequestWaiter.Wait(guid, Connection, AppConfig.Instance.PluginInvokeTimeout,
                () =>
                {
                    Send(new InvokeBody { GUID = guid, Function = function, Args = args }.ToJson());
                }, out _)).ConfigureAwait(false) && WaitingMessage.TryRemove(guid, out InvokeResult? value))
            {
                if (value.Success)
                {
                    return value.Result;
                }
                LogHelper.Error("调用失败", $"GUID={guid}, msg={value.Message}");
                return null;
            }
            else
            {
                WaitingMessage.TryRemove(guid, out _);
                LogHelper.Error("调用超时", "Timeout");
                return null;
            }
        }

        public virtual void Send(string message)
        {
            throw new NotImplementedException();
        }

        public virtual void SendHeartBeat()
        {
            Send(new InvokeBody()
            {
                Function = "HeartBeat"
            }.ToJson());
        }

        public virtual bool SetConnectionConfig()
        {
            return false;
        }

        public virtual void ShowErrorDialog(string guid, string title, string content, bool canIgnore)
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
            // 这里没有任何人在等这个 GUID（服务端的回执会被忽略），
            // 登记进 WaitingMessage 只会让字典无限增长。
        }

        #endregion Virtual
    }
}