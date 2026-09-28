using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using Triply.Infrastructure.Caching;

namespace Triply.Infrastructure.Services.Cache;

public partial class RedisCacheService
{
    /// <summary>Invalidates all cached values belonging to the given group.</summary>
    /// <param name="group">The group whose cached values should be invalidated.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public async Task InvalidateGroupAsync(string group, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        try
        {
            var versionKey = CacheKeyBuilder.BuildVersionKey(group);

            var newVersion = (long)await _db.ScriptEvaluateAsync(IncrementVersionScript, [versionKey]);

            logger.LogInformation(
                "Cache group {Group} invalidated, new version is {Version}", group, newVersion);
        }
        catch (RedisException exp)
        {
            logger.LogWarning(exp, "Failed to invalidate cache group {Group}", group);
            throw;
        }
    }
}
