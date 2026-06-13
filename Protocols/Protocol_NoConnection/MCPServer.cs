using Another_Mirai_Native.DB;
using System;
using System.Threading;
using System.Threading.Tasks;
#if NET5_0_OR_GREATER
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;
#endif

namespace Protocol_NoConnection
{
    #if NET5_0_OR_GREATER
    /// <summary>
    /// MCP (Model Context Protocol) 服务器
    /// 基于 ModelContextProtocol.AspNetCore 官方 SDK，使用 Kestrel 提供 HTTP 传输
    /// 为 NoConnection 协议提供 AI 工具调用能力
    /// 仅在 .NET 9 环境下可用
    /// </summary>
    public class MCPServer
    {
        private WebApplication _app;
        private CancellationTokenSource _cts;

        public MCPServer(string ip, ushort port, Protocol protocol)
        {
            ListenIP = ip;
            Port = port;
            Protocol = protocol;
        }

        public bool Running => _app != null && _cts != null && !_cts.IsCancellationRequested;

        public string ListenURL => $"http://{ListenIP}:{Port}/";

        private string ListenIP { get; set; }

        private ushort Port { get; set; }

        private Protocol Protocol { get; set; }

        public bool Start()
        {
            try
            {
                if (Running)
                {
                    Stop();
                }

                _cts = new CancellationTokenSource();

                var builder = WebApplication.CreateBuilder(new string[0]);

                // 配置 Kestrel 监听地址
                builder.WebHost.UseUrls(ListenURL);

                // 抑制 ASP.NET Core 日志（由 AMN 日志系统接管）
                builder.Logging.ClearProviders();
                builder.Logging.AddProvider(new AMNLoggerProvider());

                // 添加 MCP 服务，显式指定工具所在程序集
                builder.Services.AddMcpServer()
                    .WithHttpTransport(options =>
                    {
                        options.Stateless = true;
                    })
                    .WithToolsFromAssembly(typeof(MCPTools).Assembly);

                _app = builder.Build();

                // 映射 MCP 端点
                _app.MapMcp();

                // 启动 Kestrel
                _ = Task.Run(async () =>
                {
                    try
                    {
                        LogHelper.Info("启动MCP服务器", $"MCP 服务已启动，监听 {ListenURL}");
                        await _app.RunAsync();
                    }
                    catch (OperationCanceledException)
                    {
                        // 正常关闭
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Error("MCP服务器运行异常", ex);
                    }
                });

                return true;
            }
            catch (Exception e)
            {
                LogHelper.Error("启动MCP服务器", e);
                return false;
            }
        }

        public bool Stop()
        {
            try
            {
                _cts?.Cancel();
                if (_app != null)
                {
                    using var stopCts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
                    _app.StopAsync(stopCts.Token).GetAwaiter().GetResult();
                    _app.DisposeAsync().GetAwaiter().GetResult();
                    _app = null;
                }
                _cts?.Dispose();
                _cts = null;
                LogHelper.Info("停止MCP服务器", "MCP 服务已停止");
                return true;
            }
            catch (Exception e)
            {
                LogHelper.Error("停止MCP服务器", e);
                return false;
            }
        }
    }

    /// <summary>
    /// 将 ASP.NET Core 日志转发到 AMN 日志系统的 Provider
    /// </summary>
    internal class AMNLoggerProvider : ILoggerProvider
    {
        public ILogger CreateLogger(string categoryName)
        {
            return new AMNLogger(categoryName);
        }

        public void Dispose() { }
    }

    internal class AMNLogger : ILogger
    {
        private readonly string _category;

        public AMNLogger(string category)
        {
            _category = category;
        }

        public IDisposable BeginScope<TState>(TState state) where TState : notnull
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel >= Microsoft.Extensions.Logging.LogLevel.Warning;
        }

        public void Log<TState>(Microsoft.Extensions.Logging.LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            string message = formatter?.Invoke(state, exception) ?? state?.ToString() ?? "";
            if (exception != null)
            {
                message += $"\n异常: {exception.Message}";
            }

            var amnLevel = logLevel switch
            {
                Microsoft.Extensions.Logging.LogLevel.Warning => Another_Mirai_Native.Model.Enums.LogLevel.Warning,
                Microsoft.Extensions.Logging.LogLevel.Error => Another_Mirai_Native.Model.Enums.LogLevel.Error,
                Microsoft.Extensions.Logging.LogLevel.Critical => Another_Mirai_Native.Model.Enums.LogLevel.Fatal,
                _ => Another_Mirai_Native.Model.Enums.LogLevel.Info
            };

            LogHelper.WriteLog(amnLevel, $"MCP/{_category}", "MCP日志", message);
        }
    }
#else
public class MCPServer
{
        public MCPServer(string ip, ushort port, Protocol protocol)
        {
            ListenIP = ip;
            Port = port;
            Protocol = protocol;
        }

        public bool Running => false;

        public string ListenURL => $"http://{ListenIP}:{Port}/";

        private string ListenIP { get; set; }

        private ushort Port { get; set; }

        private Protocol Protocol { get; set; }

        public bool Start()
        {
            throw new NotSupportedException("MCPServer 仅在 .NET 5 及以上环境下可用");
        }

        public bool Stop()
        {
            throw new NotSupportedException("MCPServer 仅在 .NET 5 及以上环境下可用");
        }
}
#endif

}
