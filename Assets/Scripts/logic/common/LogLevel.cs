namespace gamecore.common
{
    /// <summary>
    /// Enumeration of log levels in order of severity
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// Debug level - detailed information for debugging
        /// </summary>
        Debug = 0,

        /// <summary>
        /// Info level - general information about program execution
        /// </summary>
        Info = 1,

        /// <summary>
        /// Warning level - potentially harmful situations
        /// </summary>
        Warning = 2,

        /// <summary>
        /// Error level - error events that might still allow the application to continue
        /// </summary>
        Error = 3,

        /// <summary>
        /// Fatal level - very severe error events that will presumably lead the application to abort
        /// </summary>
        Fatal = 4,
    }
}
