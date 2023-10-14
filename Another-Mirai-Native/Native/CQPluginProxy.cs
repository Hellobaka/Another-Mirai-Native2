using Another_Mirai_Native.Config;
using Another_Mirai_Native.Model;
using Another_Mirai_Native.Model.Enums;
using Another_Mirai_Native.WebSocket;
using Fleck;
using System.Runtime.InteropServices;

namespace Another_Mirai_Native.Native
{
    public class CQPluginProxy
    {
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

        private IWebSocketConnection Connection { get; set; }

        private List<string> APIAuthWhiteList { get; set; } = new()
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
            LogHelper.Info("ServerSend", caller.ToJson());
            Connection.Send(caller.ToJson());
            Server.Instance.WaitingMessage.Add(caller.GUID, new InvokeResult());
            for (int i = 0; i < AppConfig.PluginInvokeTimeout / 100; i++)
            {
                if (Server.Instance.WaitingMessage[caller.GUID].Success)
                {
                    var result = Server.Instance.WaitingMessage[caller.GUID];
                    Server.Instance.WaitingMessage.Remove(caller.GUID);
                    return result;
                }
                Thread.Sleep(100);
            }
            return new InvokeResult() { Message = "Timeout" };
        }

        public bool CheckPluginCanInvoke(string invokeName)
        {
            invokeName = invokeName.Replace("CQ_", "");
            if (APIAuthWhiteList.Any(x => x == invokeName))
            {
                return true;
            }
            if (!Enum.TryParse(invokeName, out PluginAPIType authEnum))
            {
                LogHelper.Error("CheckPluginCanInvoke", $"{invokeName} 无法转换为权限枚举");
                return false;
            }
            return CheckPluginCanInvoke(authEnum);
        }

        public bool CheckPluginCanInvoke(PluginAPIType apiType)
        {
            int id = (int)apiType;
            return AppInfo.auth.Any(x => x == id);
        }
    }
}