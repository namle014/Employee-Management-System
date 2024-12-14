using Microsoft.Extensions.Logging;

namespace OA.Service.Helpers.Logging.Internal
{
    public class BatchingLoggerOptions
    {
        private int? _batchSize;
        private int _backgroundQueueSize = 1000;
        private TimeSpan _flushPeriod = TimeSpan.FromSeconds(10);

        /// <summary>
        /// Gets or sets the period after which logs will be flushed to the store.
        /// </summary>
        public TimeSpan FlushPeriod
        {
            get => _flushPeriod;
            set
            {
                if (value > TimeSpan.Zero)
                {
                    _flushPeriod = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the maximum size of the background log message queue or null for no limit.
        /// After maximum queue size is reached log event sink would start blocking.
        /// Defaults to <c>1000</c>.
        /// </summary>
        public int BackgroundQueueSize
        {
            get => _backgroundQueueSize;
            set
            {
                if (value > 0)
                {
                    _backgroundQueueSize = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a maximum number of events to include in a single batch or null for no limit.
        /// </summary>
        /// Defaults to <c>null</c>.
        public int? BatchSize
        {
            get => _batchSize;
            set
            {
                if (value > 0)
                {
                    _batchSize = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets value indicating if logger accepts and queues writes.
        /// </summary>
        public bool IsEnabled { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether scopes should be included in the message.
        /// Defaults to <c>false</c>.
        /// </summary>
        public bool IncludeScopes { get; set; } = false;

        public LogLevel LogLevel { get; set; } = LogLevel.Debug;
    }
}