using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Triply.Application.Interfaces.Services;

namespace Triply.Infrastructure.Services.Blacklist;

/// <summary>Stores revoked token identifiers in the configured distributed cache.</summary>
public partial class RedisTokenBlacklistService(
    IDistributedCache cache,
    ILogger<RedisTokenBlacklistService> logger) : ITokenBlacklistService
{
    private const string KeyPrefix = "blacklist:token:";
}