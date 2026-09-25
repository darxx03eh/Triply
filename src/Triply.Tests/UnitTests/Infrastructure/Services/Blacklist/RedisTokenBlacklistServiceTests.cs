using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Triply.Infrastructure.Services.Blacklist;

namespace Triply.Tests.UnitTests.Infrastructure.Services.Blacklist;

public class RedisTokenBlacklistServiceTests
{
    private readonly MemoryDistributedCache _cache = new(Options.Create(new MemoryDistributedCacheOptions()));
    private readonly RedisTokenBlacklistService _service;

    public RedisTokenBlacklistServiceTests() => _service = new RedisTokenBlacklistService(_cache,
        NullLogger<RedisTokenBlacklistService>.Instance);

    [Fact]
    public async Task BlacklistTokenAsync_FutureExpiry_MakesTokenBlacklisted()
    {
        await _service.BlacklistTokenAsync("jti-1", DateTime.UtcNow.AddMinutes(10));

        Assert.True(await _service.IsBlacklistedAsync("jti-1"));
    }

    [Fact]
    public async Task BlacklistTokenAsync_UsesPrefixedKey()
    {
        await _service.BlacklistTokenAsync("jti-2", DateTime.UtcNow.AddMinutes(10));

        Assert.Equal("1", await _cache.GetStringAsync("blacklist:token:jti-2"));
    }

    [Fact]
    public async Task BlacklistTokenAsync_AlreadyExpired_IsIgnored()
    {
        await _service.BlacklistTokenAsync("jti-3", DateTime.UtcNow.AddSeconds(-1));

        Assert.False(await _service.IsBlacklistedAsync("jti-3"));
    }

    [Fact]
    public async Task IsBlacklistedAsync_UnknownToken_ReturnsFalse()
    {
        Assert.False(await _service.IsBlacklistedAsync("never-seen"));
    }

    [Fact]
    public async Task BlacklistTokenAsync_EntryExpiresWithTheToken()
    {
        var cache = new Mock<IDistributedCache>();
        DistributedCacheEntryOptions? options = null;
        cache.Setup(c => c.SetAsync(It.IsAny<string>(), 
                It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>()))
            .Callback<string, byte[], 
                DistributedCacheEntryOptions, CancellationToken>((_, _, o, _) => options = o);

        await new RedisTokenBlacklistService(cache.Object, NullLogger<RedisTokenBlacklistService>.Instance)
            .BlacklistTokenAsync("jti", DateTime.UtcNow.AddMinutes(10));

        Assert.InRange(options!.AbsoluteExpirationRelativeToNow!.Value, TimeSpan.FromMinutes(9.9), 
            TimeSpan.FromMinutes(10));
    }
}
