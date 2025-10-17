# Global File Logger

A singleton file logger that can be accessed from anywhere in the application.

## Components

### Core Classes

- **`GlobalLogger`** - Singleton file logger with static access methods
- **`LogLevel`** - Enumeration of log levels (Debug, Info, Warning, Error, Fatal)
- **`IGameLogger`** - Interface (kept for compatibility)

## Usage

### Basic Usage

```csharp
using gamecore.common;

public class MyClass
{
    public void DoSomething()
    {
        // Get the singleton instance
        var logger = GlobalLogger.Instance;
        
        // Log messages using the instance
        logger.Debug("Debug message");
        logger.Info("Info message");
        logger.Warning("Warning message");
        logger.Error("Error message");
        logger.Fatal("Fatal message");

        // Log with exception
        try
        {
            // Some code that might throw
        }
        catch (Exception ex)
        {
            logger.Error("An error occurred", ex);
        }
    }
}
```

### Initialization

```csharp
// Initialize with default settings (logs to "Logs/application.log" with Debug level)
GlobalLogger.Instance.Initialize();

// Or with custom settings
GlobalLogger.Instance.Initialize("Logs/myapp.log", LogLevel.Info);
```

### Configuration

```csharp
var logger = GlobalLogger.Instance;

// Change log level at runtime
logger.SetLogLevel(LogLevel.Warning);

// Get current log file path
string logPath = logger.GetLogFilePath();

// Check if a log level is enabled
if (logger.IsEnabled(LogLevel.Debug))
{
    logger.Debug("This debug message will be logged");
}
```

### Cleanup

```csharp
// Shutdown the logger when your application exits
GlobalLogger.Shutdown();
```

## Features

- **Singleton pattern** - Single instance with lazy initialization
- **Global access** - No need to pass logger instances around
- **Asynchronous file writing** - Uses background threads to prevent blocking
- **Multiple log levels** - Debug, Info, Warning, Error, Fatal
- **Exception logging** - Built-in support for logging exceptions with stack traces
- **Thread-safe** - Safe to use from multiple threads
- **Configurable** - Set log levels and file paths
- **Automatic directory creation** - Creates log directories if they don't exist
- **Platform independent** - Works on any .NET platform
- **Proper disposal** - Implements IDisposable for clean shutdown

## Log Format

Log entries are formatted as:
```
[2024-01-15 14:30:25.123 UTC] [INFO] [Thread-1] Your log message here
```

## Best Practices

1. **Initialize early** - Call `GlobalLogger.Instance.Initialize()` early in your application lifecycle
2. **Use appropriate log levels** - Don't log everything as Debug
3. **Shutdown properly** - Call `GlobalLogger.Shutdown()` when your application exits
4. **Check log levels** - Use `IsEnabled()` before expensive logging operations
5. **Get instance once** - Store `GlobalLogger.Instance` in a field or property for reuse
6. **Thread-safe usage** - The singleton is thread-safe, but initialization should be done on the main thread

### Example with Instance Storage

```csharp
public class MyClass
{
    private readonly GlobalLogger _logger = GlobalLogger.Instance;
    
    public void DoSomething()
    {
        _logger.Info("Using stored instance");
    }
}
```
