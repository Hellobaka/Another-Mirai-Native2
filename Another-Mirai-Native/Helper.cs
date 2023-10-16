using Newtonsoft.Json;
using System.Runtime.InteropServices;
using System.Text;

namespace Another_Mirai_Native
{
    public static class Helper
    {
        public static Encoding GB18030 = Encoding.GetEncoding("GB18030");

        public static long TimeStamp => (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;

        [DllImport("kernel32.dll", EntryPoint = "lstrlenA", CharSet = CharSet.Ansi)]
        public static extern int LstrlenA(IntPtr ptr);

        public static int MakeRandomID()
        {
            string guid = Guid.NewGuid().ToString();
            string result = "";
            for (int i = 0; i < guid.Length; i++)
            {
                if (result.Length > 10)
                {
                    break;
                }
                if (guid[i] >= '0' && guid[i] <= '9')
                {
                    result += guid[i];
                }
            }
            if (string.IsNullOrEmpty(result))
            {
                return MakeRandomID();
            }
            else
            {
                return int.TryParse(result, out int value) ? value : MakeRandomID();
            }
        }

        public static string ToJson(this object obj)
        {
            return JsonConvert.SerializeObject(obj, Formatting.None);
        }

        public static IntPtr ToNative(this string text) => Marshal.UnsafeAddrOfPinnedArrayElement(Encoding.Convert(Encoding.Unicode, GB18030, Encoding.Unicode.GetBytes(text)), 0);

        public static IntPtr ToNativeV2(this string msg)
        {
            var b = Encoding.UTF8.GetBytes(msg);
            msg = GB18030.GetString(Encoding.Convert(Encoding.UTF8, GB18030, b));
            byte[] messageBytes = GB18030.GetBytes(msg + "\0");
            var messageIntPtr = Marshal.AllocHGlobal(messageBytes.Length);
            Marshal.Copy(messageBytes, 0, messageIntPtr, messageBytes.Length);
            return messageIntPtr;
        }

        public static string ToString(this IntPtr strPtr, Encoding encoding)
        {
            encoding ??= Encoding.Default;

            int len = LstrlenA(strPtr);   //获取指针中数据的长度
            if (len == 0)
            {
                return string.Empty;
            }
            byte[] buffer = new byte[len];
            Marshal.Copy(strPtr, buffer, 0, len);
            return encoding.GetString(buffer);
        }

        public static int ToTimeStamp(this DateTime time) => (int)(time - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;

        /// <summary>
        /// 时间戳转换为DateTime
        /// </summary>
        public static DateTime TimeStamp2DateTime(long timestamp) => new DateTime(1970, 1, 1, 8, 0, 0, DateTimeKind.Local).AddSeconds(timestamp);
    }
}