using Another_Mirai_Native.DB;
using Another_Mirai_Native.Model.Enums;
using NLog;
using NLog.Common;
using NLog.Targets;

namespace Another_Mirai_Native.WebAPI.NLog;

/// <summary>
/// 将 NLog 日志桥接到框架主线 LogHelper
/// </summary>
[Target("LogHelper")]
public class LogHelperTarget : TargetWithLayout
{
    protected override void Write(LogEventInfo logEvent)
    {
        try
        {
            var model = new LogModel
            {
                priority = MapLevel(logEvent.Level),
                source = "WebAPI",
                name = logEvent.LoggerName ?? "WebAPI",
                detail = RenderLogEvent(Layout, logEvent),
                time = DateTime.Now,
                status = ""
            };

            LogHelper.WriteLog(model);
        }
        catch
        {
            // 框架未就绪时静默丢弃，文件目标仍正常工作
        }
    }

    private static int MapLevel(NLog.LogLevel level)
    {
        return level.Name switch
        {
            "Trace" => (int)LogLevel.Debug,
            "Debug" => (int)LogLevel.Debug,
            "Info"  => (int)LogLevel.Info,
            "Warn"  => (int)LogLevel.Warning,
            "Error" => (int)LogLevel.Error,
            "Fatal" => (int)LogLevel.Fatal,
            _       => (int)LogLevel.Info,
        };
    }
}
