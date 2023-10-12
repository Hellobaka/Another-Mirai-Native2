using Another_Mirai_Native.Config;
using Another_Mirai_Native.Model;
using Another_Mirai_Native.Model.Enums;
using Another_Mirai_Native.WebSocket;
using Fleck;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Another_Mirai_Native.Native
{
    public class CQPluginProxy
    {
        public CQPluginProxy(AppInfo appInfo, IWebSocketConnection connection)
        {
            AppInfo = appInfo;
            Connection = connection;
            ID = connection.ConnectionInfo.Id;
        }

        public AppInfo AppInfo { get; set; }

        public Guid ID { get; set; }

        private IWebSocketConnection Connection { get; set; }

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
    }
}