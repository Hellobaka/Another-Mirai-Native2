using Another_Mirai_Native.Config;
using Another_Mirai_Native.Model;
using Another_Mirai_Native.WebSocket;
using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Another_Mirai_Native.Export
{
    public static class Static
    {
        public static Encoding GB18030 = Encoding.GetEncoding("GB18030");

        [DllImport("kernel32.dll", EntryPoint = "lstrlenA", CharSet = CharSet.Ansi)]
        public static extern int LstrlenA(IntPtr ptr);

        public static int ToInt(this object o, int defaultValue = 0) => o != null && int.TryParse(o.ToString(), out int value) ? value : defaultValue;

        public static long ToLong(this object o, long defaultValue = 0) => o != null && long.TryParse(o.ToString(), out long value) ? value : defaultValue;

        public static IntPtr ToNative(this string text) => Marshal.UnsafeAddrOfPinnedArrayElement(Encoding.Convert(Encoding.Unicode, GB18030, Encoding.Unicode.GetBytes(text)), 0);

        public static string ToString(this IntPtr strPtr, Encoding encoding)
        {
            if (encoding == null)
            {
                encoding = Encoding.Default;
            }

            int len = LstrlenA(strPtr);   //获取指针中数据的长度
            if (len == 0)
            {
                return string.Empty;
            }
            byte[] buffer = new byte[len];
            Marshal.Copy(strPtr, buffer, 0, len);
            return encoding.GetString(buffer);
        }

        public static InvokeResult Invoke(string function, params object[] args)
        {
            string guid = Guid.NewGuid().ToString();
            Client.Instance.Send(new InvokeBody { GUID = guid, Function = "InvokeCQP_" + function, Args = args }.ToJson());
            Client.Instance.WaitingMessage.Add(guid, new InvokeResult());
            for (int i = 0; i < AppConfig.PluginInvokeTimeout; i++)
            {
                if (Client.Instance.WaitingMessage.ContainsKey(guid) && Client.Instance.WaitingMessage[guid].Success)
                {
                    var result = Client.Instance.WaitingMessage[guid];
                    Client.Instance.WaitingMessage.Remove(guid);
                    return result;
                }
            }
            LogHelper.Error("CQPInvoke", "Timeout");
            return new InvokeResult() { Message = "Timeout" };
        }
    }
}