using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace Triply.Infrastructure.Services.Blacklist;

public partial class RedisTokenBlacklistService
{
    /// <inheritdoc />
    public async Task BlacklistTokenAsync(string jti, DateTime expiryUtc,
        CancellationToken cancellationToken = default)
    {
        var remaining = expiryUtc - DateTime.UtcNow;

        if (remaining <= TimeSpan.Zero)
        {
            logger.LogDebug("Token {Jti} already expired, no need to blacklist it", jti);
            return;
        }

        try
        {
            await cache.SetStringAsync(
                $"{KeyPrefix}{jti}", "1",
                new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = remaining },
                cancellationToken);
            logger.LogDebug("Token {Jti} blacklisted in Redis for {Remaining}", jti, remaining);
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            logger.LogError(exception, "Failed to blacklist token {Jti} in Redis", jti);
            throw;
        }
    }
}