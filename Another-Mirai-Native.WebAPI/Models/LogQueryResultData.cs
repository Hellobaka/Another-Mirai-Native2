using System.ComponentModel;

namespace Another_Mirai_Native.WebAPI.Models
{
    [Description("日志分页查询结果")]
    public class LogQueryResultData
    {
        [Description("当前页日志条目")]
        public IEnumerable<LogDto> Items { get; set; } = [];

        [Description("符合条件的总记录数")]
        public int TotalCount { get; set; }

        [Description("总页数")]
        public int TotalPage { get; set; }
    }
}
