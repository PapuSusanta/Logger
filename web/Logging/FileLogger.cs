using System.Text;

namespace web.Logging;

partial class FileLogger : ILogger
{
    private readonly LogLevel _logLevel;
    private readonly Func<string, bool> _filter;
    public readonly string CategoryName;

    private readonly SemaphoreSlim _semaphore = new(1, 1);

    public FileLogger(IConfiguration configuration, Func<string, bool> filter, string categoryName)
    {
        string logLevelStr = configuration["Logging:LogLevel:Default"]!;
        _ = Enum.TryParse<LogLevel>(logLevelStr, out _logLevel);
        _filter = filter;
        CategoryName = categoryName;

        LogFileCreate();
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        return default;
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        if (logLevel < this._logLevel)
        {
            return false;
        }
        return _filter(CategoryName);
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state,
        Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel))
        {
            return;
        }
        string message = formatter(state, exception);

        message = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{logLevel.ToString().ToUpper()}]-{message}{Environment.NewLine}";

        WriteLogToFile(message).GetAwaiter().GetResult();
    }

    private async Task WriteLogToFile(string message)
    {
        try
        {
            await _semaphore.WaitAsync();

            var currentPath = LogFileCreate();
            var currentDate = $"{DateTime.Now:yyyy-MM-dd}";
            var fileName = Path.GetFileNameWithoutExtension(currentPath);
            var parts = fileName.Split(".");

            if (parts[0] != currentDate)
            {
                LogFileCreate();
            }

            using var stream = new FileStream(
                currentPath,
                FileMode.Append,
                FileAccess.Write,
                FileShare.None,
                bufferSize: 4096,
                useAsync: true);

            await stream.WriteAsync(Encoding.UTF8.GetBytes(message));

        }
        finally
        {
            _semaphore.Release();
        }
    }

    private static string LogFileCreate()
    {
        var path = Path.Combine(Directory.GetCurrentDirectory(), "logs");
        string fileName = $"{DateTime.Now:yyyy-MM-dd}.log.txt";
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        return Path.Combine(path, fileName);
    }
}