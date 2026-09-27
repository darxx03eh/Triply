using Microsoft.Extensions.Logging;

namespace Triply.Infrastructure.Services.RateLimit.General;

public partial class RedisRateLimitService
{
    /// <summary>Increments the request count for the given key within the specified time window operation.</summary>
    public async Task<long> IncrementAsync(string key, TimeSpan window, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _db.ScriptEvaluateAsync(IncrementScript, [key], [(long)window.TotalMilliseconds]);
            var count = (long)result;

            logger.LogInformation(
                "Incremented rate limit count for key {Key} to {Count} within window {WindowMs}ms",
                key, count, window.TotalMilliseconds);

            return count;
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex,
                "Failed to increment rate limit count for key {Key} within window {WindowMs}ms",
                key, window.TotalMilliseconds);
            throw;
        }
    }
}