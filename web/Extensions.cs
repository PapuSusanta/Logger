using web.Logging;

namespace web;

public static class Extensions
{
    public static IServiceCollection AddLoggerExtension(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddLogging(logBuilder =>
        {
            logBuilder.ClearProviders();
            logBuilder.AddConsole();
            logBuilder.AddProvider(new FileLoggerProvider(configuration, category => true));

        });
        return services;
    }
}