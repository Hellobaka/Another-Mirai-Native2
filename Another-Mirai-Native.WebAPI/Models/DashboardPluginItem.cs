using System.ComponentModel;

namespace Another_Mirai_Native.WebAPI.Models
{
    [Description("单个进程（插件或主框架）的资源占用")]
    public class DashboardPluginItem
    {
        [Description("插件授权码（0 表示主框架自身）")]
        public int Id { get; set; }

        [Description("进程 ID")]
        public int PID { get; set; }

        [Description("插件名称")]
        public string PluginName { get; set; } = "";

        [Description("进程是否在运行中")]
        public bool Running { get; set; }

        [Description("该进程 CPU 占用百分比")]
        public double CPUUsage { get; set; }

        [Description("该进程内存占用（MB）")]
        public double MemoryUsage { get; set; }
    }
}
