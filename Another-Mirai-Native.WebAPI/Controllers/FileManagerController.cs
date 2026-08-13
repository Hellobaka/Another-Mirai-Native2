using Another_Mirai_Native.WebAPI.Models;
using Another_Mirai_Native.WebAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

namespace Another_Mirai_Native.WebAPI.Controllers
{
    [ApiController]
    [Route("/api/files")]
    [Authorize]
    public class FileManagerController(ILogger<FileManagerController> logger) : ControllerBase
    {
        private readonly ILogger<FileManagerController> _logger = logger;

        private const long MaxUploadSize = 500L * 1024 * 1024;

        [HttpGet]
        [EndpointSummary("浏览目录")]
        [EndpointDescription("列出根目录或指定子目录下的文件与文件夹，路径相对文件管理器根目录")]
        [ProducesResponseType(typeof(ApiResponse<ListDirectoryResult>), StatusCodes.Status200OK)]
        public IActionResult List(
            [Description("相对根目录的路径，空表示根目录")][FromQuery] string path = "")
        {
            return Execute("List", () =>
            {
                var result = FileManagerService.ListDirectory(path);
                _logger.LogInformation("文件管理-浏览目录: Path={Path}, Items={Count}", path, result.Items.Count);
                return Ok(ApiResponse.Ok(result));
            });
        }

