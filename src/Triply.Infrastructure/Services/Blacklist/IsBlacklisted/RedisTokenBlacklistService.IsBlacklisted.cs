using Microsoft.Extensions.Caching.Distributed;

namespace Triply.Infrastructure.Services.Blacklist;

public partial class RedisTokenBlacklistService
{
    /// <inheritdoc />
    public async Task<bool> IsBlacklistedAsync(string jti, CancellationToken cancellationToken = default)
    {
        string? value = await cache.GetStringAsync($"{KeyPrefix}{jti}", cancellationToken);
        return value is not null;
    }
}