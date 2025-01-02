﻿using Another_Mirai_Native.Config;
using Another_Mirai_Native.DB;
using Another_Mirai_Native.Model;
using Another_Mirai_Native.RPC;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Collections.Concurrent;

namespace Another_Mirai_Native
{
    public static class Helper
    {
        public static Encoding GB18030 { get; set; } = Encoding.GetEncoding("GB18030");

        public static Random Random { get; set; } = new Random();

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

            FieldInfo fieldInfo = value.GetType().GetField(value.ToString());
            DescriptionAttribute attribute = fieldInfo.GetCustomAttribute<DescriptionAttribute>(false);
            return attribute.Description;
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

        public static int ToTimeStamp(this DateTime time) => (int)(time - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;

        public static int ToTimeStamp(this DateTime? time) => (int)((time ?? DateTime.Now) - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;

        /// <summary>
        /// 从cqimg中获取图片URL
        /// </summary>
        /// <param name="cqimg"></param>
        /// <returns></returns>
        public static string GetPicUrlFromCQImg(string cqimg)
        {
            string picPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"data\image", cqimg);
            if (!cqimg.Contains('.'))
            {
                picPath += ".cqimg";
            }
            if (File.Exists(picPath))
            {
                string picUrl = File.ReadAllText(picPath);
                picUrl = picUrl.Split('\n').Last().Replace("url=", "");
                return picUrl;
            }
            return "";
        }

        public static string GetPicName(this CQCode cqimg)
        {
            return cqimg.Items.TryGetValue("file", out var file) ? file : "";
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
                LogHelper.Error("下载文件", e);
                return false;
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
            if (AppConfig.Instance.IsCore)
            {
                ServerManager.Server.ActiveShowErrorDialog(guid,
                        0,
                        $"框架发生异常，错误窗口关闭后，框架将会退出：{ex.Message}",
                        ex.StackTrace ?? "",
                        canIgnore);
                ServerManager.Server.WaitingMessage.Add(guid, new InvokeResult());
            }
            else
            {
                ClientManager.Client.ShowErrorDialog(guid, ex.Message, ex.StackTrace, canIgnore);
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

        public static string GetPicNameFromUrl(string imageUrl)
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

        public static string GetImageUrlOrPathFromCQCode(CQCode cqCode)
        {
            string ParseUrlFromCQImg(string filePath)
            {
                Regex regex = new("url=(.*)");
                var a = regex.Match(File.ReadAllText(filePath));
                if (a.Groups.Count > 1)
                {
                    string capture = a.Groups[1].Value;
                    capture = capture.Split('\r').First();
                    return capture;
                }
                else
                {
                    return "";
                }
            }

            if (cqCode.IsImageCQCode is false)
            {
                return "";
            }
            string file = cqCode.Items["file"];
            string basePath = @"data\image";

            string filePath = Path.Combine(basePath, file);
            if (File.Exists(filePath))
            {
                if (filePath.EndsWith(".cqimg"))
                {
                    return ParseUrlFromCQImg(filePath);
                }
                else
                {
                    return new FileInfo(filePath).FullName;
                }
            }
            else
            {
                filePath += ".cqimg";
                if (File.Exists(filePath))
                {
                    return ParseUrlFromCQImg(filePath);
                }
                else
                {
                    return "";
                }
            }
        }

        /// <summary>
        /// 下载或读取缓存图片
        /// </summary>
        /// <param name="imageUrl">欲下载的图片URL</param>
        /// <returns>本地图片路径</returns>
        public static async Task<string?> DownloadImageAsync(string imageUrl, string fileName = "")
        {
            try
            {
                if (string.IsNullOrEmpty(imageUrl))
                {
                    return null;
                }
                string cacheImagePath = Path.Combine("data", "image", "cached");

                Directory.CreateDirectory(cacheImagePath);
                if (!imageUrl.StartsWith("http"))// 下载并非http请求, 则更改为本地文件
                {
                    return new FileInfo(imageUrl).FullName;
                }
                string name;
                if (fileName == "")
                {
                    name = GetPicNameFromUrl(imageUrl);// 解析图片唯一ID
                    if (string.IsNullOrEmpty(name))
                    {
                        name = imageUrl.MD5(); // 无法解析时尝试使用哈希作为文件名
                    }
                }
                else
                {
                    name = fileName;
                }
                // 尝试从本地读取缓存
                string? path = GetFromCache(cacheImagePath, name);
                if (!string.IsNullOrEmpty(path))
                {
                    return path;
                }

                return await DownloadFileFromWebAsync(cacheImagePath, name, imageUrl);
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog((int)Model.Enums.LogLevel.Debug, "DownloadImageAsync", ex.Message + ex.StackTrace);
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
    }
}