        [HttpPost("mkdir")]
        [EndpointSummary("新建文件夹")]
        [EndpointDescription("创建文件夹，父目录不存在时自动创建，同名返回 409")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public IActionResult CreateFolder([FromBody] FilePathRequest request)
        {
            return Execute("CreateFolder", () =>
            {
                FileManagerService.CreateFolder(request.Path);
                _logger.LogInformation("文件管理-新建文件夹: Path={Path}", request.Path);
                return Ok(ApiResponse.Ok());
            });
        }

        [HttpPost("mkfile")]
        [EndpointSummary("新建文件")]
        [EndpointDescription("创建空文件或带初始文本的文件，同名返回 409")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public IActionResult CreateFile([FromBody] CreateFileRequest request)
        {
            return Execute("CreateFile", () =>
            {
                FileManagerService.CreateFile(request.Path, request.Content);
                _logger.LogInformation("文件管理-新建文件: Path={Path}", request.Path);
                return Ok(ApiResponse.Ok());
            });
        }

        [HttpPost("rename")]
        [EndpointSummary("重命名")]
        [EndpointDescription("重命名文件或文件夹，仅支持同一目录内改名，同名返回 409")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public IActionResult Rename([FromBody] RenameRequest request)
        {
            return Execute("Rename", () =>
            {
                FileManagerService.Rename(request.Path, request.NewName);
                _logger.LogInformation("文件管理-重命名: Path={Path}, NewName={NewName}", request.Path, request.NewName);
                return Ok(ApiResponse.Ok());
            });
        }

        [HttpPost("copy")]
        [EndpointSummary("复制")]
        [EndpointDescription("将多个源文件/文件夹复制到目标目录，目标已存在返回 409")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public IActionResult Copy([FromBody] CopyMoveRequest request)
        {
            return Execute("Copy", () =>
            {
                FileManagerService.Copy(request.Sources, request.TargetDir);
                _logger.LogInformation("文件管理-复制: Sources={Sources}, Target={Target}", string.Join(",", request.Sources), request.TargetDir);
                return Ok(ApiResponse.Ok());
            });
        }

        [HttpPost("move")]
        [EndpointSummary("移动（剪切+粘贴）")]
        [EndpointDescription("将多个源文件/文件夹移动到目标目录，目标已存在返回 409")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public IActionResult Move([FromBody] CopyMoveRequest request)
        {
            return Execute("Move", () =>
            {
                FileManagerService.Move(request.Sources, request.TargetDir);
                _logger.LogInformation("文件管理-移动: Sources={Sources}, Target={Target}", string.Join(",", request.Sources), request.TargetDir);
                return Ok(ApiResponse.Ok());
            });
        }

        [HttpPost("delete")]
        [EndpointSummary("删除到回收站")]
        [EndpointDescription("批量删除文件/文件夹，删除后进入系统回收站，失败时不回退为永久删除")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public IActionResult Delete([FromBody] DeleteRequest request)
        {
            return Execute("Delete", () =>
            {
                FileManagerService.Delete(request.Paths);
                _logger.LogInformation("文件管理-删除: Paths={Paths}", string.Join(",", request.Paths));
                return Ok(ApiResponse.Ok());
            });
        }

        [HttpGet("text")]
        [EndpointSummary("读取文本文件")]
        [EndpointDescription("读取文件文本内容，默认自动识别编码，也可通过 encoding 参数指定；超过 10MB 或二进制文件拒绝")]
        [ProducesResponseType(typeof(ApiResponse<ReadTextResult>), StatusCodes.Status200OK)]
        public IActionResult ReadText(
            [Description("相对根目录的路径")][FromQuery] string path,
            [Description("指定编码：utf-8 / gbk / gb18030 / ansi / utf-16 / utf-16be，留空自动探测")][FromQuery] string? encoding = null)
        {
            return Execute("ReadText", () =>
            {
                var result = FileManagerService.ReadText(path, encoding);
                _logger.LogInformation("文件管理-读取文本: Path={Path}, Encoding={Encoding}", path, result.Encoding);
                return Ok(ApiResponse.Ok(result));
            });
        }

        [HttpGet("text/raw")]
        [EndpointSummary("流式读取文本文件")]
        [EndpointDescription("以 text/plain 流式返回文件原始内容，响应头携带正确 charset，浏览器可直接解码；可通过 encoding 参数指定编码；适合大文件")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult ReadTextRaw(
            [Description("相对根目录的路径")][FromQuery] string path,
            [Description("指定编码：utf-8 / gbk / gb18030 / ansi / utf-16 / utf-16be，留空自动探测")][FromQuery] string? encoding = null)
        {
            return Execute("ReadTextRaw", () =>
            {
                var (fullPath, contentType) = FileManagerService.GetRawTextInfo(path, encoding);
                _logger.LogInformation("文件管理-流式读取文本: Path={Path}, Encoding={Encoding}", path, encoding ?? "auto");
                Response.Headers.CacheControl = "no-store";
                return PhysicalFile(fullPath, contentType);
            });
        }

        [HttpPost("text")]
        [EndpointSummary("写入文本文件")]
        [EndpointDescription("写回文本内容，默认按原文件识别的编码，也可通过 encoding 字段指定；文件不存在返回 404，内容上限 10MB")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public IActionResult WriteText([FromBody] WriteTextRequest request)
        {
            return Execute("WriteText", () =>
            {
                FileManagerService.WriteText(request.Path, request.Content, request.Encoding);
                _logger.LogInformation("文件管理-写入文本: Path={Path}, Length={Length}, Encoding={Encoding}", request.Path, request.Content.Length, request.Encoding ?? "auto");
                return Ok(ApiResponse.Ok());
            });
        }

        [HttpGet("download")]
        [EndpointSummary("下载文件/文件夹（支持多选打包）")]
        [EndpointDescription("单文件直接下载；单文件夹或选择多个条目时流式打包为 ZIP 下载，不产生临时文件；多选通过重复 path 参数传入")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult Download(
            [Description("相对根目录的路径，多选时重复该参数")][FromQuery] string[] path)
        {
            return Execute("Download", () =>
            {
                if (path.Length == 1 && !FileManagerService.IsDirectory(path[0]))
                {
                    var (fullPath, contentType, fileName) = FileManagerService.GetDownloadInfo(path[0]);
                    _logger.LogInformation("文件管理-下载文件: Path={Path}", path[0]);
                    Response.Headers.CacheControl = "no-store";
                    return PhysicalFile(fullPath, contentType, fileName);
                }
                var (sources, zipName) = FileManagerService.PrepareZipSources(path);
                _logger.LogInformation("文件管理-下载打包: Paths={Paths}, ZipName={ZipName}", string.Join(",", path), zipName);
                return new ZipFileResult(sources, zipName, HttpContext.RequestAborted);
            });
        }

        [HttpGet("size")]
        [EndpointSummary("探测文件/文件夹大小")]
        [EndpointDescription("文件返回自身字节数；文件夹递归统计所有文件总字节数；指向根目录外的链接、循环链接与无法读取的文件不统计")]
        [ProducesResponseType(typeof(ApiResponse<FileSizeResult>), StatusCodes.Status200OK)]
        public Task<IActionResult> GetSize(
            [Description("相对根目录的路径，空表示根目录")][FromQuery] string path = "")
        {
            return ExecuteAsync("GetSize", async () =>
            {
                var size = await Task.Run(() => FileManagerService.GetSize(path, HttpContext.RequestAborted), HttpContext.RequestAborted);
                _logger.LogInformation("文件管理-探测大小: Path={Path}, Size={Size}", path, size);
                return Ok(ApiResponse.Ok(new FileSizeResult { Size = size }));
            });
        }

        [HttpGet("image")]
        [EndpointSummary("预览图片")]
        [EndpointDescription("以图片内容类型返回文件供浏览器直接显示（<img> 可用），仅支持图片文件；可通过 access_token 查询参数携带 JWT")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetImage(
            [Description("相对根目录的路径")][FromQuery] string path)
        {
            return Execute("GetImage", () =>
            {
                var (fullPath, contentType) = FileManagerService.GetImageInfo(path);
                _logger.LogInformation("文件管理-图片预览: Path={Path}, Type={Type}", path, contentType);
                // SVG 等图片可包含脚本，直接打开时禁止执行（同源 XSS 防护）；对 <img> 嵌入无影响
                Response.Headers["Content-Security-Policy"] = "sandbox; default-src 'none'";
                Response.Headers.CacheControl = "no-store";
                return PhysicalFile(fullPath, contentType);
            });
        }

        [HttpGet("image-token")]
        [EndpointSummary("获取图片预览令牌")]
        [EndpointDescription("校验图片文件存在后返回 5 分钟有效、仅限该路径的图片预览令牌；<img> 请求通过 access_token 参数携带，避免长期 JWT 暴露在 URL 中")]
        [ProducesResponseType(typeof(ApiResponse<ImageTokenResult>), StatusCodes.Status200OK)]
        public IActionResult GetImageToken(
            [Description("相对根目录的路径")][FromQuery] string path)
        {
            return Execute("GetImageToken", () =>
            {
                _ = FileManagerService.GetImageInfo(path);
                var token = AuthController.CreateFileImageToken(path);
                _logger.LogInformation("文件管理-获取图片令牌: Path={Path}", path);
                return Ok(ApiResponse.Ok(new ImageTokenResult
                {
                    Token = token,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(5)
                }));
            });
        }

        [HttpGet("search")]
        [EndpointSummary("按文件名搜索")]
        [EndpointDescription("在指定目录（默认根目录）递归按文件名匹配，支持 * 和 ? 通配符；无通配符时按包含匹配（忽略大小写）；命中数超过 limit 时仅返回前 limit 条")]
        [ProducesResponseType(typeof(ApiResponse<FileSearchResult>), StatusCodes.Status200OK)]
        public Task<IActionResult> Search(
            [Description("搜索关键字/通配符模式，如 *.txt、data?、core")][FromQuery] string pattern,
            [Description("搜索起始目录，相对根目录，空表示根目录")][FromQuery] string path = "",
            [Description("最多返回条数，默认 200，最大 1000")][FromQuery] int limit = 200)
        {
            return ExecuteAsync("Search", async () =>
            {
                limit = Math.Clamp(limit, 1, 1000);
                var result = await Task.Run(() => FileManagerService.Search(path, pattern, limit, HttpContext.RequestAborted), HttpContext.RequestAborted);
                _logger.LogInformation("文件管理-文件名搜索: Path={Path}, Pattern={Pattern}, Total={Total}", path, pattern, result.Total);
                return Ok(ApiResponse.Ok(result));
            });
        }

        [HttpPost("upload")]
        [RequestSizeLimit(MaxUploadSize)]
        [RequestFormLimits(MultipartBodyLengthLimit = MaxUploadSize)]
        [EndpointSummary("上传文件")]
        [EndpointDescription("将本地文件上传到目标目录，支持多文件，同名返回 409，单请求上限 500MB")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public Task<IActionResult> Upload(
            [Description("上传的文件，form-data 字段名 files")][FromForm] IFormFileCollection files,
            [Description("目标目录，相对根目录")][FromForm] string targetDir = "")
        {
            return ExecuteAsync("Upload", async () =>
            {
                if (files == null || files.Count == 0)
                {
                    return BadRequest(ApiResponse.Error(400, "未选择要上传的文件"));
                }

                var names = files.Select(f => Path.GetFileName(f.FileName)).ToList();
                if (names.Any(string.IsNullOrWhiteSpace))
                {
                    return BadRequest(ApiResponse.Error(400, "上传文件名无效"));
                }
                if (names.Distinct(StringComparer.OrdinalIgnoreCase).Count() != names.Count)
                {
                    return BadRequest(ApiResponse.Error(400, "上传文件中存在重名"));
                }

                var targets = new List<(IFormFile File, string FullPath)>();
                foreach (var file in files)
                {
                    string fullPath;
                    try
                    {
                        fullPath = FileManagerService.ResolveUploadTarget(targetDir, Path.GetFileName(file.FileName));
                    }
                    catch (FileManagerException e)
                    {
                        return StatusCode(e.StatusCode, ApiResponse.Error(e.StatusCode, e.Message));
                    }
                    if (System.IO.File.Exists(fullPath) || System.IO.Directory.Exists(fullPath))
                    {
                        var relative = FileManagerService.ToRelativePath(fullPath);
                        return StatusCode(409, ApiResponse.Error(409, $"目标已存在：{relative}"));
                    }
                    targets.Add((file, fullPath));
                }

                var written = new List<string>();
                try
                {
                    foreach (var (file, fullPath) in targets)
                    {
                        await using var stream = new FileStream(fullPath, FileMode.CreateNew, FileAccess.Write);
                        await file.CopyToAsync(stream);
                        written.Add(fullPath);
                    }
                }
                catch
                {
                    // 清理本次已写入的文件，避免多文件上传中途失败留下半成品
                    foreach (var path in written)
                    {
                        try
                        {
                            System.IO.File.Delete(path);
                        }
                        catch
                        {
                            // 清理失败不掩盖原始错误
                        }
                    }
                    throw;
                }
                _logger.LogInformation("文件管理-上传: Target={Target}, Count={Count}", targetDir, targets.Count);
                return Ok(ApiResponse.Ok());
            });
        }

        [HttpGet("sqlite/tables")]
        [EndpointSummary("SQLite 表/视图列表")]
        [EndpointDescription("列出数据库中的表与视图（排除 sqlite_ 内部对象）")]
        [ProducesResponseType(typeof(ApiResponse<List<SqliteTableInfo>>), StatusCodes.Status200OK)]
        public IActionResult SqliteTables(
            [Description("数据库文件相对根目录的路径")][FromQuery] string path)
        {
            return Execute("SqliteTables", () =>
            {
                var result = FileManagerService.SqliteTables(path);
                _logger.LogInformation("文件管理-SQLite 表列表: Path={Path}, Count={Count}", path, result.Count);
                return Ok(ApiResponse.Ok(result));
            });
        }

        [HttpGet("sqlite/schema")]
        [EndpointSummary("SQLite 表结构")]
        [EndpointDescription("获取指定表的列与索引信息")]
        [ProducesResponseType(typeof(ApiResponse<SqliteSchemaResult>), StatusCodes.Status200OK)]
        public IActionResult SqliteSchema(
            [Description("数据库文件相对根目录的路径")][FromQuery] string path,
            [Description("表名或视图名")][FromQuery] string table)
        {
            return Execute("SqliteSchema", () =>
            {
                var result = FileManagerService.SqliteSchema(path, table);
                _logger.LogInformation("文件管理-SQLite 表结构: Path={Path}, Table={Table}", path, table);
                return Ok(ApiResponse.Ok(result));
            });
        }

        [HttpGet("sqlite/data")]
        [EndpointSummary("SQLite 表数据分页预览")]
        [EndpointDescription("分页预览表数据，pageSize 最大 200")]
        [ProducesResponseType(typeof(ApiResponse<SqliteDataResult>), StatusCodes.Status200OK)]
        public IActionResult SqliteData(
            [Description("数据库文件相对根目录的路径")][FromQuery] string path,
            [Description("表名或视图名")][FromQuery] string table,
            [Description("页码，从 1 开始")][FromQuery] int page = 1,
            [Description("每页条数")][FromQuery] int pageSize = 50)
        {
            return Execute("SqliteData", () =>
            {
                var result = FileManagerService.SqliteData(path, table, page, pageSize);
                _logger.LogInformation("文件管理-SQLite 表数据: Path={Path}, Table={Table}, Page={Page}", path, table, page);
                return Ok(ApiResponse.Ok(result));
            });
        }

        [HttpPost("sqlite/query")]
        [EndpointSummary("执行 SQLite SQL")]
        [EndpointDescription("对数据库文件执行任意 SQL（单条语句）。查询类返回结果集，写入类返回受影响行数；语法错误返回 errorType=sql_syntax_error")]
        [ProducesResponseType(typeof(ApiResponse<SqliteQueryResult>), StatusCodes.Status200OK)]
        public IActionResult SqliteQuery([FromBody] SqliteQueryRequest request)
        {
            return Execute("SqliteQuery", () =>
            {
                var result = FileManagerService.SqliteQuery(request.Path, request.Sql);
                _logger.LogInformation("文件管理-SQLite 查询: Path={Path}, Type={Type}", request.Path, result.Type);
                return Ok(ApiResponse.Ok(result));
            });
        }

        private IActionResult Execute(string operation, Func<IActionResult> action)
        {
            return ExecuteAsync(operation, () => Task.FromResult(action())).GetAwaiter().GetResult();
        }

        private async Task<IActionResult> ExecuteAsync(string operation, Func<Task<IActionResult>> action)
        {
            var guard = EnsureEnabled();
            if (guard != null)
            {
                return guard;
            }
            try
            {
                return await action();
            }
            catch (OperationCanceledException)
            {
                if (HttpContext.RequestAborted.IsCancellationRequested)
                {
                    // 客户端已断开，直接让请求中止
                    throw;
                }
                _logger.LogWarning("文件管理操作已取消: Operation={Operation}", operation);
                return StatusCode(400, ApiResponse.Error(400, "操作已取消"));
            }
            catch (SqlSyntaxException e)
            {
                _logger.LogWarning("文件管理 SQL 语法错误: Error={Error}", e.Message);
                return StatusCode(400, new ApiResponse
                {
                    Code = 400,
                    Message = $"SQL 语法错误：{e.Message}",
                    Data = new { ErrorType = "sql_syntax_error" }
                });
            }
            catch (FileManagerException e)
            {
                // 二进制文件被当作文本加载属预期行为，不记录 Warning 日志
                if (e is not BinaryFileException)
                {
                    _logger.LogWarning("文件管理操作失败: Operation={Operation}, Error={Error}", operation, e.Message);
                }
                return StatusCode(e.StatusCode, ApiResponse.Error(e.StatusCode, e.Message));
            }
            catch (Exception e) when (FileManagerService.IsFileInUse(e))
            {
                _logger.LogWarning("文件管理操作失败：文件被占用 Operation={Operation}, Error={Error}", operation, e.Message);
                return StatusCode(400, new ApiResponse
                {
                    Code = 400,
                    Message = "文件被其他程序占用，请稍后重试",
                    Data = new { ErrorType = "file_in_use", Detail = e.Message }
                });
            }
            catch (Exception e)
            {
                _logger.LogError(e, "文件管理操作异常: Operation={Operation}", operation);
                return StatusCode(500, ApiResponse.Error(500, "由于服务器异常，操作失败"));
            }
        }

        private IActionResult? EnsureEnabled()
        {
            if (WebAPIConfig.Instance.EnableFileManager)
            {
                return null;
            }
            _logger.LogWarning("文件管理操作被拒绝：功能未启用");
            return StatusCode(403, ApiResponse.Error(403, "文件管理功能未启用"));
        }

        /// <summary>
        /// 将多个文件/文件夹流式打包为 ZIP 并直接写入响应体，不产生临时文件
        /// </summary>
        private sealed class ZipFileResult(IReadOnlyList<FileManagerService.ZipSource> sources, string fileName, CancellationToken cancellationToken) : IActionResult
        {
            public async Task ExecuteResultAsync(ActionContext context)
            {
                var response = context.HttpContext.Response;
                response.ContentType = "application/zip";
                response.Headers.ContentDisposition =
                    $"attachment; filename=\"{fileName}\"; filename*=UTF-8''{Uri.EscapeDataString(fileName)}";
                response.Headers.CacheControl = "no-store";
                await FileManagerService.WriteZipAsync(response.Body, sources, cancellationToken);
                await response.Body.FlushAsync();
            }
        }
    }
}
