using Microsoft.Extensions.Logging;

namespace Another_Mirai_Native.BlazorUI
{
    public class Logging : ILoggerProvider
    {
        private readonly EventLogger _logger = new EventLogger();
        
        public Logging()
        {
            Instance = this;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return _logger;
        }

        public void Dispose() { }

        public EventLogger Logger => _logger;

        public static Logging Instance { get; private set; } = new();
    }

    public class EventLogger : ILogger
    {
        /// <summary>
        /// Error msg Exception
        /// </summary>
        public event Action<bool, string, Exception?> LogEvent;

        IDisposable? ILogger.BeginScope<TState>(TState state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            var message = formatter(state, exception);
            message = $"[{logLevel}] {message}{Environment.NewLine}";
            LogEvent?.Invoke(logLevel > LogLevel.Information, message, exception);
        }
    }
}
