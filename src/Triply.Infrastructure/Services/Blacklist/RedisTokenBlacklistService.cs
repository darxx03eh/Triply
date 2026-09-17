using Microsoft.Extensions.Caching.Distributed;
using Triply.Application.Interfaces.Services;

namespace Triply.Infrastructure.Services.Blacklist;

public partial class RedisTokenBlacklistService(IDistributedCache cache) : ITokenBlacklistService
{
    private const string KeyPrefix = "blacklist:token:";
}