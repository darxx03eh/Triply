using System.Text.Json;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using Triply.Infrastructure.Caching;

namespace Triply.Infrastructure.Services.Cache;

public partial class RedisCacheService
{
    /// <summary>Gets the cached value for the given group and key, or creates it using <paramref name="factory"/> if it is not cached.</summary>
    /// <typeparam name="T">The type of the cached value.</typeparam>
    /// <param name="group">The group the key belongs to.</param>
    /// <param name="key">The cache key.</param>
    /// <param name="ttl">The duration the value stays cached before it expires.</param>
    /// <param name="factory">The factory used to create the value when it is not already cached.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public async Task<T> GetOrCreateAsync<T>(string group, string key, TimeSpan ttl,
        Func<CancellationToken, Task<T>> factory,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var versionKey = CacheKeyBuilder.BuildVersionKey(group);
            var version = (long)await _db.ScriptEvaluateAsync(GetOrInitializeVersionScript, [versionKey]);

            var cacheKey = CacheKeyBuilder.Build(group, version, key);
            var cachedValue = await _db.StringGetAsync(cacheKey);
            if (cachedValue.HasValue)
            {
                var cachedResult = JsonSerializer.Deserialize<T>(cachedValue.ToString(), JsonOptions);

                if (cachedResult is not null)
                {
                    logger.LogInformation("Cache hit for key {CacheKey} in group {Group}", cacheKey, group);
                    return cachedResult;
                }

                logger.LogWarning(
                    "Cache entry for key {CacheKey} in group {Group} could not be deserialized to {Type}, recreating",
                    cacheKey, group, typeof(T).Name);
            }
            else logger.LogInformation("Cache miss for key {CacheKey} in group {Group}", cacheKey, group);

            cancellationToken.ThrowIfCancellationRequested();

            var result = await factory(cancellationToken);
            var serializedResult = JsonSerializer.Serialize(result, JsonOptions);
            await _db.StringSetAsync(cacheKey, serializedResult, ttl);

            logger.LogInformation(
                "Cache entry created for key {CacheKey} in group {Group} with TTL {Ttl}", cacheKey, group, ttl);

            return result;
        }
        catch (RedisException exp)
        {
            logger.LogWarning(exp,
                "Redis error while getting or creating cache for group {Group} and key {Key}, " +
                "falling back to factory", group, key);

            cancellationToken.ThrowIfCancellationRequested();
            return await factory(cancellationToken);
        }
    }
}
