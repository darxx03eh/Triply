using Microsoft.Extensions.Logging;

namespace Triply.Infrastructure.Services.RateLimit.General;

public partial class RedisRateLimitService
{
    /// <summary>Gets the current request count for the given key operation.</summary>
    public async Task<long> GetCountAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            var value = await _db.StringGetAsync(key);
            if (!value.HasValue)
            {
                logger.LogInformation("Rate limit count for key {Key} not found, defaulting to 0", key);
                return 0;
            }

            var count = (long)value;
            logger.LogInformation("Rate limit count for key {Key} is {Count}", key, count);
            return count;
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to get rate limit count for key {Key}", key);
            throw;
        }
    }
}