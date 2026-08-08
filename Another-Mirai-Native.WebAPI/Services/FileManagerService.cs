using Another_Mirai_Native.WebAPI.Models;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Data.Sqlite;
using Microsoft.VisualBasic.FileIO;
using SqlSugar;
using System.Data;
using System.Globalization;
using System.IO.Compression;
using System.Text.RegularExpressions;
using System.Text;

namespace Another_Mirai_Native.WebAPI.Services
{
    /// <summary>
    /// 文件管理器操作异常，携带 HTTP 状态码
    /// </summary>
    public class FileManagerException : Exception
    {
        public int StatusCode { get; }

        public FileManagerException(int statusCode, string message) : base(message)
        {
            StatusCode = statusCode;
        }
    }

    /// <summary>
    /// SQL 语法错误，由 SQLite 解析失败时抛出
    /// </summary>
    public class SqlSyntaxException : Exception
    {
        public SqlSyntaxException(string message) : base(message)
        {
        }
    }

    /// <summary>
    /// 二进制文件被当作文本加载/保存时抛出；仅返回 400，不记录 Warning 日志
    /// </summary>
    public class BinaryFileException : FileManagerException
    {
        public BinaryFileException(string message) : base(400, message)
        {
        }
    }

    /// <summary>
    /// 文件管理器核心服务：所有文件路径均限制在配置的根目录内
    /// </summary>
    public static class FileManagerService
    {
        public const long MaxTextFileSize = 10L * 1024 * 1024;
        public const int MaxQueryRows = 1000;
        public const int MaxPreviewPageSize = 200;

        private static readonly HashSet<string> SqliteExtensions = new(StringComparer.OrdinalIgnoreCase)
        {
            ".db", ".sqlite", ".sqlite3"
        };

        private static readonly HashSet<string> ReservedFileNames = new(StringComparer.OrdinalIgnoreCase)
        {
            "CON", "PRN", "AUX", "NUL",
            "COM1", "COM2", "COM3", "COM4", "COM5", "COM6", "COM7", "COM8", "COM9",
            "LPT1", "LPT2", "LPT3", "LPT4", "LPT5", "LPT6", "LPT7", "LPT8", "LPT9"
        };

        private static readonly FileExtensionContentTypeProvider ContentTypeProvider = new();

        /// <summary>
        /// 判断异常是否由文件/数据库被其他进程占用导致（共享冲突、锁冲突、SQLite busy/locked）
        /// </summary>
        public static bool IsFileInUse(Exception exception)
        {
            if (exception is SqliteException sqlite && sqlite.SqliteErrorCode is 5 or 6)
            {
                return true;
            }
            if (exception is not (IOException or UnauthorizedAccessException))
            {
                return false;
            }

            // ERROR_SHARING_VIOLATION(32) / ERROR_LOCK_VIOLATION(33) 对应的 HRESULT
            var hresult = unchecked((uint)exception.HResult);
            if (hresult is 0x80070020 or 0x80070021)
            {
                return true;
            }
            if ((hresult & 0xFFFF0000) == 0x80070000 && (hresult & 0xFFFF) is 32 or 33)
            {
                return true;
            }

            // 某些包装异常未携带 Win32 错误码时，按消息兜底
            var message = exception.Message;
            return message.Contains("being used by another process", StringComparison.OrdinalIgnoreCase)
                || message.Contains("sharing violation", StringComparison.OrdinalIgnoreCase)
                || message.Contains("another process has locked", StringComparison.OrdinalIgnoreCase)
                || message.Contains("另一个程序正在使用此文件", StringComparison.OrdinalIgnoreCase)
                || message.Contains("正由另一进程使用", StringComparison.OrdinalIgnoreCase)
                || message.Contains("另一个进程已锁定", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// 获取文件管理器根目录：空/空白取程序目录，相对路径相对程序目录解析，绝对路径原样使用
        /// </summary>
        public static string GetRootPath()
        {
            var configured = WebAPIConfig.Instance.FileManagerRoot;
            string root;
            if (string.IsNullOrWhiteSpace(configured))
            {
                root = AppContext.BaseDirectory;
            }
            else if (Path.IsPathRooted(configured))
            {
                root = Path.GetFullPath(configured);
            }
            else
            {
                root = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, configured));
            }
            return Path.TrimEndingDirectorySeparator(root);
        }

        /// <summary>
        /// 将前端传入的相对路径解析为根目录内的绝对路径，含路径穿越与链接逃逸校验
        /// </summary>
        public static string ResolvePath(string? path)
        {
            var root = GetRootPath();
            var relative = NormalizeRelativePath(path);
            var full = relative.Length == 0
                ? root
                : Path.GetFullPath(Path.Combine(root, relative));
            EnsureWithinRoot(full, root);
            EnsureNoEscapingLink(full, root);
            return full;
        }

        /// <summary>
        /// 将根目录内绝对路径转为前端使用的相对路径（/ 分隔），根目录返回 ""
        /// </summary>
        public static string ToRelativePath(string fullPath, string? root = null)
        {
            var rootPath = root ?? GetRootPath();
            if (string.IsNullOrEmpty(fullPath) || !IsWithinRoot(fullPath, rootPath))
            {
                return "";
            }
            var relative = Path.GetRelativePath(rootPath, fullPath);
            return relative == "." ? "" : relative.Replace('\\', '/');
        }

        public static ListDirectoryResult ListDirectory(string? path)
        {
            var full = ResolvePath(path);
            var directory = new DirectoryInfo(full);
            if (!directory.Exists)
            {
                throw new FileManagerException(404, "目录不存在");
            }

            var root = GetRootPath();
            var items = new List<FileEntryDto>();
            try
            {
                foreach (var info in directory.EnumerateFileSystemInfos()
                    .OrderBy(x => x is DirectoryInfo ? 0 : 1)
                    .ThenBy(x => x.Name, StringComparer.OrdinalIgnoreCase))
                {
                    items.Add(ToFileEntry(info, root));
                }
            }
            catch (UnauthorizedAccessException)
            {
                throw new FileManagerException(403, "没有权限访问该目录");
            }

            var parentFull = directory.Parent?.FullName;
            return new ListDirectoryResult
            {
                Root = root,
                Path = ToRelativePath(full, root),
                Parent = parentFull == null ? "" : ToRelativePath(parentFull, root),
                Items = items
            };
        }

        public static void CreateFolder(string? path)
        {
            var full = ResolvePath(path);
            EnsureNotRoot(full, "根目录已存在");
            if (File.Exists(full) || Directory.Exists(full))
            {
                throw new FileManagerException(409, "同名文件或文件夹已存在");
            }
            Directory.CreateDirectory(full);
        }

