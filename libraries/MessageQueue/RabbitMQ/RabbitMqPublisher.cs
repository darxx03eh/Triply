using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.Collections.Concurrent;
using System.Text.Json;
using MessageQueue.IRabbitMQ;
using MessageQueue.Options;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace MessageQueue.RabbitMQ;

/// <summary>
/// RabbitMQ implementation of <see cref="IMessagePublisher"/>.
/// Lazily creates and reuses a single connection/channel across publishes,
/// and declares each target exchange only once per process lifetime (cached
/// in <see cref="_declaredExchanges"/>) to avoid redundant round-trips.
/// </summary>
public class RabbitMqPublisher(IOptions<RabbitMqOptions> options, ILogger<RabbitMqPublisher>? logger = null)
    : IMessagePublisher
{
    private readonly ILogger _logger = logger ?? NullLogger<RabbitMqPublisher>.Instance;

    /// <summary>Resolved RabbitMQ configuration options.</summary>
    private readonly RabbitMqOptions _options
        = options.Value ?? throw new ArgumentNullException(nameof(options));

    /// <summary>Guards lazy connection/channel creation so only one caller initializes them at a time.</summary>
    private readonly SemaphoreSlim _initLock = new(1, 1);

    /// <summary>
    /// Tracks which exchanges have already been declared on the current channel,
    /// so <see cref="EnsureExchangeDeclaredAsync"/> only declares each one once.
    /// The byte value is unused; this is effectively a thread-safe set.
    /// </summary>
    private readonly ConcurrentDictionary<string, byte> _declaredExchanges = new();

    /// <summary>Underlying RabbitMQ connection. Created lazily on first publish.</summary>
    private IConnection? _connection;

    /// <summary>Underlying RabbitMQ channel used for declarations and publishing. Created lazily.</summary>
    private IChannel? _channel;

    /// <summary>
    /// Serializes <paramref name="message"/> to JSON and publishes it to the given
    /// (or default) exchange using <paramref name="topic"/> as the routing key.
    /// </summary>
    /// <typeparam name="T">The message payload type, serialized as JSON.</typeparam>
    /// <param name="topic">Routing key used to route the message (e.g. "order.created").</param>
    /// <param name="message">The message payload to serialize and publish.</param>
    /// <param name="exchange">
    /// Optional exchange name to publish to. If null/empty, falls back to
    /// <see cref="RabbitMqOptions.ExchangeName"/>.
    /// </param>
    /// <param name="cancellationToken">Token used to cancel connection setup and publishing.</param>
    /// <exception cref="ArgumentException">Thrown if <paramref name="topic"/> is null/empty/whitespace.</exception>
    public async Task PublishAsync<T>(string topic, T message, string? exchange = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(topic))
            throw new ArgumentException("Topic must not be null or empty.", nameof(topic));

        string targetExchange = string.IsNullOrWhiteSpace(exchange) ? _options.ExchangeName : exchange;

        var channel = await GetOrCreateChannelAsync(cancellationToken).ConfigureAwait(false);
        await EnsureExchangeDeclaredAsync(channel, targetExchange, cancellationToken).ConfigureAwait(false);

        byte[] payload = JsonSerializer.SerializeToUtf8Bytes(message);

        // Persistent delivery mode so the broker keeps the message on disk
        // (paired with a durable queue) in case of a broker restart.
        var properties = new BasicProperties
        {
            ContentType = "application/json",
            DeliveryMode = DeliveryModes.Persistent,
            Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds())
        };

        try
        {
            await channel.BasicPublishAsync(
                exchange: targetExchange,
                routingKey: topic,
                mandatory: false,
                basicProperties: properties,
                body: payload,
                cancellationToken: cancellationToken).ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Failed to publish {MessageType} to exchange {Exchange} " +
                                        "with routing key {RoutingKey}",
                typeof(T).Name, targetExchange, topic);
            throw;
        }

        _logger.LogInformation("Published {MessageType} to exchange {Exchange} with routing key " +
                               "{RoutingKey} ({PayloadSize} bytes)",
            typeof(T).Name, targetExchange, topic, payload.Length);
    }

    /// <summary>
    /// Declares <paramref name="exchangeName"/> as a durable/topic exchange if it
    /// hasn't already been declared by this instance, caching the result to skip
    /// redundant declarations on subsequent publishes.
    /// </summary>
    /// <param name="channel">The channel to declare the exchange on.</param>
    /// <param name="exchangeName">The exchange name to ensure exists.</param>
    /// <param name="cancellationToken">Token used to cancel the declare call.</param>
    private async Task EnsureExchangeDeclaredAsync(IChannel channel, string exchangeName,
        CancellationToken cancellationToken)
    {
        if (_declaredExchanges.ContainsKey(exchangeName))
            return;

        await channel.ExchangeDeclareAsync(exchangeName, ExchangeType.Topic, _options.DurableExchange,
            autoDelete: false, cancellationToken: cancellationToken).ConfigureAwait(false);

        _declaredExchanges.TryAdd(exchangeName, 0);
        _logger.LogDebug("Exchange {Exchange} declared", exchangeName);
    }

    /// <summary>
    /// Returns the existing open channel, or lazily creates a new connection/channel
    /// (double-checked under <see cref="_initLock"/> to avoid duplicate connections
    /// under concurrent calls).
    /// </summary>
    /// <param name="cancellationToken">Token used to cancel connection/channel creation.</param>
    /// <returns>An open <see cref="IChannel"/> ready for use.</returns>
    private async Task<IChannel> GetOrCreateChannelAsync(CancellationToken cancellationToken)
    {
        if (_channel is { IsOpen: true })
            return _channel;

        await _initLock.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            // Re-check after acquiring the lock: another caller may have
            // already created the channel while we were waiting.
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

            try
            {
                _connection = await factory.CreateConnectionAsync(cancellationToken).ConfigureAwait(false);
                _channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken)
                    .ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Publisher could not connect to RabbitMQ at {HostName}:{Port}",
                    _options.HostName, _options.Port);
                throw;
            }

            _logger.LogInformation("Publisher connected to RabbitMQ at {HostName}:{Port}", _options.HostName, _options.Port);
            return _channel;
        }
        finally
        {
            _initLock.Release();
        }
    }

    /// <summary>
    /// Disposes the channel, connection, and init lock, if they were created.
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        if (_channel is not null)
            await _channel.DisposeAsync().ConfigureAwait(false);
        if (_connection is not null)
            await _connection.DisposeAsync().ConfigureAwait(false);
        _initLock.Dispose();
    }
}