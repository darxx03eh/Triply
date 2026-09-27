using Microsoft.Extensions.Logging;

namespace Triply.Infrastructure.Services.RateLimit.General;

public partial class RedisRateLimitService
{
    /// <summary>Resets the request count for the given key operation.</summary>
    public async Task ResetAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            var deleted = await _db.KeyDeleteAsync(key);

            if (deleted) logger.LogInformation("Rate limit count reset for key {Key}", key);
            else
                logger.LogInformation("Rate limit reset requested for key {Key}, but no existing entry was found", key);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to reset rate limit count for key {Key}", key);
            throw;
        }
    }
}