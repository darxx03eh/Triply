using Microsoft.Extensions.Caching.Distributed;

namespace Triply.Infrastructure.Services.Blacklist;

public partial class RedisTokenBlacklistService
{
    /// <inheritdoc />
    public async Task BlacklistTokenAsync(string jti, DateTime expiryUtc, CancellationToken cancellationToken = default)
    {
        var remaining = expiryUtc - DateTime.UtcNow;

        if (remaining <= TimeSpan.Zero)
            return;

        await cache.SetStringAsync(
            $"{KeyPrefix}{jti}", "1",
            new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = remaining },
            cancellationToken);
    }
}