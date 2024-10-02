namespace web.Logging;

public class FileLoggerProvider : ILoggerProvider
{
    private readonly IConfiguration _configuration;
    private readonly Func<string, bool> filter;

    public FileLoggerProvider(IConfiguration configuration, Func<string, bool> filter)
    {
        _configuration = configuration;
        this.filter = filter;
    }

    public ILogger CreateLogger(string categoryName)
    {
        return new FileLogger(_configuration, filter, categoryName);
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }
}

