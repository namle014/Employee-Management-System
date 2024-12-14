using Microsoft.Extensions.Logging;
using OA.Service.Helpers.Logging.Internal;

namespace OA.Service.Helpers.Logging
{
    /// <summary>
    ///     Options for file logging.
    /// </summary>
    public class FileLoggerOptions : BatchingLoggerOptions
    {
        private int _fileSizeLimit = 5 * 1024 * 1024;
        private string _fileName = "vCAS_Logs_{0}.txt";
        private string _logDirectory = "logs";
        private LogLevel _logLevel = LogLevel.Debug;

        /// <summary>
        /// Gets or sets a strictly positive value representing the maximum log size in bytes or null for no limit.
        /// Once the log is full, no more messages will be appended.
        /// Defaults to <c>5MB</c>.
        /// </summary>
        public int FileSizeLimit
        {
            get => _fileSizeLimit;
            set
            {
                if (value > 0)
                {
                    _fileSizeLimit = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the filename to use for log files.
        /// </summary>
        public string FileName
        {
            get => _fileName;
            set
            {
                if (string.IsNullOrWhiteSpace(value) == false)
                {
                    _fileName = value;
                }
            }
        }

        /// <summary>
        /// The directory in which log files will be written, relative to the app process.
        /// Default to <c>Logs</c>
        /// </summary>
        public string LogDirectory
        {
            get => _logDirectory;
            set
            {
                if (string.IsNullOrWhiteSpace(value) == false)
                {
                    _logDirectory = value;
                }
            }
        }

        /// <summary>
        /// The directory in which log files will be written, relative to the app process.
        /// Default to <c>Logs</c>
        /// </summary>
        public new LogLevel LogLevel
        {
            get => _logLevel;
            set
            {
                try
                {
                    _logLevel = value;
                }
                catch
                {
                    // ignored                    
                }
            }
        }
    }
}