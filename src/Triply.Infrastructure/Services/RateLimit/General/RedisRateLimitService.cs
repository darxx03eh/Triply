using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using Triply.Application.Interfaces.Services;

namespace Triply.Infrastructure.Services.RateLimit.General;

/// <summary>Handles the rate limiting side.</summary>
public partial class RedisRateLimitService(
    IConnectionMultiplexer redis,
    ILogger<RedisRateLimitService> logger) : IRateLimitService
{
    private readonly IDatabase _db = redis.GetDatabase();

    private const string IncrementScript = """
                                           local count = redis.call('INCR', KEYS[1])
                                           if count == 1 
                                           then redis.call('PEXPIRE', KEYS[1], ARGV[1])
                                           end
                                           return count
                                           """;
}