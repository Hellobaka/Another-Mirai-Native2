using System.ComponentModel;
using Another_Mirai_Native.Model;

namespace Another_Mirai_Native.WebAPI.Models
{
    [Description("单条日志记录")]
    public class LogDto
    {
        [Description("日志 ID")]
        public int Id { get; set; }

        [Description("记录时间")]
        public DateTime Time { get; set; }

        [Description("日志等级：0=Debug, 1=Info, 2=InfoSend, 3=Warning, 4=Error, 5=Fatal")]
        public int Priority { get; set; }

        [Description("产生日志的组件或插件名")]
        public string Source { get; set; } = "";

        [Description("处理状态：unread / read")]
        public string Status { get; set; } = "";

        [Description("日志分类名")]
        public string Name { get; set; } = "";

        [Description("日志详细内容")]
        public string Detail { get; set; } = "";

        public static LogDto CreateFromLogModel(LogModel log)
        {
            return new LogDto
            {
                Id = log.id,
                Time = Helper.TimeStamp2DateTime(log.time),
                Priority = log.priority,
                Source = log.source,
                Status = log.status,
                Name = log.name,
                Detail = log.detail,
            };
        }
    }
}
