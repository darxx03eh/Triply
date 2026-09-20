using Serilog.Context;

namespace Triply.Api.Middlewares;

/// <summary>
/// Gives every request a correlation id (taken from the X-Correlation-Id header or generated),
/// returns it in the response and attaches it to every log written while handling the request.
/// </summary>
public class CorrelationIdMiddleware(RequestDelegate next)
{
    /// <summary>The header name.</summary>
    public const string HeaderName = "X-Correlation-Id";

    /// <summary>Runs the middleware for the current request.</summary>
    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = context.Request.Headers[HeaderName].FirstOrDefault();
        if (string.IsNullOrWhiteSpace(correlationId) || correlationId.Length > 64)
            correlationId = context.TraceIdentifier;

        context.TraceIdentifier = correlationId;
        context.Response.OnStarting(() =>
        {
            context.Response.Headers[HeaderName] = correlationId;
            return Task.CompletedTask;
        });

        using (LogContext.PushProperty("CorrelationId", correlationId))
            await next(context);
    }
}
