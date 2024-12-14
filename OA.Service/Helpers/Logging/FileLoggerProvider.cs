using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OA.Service.Helpers.Logging.Internal;

namespace OA.Service.Helpers.Logging
{
    /// <summary>
    /// An <see cref="ILoggerProvider" /> that writes logs to a file
    /// </summary>
    [ProviderAlias("File")]
    public class FileLoggerProvider : BatchingLoggerProvider
    {
        private readonly string _path;
        private readonly string _fileName;
        private readonly int _maxFileSize;

        private string? _folder;

        private string? _filePath;
        private int _fileSize;

        /// <summary>
        /// Creates an instance of the <see cref="FileLoggerProvider" /> 
        /// </summary>
        /// <param name="options">The options object controlling the logger</param>
        public FileLoggerProvider(IOptionsMonitor<FileLoggerOptions> options) : base(options)
        {
            FileLoggerOptions loggerOptions = options.CurrentValue;
            _path = loggerOptions.LogDirectory;
            _fileName = loggerOptions.FileName;
            _maxFileSize = loggerOptions.FileSizeLimit;
        }

        /// <inheritdoc />
        protected override async Task WriteMessagesAsync(IEnumerable<LogMessage> messages, CancellationToken cancellationToken)
        {
            string currentDate = DateTime.Now.ToString("yyyyMMdd");
            if (string.IsNullOrWhiteSpace(_folder) ||
                _folder != currentDate)
            {
                _folder = currentDate;
            }

            string folderPath = Path.Combine(_path, _folder);
            if (Directory.Exists(folderPath) == false)
            {
                Directory.CreateDirectory(folderPath);
            }

            if (string.IsNullOrWhiteSpace(_filePath) ||
                _fileSize >= _maxFileSize)
            {
                string fileName = string.Format(_fileName, DateTime.Now.ToString("yyyyMMddHHmmss"));
                _filePath = Path.Combine(folderPath, fileName);
                _fileSize = 0;
            }

            using (var streamWriter = File.AppendText(_filePath))
            {
                foreach (LogMessage message in messages)
                {
                    string content = message.Message;
                    _fileSize += content.Length;

                    await streamWriter.WriteAsync(content);
                }
            }
        }
    }
}