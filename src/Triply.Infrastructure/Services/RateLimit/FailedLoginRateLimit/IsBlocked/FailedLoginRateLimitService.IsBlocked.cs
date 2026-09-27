using Microsoft.Extensions.Logging;

namespace Triply.Infrastructure.Services.RateLimit.FailedLoginRateLimit;

public partial class FailedLoginRateLimitService
{
    /// <summary>Determines whether the given IP address and user are currently blocked due to failed login attempts operation.</summary>
    public async Task<bool> IsBlockedAsync(string ip, string userId, CancellationToken cancellationToken = default)
    {
        var key = GetKey(ip, userId);

        try
        {
            var count = await rateLimitService.GetCountAsync(key);
            var isBlocked = count > MaxAttempts;

            if (isBlocked)
                logger.LogWarning(
                    "IP {Ip} and user {UserId} are blocked due to failed login attempts, count {Count} >= max {MaxAttempts}",
                    ip, userId, count, MaxAttempts);
            else
                logger.LogInformation(
                    "IP {Ip} and user {UserId} are not blocked, count {Count} < max {MaxAttempts}",
                    ip, userId, count, MaxAttempts);

            return isBlocked;
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex,
                "Failed to check blocked status for IP {Ip} and user {UserId}", ip, userId);
            throw;
        }
    }
}