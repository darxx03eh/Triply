using Microsoft.Extensions.Logging;

namespace Triply.Infrastructure.Services.RateLimit.FailedLoginRateLimit;

public partial class FailedLoginRateLimitService
{
    /// <summary>Resets the recorded failed login attempts for the given IP address and user operation.</summary>
    public async Task ResetAsync(string ip, string userId, CancellationToken cancellationToken = default)
    {
        var key = GetKey(ip, userId);

        try
        {
            await rateLimitService.ResetAsync(key, cancellationToken);
            logger.LogInformation(
                "Failed login attempts reset for IP {Ip} and user {UserId}", ip, userId);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex,
                "Failed to reset failed login attempts for IP {Ip} and user {UserId}", ip, userId);
            throw;
        }
    }
}