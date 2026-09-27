using Microsoft.Extensions.Logging;
using Triply.Application.Interfaces.Services;

namespace Triply.Infrastructure.Services.RateLimit.FailedLoginRateLimit;

/// <summary>Provides rate limiting operations for failed login requests.</summary>
public partial class FailedLoginRateLimitService(
    IRateLimitService rateLimitService,
    ILogger<FailedLoginRateLimitService> logger
) : IFailedLoginRateLimitService
{
    /// <summary>The time window during which failed login attempts are counted.</summary>
    private static readonly TimeSpan Window = TimeSpan.FromMinutes(1);

    /// <summary>The maximum number of failed login attempts allowed within the window before being blocked.</summary>
    private const int MaxAttempts = 3;

    /// <summary>Builds the rate limit key for the given IP address and user.</summary>
    private string GetKey(string ip, string userId)
        => $"login-failures:{ip}:{userId}";
}