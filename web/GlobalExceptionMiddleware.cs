using System.Diagnostics;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace web;

public sealed class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleException(context, ex);
        }
    }

    private async Task HandleException(HttpContext context, Exception ex)
    {
        var traceId = Activity.Current?.Id ?? context.TraceIdentifier;

        // 🔥 Structured logging (best practice)
        _logger.LogError(ex,
            """
            🚨 Unhandled Exception
            TraceId   : {TraceId}
            Request   : {Method} {Path}
            Error     : {Message}
            StackTrace : {StackTrace}
            """,
            traceId,
            context.Request.Method,
            context.Request.Path,
            ex.Message,
            ex.StackTrace);

        // 🎯 Default response
        var status = StatusCodes.Status500InternalServerError;
        var title = "An unexpected error occurred.";
        var detail = "Please contact support with the provided traceId.";

        // 🧠 Handle custom exceptions
        // if (ex is AppException appEx)
        // {
        //     status = appEx.StatusCode;
        //     title = appEx.Message;
        //     detail = "A handled application error occurred.";
        // }

        var problem = new ProblemDetails
        {
            Status = status,
            Title = title,
            Detail = detail,
            Instance = context.Request.Path
        };

        // ✅ Add traceId to response
        problem.Extensions["traceId"] = traceId;

        // ✅ Optional but very useful
        context.Response.Headers["X-Trace-Id"] = traceId;

        await context.Response.WriteAsync(
            JsonSerializer.Serialize(problem));
    }
}