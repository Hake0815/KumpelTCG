using System;

namespace gamecore.common
{
    /// <summary>
    /// Interface for logging functionality
    /// </summary>
    public interface IGameLogger
    {
        /// <summary>
        /// Logs a message with the specified log level
        /// </summary>
        /// <param name="level">The log level</param>
        /// <param name="message">The message to log</param>
        void Log(LogLevel level, string message);

        /// <summary>
        /// Logs a message with the specified log level and exception
        /// </summary>
        /// <param name="level">The log level</param>
        /// <param name="message">The message to log</param>
        /// <param name="exception">The exception to log</param>
        void Log(LogLevel level, string message, Exception exception);

        /// <summary>
        /// Logs a debug message
        /// </summary>
        /// <param name="message">The message to log</param>
        void Debug(string message);

        /// <summary>
        /// Logs an info message
        /// </summary>
        /// <param name="message">The message to log</param>
        void Info(string message);

        /// <summary>
        /// Logs a warning message
        /// </summary>
        /// <param name="message">The message to log</param>
        void Warning(string message);

        /// <summary>
        /// Logs an error message
        /// </summary>
        /// <param name="message">The message to log</param>
        void Error(string message);

        /// <summary>
        /// Logs an error message with exception
        /// </summary>
        /// <param name="message">The message to log</param>
        /// <param name="exception">The exception to log</param>
        void Error(string message, Exception exception);

        /// <summary>
        /// Logs a fatal error message
        /// </summary>
        /// <param name="message">The message to log</param>
        void Fatal(string message);

        /// <summary>
        /// Logs a fatal error message with exception
        /// </summary>
        /// <param name="message">The message to log</param>
        /// <param name="exception">The exception to log</param>
        void Fatal(string message, Exception exception);

        /// <summary>
        /// Checks if the specified log level is enabled
        /// </summary>
        /// <param name="level">The log level to check</param>
        /// <returns>True if the level is enabled, false otherwise</returns>
        bool IsEnabled(LogLevel level);
    }
}
