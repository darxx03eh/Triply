using System.Net;
using Serilog;
using Triply.Api.Common;
using Triply.Api.ResponseHelper;
using Triply.Application.Interfaces.Services;

namespace Triply.Api.Middlewares;

/// <summary>Middleware that enforces rate limiting on endpoints decorated with <see cref="RateLimitMetadata"/>.</summary>
public class RateLimitMiddleware(
    RequestDelegate next,
    ILogger<RateLimitMiddleware> logger,
    IDiagnosticContext diagnostic
)
{
    /// <summary>The maximum number of requests a user can make within the window.</summary>
    private const int MaxAttempts = 10;

    /// <summary>Runs the middleware for the current request.</summary>
    /// <param name="context">The current HTTP context.</param>
    /// <param name="rateLimitService">The service used to track and increment request counts.</param>
    public async Task InvokeAsync(
        HttpContext context, IRateLimitService rateLimitService)
    {
        var endpoint = context.GetEndpoint();
        var metadata = endpoint?.Metadata.GetMetadata<RateLimitMetadata>();
        if (metadata is null)
        {
            await next(context);
            return;
        }

        var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        var key = $"rate-limit:{metadata.Name}:{ip}";

        var count = await rateLimitService.IncrementAsync(key, metadata.Window, context.RequestAborted);
        logger.LogInformation(
            "Rate limit count for policy {Policy} and IP {Ip} is {Count}",
            metadata.Name, ip, count);

        if (count > metadata.PermitLimit)
        {
            logger.LogWarning(
                "Rate limit exceeded for policy {Policy} and IP {Ip}, request count {Count} > limit {PermitLimit}",
                metadata.Name, ip, count, metadata.PermitLimit);

            diagnostic.Set("ResultCode", metadata.Response.Code);
            context.Response.Headers.RetryAfter = ((int)metadata.Window.TotalMilliseconds).ToString();
            await ResponseHandler.WriteJsonResponse(context, HttpStatusCode.TooManyRequests,
                metadata.Response.Message,
                metadata.Response.Code);
            return;
        }

        await next(context);
    }
}