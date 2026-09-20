using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace Triply.Infrastructure.Services.Blacklist;

public partial class RedisTokenBlacklistService
{
    /// <inheritdoc />
    public async Task<bool> IsBlacklistedAsync(string jti, CancellationToken cancellationToken = default)
    {
        try
        {
            string? value = await cache.GetStringAsync($"{KeyPrefix}{jti}", cancellationToken);
            if (value is not null)
                logger.LogWarning("Rejected a request with the blacklisted token {Jti}", jti);
            return value is not null;
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            logger.LogError(exception, "Failed to check the token {Jti} in the Redis blacklist", jti);
            throw;
        }
    }
}