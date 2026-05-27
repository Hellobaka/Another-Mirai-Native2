using Another_Mirai_Native.DB;
using Another_Mirai_Native.Model;
using NLog;
using NLog.Targets;

namespace Another_Mirai_Native.WebAPI;

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
                time = Helper.DateTime2TimeStamp(DateTime.Now),
                status = ""
            };

            LogHelper.WriteLog(model);
        }
        catch
        {
        }
    }

    private static int MapLevel(NLog.LogLevel level)
    {
        return level.Name switch
        {
            "Trace" => (int)Model.Enums.LogLevel.Debug,
            "Debug" => (int)Model.Enums.LogLevel.Debug,
            "Info"  => (int)Model.Enums.LogLevel.Info,
            "Warn"  => (int)Model.Enums.LogLevel.Warning,
            "Error" => (int)Model.Enums.LogLevel.Error,
            "Fatal" => (int)Model.Enums.LogLevel.Fatal,
            _       => (int)Model.Enums.LogLevel.Info,
        };
    }
}
