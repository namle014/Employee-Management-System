using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;

namespace OA.Service.Helpers.Logging.Internal
{
    public abstract class BatchingLoggerProvider : ILoggerProvider
    {
        private readonly List<LogMessage> _currentBatch = new List<LogMessage>();
        private readonly TimeSpan _interval;
        private readonly int? _queueSize;
        private readonly int? _batchSize;
        private readonly IDisposable? _optionsChangeToken;

        private BlockingCollection<LogMessage> _messageQueue = new BlockingCollection<LogMessage>();
        private Task? _outputTask;
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        private readonly LogLevel _logLevel;

        protected BatchingLoggerProvider(IOptionsMonitor<BatchingLoggerOptions> options)
        {
            // NOTE: Only IsEnabled and IncludeScopes are monitored
            BatchingLoggerOptions loggerOptions = options.CurrentValue;

            _interval = loggerOptions.FlushPeriod;
            _batchSize = loggerOptions.BatchSize;
            _queueSize = loggerOptions.BackgroundQueueSize;
            _logLevel = loggerOptions.LogLevel;

            _optionsChangeToken = options.OnChange(UpdateOptions);
            UpdateOptions(options.CurrentValue);
        }

        public bool IsEnabled { get; private set; }

        public bool IsLog(LogLevel logLevel)
        {
            return logLevel >= _logLevel;
        }

        private void UpdateOptions(BatchingLoggerOptions options)
        {
            var oldIsEnabled = IsEnabled;
            IsEnabled = options.IsEnabled;

            if (oldIsEnabled != IsEnabled)
            {
                if (IsEnabled)
                {
                    Start();
                }
                else
                {
                    Stop();
                }
            }

        }

        protected abstract Task WriteMessagesAsync(IEnumerable<LogMessage> messages, CancellationToken token);

        private async Task ProcessLogQueue()
        {
            while (!_cancellationTokenSource.IsCancellationRequested)
            {
                var limit = _batchSize ?? int.MaxValue;

                while (limit > 0 && _messageQueue.TryTake(out var message))
                {
                    _currentBatch.Add(message);
                    limit--;
                }

                if (_currentBatch.Count > 0)
                {
                    try
                    {
                        await WriteMessagesAsync(_currentBatch, _cancellationTokenSource.Token);
                    }
                    catch
                    {
                        // ignored
                    }

                    _currentBatch.Clear();
                }

                await IntervalAsync(_interval, _cancellationTokenSource.Token);
            }
        }

        protected virtual Task IntervalAsync(TimeSpan interval, CancellationToken cancellationToken)
        {
            return Task.Delay(interval, cancellationToken);
        }

        internal void AddMessage(DateTimeOffset timestamp, string message)
        {
            if (!_messageQueue.IsAddingCompleted)
            {
                try
                {
                    _messageQueue.Add(new LogMessage { Message = message, Timestamp = timestamp }, _cancellationTokenSource.Token);
                }
                catch
                {
                    //cancellation token canceled or CompleteAdding called
                }
            }
        }

        private void Start()
        {
            _messageQueue = _queueSize == null ?
                new BlockingCollection<LogMessage>(new ConcurrentQueue<LogMessage>()) :
                new BlockingCollection<LogMessage>(new ConcurrentQueue<LogMessage>(), _queueSize.Value);

            _cancellationTokenSource = new CancellationTokenSource();
            _outputTask = Task.Run(ProcessLogQueue);
        }

        private void Stop()
        {
            _cancellationTokenSource.Cancel();
            _messageQueue.CompleteAdding();

            try
            {
                _outputTask?.Wait(_interval);
            }
            catch (TaskCanceledException)
            {
            }
            catch (AggregateException ex) when (ex.InnerExceptions.Count == 1 && ex.InnerExceptions[0] is TaskCanceledException)
            {
            }
        }

        public void Dispose()
        {
            _optionsChangeToken?.Dispose();
            if (IsEnabled)
            {
                Stop();
            }
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new BatchingLogger(this, categoryName);
        }
    }
}
