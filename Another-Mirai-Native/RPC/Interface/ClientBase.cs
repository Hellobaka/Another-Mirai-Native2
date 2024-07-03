using Another_Mirai_Native.Config;
using Another_Mirai_Native.DB;
using Another_Mirai_Native.Model;
using Another_Mirai_Native.Model.Enums;
using Another_Mirai_Native.Native;
using Newtonsoft.Json.Linq;
using System.Diagnostics;

namespace Another_Mirai_Native.RPC.Interface
{
    public class ClientBase : IClient
    {
        public event Action<bool> OnReceiveHeartBeat;

        public static int PID => Process.GetCurrentProcess().Id;

        public object Connection { get; set; }

        public int HeartBeatLostCount { get; set; }

        public Dictionary<string, InvokeResult> WaitingMessage { get; set; } = new();

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
                    else if (caller.Function == "CurrentQQChanged")
                    {
                        if (caller.Args.Length == 2 
                            && long.TryParse(caller.Args[0].ToString(), out long v)
                            && string.IsNullOrEmpty(caller.Args[1]?.ToString()))
                        {
                            AppConfig.Instance.CurrentQQ = v;
                            AppConfig.Instance.CurrentNickName = caller.Args[1].ToString();
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
            InvokeCQPFuntcion("InvokeCore_AddLog", false, model);
        }

        public virtual void ClientStartUp()
        {
            Send(new InvokeResult() { Type = $"ClientStartUp_{PID}", Result = PluginManager.LoadedPlugin.AppInfo.AppId }.ToJson());
        }

        public virtual void Close()
        {
        }

        public virtual bool Connect()
        {
            return false;
        }

        public virtual object InvokeCQPFuntcion(string function, bool waiting, params object[] args)
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
            if (RequestWaiter.Wait(guid, Connection, AppConfig.Instance.PluginInvokeTimeout, out _) && WaitingMessage.TryGetValue(guid, out InvokeResult? value))
            {
                var result = value;
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
            WaitingMessage.Add(guid, new InvokeResult());
        }

        #endregion Virtual
    }
}