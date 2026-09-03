using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using VerticalSlice.Domain;

namespace VerticalSlice.Api.Middleware;

internal sealed class ApiExceptionHandler(
    ILogger<ApiExceptionHandler> logger,
    IProblemDetailsService problemDetailsService) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (statusCode, title, detail) = exception switch
        {
            ArgumentException => (
                StatusCodes.Status400BadRequest,
                "Bad request",
                "One or more request values are invalid."),
            KeyNotFoundException => (
                StatusCodes.Status404NotFound,
                "Not found",
                "The requested resource was not found."),
            ConflictException => (
                StatusCodes.Status409Conflict,
                "Conflict",
                "The request conflicts with the current resource state."),
            _ => (
                StatusCodes.Status500InternalServerError,
                "Internal server error",
                "An unexpected error occurred while processing the request.")
        };

        if (statusCode >= StatusCodes.Status500InternalServerError)
        {
            logger.LogError(exception, "Unhandled API exception. TraceId={TraceId}", httpContext.TraceIdentifier);
        }
        else
        {
            logger.LogWarning(exception, "Handled API exception. TraceId={TraceId}", httpContext.TraceIdentifier);
        }

        httpContext.Response.StatusCode = statusCode;

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Instance = httpContext.Request.Path
        };
        problemDetails.Extensions["traceId"] = httpContext.TraceIdentifier;

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = problemDetails
        });
    }
}