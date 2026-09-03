using System.Diagnostics;

namespace VerticalSlice.Api.Logging;

internal sealed partial class StructuredRequestLoggingMiddleware(
    RequestDelegate next,
    ILogger<StructuredRequestLoggingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var startedAt = Stopwatch.GetTimestamp();
        using var scope = logger.BeginScope(new Dictionary<string, object>
        {
            ["TraceId"] = context.TraceIdentifier
        });

        try
        {
            await next(context);
        }
        finally
        {
            LogRequestCompleted(
                logger,
                context.Request.Method,
                context.Request.Path,
                context.Response.StatusCode,
                Stopwatch.GetElapsedTime(startedAt).TotalMilliseconds);
        }
    }

    [LoggerMessage(
        EventId = 1000,
        Level = LogLevel.Information,
        Message = "HTTP {Method} {Path} responded {StatusCode} in {ElapsedMilliseconds:F2} ms")]
    private static partial void LogRequestCompleted(
        ILogger logger,
        string method,
        string path,
        int statusCode,
        double elapsedMilliseconds);
}