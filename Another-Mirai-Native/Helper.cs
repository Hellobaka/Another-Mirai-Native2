using Another_Mirai_Native.Config;
using Another_Mirai_Native.Model;
using Another_Mirai_Native.Native;
using Another_Mirai_Native.WebSocket;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace Another_Mirai_Native
{
    public static class Helper
    {
        public static Encoding GB18030 { get; set; } = Encoding.GetEncoding("GB18030");

        public static long TimeStamp => (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;

        /// <summary>
        /// 读取 <see cref="System.Enum"/> 标记 <see cref="System.ComponentModel.DescriptionAttribute"/> 的值
        /// </summary>
        /// <param name="value">原始 <see cref="System.Enum"/> 值</param>
        /// <returns></returns>
        public static string GetDescription(this System.Enum value)
        {
            if (value == null)
            {
                return string.Empty;
            }

            FieldInfo fieldInfo = value.GetType().GetField(value.ToString());
            DescriptionAttribute attribute = fieldInfo.GetCustomAttribute<DescriptionAttribute>(false);
            return attribute.Description;
        }

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
                if (guid[i] is >= '0' and <= '9')
                {
                    result += guid[i];
                }
            }
            return string.IsNullOrEmpty(result) ? MakeRandomID() : int.TryParse(result, out int value) ? value : MakeRandomID();
        }

        /// <summary>
        /// 图片转Base64
        /// </summary>
        /// <param name="picPath">图片路径</param>
        public static string ParsePic2Base64(string picPath)
        {
            if (File.Exists(picPath) is false)
            {
                return "";
            }
            var buffer = File.ReadAllBytes(picPath);
            return Convert.ToBase64String(buffer);
        }

        public static string[] Split(this string message, string pattern)
        {
            List<string> p = new();// 记录下文本与CQ码的位置关系
            string tmp = "";
            for (int i = 0; i < message.Length; i++)// 将消息中的CQ码与文本分离开
            {
                tmp += message[i];// 文本
                if (tmp == pattern)// 此消息中没有其他文本, 只有CQ码
                {
                    p.Add(pattern);
                    tmp = "";
                }
                else if (tmp.EndsWith(pattern))// 消息以CQ码结尾
                {
                    p.Add(tmp.Substring(0, tmp.Length - pattern.Length));// 记录文本位置
                    p.Add(pattern);// 记录CQ码位置
                    tmp = "";
                }
            }
            if (tmp != "")// 文本中没有CQ码, 或不以CQ码结尾
            {
                p.Add(tmp);
            }

            return p.ToArray();
        }

        /// <summary>
        /// 从文本转换为枚举对象
        /// </summary>
        /// <typeparam name="T">待转换枚举</typeparam>
        /// <param name="value">待转换文本</param>
        /// <returns>枚举对象</returns>
        public static T String2Enum<T>(string value)
        {
            return (T)Enum.Parse(typeof(T), value);
        }

        /// <summary>
        /// 时间戳转换为DateTime
        /// </summary>
        public static DateTime TimeStamp2DateTime(long timestamp) => new DateTime(1970, 1, 1, 8, 0, 0, DateTimeKind.Local).AddSeconds(timestamp);

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
        /// 从cqimg中获取图片URL
        /// </summary>
        /// <param name="cqimg"></param>
        /// <returns></returns>
        public static string GetPicUrlFromCQImg(string cqimg)
        {
            string picPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"data\image", cqimg);
            if (!File.Exists(picPath))
            {
                string picUrl = File.ReadAllText(picPath + ".cqimg");
                picUrl = picUrl.Split('\n').Last().Replace("url=", "");
                return picUrl;
            }
            return "";
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="url">网址</param>
        /// <param name="path">目标文件夹</param>
        /// <param name="overwrite">重复时是否覆写</param>
        /// <returns></returns>
        public static async Task<bool> DownloadFile(string url, string fileName, string path, bool overwrite = false)
        {
            using var http = new HttpClient();
            try
            {
                if (string.IsNullOrWhiteSpace(url)) return false;
                if (!overwrite && File.Exists(Path.Combine(path, fileName))) return true;
                var r = await http.GetAsync(url);
                byte[] buffer = await r.Content.ReadAsByteArrayAsync();
                Directory.CreateDirectory(path);
                File.WriteAllBytes(Path.Combine(path, fileName), buffer);
                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return false;
            }
        }

        public static void ShowErrorDialog(Exception ex, bool canIgnore)
        {
            string guid = Guid.NewGuid().ToString();
            if (AppConfig.IsCore)
            {
                Server.Instance.ActiveShowErrorDialog(new InvokeBody
                {
                    GUID = guid,
                    Args = new object[]
                    {
                        0,
                        $"框架发生异常，错误窗口关闭后，框架将会退出：{ex.Message}",
                        ex.StackTrace ?? "",
                        canIgnore ? 1 : 0
                    }
                });
            }
            else
            {
                Client.Instance.Send(new InvokeBody
                {
                    GUID = guid,
                    Args = new object[]
                    {
                        PluginManager.LoadedPlugin.AuthCode,
                        ex.Message,
                        ex.StackTrace ?? "",
                        canIgnore ? 1 : 0
                    },
                    Function = "ShowErrorDialog"
                }.ToJson());
            }
            while (!Server.Instance.WaitingMessage[guid].Success)
            {
                Thread.Sleep(100);
            }
        }
    }
}