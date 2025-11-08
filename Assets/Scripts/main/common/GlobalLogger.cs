using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace gamecore.common
{
    /// <summary>
    /// Global file logger singleton that can be accessed from anywhere in the application
    /// </summary>
    public sealed class GlobalLogger : IDisposable
    {
        private static readonly Lazy<GlobalLogger> _instance = new(() => new GlobalLogger());
        public static GlobalLogger Instance => _instance.Value;

        private readonly ConcurrentQueue<LogEntry> _logQueue = new();
        private readonly CancellationTokenSource _cancellationTokenSource = new();
        private readonly Task _writerTask;
        private readonly object _lockObject = new();
        private string _filePath = "application.log";
        private LogLevel _minLogLevel = LogLevel.Debug;
        private bool _initialized = false;
        private bool _disposed = false;

        private GlobalLogger()
        {
            File.WriteAllText(_filePath, string.Empty);
            // Start the background writer task
            _writerTask = Task.Run(WriteLogsAsync, _cancellationTokenSource.Token);
            Initialize();
        }

        /// <summary>
        /// Initializes the global logger with the specified file path and log level
        /// </summary>
        /// <param name="filePath">The path to the log file</param>
        /// <param name="minLogLevel">The minimum log level to write</param>
        private void Initialize()
        {
            if (_initialized)
                return;

            // Ensure directory exists
            var directory = Path.GetDirectoryName(_filePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            _initialized = true;
        }

        public void SetLogFilePath(string filePath)
        {
            _filePath = filePath;
            var directory = Path.GetDirectoryName(_filePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }

        /// <summary>
        /// Logs a message with the specified log level
        /// </summary>
        public void Log(LogLevel level, string message)
        {
            if (!IsEnabled(level) || string.IsNullOrEmpty(message))
                return;

            var logEntry = new LogEntry
            {
                Timestamp = DateTime.UtcNow,
                Level = level,
                Message = message,
                ThreadId = Thread.CurrentThread.ManagedThreadId,
            };

            _logQueue.Enqueue(logEntry);
        }

        /// <summary>
        /// Logs a message with the specified log level and exception
        /// </summary>
        public void Log(LogLevel level, string message, Exception exception)
        {
            if (!IsEnabled(level) || string.IsNullOrEmpty(message))
                return;

            var fullMessage =
                exception != null
                    ? $"{message}\nException: {exception.GetType().Name}: {exception.Message}\nStack Trace: {exception.StackTrace}"
                    : message;

            Log(level, fullMessage);
        }

        /// <summary>
        /// Logs a debug message
        /// </summary>
        public void Debug(string message)
        {
            Log(LogLevel.Debug, message);
        }

        /// <summary>
        /// Logs an info message
        /// </summary>
        public void Info(string message)
        {
            Log(LogLevel.Info, message);
        }

        /// <summary>
        /// Logs a warning message
        /// </summary>
        public void Warning(string message)
        {
            Log(LogLevel.Warning, message);
        }

        /// <summary>
        /// Logs an error message
        /// </summary>
        public void Error(string message)
        {
            Log(LogLevel.Error, message);
        }

        /// <summary>
        /// Logs an error message with exception
        /// </summary>
        public void Error(string message, Exception exception)
        {
            Log(LogLevel.Error, message, exception);
        }

        /// <summary>
        /// Logs a fatal error message
        /// </summary>
        public void Fatal(string message)
        {
            Log(LogLevel.Fatal, message);
        }

        /// <summary>
        /// Logs a fatal error message with exception
        /// </summary>
        public void Fatal(string message, Exception exception)
        {
            Log(LogLevel.Fatal, message, exception);
        }

        /// <summary>
        /// Checks if the specified log level is enabled
        /// </summary>
        public bool IsEnabled(LogLevel level)
        {
            return level >= _minLogLevel;
        }

        /// <summary>
        /// Sets the minimum log level
        /// </summary>
        public void SetLogLevel(LogLevel level)
        {
            _minLogLevel = level;
        }

        /// <summary>
        /// Gets the current log file path
        /// </summary>
        public string GetLogFilePath()
        {
            return _filePath;
        }

        /// <summary>
        /// Background task that writes log entries to file
        /// </summary>
        private async Task WriteLogsAsync()
        {
            try
            {
                while (!_cancellationTokenSource.Token.IsCancellationRequested)
                {
                    var hasEntries = false;
                    var stringBuilder = new StringBuilder();

                    // Collect log entries from queue
                    while (_logQueue.TryDequeue(out var logEntry))
                    {
                        hasEntries = true;
                        stringBuilder.AppendLine(FormatLogEntry(logEntry));
                    }

                    // Write to file if we have entries
                    if (hasEntries)
                    {
                        await WriteToFileAsync(stringBuilder.ToString());
                    }

                    // Small delay to prevent excessive CPU usage
                    await Task.Delay(100, _cancellationTokenSource.Token);
                }
            }
            catch (OperationCanceledException)
            {
                // Expected when cancellation is requested
            }
            catch (Exception ex)
            {
                // Log to console as fallback
                Console.WriteLine($"GlobalLogger error: {ex.Message}");
            }
        }

        /// <summary>
        /// Writes the log content to file
        /// </summary>
        private Task WriteToFileAsync(string content)
        {
            try
            {
                lock (_lockObject)
                {
                    File.AppendAllText(_filePath, content, Encoding.UTF8);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to write to log file: {ex.Message}");
            }
            return Task.CompletedTask;
        }

        /// <summary>
        /// Formats a log entry for output
        /// </summary>
        private static string FormatLogEntry(LogEntry entry)
        {
            return $"[{entry.Timestamp:yyyy-MM-dd HH:mm:ss.fff} UTC] [{entry.Level.ToString().ToUpper()}] [Thread-{entry.ThreadId}] {entry.Message}";
        }

        /// <summary>
        /// Shuts down the logger and flushes remaining logs
        /// </summary>
        public static void Shutdown()
        {
            if (_instance.IsValueCreated)
            {
                _instance.Value.Dispose();
            }
        }

        /// <summary>
        /// Disposes the logger and flushes remaining logs
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Protected dispose method
        /// </summary>
        private void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                try
                {
                    // Cancel the writer task
                    _cancellationTokenSource.Cancel();

                    // Wait for the task to complete with a timeout
                    _writerTask.Wait(TimeSpan.FromSeconds(5));
                }
                catch (AggregateException)
                {
                    // Expected when task is cancelled
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error shutting down GlobalLogger: {ex.Message}");
                }
                finally
                {
                    _cancellationTokenSource.Dispose();
                }
            }

            _disposed = true;
        }

        /// <summary>
        /// Finalizer
        /// </summary>
        ~GlobalLogger()
        {
            Dispose(false);
        }

        /// <summary>
        /// Internal structure for log entries
        /// </summary>
        private struct LogEntry
        {
            public DateTime Timestamp;
            public LogLevel Level;
            public string Message;
            public int ThreadId;
        }
    }
}
