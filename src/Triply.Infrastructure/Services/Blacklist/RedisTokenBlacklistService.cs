using Microsoft.Extensions.Caching.Distributed;
using Triply.Application.Interfaces.Services;

namespace Triply.Infrastructure.Services.Blacklist;

/// <summary>Stores revoked token identifiers in the configured distributed cache.</summary>
public partial class RedisTokenBlacklistService(IDistributedCache cache) : ITokenBlacklistService
{
    private const string KeyPrefix = "blacklist:token:";
}