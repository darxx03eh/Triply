using System.Text.Json;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using Triply.Application.Interfaces.Services;

namespace Triply.Infrastructure.Services.Cache;

/// <summary>Provides caching operations backed by Redis.</summary>
public partial class RedisCacheService(
    IConnectionMultiplexer redis,
    ILogger<RedisCacheService> logger
    ) : ICacheService
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private const string GetOrInitializeVersionScript = """
        local version = redis.call('GET', KEYS[1])
        if not version then
            redis.call('SET', KEYS[1], 1)
            return 1
        end
        return tonumber(version)
        """;

    private const string IncrementVersionScript = """
        local version = redis.call('GET', KEYS[1])
        if not version then
            redis.call('SET', KEYS[1], 2)
            return 2
        end
        return redis.call('INCR', KEYS[1])
        """;

    private readonly IDatabase _db = redis.GetDatabase();
}
