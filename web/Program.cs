using web;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddLoggerExtension(builder.Configuration);

var app = builder.Build();

app.UseHttpsRedirection();

app.MapGet("/", (ILogger<Program> logger) =>
{
    logger.LogInformation("Hello World!");
    logger.LogTrace("Hello World!");
    logger.LogCritical("Hello World!");
    return "Hello World!";
});

await app.RunAsync();