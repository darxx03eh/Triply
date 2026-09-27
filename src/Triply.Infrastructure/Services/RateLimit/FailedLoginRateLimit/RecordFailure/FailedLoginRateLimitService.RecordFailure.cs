using Microsoft.Extensions.Logging;

namespace Triply.Infrastructure.Services.RateLimit.FailedLoginRateLimit;

public partial class FailedLoginRateLimitService
{
    /// <summary>Records a failed login attempt for the given IP address and user, and determines whether this causes them to become blocked operation.</summary>
    public async Task<long> RecordFailureAsync(string ip, string userId, CancellationToken cancellationToken = default)
    {
        var key = GetKey(ip, userId);

        try
        {
            var count = await rateLimitService.IncrementAsync(key, Window, cancellationToken);

            logger.LogInformation(
                "Recorded failed login attempt for IP {Ip} and user {UserId}, count is now {Count}",
                ip, userId, count);

            return count;
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex,
                "Failed to record failed login attempt for IP {Ip} and user {UserId}", ip, userId);
            throw;
        }
    }
}