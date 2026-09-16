using System.Collections.Concurrent;
using System.Text.Json;
using MessageQueue.IRabbitMQ;
using MessageQueue.Options;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace MessageQueue.RabbitMQ;

public class RabbitMqPublisher(IOptions<RabbitMqOptions> options) : IMessagePublisher
{
    private readonly RabbitMqOptions _options
        = options.Value ?? throw new ArgumentNullException(nameof(options));
    private readonly SemaphoreSlim _initLock = new(1, 1);
    private readonly ConcurrentDictionary<string, byte> _declaredExchanges = new();
    private IConnection? _connection;
    private IChannel? _channel;

    public async Task PublishAsync<T>(string topic, T message, string? exchange = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(topic))
            throw new ArgumentException("Topic must not be null or empty.", nameof(topic));

        string targetExchange = string.IsNullOrWhiteSpace(exchange) ? _options.ExchangeName : exchange;

        var channel = await GetOrCreateChannelAsync(cancellationToken).ConfigureAwait(false);
        await EnsureExchangeDeclaredAsync(channel, targetExchange, cancellationToken).ConfigureAwait(false);

        byte[] payload = JsonSerializer.SerializeToUtf8Bytes(message);

        var properties = new BasicProperties
        {
            ContentType = "application/json",
            DeliveryMode = DeliveryModes.Persistent,
            Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds())
        };

        await channel.BasicPublishAsync(
            exchange: targetExchange,
            routingKey: topic,
            mandatory: false,
            basicProperties: properties,
            body: payload,
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    private async Task EnsureExchangeDeclaredAsync(IChannel channel, string exchangeName,
        CancellationToken cancellationToken)
    {
        if (_declaredExchanges.ContainsKey(exchangeName))
            return;

        await channel.ExchangeDeclareAsync(exchangeName, ExchangeType.Topic, _options.DurableExchange,
            autoDelete: false, cancellationToken: cancellationToken).ConfigureAwait(false);

        _declaredExchanges.TryAdd(exchangeName, 0);
    }

    private async Task<IChannel> GetOrCreateChannelAsync(CancellationToken cancellationToken)
    {
        if (_channel is { IsOpen: true })
            return _channel;

        await _initLock.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            if (_channel is { IsOpen: true })
                return _channel;

            var factory = new ConnectionFactory
            {
                HostName = _options.HostName,
                UserName = _options.UserName,
                Password = _options.Password,
                Port = _options.Port,
                VirtualHost = _options.VirtualHost,
                AutomaticRecoveryEnabled = true,
                TopologyRecoveryEnabled = true
            };

            _connection = await factory.CreateConnectionAsync(cancellationToken).ConfigureAwait(false);
            _channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            return _channel;
        }
        finally
        {
            _initLock.Release();
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_channel is not null)
            await _channel.DisposeAsync().ConfigureAwait(false);
        if (_connection is not null)
            await _connection.DisposeAsync().ConfigureAwait(false);
        _initLock.Dispose();
    }
}