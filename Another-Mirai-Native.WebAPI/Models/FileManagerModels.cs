using System.ComponentModel;

namespace Another_Mirai_Native.WebAPI.Models
{
    [Description("文件/文件夹条目")]
    public class FileEntryDto
    {
        [Description("名称")]
        public string Name { get; set; } = string.Empty;

        [Description("相对根目录的路径，使用 / 分隔")]
        public string Path { get; set; } = string.Empty;

        [Description("是否为文件夹")]
        public bool IsDirectory { get; set; }

        [Description("文件大小（字节），文件夹为 null")]
        public long? Size { get; set; }

        [Description("最后写入时间")]
        public DateTime LastWriteTime { get; set; }
    }

    [Description("目录浏览结果")]
    public class ListDirectoryResult
    {
        [Description("文件管理器根目录绝对路径")]
        public string Root { get; set; } = string.Empty;

        [Description("当前目录相对根目录的路径")]
        public string Path { get; set; } = string.Empty;

        [Description("父目录相对根目录的路径，根目录时为 \"\"")]
        public string Parent { get; set; } = string.Empty;

        [Description("目录内容列表")]
        public List<FileEntryDto> Items { get; set; } = new();
    }

    [Description("路径请求体")]
    public class FilePathRequest
    {
        [Description("相对根目录的路径")]
        public string Path { get; set; } = string.Empty;
    }

    [Description("新建文件请求")]
    public class CreateFileRequest
    {
        [Description("相对根目录的路径")]
        public string Path { get; set; } = string.Empty;

        [Description("初始文本内容（可选）")]
        public string? Content { get; set; }
    }

    [Description("重命名请求")]
    public class RenameRequest
    {
        [Description("相对根目录的路径")]
        public string Path { get; set; } = string.Empty;

        [Description("新名称（仅文件名，不含路径）")]
        public string NewName { get; set; } = string.Empty;
    }

    [Description("复制/移动请求")]
    public class CopyMoveRequest
    {
        [Description("源路径列表，相对根目录")]
        public List<string> Sources { get; set; } = new();

        [Description("目标目录，相对根目录")]
        public string TargetDir { get; set; } = string.Empty;
    }

    [Description("删除请求")]
    public class DeleteRequest
    {
        [Description("待删除路径列表，相对根目录")]
        public List<string> Paths { get; set; } = new();
    }

    [Description("文本文件读取结果")]
    public class ReadTextResult
    {
        [Description("相对根目录的路径")]
        public string Path { get; set; } = string.Empty;

        [Description("文本内容")]
        public string Content { get; set; } = string.Empty;

        [Description("识别的编码：UTF-8 / UTF-16 / GBK")]
        public string Encoding { get; set; } = string.Empty;
    }

    [Description("文本文件写入请求")]
    public class WriteTextRequest
    {
        [Description("相对根目录的路径")]
        public string Path { get; set; } = string.Empty;

        [Description("新的文本内容")]
        public string Content { get; set; } = string.Empty;

        [Description("指定编码：utf-8 / gbk / gb18030 / ansi / utf-16 / utf-16be；留空则按文件自动识别")]
        public string? Encoding { get; set; }
    }

    [Description("SQLite 表/视图信息")]
    public class SqliteTableInfo
    {
        [Description("名称")]
        public string Name { get; set; } = string.Empty;

        [Description("类型：table / view")]
        public string Type { get; set; } = string.Empty;

        [Description("建表/建视图 SQL")]
        public string Sql { get; set; } = string.Empty;
    }

    [Description("SQLite 列信息")]
    public class SqliteColumnInfo
    {
        [Description("列名")]
        public string Name { get; set; } = string.Empty;

        [Description("数据类型")]
        public string DataType { get; set; } = string.Empty;

        [Description("是否非空")]
        public bool NotNull { get; set; }

        [Description("默认值")]
        public string? DefaultValue { get; set; }

        [Description("是否主键（0 或主键序号）")]
        public int PrimaryKey { get; set; }
    }

    [Description("SQLite 索引信息")]
    public class SqliteIndexInfo
    {
        [Description("索引名")]
        public string Name { get; set; } = string.Empty;

        [Description("是否唯一索引")]
        public bool Unique { get; set; }

        [Description("索引包含的列")]
        public List<string> Columns { get; set; } = new();
    }

    [Description("SQLite 表结构结果")]
    public class SqliteSchemaResult
    {
        [Description("表/视图名")]
        public string Table { get; set; } = string.Empty;

        [Description("列信息")]
        public List<SqliteColumnInfo> Columns { get; set; } = new();

        [Description("索引信息")]
        public List<SqliteIndexInfo> Indexes { get; set; } = new();
    }

    [Description("SQLite 表数据分页结果")]
    public class SqliteDataResult
    {
        [Description("列名")]
        public List<string> Columns { get; set; } = new();

        [Description("数据行，值与 Columns 顺序对应")]
        public List<List<object?>> Rows { get; set; } = new();

        [Description("总行数")]
        public long Total { get; set; }

        [Description("页码")]
        public int Page { get; set; }

        [Description("每页条数")]
        public int PageSize { get; set; }
    }

    [Description("SQLite 查询执行请求")]
    public class SqliteQueryRequest
    {
        [Description("相对根目录的数据库文件路径")]
        public string Path { get; set; } = string.Empty;

        [Description("要执行的 SQL（单条语句）")]
        public string Sql { get; set; } = string.Empty;
    }

    [Description("SQLite 查询执行结果")]
    public class SqliteQueryResult
    {
        [Description("结果类型：query=返回结果集，execute=执行写入语句")]
        public string Type { get; set; } = string.Empty;

        [Description("结果集列名（query 类型）")]
        public List<string>? Columns { get; set; }

        [Description("结果集数据（query 类型）")]
        public List<List<object?>>? Rows { get; set; }

        [Description("结果是否超过 1000 行被截断")]
        public bool Truncated { get; set; }

        [Description("受影响行数（execute 类型）")]
        public int AffectedRows { get; set; }
    }

    [Description("文件/文件夹大小探测结果")]
    public class FileSizeResult
    {
        [Description("总字节数")]
        public long Size { get; set; }
    }

    [Description("文件名搜索结果")]
    public class FileSearchResult
    {
        [Description("命中的条目（最多 limit 条）")]
        public List<FileEntryDto> Items { get; set; } = new();

        [Description("全部命中数量（可能超过返回条数）")]
        public long Total { get; set; }
    }
}
