using Another_Mirai_Native.DB;
using Another_Mirai_Native.Model.Enums;
using System.Net.Http;

namespace Another_Mirai_Native
{
    /// <summary>解析视频的本地路径、缓存哈希、URL 和 base64 数据。</summary>
    public static class VideoFileHelper
    {
        private static readonly HttpClient Client = new();

        public static bool IsHttpUrl(string file)
        {
            return Uri.TryCreate(file, UriKind.Absolute, out var uri)
                && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
        }

        public static string? GetLocalPath(string file)
        {
            if (file.StartsWith("base64://", StringComparison.OrdinalIgnoreCase) || IsHttpUrl(file))
            {
                return null;
            }
            if (Uri.TryCreate(file, UriKind.Absolute, out var uri) && uri.IsFile)
            {
                file = uri.LocalPath;
            }
            string path = Path.Combine(Helper.GetCacheDirectoryByCachedFileType(CachedFileType.Video, false), file);
            if (File.Exists(path))
            {
                return Path.GetFullPath(path);
            }
            var cached = CachedFile.GetCachedVideoByHash(file);
            if (cached != null)
            {
                path = Path.Combine(Helper.GetCacheDirectoryByCachedFileType(CachedFileType.Video), cached.FileName);
                if (File.Exists(path))
                {
                    return path;
                }
            }
            return null;
        }

        /// <summary>读取发送数据；URL 和 base64 不会写入本地文件。</summary>
        public static async Task<byte[]> ReadAsync(string file)
        {
            if (file.StartsWith("base64://", StringComparison.OrdinalIgnoreCase))
            {
                return Convert.FromBase64String(file.Substring("base64://".Length));
            }
            if (IsHttpUrl(file))
            {
                return await Client.GetByteArrayAsync(file).ConfigureAwait(false);
            }
            string? path = GetLocalPath(file);
            if (path != null)
            {
                return File.ReadAllBytes(path);
            }
            var cached = CachedFile.GetCachedVideoByHash(file);
            if (cached != null && IsHttpUrl(cached.Url))
            {
                return await Client.GetByteArrayAsync(cached.Url).ConfigureAwait(false);
            }
            throw new FileNotFoundException($"视频文件不存在：{file}");
        }
    }
}
