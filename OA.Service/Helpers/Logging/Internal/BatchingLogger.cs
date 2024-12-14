using Microsoft.Extensions.Logging;
using System.Text;

namespace OA.Service.Helpers.Logging.Internal
{
    public class BatchingLogger : ILogger
    {
        private readonly BatchingLoggerProvider _provider;
        private readonly string _category;

        public BatchingLogger(BatchingLoggerProvider loggerProvider, string categoryName)
        {
            _provider = loggerProvider;
            _category = categoryName;
        }

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull
        {
            var s = state as IDisposable;
            return s;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return _provider.IsLog(logLevel);
        }

        public void Log<TState>(DateTimeOffset timestamp, LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            var builder = new StringBuilder();
            builder.Append(timestamp.ToString("yyyy-MM-dd HH:mm:ss"));
            builder.Append($" [{logLevel.ToString()}] {_category}: ");
            builder.AppendLine(formatter(state, exception));

            if (exception != null)
            {
                builder.AppendLine(exception.ToString());
            }

            _provider.AddMessage(timestamp, builder.ToString());
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception, string> formatter)
        {
            Log(DateTimeOffset.Now, logLevel, eventId, state, exception ?? new Exception(), formatter);
        }
    }
}