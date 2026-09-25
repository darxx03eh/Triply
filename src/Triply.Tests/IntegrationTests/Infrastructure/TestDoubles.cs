using MessageQueue.IRabbitMQ;
using Triply.Application.Interfaces.Services;

namespace Triply.Tests.IntegrationTests.Infrastructure;

/// <summary>In-memory token blacklist used to keep authentication tests independent of Redis.</summary>
public sealed class InMemoryTokenBlacklistService : ITokenBlacklistService
{
    private readonly HashSet<string> _blacklistedTokens = [];

    public Task BlacklistTokenAsync(string jti, DateTime expiryUtc, CancellationToken cancellationToken = default)
    {
        _blacklistedTokens.Add(jti);
        return Task.CompletedTask;
    }

    public Task<bool> IsBlacklistedAsync(string jti, CancellationToken cancellationToken = default)
        => Task.FromResult(_blacklistedTokens.Contains(jti));
}

/// <summary>Captures published messages without opening a RabbitMQ connection.</summary>
public sealed class InMemoryMessagePublisher : IMessagePublisher
{
    public List<(string Topic, object Message)> PublishedMessages { get; } = [];

    public Task PublishAsync<T>(string topic, T message, string? exchange = null,
        CancellationToken cancellationToken = default)
    {
        PublishedMessages.Add((topic, message!));
        return Task.CompletedTask;
    }

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;
}
