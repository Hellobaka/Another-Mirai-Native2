using System.ComponentModel;

namespace Another_Mirai_Native.WebAPI.Models
{
    [Description("系统基础信息")]
    public class DashboardInfoData
    {
        [Description("操作系统名称与版本")]
        public string OSVersion { get; set; } = string.Empty;

        [Description("CPU 型号与基准频率")]
        public string CPU { get; set; } = string.Empty;

        [Description("物理内存总量（MB）")]
        public ulong TotalMemory { get; set; }

        [Description("框架运行时长（d:hh:mm:ss）")]
        public string StartedTime { get; set; } = string.Empty;

        [Description("框架版本号")]
        public string Version { get; set; } = string.Empty;

        [Description("当前登录的 Bot QQ 号")]
        public long CurrentBotQQ { get; set; }

        [Description("当前登录的 Bot 昵称")]
        public string CurrentBotNick { get; set; } = string.Empty;

        [Description("已启用的插件数量")]
        public int LoadedPluginCount { get; set; }

        [Description("运行时使用的 .NET 版本")]
        public string DotNetRuntimeVersion { get; set; } = string.Empty;

        [Description("当前工作目录")]
        public string WorkingDirectory { get; set; } = string.Empty;

        [Description("当前磁盘剩余空间（GB）")]
        public double DiskFreeSpaceInGB { get; set; }

        [Description("当前磁盘总空间（GB）")]
        public double DiskTotalSpaceInGB { get; set; }
    }

    [Description("系统资源占用（整体 CPU / 内存）")]
    public class UsageData
    {
        [Description("系统整体 CPU 占用百分比")]
        public float CPUUsage { get; set; }

        [Description("内存占用百分比")]
        public double MemoryUsage { get; set; }

        [Description("已用内存（MB）")]
        public ulong UsedMemoryInMB { get; set; }

        [Description("总内存（MB）")]
        public ulong TotalMemoryInMB { get; set; }
    }

    [Description("各进程资源占用汇总")]
    public class PluginUsageData
    {
        [Description("所有进程内存合计（MB）")]
        public double TotalProcessMemory { get; set; }

        [Description("所有进程 CPU 合计（%）")]
        public double TotalProcessCPU { get; set; }

        [Description("已处理的消息数量")]
        public int ProcessedMessageCount { get; set; }
        
        [Description("已发送的消息数量")]
        public int SentMessageCount { get; set; }

        [Description("各进程详情")]
        public List<DashboardPluginItem> PluginUsages { get; set; } = [];
    }
}
