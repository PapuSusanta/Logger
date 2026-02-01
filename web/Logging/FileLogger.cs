using System.Text;

namespace web.Logging;

partial class FileLogger : ILogger
{
    private readonly string _logPath;
    private readonly LogLevel logLevel;
    private readonly Func<string, bool> _filter;
    public readonly string _categoryName;

    public string CurrentPath { get; private set; } = string.Empty;

    private SemaphoreSlim semaphore = new(1, 1);

    public FileLogger(IConfiguration configuration, Func<string, bool> filter, string categoryName)
    {
        _logPath = configuration["Logging:File:LogPath"]!;
        string logLevelStr = configuration["Logging:LogLevel:Default"]!;
        _ = Enum.TryParse<LogLevel>(logLevelStr, out logLevel);
        _filter = filter;
        _categoryName = categoryName;

        LogFileCreate();
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        return default;
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        if (logLevel < this.logLevel)
        {
            return false;
        }
        return _filter(_categoryName);
    }

    public async void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel))
        {
            return;
        }
        string message = formatter(state, exception);

        message = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{logLevel.ToString().ToUpper()}]-{message}{Environment.NewLine}";

        await WriteLogToFile(message);
    }

    private async Task WriteLogToFile(string message)
    {
        try
        {
            await semaphore.WaitAsync();
            var currentDate = $"{DateTime.Now:yyyy-MM-dd}";
            var fileName = Path.GetFileNameWithoutExtension(CurrentPath);
            var parts = fileName.Split(".");

            if (parts[0] != currentDate)
            {
                LogFileCreate();
            }

            using var stream = new FileStream(
                CurrentPath,
                FileMode.Append,
                FileAccess.Write,
                FileShare.None,
                bufferSize: 4096,
                useAsync: true);
            await stream.WriteAsync(Encoding.UTF8.GetBytes(message));

        }
        finally
        {
            semaphore.Release();
        }
    }

    private void LogFileCreate()
    {
        string directory = Path.GetDirectoryName(_logPath)!;
        string fileName = $"{DateTime.Now:yyyy-MM-dd}.log.txt";
        var logPath = Path.Combine(directory, fileName);
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
        CurrentPath = logPath;
    }
}