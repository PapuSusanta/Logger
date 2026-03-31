namespace web.Logging;

public class FileLoggerProvider(IConfiguration configuration, Func<string, bool> filter) : ILoggerProvider
{
    private readonly IConfiguration _configuration = configuration;
    private readonly Func<string, bool> filter = filter;

    public ILogger CreateLogger(string categoryName)
    {
        return new FileLogger(_configuration, filter, categoryName);
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }
}

