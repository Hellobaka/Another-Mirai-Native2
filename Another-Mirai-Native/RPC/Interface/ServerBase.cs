using Another_Mirai_Native.Config;
using Another_Mirai_Native.DB;
using Another_Mirai_Native.Model;
using Another_Mirai_Native.Model.Enums;
using Another_Mirai_Native.Native;
using System.Reflection;

namespace Another_Mirai_Native.RPC.Interface
{
    public class ServerBase : IServer
    {
        public Dictionary<string, InvokeResult> WaitingMessage { get; set; } = new();

        public event Action<string, int, string, string, bool> OnShowErrorDialogCalled;

        public bool ShowErrorDialogEventHasSubscribed => OnShowErrorDialogCalled?.GetInvocationList().Length > 0;

        public void ActiveShowErrorDialog(string guid, int authCode, string title, string content, bool canIgnore)
        {
            OnShowErrorDialogCalled?.Invoke(guid, authCode, title, content, canIgnore);
        }

        public void ShowErrorDialog(string guid, int authCode, string title, string content, bool canIgnore)
        {
            WaitingMessage.Add(guid, new InvokeResult());
            OnShowErrorDialogCalled?.Invoke(guid, authCode, title, content, canIgnore);
            RequestWaiter.Wait(guid, -1, out _);
            WaitingMessage.Remove(guid);
        }

        public int? AddLog(LogModel log)
        {
            return log == null ? null : LogHelper.WriteLog(log);
        }

        public void ClientStartup(int pid, string appId)
        {
            var proxy = PluginManagerProxy.Proxies.FirstOrDefault(x => x.PluginProcess != null && x.PluginProcess.Id == pid);
            if (proxy != null)
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

        #region Virtual

        public virtual void ClientDisconnect(CQPluginProxy plugin)
        {
            PluginManagerProxy.SetProxyDisconnected(plugin);
        }

        public virtual int? InvokeEvents(CQPluginProxy target, PluginEventType eventType, params object[] args)
        {
            return null;
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