        public static void CreateFile(string? path, string? content)
        {
            var full = ResolvePath(path);
            EnsureNotRoot(full, "无法在根目录位置新建文件");
            if (File.Exists(full) || Directory.Exists(full))
            {
                throw new FileManagerException(409, "同名文件或文件夹已存在");
            }
            var parent = Path.GetDirectoryName(full);
            if (!string.IsNullOrEmpty(parent))
            {
                Directory.CreateDirectory(parent);
            }
            if (string.IsNullOrEmpty(content))
            {
                File.WriteAllBytes(full, Array.Empty<byte>());
            }
            else
            {
                File.WriteAllText(full, content, new UTF8Encoding(false));
            }
        }

        public static void Rename(string? path, string newName)
        {
            var full = ResolvePath(path);
            EnsureNotRoot(full, "不能重命名根目录");
            var normalizedName = NormalizeFileName(newName);
            if (!File.Exists(full) && !Directory.Exists(full))
            {
                throw new FileManagerException(404, "文件或文件夹不存在");
            }

            var parent = Path.GetDirectoryName(full);
            if (string.IsNullOrEmpty(parent))
            {
                throw new FileManagerException(400, "无法获取父目录");
            }
            var target = Path.Combine(parent, normalizedName);
            if (File.Exists(target) || Directory.Exists(target))
            {
                throw new FileManagerException(409, "同名文件或文件夹已存在");
            }

            if (File.Exists(full))
            {
                File.Move(full, target);
            }
            else
            {
                Directory.Move(full, target);
            }
        }

        public static void Copy(List<string> sources, string? targetDir)
        {
            var targetFull = ResolvePath(targetDir);
            EnsureDirectoryExists(targetFull, "目标目录不存在");
            var root = GetRootPath();

            foreach (var source in sources)
            {
                var sourceFull = ResolvePath(source);
                EnsureNotRoot(sourceFull, "不能复制根目录");
                if (!File.Exists(sourceFull) && !Directory.Exists(sourceFull))
                {
                    throw new FileManagerException(404, $"源路径不存在：{source}");
                }

                var name = Path.GetFileName(sourceFull);
                var destination = Path.Combine(targetFull, name);
                if (File.Exists(destination) || Directory.Exists(destination))
                {
                    throw new FileManagerException(409, $"目标已存在：{ToRelativePath(destination, root)}");
                }

                if (Directory.Exists(sourceFull))
                {
                    if (IsWithinRoot(targetFull, sourceFull))
                    {
                        throw new FileManagerException(400, "不能将文件夹复制到其自身内部");
                    }
                    CopyDirectory(sourceFull, destination, root, new HashSet<string>
                    {
                        Path.TrimEndingDirectorySeparator(sourceFull)
                    });
                }
                else
                {
                    File.Copy(sourceFull, destination);
                }
            }
        }

        public static void Move(List<string> sources, string? targetDir)
        {
            var targetFull = ResolvePath(targetDir);
            EnsureDirectoryExists(targetFull, "目标目录不存在");
            var root = GetRootPath();

            foreach (var source in sources)
            {
                var sourceFull = ResolvePath(source);
                EnsureNotRoot(sourceFull, "不能移动根目录");
                if (!File.Exists(sourceFull) && !Directory.Exists(sourceFull))
                {
                    throw new FileManagerException(404, $"源路径不存在：{source}");
                }

                var name = Path.GetFileName(sourceFull);
                var destination = Path.Combine(targetFull, name);
                if (File.Exists(destination) || Directory.Exists(destination))
                {
                    throw new FileManagerException(409, $"目标已存在：{ToRelativePath(destination, root)}");
                }

                if (Directory.Exists(sourceFull))
                {
                    if (IsWithinRoot(targetFull, sourceFull))
                    {
                        throw new FileManagerException(400, "不能将文件夹移动到其自身内部");
                    }
                    Directory.Move(sourceFull, destination);
                }
                else
                {
                    File.Move(sourceFull, destination);
                }
            }
        }

        public static void Delete(List<string> paths)
        {
            foreach (var path in paths)
            {
                var full = ResolvePath(path);
                EnsureNotRoot(full, "不能删除根目录");
                if (File.Exists(full))
                {
                    FileSystem.DeleteFile(full, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin, UICancelOption.ThrowException);
                }
                else if (Directory.Exists(full))
                {
                    FileSystem.DeleteDirectory(full, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin, UICancelOption.ThrowException);
                }
                else
                {
                    throw new FileManagerException(404, $"文件或文件夹不存在：{path}");
                }
            }
        }

        public static ReadTextResult ReadText(string? path, string? encodingName = null)
        {
            var full = ResolvePath(path);
            if (!File.Exists(full))
            {
                throw new FileManagerException(404, "文件不存在");
            }
            var info = new FileInfo(full);
            if (info.Length > MaxTextFileSize)
            {
                throw new FileManagerException(400, $"文件超过 {MaxTextFileSize / 1024 / 1024}MB，无法编辑");
            }

            var bytes = File.ReadAllBytes(full);
            var (encoding, hadBom, content) = DecodeTextBytes(bytes, ResolveExplicitEncoding(encodingName));

            return new ReadTextResult
            {
                Path = ToRelativePath(full),
                Content = content,
                Encoding = ResolveEncodingLabel(encodingName) ?? EncodingName(encoding, hadBom)
            };
        }

        public static void WriteText(string? path, string content, string? encodingName = null)
        {
            var full = ResolvePath(path);
            if (!File.Exists(full))
            {
                throw new FileManagerException(404, "文件不存在");
            }
            var info = new FileInfo(full);
            if (info.Length > MaxTextFileSize)
            {
                throw new FileManagerException(400, $"文件超过 {MaxTextFileSize / 1024 / 1024}MB，无法编辑");
            }

            var existing = File.ReadAllBytes(full);
            if (!string.IsNullOrWhiteSpace(encodingName))
            {
                var requestedEncoding = ResolveExplicitEncoding(encodingName);
                if (requestedEncoding == null)
                {
                    // 未知编码名回退为自动探测写回
                    var (detectedEncoding, detectedBom, _) = DecodeTextBytes(existing);
                    if (detectedEncoding.GetByteCount(content) > MaxTextFileSize)
                    {
                        throw new FileManagerException(400, $"文件内容超过 {MaxTextFileSize / 1024 / 1024}MB，无法保存");
                    }
                    var fallbackPreamble = detectedBom ? detectedEncoding.GetPreamble() : Array.Empty<byte>();
                    var fallbackBody = detectedEncoding.GetBytes(content);
                    var fallbackOutput = new byte[fallbackPreamble.Length + fallbackBody.Length];
                    Buffer.BlockCopy(fallbackPreamble, 0, fallbackOutput, 0, fallbackPreamble.Length);
                    Buffer.BlockCopy(fallbackBody, 0, fallbackOutput, fallbackPreamble.Length, fallbackBody.Length);
                    File.WriteAllBytes(full, fallbackOutput);
                    return;
                }

                // 写入使用替换回退：目标编码表示不了的字符以替换符保存，不报错
                var targetEncoding = requestedEncoding;
                var encodedBody = targetEncoding.GetBytes(content);
                if (encodedBody.Length > MaxTextFileSize)
                {
                    throw new FileManagerException(400, $"文件内容超过 {MaxTextFileSize / 1024 / 1024}MB，无法保存");
                }
                var preambleLength = GetBomOffset(existing, targetEncoding);
                var encodedPreamble = preambleLength > 0 ? targetEncoding.GetPreamble() : Array.Empty<byte>();
                var encodedOutput = new byte[encodedPreamble.Length + encodedBody.Length];
                Buffer.BlockCopy(encodedPreamble, 0, encodedOutput, 0, encodedPreamble.Length);
                Buffer.BlockCopy(encodedBody, 0, encodedOutput, encodedPreamble.Length, encodedBody.Length);
                File.WriteAllBytes(full, encodedOutput);
                return;
            }

            var (encoding, hadBom, _) = DecodeTextBytes(existing);
            if (encoding.GetByteCount(content) > MaxTextFileSize)
            {
                throw new FileManagerException(400, $"文件内容超过 {MaxTextFileSize / 1024 / 1024}MB，无法保存");
            }

            var preamble = hadBom ? encoding.GetPreamble() : Array.Empty<byte>();
            var body = encoding.GetBytes(content);
            var output = new byte[preamble.Length + body.Length];
            Buffer.BlockCopy(preamble, 0, output, 0, preamble.Length);
            Buffer.BlockCopy(body, 0, output, preamble.Length, body.Length);
            File.WriteAllBytes(full, output);
        }

