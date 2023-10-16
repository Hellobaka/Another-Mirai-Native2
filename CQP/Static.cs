using Another_Mirai_Native.Config;
using Another_Mirai_Native.DB;
using Another_Mirai_Native.Model;
using Another_Mirai_Native.WebSocket;
using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace Another_Mirai_Native.Export
{
    public static class Static
    {
        public static int ToInt(this object o, int defaultValue = 0) => o != null && int.TryParse(o.ToString(), out int value) ? value : defaultValue;

        public static long ToLong(this object o, long defaultValue = 0) => o != null && long.TryParse(o.ToString(), out long value) ? value : defaultValue;

        public static InvokeResult Invoke(string function, params object[] args)
        {
            string guid = Guid.NewGuid().ToString();

            Client.Instance.Send(new InvokeBody { GUID = guid, Function = "InvokeCQP_" + function, Args = args }.ToJson());

            Client.Instance.WaitingMessage.Add(guid, new InvokeResult());
            for (int i = 0; i < AppConfig.PluginInvokeTimeout / 100; i++)
            {
                if (Client.Instance.WaitingMessage.ContainsKey(guid) && Client.Instance.WaitingMessage[guid].Success)
                {
                    var result = Client.Instance.WaitingMessage[guid];
                    Client.Instance.WaitingMessage.Remove(guid);
                    return result;
                }
                Thread.Sleep(100);
            }
            LogHelper.Error("CQPInvoke", "Timeout");
            return new InvokeResult() { Message = "Timeout" };
        }
    }
}