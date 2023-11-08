using Another_Mirai_Native.Config;
using Another_Mirai_Native.DB;
using Another_Mirai_Native.Model;
using Another_Mirai_Native.Model.Enums;
using Another_Mirai_Native.WebSocket;
using Fleck;
using System.Diagnostics;

namespace Another_Mirai_Native.Native
{
    public class CQPluginProxy
    {
        public CQPluginProxy()
        {
        }

        public CQPluginProxy(AppInfo appInfo, IWebSocketConnection connection)
        {
            AppInfo = appInfo;
            Connection = connection;
            ConnectionID = connection.ConnectionInfo.Id;
        }

        public AppInfo AppInfo { get; set; }

        public Guid ConnectionID { get; set; }

        public bool Enabled { get; set; }

        public string PluginName => AppInfo.name;

        public string PluginId => AppInfo.AppId;

        public string PluginPath { get; set; } = "";

        public bool HasConnection { get; set; }

        public IWebSocketConnection Connection { get; set; }

        private static List<string> APIAuthWhiteList { get; set; } = new()
        {
            "getImage",
            "getRecordV2",
            "addLog",
            "setFatal",
            "getAppDirectory",
            "getLoginQQ",
            "getLoginNick",
        };

        public InvokeResult Invoke(InvokeBody caller)
        {
            //LogHelper.Debug("服务端发送", caller.ToJson());
            if (HasConnection is false)
            {
                return null;
            }
            Connection.Send(caller.ToJson());
            Server.Instance.WaitingMessage.Add(caller.GUID, new InvokeResult());
            if (RequestWaiter.Wait(caller.GUID, this, AppConfig.PluginInvokeTimeout)
                && Server.Instance.WaitingMessage.TryGetValue(caller.GUID, out InvokeResult result))
            {
                Server.Instance.WaitingMessage.Remove(caller.GUID);
                return result;
            }
            else
            {
                return new InvokeResult() { Message = "Timeout" };
            }
        }

        public bool CheckPluginCanInvoke(string invokeName)
        {
            invokeName = invokeName.Replace("CQ_", "");
            if (APIAuthWhiteList.Any(x => x == invokeName))
            {
                return true;
            }
            invokeName = invokeName.Replace("sendGroupQuoteMsg", "sendGroupMsg");
            if (!Enum.TryParse(invokeName, out PluginAPIType authEnum))
            {
                LogHelper.Error("调用权限检查", $"{invokeName} 无法转换为权限枚举");
                return false;
            }
            return CheckPluginCanInvoke(authEnum);
        }

        public bool CheckPluginCanInvoke(PluginAPIType apiType)
        {
            int id = (int)apiType;
            return AppInfo.auth.Any(x => x == id);
        }

        public void KillProcess()
        {
            if (HasConnection)
            {
                Invoke(new InvokeBody { Function = "KillProcess" });
            }
            else
            {
                if (PluginManagerProxy.PluginProcessMap.TryGetValue(AppInfo.PID, out Process process))
                {
                    process.Kill();
                }
            }
        }
    }
}