        /// <summary>
        /// 获取文件下载信息
        /// </summary>
        public static (string FullPath, string ContentType, string FileName) GetDownloadInfo(string? path)
        {
            var full = ResolvePath(path);
            if (!File.Exists(full))
            {
                throw new FileManagerException(404, "文件不存在");
            }
            if (!ContentTypeProvider.TryGetContentType(full, out var contentType))
            {
                contentType = "application/octet-stream";
            }
            return (full, contentType, Path.GetFileName(full));
        }

        /// <summary>
        /// 获取图片预览信息：仅允许图片类型文件
        /// </summary>
        public static (string FullPath, string ContentType) GetImageInfo(string? path)
        {
            var full = ResolvePath(path);
            if (!File.Exists(full))
            {
                throw new FileManagerException(404, "文件不存在");
            }
            if (!ContentTypeProvider.TryGetContentType(full, out var contentType)
                || !contentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
            {
                throw new FileManagerException(400, "仅支持图片文件");
            }
            return (full, contentType);
        }

        /// <summary>
        /// 判断路径是否为目录
        /// </summary>
        public static bool IsDirectory(string? path)
        {
            return Directory.Exists(ResolvePath(path));
        }

        /// <summary>
        /// 探测文件/文件夹大小：文件返回自身字节数，文件夹递归统计所有文件；
        /// 指向根目录外的链接、循环链接与无法读取的文件不统计
        /// </summary>
        public static long GetSize(string? path)
        {
            var full = ResolvePath(path);
            if (File.Exists(full))
            {
                try
                {
                    return new FileInfo(full).Length;
                }
                catch (IOException)
                {
                    return 0;
                }
                catch (UnauthorizedAccessException)
                {
                    return 0;
                }
            }
            if (!Directory.Exists(full))
            {
                throw new FileManagerException(404, "文件或文件夹不存在");
            }

            var root = GetRootPath();
            var visitedDirectories = new HashSet<string>
            {
                Path.TrimEndingDirectorySeparator(full)
            };
            return GetDirectorySize(full, root, visitedDirectories);
        }

        /// <summary>
        /// 按文件名递归搜索：支持 * 和 ? 通配符，无通配符时按包含匹配（忽略大小写）；
        /// 指向根目录外的链接与循环链接不搜索；命中数超过 limit 时仅返回前 limit 条
        /// </summary>
        public static FileSearchResult Search(string? path, string pattern, int limit)
        {
            if (string.IsNullOrWhiteSpace(pattern))
            {
                throw new FileManagerException(400, "搜索关键字不能为空");
            }
            var full = ResolvePath(path);
            if (!Directory.Exists(full))
            {
                throw new FileManagerException(404, "文件或文件夹不存在");
            }

            var matcher = BuildNameMatcher(pattern);
            var root = GetRootPath();
            var visitedDirectories = new HashSet<string>
            {
                Path.TrimEndingDirectorySeparator(full)
            };
            var items = new List<FileEntryDto>();
            long total = 0;
            SearchDirectory(full, root, matcher, limit, items, ref total, visitedDirectories);
            return new FileSearchResult
            {
                Items = items,
                Total = total
            };
        }

        private static long GetDirectorySize(string directoryPath, string root, HashSet<string> visitedDirectories)
        {
            long total = 0;
            foreach (var info in new DirectoryInfo(directoryPath).EnumerateFileSystemInfos())
            {
                if (info is DirectoryInfo)
                {
                    var resolvedPath = info.FullName;
                    if ((info.Attributes & FileAttributes.ReparsePoint) != 0)
                    {
                        var resolved = TryResolveLinkTarget(info);
                        if (resolved == null || !IsWithinRoot(resolved.FullName, root))
                        {
                            // 指向根目录外的链接或损坏链接：不统计
                            continue;
                        }
                        resolvedPath = resolved.FullName;
                    }
                    var key = Path.TrimEndingDirectorySeparator(resolvedPath);
                    if (!visitedDirectories.Add(key))
                    {
                        // 循环链接：不重复统计
                        continue;
                    }
                    total += GetDirectorySize(info.FullName, root, visitedDirectories);
                    visitedDirectories.Remove(key);
                }
                else
                {
                    if ((info.Attributes & FileAttributes.ReparsePoint) != 0)
                    {
                        var resolved = TryResolveLinkTarget(info);
                        if (resolved == null || !IsWithinRoot(resolved.FullName, root))
                        {
                            continue;
                        }
                    }
                    try
                    {
                        total += new FileInfo(info.FullName).Length;
                    }
                    catch (IOException)
                    {
                        // 单个文件无法读取时跳过，不中断整个探测
                    }
                    catch (UnauthorizedAccessException)
                    {
                        // 同上
                    }
                }
            }
            return total;
        }

        private static FileSystemInfo? TryResolveLinkTarget(FileSystemInfo info)
        {
            try
            {
                return info.ResolveLinkTarget(true);
            }
            catch (IOException)
            {
                return null;
            }
        }

        private static void SearchDirectory(
            string directoryPath,
            string root,
            Func<string, bool> matcher,
            int limit,
            List<FileEntryDto> items,
            ref long total,
            HashSet<string> visitedDirectories)
        {
            foreach (var info in new DirectoryInfo(directoryPath).EnumerateFileSystemInfos())
            {
                if (info is DirectoryInfo)
                {
                    var resolvedPath = info.FullName;
                    if ((info.Attributes & FileAttributes.ReparsePoint) != 0)
                    {
                        var resolved = TryResolveLinkTarget(info);
                        if (resolved == null || !IsWithinRoot(resolved.FullName, root))
                        {
                            continue;
                        }
                        resolvedPath = resolved.FullName;
                    }
                    var key = Path.TrimEndingDirectorySeparator(resolvedPath);
                    if (!visitedDirectories.Add(key))
                    {
                        continue;
                    }
                    if (matcher(info.Name))
                    {
                        total++;
                        if (items.Count < limit)
                        {
                            items.Add(ToFileEntry(info, root));
                        }
                    }
                    SearchDirectory(info.FullName, root, matcher, limit, items, ref total, visitedDirectories);
                    visitedDirectories.Remove(key);
                }
                else
                {
                    if ((info.Attributes & FileAttributes.ReparsePoint) != 0)
                    {
                        var resolved = TryResolveLinkTarget(info);
                        if (resolved == null || !IsWithinRoot(resolved.FullName, root))
                        {
                            continue;
                        }
                    }
                    if (matcher(info.Name))
                    {
                        total++;
                        if (items.Count < limit)
                        {
                            items.Add(ToFileEntry(info, root));
                        }
                    }
                }
            }
        }

        private static Func<string, bool> BuildNameMatcher(string pattern)
        {
            if (pattern.IndexOfAny(new[] { '*', '?' }) < 0)
            {
                return name => name.Contains(pattern, StringComparison.OrdinalIgnoreCase);
            }
            var regexPattern = "^" + Regex.Escape(pattern).Replace("\\*", ".*").Replace("\\?", ".") + "$";
            var regex = new Regex(regexPattern, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
            return name => regex.IsMatch(name);
        }

        private static FileEntryDto ToFileEntry(FileSystemInfo info, string root)
        {
            long? size = null;
            if (info is FileInfo fileInfo)
            {
                try
                {
                    size = fileInfo.Length;
                }
                catch
                {
                    // 权限不足时无法读取大小，置空即可
                }
            }
            return new FileEntryDto
            {
                Name = info.Name,
                Path = ToRelativePath(info.FullName, root),
                IsDirectory = info is DirectoryInfo,
                Size = size,
                LastWriteTime = info.LastWriteTime
            };
        }

        public sealed record ZipSource(string FullPath, string EntryName, bool IsDirectory);

        /// <summary>
        /// 准备 ZIP 流式下载：支持单个/多个文件与文件夹，先完整预检（链接、循环、可读性），确保错误在响应开始前暴露
        /// </summary>
        public static (List<ZipSource> Sources, string FileName) PrepareZipSources(string[] paths)
        {
            if (paths.Length == 0)
            {
                throw new FileManagerException(400, "请指定要下载的路径");
            }

            var root = GetRootPath();
            var sources = new List<ZipSource>(paths.Length);
            var entryNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var path in paths)
            {
                var full = ResolvePath(path);
                string entryName;
                if (File.Exists(full))
                {
                    ValidateZipFile(full, root);
                    entryName = Path.GetFileName(full);
                    sources.Add(new ZipSource(full, entryName, false));
                }
                else if (Directory.Exists(full))
                {
                    var visitedDirectories = new HashSet<string>
                    {
                        Path.TrimEndingDirectorySeparator(full)
                    };
                    ValidateZipTree(full, root, visitedDirectories);
                    entryName = Path.GetFileName(Path.TrimEndingDirectorySeparator(full));
                    if (string.IsNullOrEmpty(entryName))
                    {
                        entryName = "download";
                    }
                    sources.Add(new ZipSource(full, entryName, true));
                }
                else
                {
                    throw new FileManagerException(404, $"文件或文件夹不存在：{path}");
                }

                if (!entryNames.Add(entryName))
                {
                    throw new FileManagerException(400, $"选择的条目中存在同名文件或文件夹：{entryName}");
                }
            }

            var fileName = sources.Count == 1 && sources[0].IsDirectory
                ? sources[0].EntryName + ".zip"
                : "download.zip";
            return (sources, fileName);
        }

        /// <summary>
        /// 将多个文件/文件夹直接打包写入指定流（非 seekable 流也可），不产生临时文件
        /// </summary>
        public static async Task WriteZipAsync(Stream output, IReadOnlyList<ZipSource> sources)
        {
            var root = GetRootPath();
            await Task.Run(() =>
            {
                using var archive = new ZipArchive(output, ZipArchiveMode.Create, leaveOpen: true);
                foreach (var source in sources)
                {
                    if (source.IsDirectory)
                    {
                        var visitedDirectories = new HashSet<string>
                        {
                            Path.TrimEndingDirectorySeparator(source.FullPath)
                        };
                        AddDirectoryToZip(archive, source.FullPath, source.EntryName, root, visitedDirectories);
                    }
                    else
                    {
                        AddFileToZip(archive, source.FullPath, source.EntryName, root);
                    }
                }
            });
        }

        /// <summary>
        /// 获取文本文件的流式读取信息：完整路径与带 charset 的 text/plain 内容类型
        /// </summary>
        public static (string FullPath, string ContentType) GetRawTextInfo(string? path, string? encodingName = null)
        {
            var full = ResolvePath(path);
            if (!File.Exists(full))
            {
                throw new FileManagerException(404, "文件不存在");
            }
            var info = new FileInfo(full);
            if (info.Length > MaxTextFileSize)
            {
                throw new FileManagerException(400, $"文件超过 {MaxTextFileSize / 1024 / 1024}MB，无法编辑");
            }

            // 正文由控制器原样流式返回，这里只读文件头推断 charset 并做二进制检查
            var head = ReadHeadBytes(full);
            var encoding = ResolveExplicitEncoding(encodingName);
            if (encoding == null)
            {
                try
                {
                    encoding = DetectEncoding(head).Encoding;
                }
                catch (DecoderFallbackException)
                {
                    // 全部尝试失败时按 GBK 兜底，乱码也是一种解
                    encoding = Encoding.GetEncoding(936);
                }
            }
            var offset = GetBomOffset(head, encoding);
            var headText = encoding.GetString(head, offset, head.Length - offset);
            if (IsBinaryContent(headText))
            {
                throw new BinaryFileException("不支持的文件格式");
            }
            var charset = GetCharsetName(encoding);
            return (full, $"text/plain; charset={charset}");
        }

        public static string ResolveUploadTarget(string? targetDir, string fileName)
        {
            var targetFull = ResolvePath(targetDir);
            EnsureDirectoryExists(targetFull, "目标目录不存在");
            var normalizedName = NormalizeFileName(fileName);
            return Path.Combine(targetFull, normalizedName);
        }

        public static List<SqliteTableInfo> SqliteTables(string? path)
        {
            var full = ResolveSqliteFile(path);
            using var db = CreateClient(full, true);
            var table = db.Ado.GetDataTable(
                "SELECT name, type, sql FROM sqlite_master WHERE type IN ('table','view') AND name NOT LIKE 'sqlite_%' ORDER BY name");
            var result = new List<SqliteTableInfo>();
            foreach (DataRow row in table.Rows)
            {
                result.Add(new SqliteTableInfo
                {
                    Name = row["name"]?.ToString() ?? "",
                    Type = row["type"]?.ToString() ?? "",
                    Sql = row["sql"]?.ToString() ?? ""
                });
            }
            return result;
        }

        public static SqliteSchemaResult SqliteSchema(string? path, string table)
        {
            var full = ResolveSqliteFile(path);
            using var db = CreateClient(full, true);
            EnsureTableExists(db, table);
            var quoted = QuoteIdentifier(table);

            var columns = new List<SqliteColumnInfo>();
            var columnTable = db.Ado.GetDataTable($"PRAGMA table_info({quoted})");
            foreach (DataRow row in columnTable.Rows)
            {
                columns.Add(new SqliteColumnInfo
                {
                    Name = row["name"]?.ToString() ?? "",
                    DataType = row["type"]?.ToString() ?? "",
                    NotNull = Convert.ToInt32(row["notnull"]) != 0,
                    DefaultValue = row["dflt_value"] is DBNull ? null : row["dflt_value"]?.ToString(),
                    PrimaryKey = Convert.ToInt32(row["pk"])
                });
            }

            var indexes = new List<SqliteIndexInfo>();
            var indexTable = db.Ado.GetDataTable($"PRAGMA index_list({quoted})");
            foreach (DataRow row in indexTable.Rows)
            {
                var indexName = row["name"]?.ToString() ?? "";
                var indexColumns = new List<string>();
                var infoTable = db.Ado.GetDataTable($"PRAGMA index_info({QuoteIdentifier(indexName)})");
                foreach (DataRow infoRow in infoTable.Rows)
                {
                    indexColumns.Add(infoRow["name"]?.ToString() ?? "");
                }
                indexes.Add(new SqliteIndexInfo
                {
                    Name = indexName,
                    Unique = Convert.ToInt32(row["unique"]) != 0,
                    Columns = indexColumns
                });
            }

            return new SqliteSchemaResult
            {
                Table = table,
                Columns = columns,
                Indexes = indexes
            };
        }

        public static SqliteDataResult SqliteData(string? path, string table, int page, int pageSize)
        {
            var full = ResolveSqliteFile(path);
            using var db = CreateClient(full, true);
            EnsureTableExists(db, table);

            page = Math.Max(1, page);
            pageSize = Math.Clamp(pageSize, 1, MaxPreviewPageSize);
            var quoted = QuoteIdentifier(table);
            var total = Convert.ToInt64(db.Ado.GetDataTable($"SELECT COUNT(1) AS C FROM {quoted}").Rows[0]["C"]);
            var data = db.Ado.GetDataTable(
                $"SELECT * FROM {quoted} LIMIT @limit OFFSET @offset",
                new SugarParameter("@limit", pageSize),
                new SugarParameter("@offset", (page - 1) * pageSize));

            return new SqliteDataResult
            {
                Columns = ColumnNames(data),
                Rows = Rows(data),
                Total = total,
                Page = page,
                PageSize = pageSize
            };
        }

        public static SqliteQueryResult SqliteQuery(string? path, string sql)
        {
            if (string.IsNullOrWhiteSpace(sql))
            {
                throw new FileManagerException(400, "SQL 不能为空");
            }
            var full = ResolveSqliteFile(path);

            // 安全守卫：禁止通过 SQL 访问根目录之外的文件
            var stripped = StripSqlComments(sql);
            var keyword = FirstKeyword(stripped);
            if (keyword is "ATTACH" or "DETACH")
            {
                throw new FileManagerException(400, "出于安全考虑，不允许执行 ATTACH/DETACH 语句");
            }
            if (keyword == "VACUUM" && HasWord(stripped, "INTO"))
            {
                throw new FileManagerException(400, "出于安全考虑，不允许执行 VACUUM INTO 语句");
            }

            using var db = CreateClient(full, false);

            try
            {
                if (IsQueryStatement(stripped))
                {
                    var data = db.Ado.GetDataTable(sql);
                    var rows = Rows(data);
                    var truncated = rows.Count > MaxQueryRows;
                    if (truncated)
                    {
                        rows = rows.Take(MaxQueryRows).ToList();
                    }
                    return new SqliteQueryResult
                    {
                        Type = "query",
                        Columns = ColumnNames(data),
                        Rows = rows,
                        Truncated = truncated
                    };
                }

                var affected = db.Ado.ExecuteCommand(sql);
                return new SqliteQueryResult
                {
                    Type = "execute",
                    AffectedRows = affected
                };
            }
            catch (SqliteException e) when (IsSyntaxError(e))
            {
                throw new SqlSyntaxException(e.Message);
            }
            catch (SqliteException e)
            {
                throw new FileManagerException(400, $"SQL 执行失败：{e.Message}");
            }
        }

        private static string NormalizeRelativePath(string? path)
        {
            if (string.IsNullOrWhiteSpace(path) || path.Trim() == ".")
            {
                return "";
            }

            var normalized = path.Trim().Replace('\\', '/');
            if (normalized.Length >= 2 && normalized[1] == ':')
            {
                throw new FileManagerException(400, "路径不能包含盘符");
            }
            if (normalized.StartsWith('/'))
            {
                throw new FileManagerException(400, "路径不能是绝对路径");
            }

            var segments = new List<string>();
            foreach (var segment in normalized.Split('/', StringSplitOptions.RemoveEmptyEntries))
            {
                // Win32 打开文件时会剥离段尾的空格和点，这里先做同样处理再校验，避免 ".. " 之类变体绕过
                var cleaned = segment.TrimEnd(' ', '.');
                if (cleaned.Length == 0)
                {
                    throw new FileManagerException(400, "路径段无效");
                }
                if (cleaned == ".")
                {
                    continue;
                }
                if (cleaned == "..")
                {
                    throw new FileManagerException(400, "路径不能包含上级目录引用 (..)");
                }
                ValidatePathSegment(cleaned);
                segments.Add(cleaned);
            }
            return string.Join(Path.DirectorySeparatorChar, segments);
        }

        private static void ValidatePathSegment(string segment)
        {
            if (segment.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
            {
                throw new FileManagerException(400, "路径包含非法字符");
            }
            var baseName = segment;
            var dotIndex = segment.IndexOf('.');
            if (dotIndex > 0)
            {
                baseName = segment[..dotIndex];
            }
            if (ReservedFileNames.Contains(baseName))
            {
                throw new FileManagerException(400, $"路径包含系统保留名称：{segment}");
            }
        }

        private static void EnsureWithinRoot(string fullPath, string root)
        {
            if (!IsWithinRoot(fullPath, root))
            {
                throw new FileManagerException(403, "路径超出文件管理器根目录");
            }
        }

        private static bool IsWithinRoot(string path, string root)
        {
            var full = Path.TrimEndingDirectorySeparator(path);
            var rootPath = Path.TrimEndingDirectorySeparator(root);
            return full.Equals(rootPath, StringComparison.OrdinalIgnoreCase)
                || full.StartsWith(rootPath + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase);
        }

        private static void EnsureNoEscapingLink(string fullPath, string root)
        {
            // 根目录本身可能是管理员有意配置的联接/符号链接，不做越界拦截
            if (!fullPath.Equals(root, StringComparison.OrdinalIgnoreCase))
            {
                CheckReparsePoint(fullPath, root);
            }
            var current = Path.GetDirectoryName(fullPath);
            while (!string.IsNullOrEmpty(current))
            {
                if (!IsWithinRoot(current, root))
                {
                    break;
                }
                if (current.Equals(root, StringComparison.OrdinalIgnoreCase))
                {
                    break;
                }
                CheckReparsePoint(current, root);
                current = Path.GetDirectoryName(current);
            }
        }

        private static void CheckReparsePoint(string path, string root)
        {
            FileSystemInfo info = File.Exists(path) ? new FileInfo(path) : new DirectoryInfo(path);
            if (!info.Exists)
            {
                return;
            }
            if ((info.Attributes & FileAttributes.ReparsePoint) == 0)
            {
                return;
            }
            try
            {
                var target = info.ResolveLinkTarget(true);
                if (target != null && !IsWithinRoot(target.FullName, root))
                {
                    throw new FileManagerException(403, "不允许访问指向根目录之外的链接");
                }
            }
            catch (FileManagerException)
            {
                throw;
            }
            catch (IOException)
            {
                throw new FileManagerException(403, "无法解析指向根目录之外的链接");
            }
        }

        private static void EnsureNotRoot(string fullPath, string message)
        {
            if (fullPath.Equals(GetRootPath(), StringComparison.OrdinalIgnoreCase))
            {
                throw new FileManagerException(400, message);
            }
        }

        private static void EnsureDirectoryExists(string fullPath, string message)
        {
            if (!Directory.Exists(fullPath))
            {
                throw new FileManagerException(404, message);
            }
        }

        private static string NormalizeFileName(string name)
        {
            var cleaned = name.TrimEnd(' ', '.');
            if (string.IsNullOrEmpty(cleaned) || cleaned is "." or "..")
            {
                throw new FileManagerException(400, "无效的文件名");
            }
            if (cleaned.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
            {
                throw new FileManagerException(400, "文件名包含非法字符");
            }
            var baseName = cleaned;
            var dotIndex = cleaned.IndexOf('.');
            if (dotIndex > 0)
            {
                baseName = cleaned[..dotIndex];
            }
            if (ReservedFileNames.Contains(baseName))
            {
                throw new FileManagerException(400, "文件名是系统保留名称");
            }
            return cleaned;
        }

        private static void CopyDirectory(string source, string destination, string root, HashSet<string> visitedDirectories)
        {
            Directory.CreateDirectory(destination);
            foreach (var info in new DirectoryInfo(source).EnumerateFileSystemInfos())
            {
                var target = Path.Combine(destination, info.Name);
                if (info is DirectoryInfo)
                {
                    var directoryPath = info.FullName;
                    if ((info.Attributes & FileAttributes.ReparsePoint) != 0)
                    {
                        directoryPath = ResolveLinkTargetWithinRoot(info, root);
                    }
                    var key = Path.TrimEndingDirectorySeparator(directoryPath);
                    if (!visitedDirectories.Add(key))
                    {
                        throw new FileManagerException(400, "检测到循环链接，无法复制");
                    }
                    CopyDirectory(info.FullName, target, root, visitedDirectories);
                    visitedDirectories.Remove(key);
                }
                else
                {
                    if ((info.Attributes & FileAttributes.ReparsePoint) != 0)
                    {
                        _ = ResolveLinkTargetWithinRoot(info, root);
                    }
                    File.Copy(info.FullName, target);
                }
            }
        }

        private static string ResolveLinkTargetWithinRoot(FileSystemInfo info, string root)
        {
            FileSystemInfo? resolved;
            try
            {
                resolved = info.ResolveLinkTarget(true);
            }
            catch (IOException)
            {
                throw new FileManagerException(403, "无法解析链接目标");
            }
            if (resolved == null || !IsWithinRoot(resolved.FullName, root))
            {
                throw new FileManagerException(403, "不允许访问指向根目录之外的链接");
            }
            return resolved.FullName;
        }

        /// <summary>
        /// 打包前预检：检查链接、循环链接与文件可读性，确保错误在响应开始前暴露
        /// </summary>
        private static void ValidateZipTree(string directoryPath, string root, HashSet<string> visitedDirectories)
        {
            foreach (var info in new DirectoryInfo(directoryPath).EnumerateFileSystemInfos())
            {
                if (info is DirectoryInfo)
                {
                    var resolvedPath = info.FullName;
                    if ((info.Attributes & FileAttributes.ReparsePoint) != 0)
                    {
                        resolvedPath = ResolveLinkTargetWithinRoot(info, root);
                    }
                    var key = Path.TrimEndingDirectorySeparator(resolvedPath);
                    if (!visitedDirectories.Add(key))
                    {
                        throw new FileManagerException(400, "检测到循环链接，无法打包");
                    }
                    ValidateZipTree(info.FullName, root, visitedDirectories);
                    visitedDirectories.Remove(key);
                }
                else
                {
                    if ((info.Attributes & FileAttributes.ReparsePoint) != 0)
                    {
                        _ = ResolveLinkTargetWithinRoot(info, root);
                    }
                    // 预检可读性：打开后立即关闭，让占用问题在响应开始前返回 400
                    using var probe = new FileStream(info.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                }
            }
        }

        private static void ValidateZipFile(string fullPath, string root)
        {
            var info = new FileInfo(fullPath);
            if ((info.Attributes & FileAttributes.ReparsePoint) != 0)
            {
                _ = ResolveLinkTargetWithinRoot(info, root);
            }
            // 预检可读性：打开后立即关闭，让占用问题在响应开始前返回 400
            using var probe = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        }

        private static void AddDirectoryToZip(ZipArchive archive, string sourceDir, string entryPrefix, string root, HashSet<string> visitedDirectories)
        {
            foreach (var info in new DirectoryInfo(sourceDir).EnumerateFileSystemInfos())
            {
                var entryName = entryPrefix.Length == 0 ? info.Name : entryPrefix + "/" + info.Name;
                if (info is DirectoryInfo)
                {
                    var directoryPath = info.FullName;
                    if ((info.Attributes & FileAttributes.ReparsePoint) != 0)
                    {
                        directoryPath = ResolveLinkTargetWithinRoot(info, root);
                    }
                    var key = Path.TrimEndingDirectorySeparator(directoryPath);
                    if (!visitedDirectories.Add(key))
                    {
                        throw new FileManagerException(400, "检测到循环链接，无法打包");
                    }
                    _ = archive.CreateEntry(entryName + "/");
                    AddDirectoryToZip(archive, info.FullName, entryName, root, visitedDirectories);
                    visitedDirectories.Remove(key);
                }
                else
                {
                    AddFileToZip(archive, info.FullName, entryName, root);
                }
            }
        }

        private static void AddFileToZip(ZipArchive archive, string fullPath, string entryName, string root)
        {
            var info = new FileInfo(fullPath);
            if ((info.Attributes & FileAttributes.ReparsePoint) != 0)
            {
                _ = ResolveLinkTargetWithinRoot(info, root);
            }
            var entry = archive.CreateEntry(entryName, CompressionLevel.Fastest);
            using var entryStream = entry.Open();
            using var fileStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            fileStream.CopyTo(entryStream);
        }

        private static (Encoding Encoding, bool HadBom) DetectEncoding(byte[] bytes)
        {
            if (bytes.Length >= 3 && bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF)
            {
                return (new UTF8Encoding(true), true);
            }
            if (bytes.Length >= 2 && bytes[0] == 0xFF && bytes[1] == 0xFE)
            {
                return (Encoding.Unicode, true);
            }
            if (bytes.Length >= 2 && bytes[0] == 0xFE && bytes[1] == 0xFF)
            {
                return (Encoding.BigEndianUnicode, true);
            }

            // 无 BOM：依次尝试严格解码，UTF-8 → GBK → GB18030 → ANSI（系统代码页）
            try
            {
                _ = new UTF8Encoding(false, true).GetString(bytes);
                return (new UTF8Encoding(false), false);
            }
            catch (DecoderFallbackException)
            {
                // 继续尝试下一种编码
            }

            try
            {
                var gbkStrict = Encoding.GetEncoding(936, EncoderFallback.ExceptionFallback, DecoderFallback.ExceptionFallback);
                _ = gbkStrict.GetString(bytes);
                return (Encoding.GetEncoding(936), false);
            }
            catch (DecoderFallbackException)
            {
                // 继续尝试下一种编码
            }

            try
            {
                var gb18030Strict = Encoding.GetEncoding(54936, EncoderFallback.ExceptionFallback, DecoderFallback.ExceptionFallback);
                _ = gb18030Strict.GetString(bytes);
                return (Encoding.GetEncoding(54936), false);
            }
            catch (DecoderFallbackException)
            {
                // 继续尝试下一种编码
            }

            var ansiCodePage = CultureInfo.CurrentCulture.TextInfo.ANSICodePage;
            if (ansiCodePage is not (65001 or 936 or 54936))
            {
                try
                {
                    var ansiStrict = Encoding.GetEncoding(ansiCodePage, EncoderFallback.ExceptionFallback, DecoderFallback.ExceptionFallback);
                    _ = ansiStrict.GetString(bytes);
                    return (Encoding.GetEncoding(ansiCodePage), false);
                }
                catch (DecoderFallbackException)
                {
                    // 全部尝试失败
                }
            }

            // 全部严格解码失败时按 GBK 替换解码兜底：乱码也是一种解，不报错
            return (Encoding.GetEncoding(936), false);
        }

        /// <summary>
        /// 解码文本字节：默认自动探测编码，也可指定编码严格解码；去除 BOM 并拒绝二进制内容
        /// </summary>
        private static (Encoding Encoding, bool HadBom, string Content) DecodeTextBytes(byte[] bytes, Encoding? explicitEncoding = null)
        {
            if (explicitEncoding != null)
            {
                return DecodeTextBytesWithEncoding(bytes, explicitEncoding);
            }

            return DecodeTextBytesWithDetection(bytes);
        }

        private static (Encoding Encoding, bool HadBom, string Content) DecodeTextBytesWithDetection(byte[] bytes)
        {
            var (encoding, hadBom) = DetectEncoding(bytes);
            var preambleLength = hadBom ? encoding.GetPreamble().Length : 0;
            var content = encoding.GetString(bytes, preambleLength, bytes.Length - preambleLength);
            if (IsBinaryContent(content))
            {
                throw new BinaryFileException("不支持的文件格式");
            }
            return (encoding, hadBom, content);
        }

        private static byte[] ReadHeadBytes(string fullPath, int maxBytes = 4096)
        {
            using var stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            var head = new byte[(int)Math.Min(maxBytes, stream.Length)];
            var read = 0;
            while (read < head.Length)
            {
                var count = stream.Read(head, read, head.Length - read);
                if (count <= 0)
                {
                    break;
                }
                read += count;
            }
            if (read < head.Length)
            {
                Array.Resize(ref head, read);
            }
            return head;
        }

        private static (Encoding Encoding, bool HadBom, string Content) DecodeTextBytesWithEncoding(byte[] bytes, Encoding encoding)
        {
            var offset = GetBomOffset(bytes, encoding);
            // 显式指定编码时宽容解码：无法表示的字节显示为替换符，不报错
            var content = encoding.GetString(bytes, offset, bytes.Length - offset);
            if (IsBinaryContent(content))
            {
                throw new BinaryFileException("不支持的文件格式");
            }
            return (encoding, offset > 0, content);
        }

        /// <summary>
        /// 解析前端传入的编码参数；返回 null 表示使用自动探测（未知编码名也回退自动探测）
        /// </summary>
        private static Encoding? ResolveExplicitEncoding(string? encodingName)
        {
            if (string.IsNullOrWhiteSpace(encodingName))
            {
                return null;
            }

            var name = encodingName.Trim().ToLowerInvariant();
            return name switch
            {
                "utf-8" or "utf8" => new UTF8Encoding(false),
                "gbk" or "gb2312" or "936" => Encoding.GetEncoding(936),
                "gb18030" or "54936" => Encoding.GetEncoding(54936),
                "ansi" => Encoding.GetEncoding(CultureInfo.CurrentCulture.TextInfo.ANSICodePage),
                "utf-16" or "utf-16le" or "1200" => Encoding.Unicode,
                "utf-16be" or "1201" => Encoding.BigEndianUnicode,
                _ => null
            };
        }

        /// <summary>
        /// 返回显式指定编码的展示名；未指定或未知时返回 null（自动探测）
        /// </summary>
        private static string? ResolveEncodingLabel(string? encodingName)
        {
            if (string.IsNullOrWhiteSpace(encodingName))
            {
                return null;
            }
            return encodingName.Trim().ToLowerInvariant() switch
            {
                "utf-8" or "utf8" => "UTF-8",
                "gbk" or "gb2312" or "936" => "GBK",
                "gb18030" or "54936" => "GB18030",
                "ansi" => "ANSI",
                "utf-16" or "utf-16le" or "1200" or "utf-16be" or "1201" => "UTF-16",
                _ => null
            };
        }

        /// <summary>
        /// 将用于展示的回退编码转为严格编码，解码/编码失败时抛异常而不是替换字符
        /// </summary>
        private static Encoding ToStrictEncoding(Encoding encoding)
        {
            Encoding Strict(int codePage) => Encoding.GetEncoding(
                codePage, EncoderFallback.ExceptionFallback, DecoderFallback.ExceptionFallback);

            return encoding.CodePage switch
            {
                65001 => new UTF8Encoding(false, true),
                1200 => Strict(1200),
                1201 => Strict(1201),
                _ => Strict(encoding.CodePage)
            };
        }

        private static int GetBomOffset(byte[] bytes, Encoding encoding)
        {
            if (bytes.Length >= 3 && bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF && encoding.CodePage == 65001)
            {
                return 3;
            }
            if (bytes.Length >= 2 && bytes[0] == 0xFF && bytes[1] == 0xFE && encoding.CodePage == 1200)
            {
                return 2;
            }
            if (bytes.Length >= 2 && bytes[0] == 0xFE && bytes[1] == 0xFF && encoding.CodePage == 1201)
            {
                return 2;
            }
            return 0;
        }

        private static string GetCharsetName(Encoding encoding)
        {
            return encoding.CodePage switch
            {
                65001 => "utf-8",
                1200 => "utf-16le",
                1201 => "utf-16be",
                936 => "gbk",
                54936 => "gb18030",
                _ => encoding.WebName
            };
        }

        private static string EncodingName(Encoding encoding, bool hadBom)
        {
            return encoding.CodePage switch
            {
                65001 => "UTF-8",
                936 => "GBK",
                54936 => "GB18030",
                1200 or 1201 => "UTF-16",
                _ => "ANSI"
            };
        }

        private static bool IsBinaryContent(string content)
        {
            if (content.Contains('\0'))
            {
                return true;
            }
            // 文本文件允许 \t \r \n，其余 C0 控制字符视为二进制内容
            foreach (var c in content)
            {
                if (c < 0x20 && c is not '\t' and not '\r' and not '\n')
                {
                    return true;
                }
            }
            return false;
        }

        private static string ResolveSqliteFile(string? path)
        {
            var full = ResolvePath(path);
            if (!File.Exists(full))
            {
                throw new FileManagerException(404, "文件不存在");
            }
            if (!SqliteExtensions.Contains(Path.GetExtension(full)))
            {
                throw new FileManagerException(400, "仅支持 .db/.sqlite/.sqlite3 文件");
            }
            return full;
        }

        private static SqlSugarClient CreateClient(string fullPath, bool readOnly)
        {
            var connectionString = readOnly
                ? $"Data Source={fullPath};Mode=ReadOnly"
                : $"Data Source={fullPath}";
            return new SqlSugarClient(new ConnectionConfig
            {
                ConnectionString = connectionString,
                DbType = SqlSugar.DbType.Sqlite,
                IsAutoCloseConnection = true,
                InitKeyType = InitKeyType.Attribute
            });
        }

        private static void EnsureTableExists(SqlSugarClient db, string table)
        {
            if (string.IsNullOrWhiteSpace(table))
            {
                throw new FileManagerException(400, "表名不能为空");
            }
            var exists = db.Ado.GetDataTable(
                "SELECT COUNT(1) AS C FROM sqlite_master WHERE type IN ('table','view') AND name = @name",
                new SugarParameter("@name", table));
            if (Convert.ToInt64(exists.Rows[0]["C"]) == 0)
            {
                throw new FileManagerException(404, $"表或视图不存在：{table}");
            }
        }

        private static string QuoteIdentifier(string name)
        {
            return "\"" + name.Replace("\"", "\"\"") + "\"";
        }

        private static bool IsQueryStatement(string sql)
        {
            return FirstKeyword(sql) is "SELECT" or "WITH" or "PRAGMA" or "EXPLAIN" or "VALUES";
        }

        private static string FirstKeyword(string sql)
        {
            var trimmed = sql.TrimStart();
            if (trimmed.StartsWith('('))
            {
                trimmed = trimmed[1..].TrimStart();
            }
            var end = trimmed.IndexOfAny(new[] { ' ', '\t', '\r', '\n' });
            return (end < 0 ? trimmed : trimmed[..end]).ToUpperInvariant();
        }

        private static bool IsSyntaxError(SqliteException e)
        {
            if (e.SqliteErrorCode != 1)
            {
                return false;
            }
            return e.Message.Contains("syntax error", StringComparison.OrdinalIgnoreCase)
                || e.Message.Contains("unrecognized token", StringComparison.OrdinalIgnoreCase)
                || e.Message.Contains("incomplete input", StringComparison.OrdinalIgnoreCase);
        }

        private static bool HasWord(string sql, string word)
        {
            return sql.Split(new[] { ' ', '\t', '\r', '\n', '(', ')', ';', ',', '\'', '"', '`' }, StringSplitOptions.RemoveEmptyEntries)
                .Any(token => token.Equals(word, StringComparison.OrdinalIgnoreCase));
        }

        private static string StripSqlComments(string sql)
        {
            var builder = new StringBuilder(sql.Length);
            var i = 0;
            while (i < sql.Length)
            {
                if (i + 1 < sql.Length && sql[i] == '-' && sql[i + 1] == '-')
                {
                    while (i < sql.Length && sql[i] != '\n')
                    {
                        i++;
                    }
                }
                else if (i + 1 < sql.Length && sql[i] == '/' && sql[i + 1] == '*')
                {
                    i += 2;
                    while (i + 1 < sql.Length && !(sql[i] == '*' && sql[i + 1] == '/'))
                    {
                        i++;
                    }
                    i = Math.Min(i + 2, sql.Length);
                }
                else
                {
                    builder.Append(sql[i]);
                    i++;
                }
            }
            return builder.ToString();
        }

        private static List<string> ColumnNames(DataTable table)
        {
            return table.Columns.Cast<DataColumn>().Select(c => c.ColumnName).ToList();
        }

        private static List<List<object?>> Rows(DataTable table)
        {
            var rows = new List<List<object?>>(table.Rows.Count);
            foreach (DataRow row in table.Rows)
            {
                var values = new List<object?>(row.ItemArray.Length);
                foreach (var value in row.ItemArray)
                {
                    values.Add(value is DBNull ? null : value);
                }
                rows.Add(values);
            }
            return rows;
        }
    }
}
