using Another_Mirai_Native.Abstractions.Enums;
using Another_Mirai_Native.Abstractions.Models;
using Another_Mirai_Native.Abstractions.Models.MessageItem;
using Another_Mirai_Native.Config;
using Another_Mirai_Native.DB;
using Another_Mirai_Native.Model;
using Another_Mirai_Native.Model.Enums;
using Another_Mirai_Native.RPC;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Net.Http;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Another_Mirai_Native
{
    public static class Helper
    {
        public static Encoding GB18030 { get; set; } = Encoding.GetEncoding("GB18030");

        private static int Number { get; set; } = 10000;

        private static object LockObject { get; set; } = new object();

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

            FieldInfo? fieldInfo = value.GetType().GetField(value.ToString());
            DescriptionAttribute? attribute = fieldInfo?.GetCustomAttribute<DescriptionAttribute>(false);
            return attribute?.Description ?? "";
        }

        [DllImport("kernel32.dll", EntryPoint = "lstrlenA", CharSet = CharSet.Ansi)]
        public static extern int LstrlenA(IntPtr ptr);

        public static int MakeUniqueID()
        {
            lock (LockObject)
            {
                Number++;
                if (Number > int.MaxValue - 10)
                {
                    Number = 10000;
                }
                return Number;
            }
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

        public static string[] SplitV1(this string message, string pattern)
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

        public static string[] SplitV2(this string message, string pattern)
        {
            string regexPattern = $"({pattern})";
            var parts = Regex.Split(message, regexPattern);

            var ls = parts.ToList();
            ls.RemoveAll(string.IsNullOrEmpty);
            return ls.ToArray();
        }

        public static MessageItemBase[] ToMessageChain(this string text)
        {
            var parts = text.SplitV2("\\[CQ:.*?\\]");
            List<MessageItemBase> messageChain = [];
            foreach (var item in parts)
            {
                if (item.StartsWith("[CQ:"))
                {
                    var cqcode = CQCode.Parse(item).FirstOrDefault();
                    if (cqcode == null)
                    {
                        continue;
                    }
                    switch (cqcode.Function)
                    {
                        case MessageItemType.Face:
                            if (int.TryParse(cqcode.Items["id"], out int id))
                            {
                                messageChain.Add(new Face(id));
                            }
                            break;

                        case MessageItemType.Bface:
                            if (int.TryParse(cqcode.Items["id"], out id))
                            {
                                messageChain.Add(new BFace(id));
                            }
                            break;

                        case MessageItemType.Image:
                            string file = cqcode.Items["file"];
                            bool isFlash = cqcode.Items.ContainsKey("flash") && cqcode.Items["flash"] == "true";
                            bool isPath = file.Contains("\\");
                            bool isEmoji = cqcode.Items.ContainsKey("sub_type") && cqcode.Items["sub_type"] == "1";
                            if (isPath)
                            {
                                messageChain.Add(new Abstractions.Models.MessageItem.Image(filePath: file, isFlash: isFlash, isEmoji: isEmoji));
                            }
                            else
                            {
                                messageChain.Add(new Abstractions.Models.MessageItem.Image(hash: file, isFlash: isFlash, isEmoji: isEmoji));
                            }

                            break;

                        case MessageItemType.Record:
                            file = cqcode.Items["file"];
                            isPath = file.Contains("\\");
                            if (isPath)
                            {
                                messageChain.Add(new Record(filePath: file));
                            }
                            else
                            {
                                messageChain.Add(new Record(hash: file));
                            }
                            break;

                        case MessageItemType.At:
                            var qq = cqcode.Items["qq"];
                            if (qq == "all")
                            {
                                messageChain.Add(new At(0, true));
                            }
                            else if (long.TryParse(qq, out long qqNum))
                            {
                                messageChain.Add(new At(qqNum, false));
                            }
                            break;

                        case MessageItemType.Rps:
                            if (int.TryParse(cqcode.Items["type"], out int type))
                            {
                                messageChain.Add(new RPS((RpsType)type));
                            }
                            break;

                        case MessageItemType.Shake:
                            messageChain.Add(new Shake());
                            break;

                        case MessageItemType.Dice:
                            if (int.TryParse(cqcode.Items["type"], out type))
                            {
                                messageChain.Add(new Dice(type));
                            }
                            break;

                        case MessageItemType.Poke:
                            messageChain.Add(new Poke(cqcode.Items["name"]));
                            break;

                        case MessageItemType.Rich:
                            RichContentType richContentType = Enum.TryParse<RichContentType>(cqcode.Items["type"], true, out var result) ? result : RichContentType.Json;
                            messageChain.Add(new RichContent(richContentType, cqcode.Items["content"]));
                            break;

                        case MessageItemType.Reply:
                            if (int.TryParse(cqcode.Items["id"], out id))
                            {
                                messageChain.Add(new Reply(id));
                            }
                            break;

                        case MessageItemType.File:
                            if (int.TryParse(cqcode.Items["file_size"], out int fileSize)
                                && cqcode.Items.TryGetValue("file", out string fileName))
                            {
                                messageChain.Add(new FileItem(fileName, fileSize));
                            }
                            break;

                        default:
                            // 无效的CQ码
                            messageChain.Add(new Unknown(item));
                            continue;
                    }
                }
                else
                {
                    messageChain.Add(new Text(item));
                }
            }

            return messageChain.ToArray();
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
      
        public static long DateTime2TimeStamp(DateTime time) => (long)(time.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;

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

        public static DateTime ToDateTime(this long timestamp) => TimeStamp2DateTime(timestamp);

        public static DateTime ToDateTime(this int timestamp) => TimeStamp2DateTime(timestamp);

        public static int ToTimeStamp(this DateTime time) => (int)DateTime2TimeStamp(time);

        public static int ToTimeStamp(this DateTime? time) => (int)DateTime2TimeStamp(time ?? DateTime.Now);

        internal static string GetPicName(this CQCode cqimg)
        {
            return cqimg.Items.TryGetValue("file", out var file) ? file : "";
        }

        public static string GetCachePictureDirectory(bool cache = true)
        {
            return GetCacheDirectoryByCachedFileType(CachedFileType.Image, cache);
        }

        public static string GetCacheDirectoryByCachedFileType(CachedFileType cachedFileType, bool cache = true)
        {
            string type = cachedFileType switch
            {
                CachedFileType.Video => "video",
                CachedFileType.Record => "record",
                _ => "image"
            };
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, cache ?  Path.Combine("data", type, "cached") : Path.Combine("data", type));
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="url">网址</param>
        /// <param name="path">目标文件夹</param>
        /// <param name="fileName">无法获取文件名时的回滚值</param>
        /// <param name="overwrite">重复时是否覆写</param>
        /// <returns></returns>
        public static async Task<(bool success, string fullPath)> DownloadFile(string url, string fileName, string path, bool overwrite = false)
        {
            using var http = new HttpClient();
            try
            {
                if (string.IsNullOrWhiteSpace(url)) return (false, "");
                if (!overwrite && File.Exists(Path.Combine(path, fileName))) return (true, Path.Combine(path, fileName));
                var r = await http.GetAsync(url);
                r.EnsureSuccessStatusCode();

                var contentDisposition = r.Content.Headers.ContentDisposition;
                if (contentDisposition != null && !string.IsNullOrEmpty(contentDisposition.FileName))
                {
                    fileName = contentDisposition.FileName.Trim('"');
                }
                // 从请求中获取扩展名
                string? extension = "";
                if (!string.IsNullOrEmpty(r.Content.Headers.ContentType?.MediaType))
                {
                    extension = r.Content.Headers.ContentType?.MediaType.Split('/').Last();
                }
                if (!string.IsNullOrEmpty(extension))
                {
                    fileName = Path.ChangeExtension(fileName, extension);
                }

                byte[] buffer = await r.Content.ReadAsByteArrayAsync();
                Directory.CreateDirectory(path);
                File.WriteAllBytes(Path.Combine(path, fileName), buffer);

                return (true, Path.Combine(path, fileName));
            }
            catch (Exception e)
            {
                LogHelper.Error("下载文件", e);
                return (false, "");
            }
        }
        
        public static async Task<string> DownloadString(string url)
        {
            using var http = new HttpClient();
            try
            {
                if (string.IsNullOrWhiteSpace(url)) return "";
                var r = await http.GetAsync(url);
                byte[] buffer = await r.Content.ReadAsByteArrayAsync();
                return Encoding.UTF8.GetString(buffer);
            }
            catch (Exception e)
            {
                LogHelper.Error("下载字符串", e);
                return "";
            }
        }

        public static void ShowErrorDialog(Exception ex, bool canIgnore)
        {
            string guid = Guid.NewGuid().ToString();
            if (ServerManager.Server == null)
            {
                // TODO: 控制台版本如何处理
                return;
            }
            if (AppConfig.Instance.IsCore)
            {
                ServerManager.Server.ActiveShowErrorDialog(guid,
                        0,
                        $"框架发生异常，错误窗口关闭后，框架将会退出：{ex.Message}",
                        ex.StackTrace ?? "",
                        canIgnore);
                ServerManager.Server.WaitingMessage.AddOrUpdate(guid, new InvokeResult(), (key, oldValue) => new InvokeResult());
            }
            else
            {
                ClientManager.Client.ShowErrorDialog(guid, ex.Message, ex.StackTrace ?? "", canIgnore);
            }
            if (ServerManager.Server.ShowErrorDialogEventHasSubscribed)
            {
                ManualResetEvent waitEvent = new(false);
                RequestWaiter.CommonWaiter.TryAdd(guid, new WaiterInfo
                {
                    WaitSignal = waitEvent,
                });
                waitEvent.WaitOne();
            }
        }

        public static string GetFileNameFromUrl(string imageUrl)
        {
            Regex regex = new(".*gchat\\.qpic\\.cn\\/gchatpic_new.*?-.*?-(.*)\\/.*");
            if (regex.Match(imageUrl).Success)
            {
                return regex.Match(imageUrl).Groups[1].Value;
            }
            regex = new(".*q\\.qlogo\\.cn.*?&nk=(.*?)&s.*");
            if (regex.Match(imageUrl).Success)
            {
                return regex.Match(imageUrl).Groups[1].Value;
            }
            regex = new(".*p\\.qlogo\\.cn\\/gh\\/(.*?)\\/.*");
            if (regex.Match(imageUrl).Success)
            {
                return regex.Match(imageUrl).Groups[1].Value;
            }
            return "";
        }

        public static string MD5(this string raw)
        {
            if (raw == null)
            {
                return "";
            }
            return Encoding.UTF8.GetBytes(raw).MD5();
        }

        public static string MD5(this byte[] raw)
        {
            if (raw == null)
            {
                return "";
            }
            var md5 = System.Security.Cryptography.MD5.Create();
            return BitConverter.ToString(md5.ComputeHash(raw)).Replace("-", "").ToUpper();
        }

        public static void OpenFolder(string path)
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = path,
                UseShellExecute = true,
            };
            Process.Start(startInfo);
        }

        public static string GetRelativePath(string value, string currentDirectory)
        {
            if (File.Exists(value))
            {
                string fullPath = Path.GetFullPath(value).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
                string currentDir = currentDirectory.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar) + Path.DirectorySeparatorChar;

                if (fullPath.StartsWith(currentDir, StringComparison.OrdinalIgnoreCase))
                {
                    return fullPath.Substring(currentDir.Length);
                }
                else
                {
                    return fullPath;
                }
            }
            else
            {
                return string.Empty;
            }
        }

        public static void CreateDirectoryLink(string path1, string path2)
        {
            if (Directory.Exists(path1))
            {
                return;
            }
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                Process.Start("cmd.exe", $"/C mklink /J {path1} {path2}").WaitForExit();
            }
            else
            {
                throw new PlatformNotSupportedException("Only Support Windows");
            }
        }

        /// <summary>
        /// 下载或读取缓存文件
        /// </summary>
        /// <param name="fileUrl">欲下载的文件URL</param>
        /// <returns>本地文件绝对路径</returns>
        public static async Task<string?> DownloadFileAsync(string baseDirectory, string fileUrl, string fileName = "")
        {
            try
            {
                if (string.IsNullOrEmpty(fileUrl))
                {
                    return null;
                }

                Directory.CreateDirectory(baseDirectory);
                if (!fileUrl.StartsWith("http"))// 下载并非http请求, 则更改为本地文件
                {
                    return new FileInfo(fileUrl).FullName;
                }
                string name;
                if (fileName == "")
                {
                    name = GetFileNameFromUrl(fileUrl);// 解析图片唯一ID
                    if (string.IsNullOrEmpty(name))
                    {
                        name = fileUrl.MD5(); // 无法解析时尝试使用哈希作为文件名
                    }
                }
                else
                {
                    name = fileName;
                }
                // 尝试从本地读取缓存
                string? path = GetFromCache(baseDirectory, name);
                if (!string.IsNullOrEmpty(path))
                {
                    return path;
                }

                return await DownloadFileFromWebAsync(baseDirectory, name, fileUrl);
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog((int)Model.Enums.LogLevel.Debug, "DownloadFileAsync", ex.Message + ex.StackTrace);
                return null;
            }
        }

        /// <summary>
        /// 下载图片
        /// </summary>
        /// <param name="cacheImagePath">缓存图片路径</param>
        /// <param name="name">文件名</param>
        /// <param name="imageUrl">目标下载Url</param>
        /// <returns>下载后完全路径</returns>
        private static async Task<string?> DownloadFileFromWebAsync(string cacheImagePath, string name, string imageUrl)
        {
            using var client = new HttpClient();
            var response = await client.GetAsync(imageUrl);
            response.EnsureSuccessStatusCode();
            // 文件类型已知
            if (string.IsNullOrEmpty(response.Content.Headers.ContentType?.MediaType))
            {
                return null;
            }
            string path = Path.Combine(cacheImagePath, name + "." + (response.Content.Headers.ContentType?.MediaType.Split('/').Last() ?? "jpg"));
            var imageBytes = await response.Content.ReadAsByteArrayAsync();
            File.WriteAllBytes(path, imageBytes);

            return new DirectoryInfo(path).FullName;
        }

        /// <summary>
        /// 从缓存获取图片
        /// </summary>
        /// <param name="cacheImagePath">图片缓存文件夹</param>
        /// <param name="name">文件名</param>
        /// <param name="avatarTypes"></param>
        /// <returns>文件的完全路径</returns>
        private static string? GetFromCache(string cacheImagePath, string name)
        {
            // 检测文件是否已经存在
            string? path = Directory.GetFiles(cacheImagePath).FirstOrDefault(x => Path.GetFileNameWithoutExtension(x) == name);
            if (!string.IsNullOrEmpty(path))
            {
                return new DirectoryInfo(path).FullName;
            }

            return null;
        }

        public static long ToLong(this uint? value) => value ?? 0;

        public static string Clean(this string input)
        {
            StringBuilder sb = new();
            foreach (char c in input)
            {
                UnicodeCategory category = char.GetUnicodeCategory(c);

                // 过滤异常字符
                if (category != UnicodeCategory.NonSpacingMark &&
                    category != UnicodeCategory.OtherNotAssigned)
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        public static int IndexOf(this List<byte> buffer, byte[] delimiter)
        {
            for (int i = 0; i <= buffer.Count - delimiter.Length; i++)
            {
                if (buffer.Skip(i).Take(delimiter.Length).SequenceEqual(delimiter))
                {
                    return i;
                }
            }
            return -1;
        }

        #region Random
#if NET5_0_OR_GREATER
        public static int RandomNext(int min, int max) => Random.Shared.Next(min, max);
      
        public static int RandomNext() => Random.Shared.Next();

        public static double RandomNextDouble() => Random.Shared.NextDouble();
#else
        private static RNGCryptoServiceProvider Rng { get; set; } = new();

        public static int RandomNext()
        {
            return RandomNext(0, int.MaxValue);
        }

        public static int RandomNext(int minValue, int maxValue)
        {
            if (minValue >= maxValue)
            {
                throw new ArgumentOutOfRangeException();
            }

            long diff = (long)maxValue - minValue;
            byte[] uint32Buffer = new byte[4];
            uint rand;
            do
            {
                Rng.GetBytes(uint32Buffer);
                rand = BitConverter.ToUInt32(uint32Buffer, 0);
            }
            while (rand >= (uint.MaxValue - (((uint.MaxValue % diff) + 1) % diff)));

            return (int)(minValue + (rand % diff));
        }

        public static double RandomNextDouble()
        {
            byte[] bytes = new byte[8];
            Rng.GetBytes(bytes);
            ulong ul = BitConverter.ToUInt64(bytes, 0) >> 11; // 53位精度
            return ul / (double)(1UL << 53);
        }
#endif
        #endregion
    }
}