using System.Text;
using System.Text.Json;
using MessageQueue.IRabbitMQ;
using MessageQueue.Options;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace MessageQueue.RabbitMQ;

/// <summary>
/// RabbitMQ implementation of <see cref="IMessageConsumer"/>.
/// Consumes messages from a topic-based exchange with automatic retry
/// (via a TTL-based retry queue) and a final Dead Letter Queue (DLQ)
/// once the maximum retry count is exceeded.
/// </summary>
/// <remarks>
/// Message flow:
/// 1. Messages are published to the main exchange and routed to the main queue
///    based on <c>topicPattern</c>.
/// 2. If handling a message throws, the message is Nacked (without requeue),
///    which triggers RabbitMQ's dead-lettering to the retry exchange/queue.
/// 3. The retry queue holds the message for <see cref="RabbitMqOptions.RetryDelayMilliseconds"/>
///    (via TTL), then dead-letters it back to the main exchange for another attempt.
/// 4. Once the number of retries (read from the "x-death" header) reaches
///    <see cref="RabbitMqOptions.MaxRetryCount"/>, the message is published
///    directly to the final Dead Letter Exchange/Queue instead of being retried again.
/// </remarks>
public class RabbitMqConsumer(IOptions<RabbitMqOptions> options) : IMessageConsumer
{
    private readonly RabbitMqOptions _options
        = options.Value ?? throw new ArgumentNullException(nameof(options));
    private IConnection? _connection;
    private IChannel? _channel;
    /// <summary>Resolved RabbitMQ configuration options.</summary>private readonly RabbitMqOptions _options = options.Value ?? throw new ArgumentNullException(nameof(options));
    /// <summary>Underlying RabbitMQ connection. Created on first <see cref="SubscribeAsync{T}"/> call.</summary>private IConnection? _connection;
    /// <summary>Underlying RabbitMQ channel used for all exchange/queue operations and consuming.</summary>private IChannel? _channel;
    /// <summary>
    /// Sets up the required exchanges/queues (main, retry, and final DLQ), then starts
    /// consuming messages matching <paramref name="topicPattern"/>, invoking
    /// <paramref name="handler"/> for each deserialized message.
    /// </summary>
    /// <typeparam name="T">The type to deserialize the message body into (via JSON).</typeparam>
    /// <param name="topicPattern">
    /// The routing key pattern (topic exchange syntax, e.g. "order.*") used to bind the
    /// main queue and the retry queue.
    /// </param>
    /// <param name="handler">
    /// Callback invoked for each successfully deserialized message. Receives the payload,
    /// the message's routing key, and the cancellation token. If this throws, the message
    /// is routed through the retry/DLQ flow described in <see cref="RabbitMqConsumer"/>.
    /// </param>
    /// <param name="cancellationToken">Token used to cancel setup and consumption.</param>
    /// <exception cref="ArgumentException">Thrown if <paramref name="topicPattern"/> is null/empty/whitespace.</exception>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="handler"/> is null.</exception>
    public Task SubscribeAsync<T>(string topicPattern, Func<T, string, CancellationToken, Task> handler,
        CancellationToken cancellationToken = default)
        => SubscribeAsync(topicPattern, handler, onRetriesExhausted: null, cancellationToken);

    /// <inheritdoc />
    public async Task SubscribeAsync<T>(string topicPattern, Func<T, string, CancellationToken, Task> handler,
        Func<T, Exception, CancellationToken, Task>? onRetriesExhausted,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(topicPattern))
            throw new ArgumentException("Topic pattern must not be null or empty.", nameof(topicPattern));

        ArgumentNullException.ThrowIfNull(handler, nameof(handler));

        var factory = new ConnectionFactory
        {
            HostName = _options.HostName,
            Port = _options.Port,
            UserName = _options.UserName,
            Password = _options.Password,
            VirtualHost = _options.VirtualHost,
            AutomaticRecoveryEnabled = true,
            TopologyRecoveryEnabled = true
        };

        _connection = await factory.CreateConnectionAsync(cancellationToken).ConfigureAwait(false);
        _channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken).ConfigureAwait(false);

        await _channel.ExchangeDeclareAsync(
            exchange: _options.ExchangeName,
            type: ExchangeType.Topic,
            durable: _options.DurableExchange,
            autoDelete: false,
            cancellationToken: cancellationToken).ConfigureAwait(false);

        string baseQueueName = _options.QueueName ?? $"{topicPattern}.queue";
        string retryExchangeName = _options.RetryExchangeName ?? $"{_options.ExchangeName}.retry";
        string retryQueueName = _options.RetryQueueName ?? $"{baseQueueName}.retry";

        // Final Dead-letter (checked only after retries are exhausted)
        string? finalDlxName = _options.DeadLetterExchangeName;
        if (!string.IsNullOrWhiteSpace(finalDlxName))
        {
            await _channel.ExchangeDeclareAsync(
                exchange: finalDlxName,
                type: ExchangeType.Topic,
                durable: true,
                autoDelete: false,
                cancellationToken: cancellationToken).ConfigureAwait(false);

            string dlqName = _options.DeadLetterQueueName ?? $"{baseQueueName}.dlq";
            string dlqRoutingKey = _options.DeadLetterRoutingKey ?? "#";

            await _channel.QueueDeclareAsync(
                queue: dlqName, durable: true, exclusive: false, autoDelete: false,
                cancellationToken: cancellationToken).ConfigureAwait(false);

            await _channel.QueueBindAsync(
                queue: dlqName, exchange: finalDlxName, routingKey: dlqRoutingKey,
                cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        // Retry Exchange + Retry Queue (TTL-based delay)
        await _channel.ExchangeDeclareAsync(
            exchange: retryExchangeName, type: ExchangeType.Topic, durable: true, autoDelete: false,
            cancellationToken: cancellationToken).ConfigureAwait(false);

        var retryQueueArguments = new Dictionary<string, object?>
        {
            ["x-dead-letter-exchange"] = _options.ExchangeName,   // after TTL expires, goes back to the main exchange
            ["x-message-ttl"] = _options.RetryDelayMilliseconds
        };

        await _channel.QueueDeclareAsync(
            queue: retryQueueName, durable: true, exclusive: false, autoDelete: false,
            arguments: retryQueueArguments, cancellationToken: cancellationToken).ConfigureAwait(false);

        await _channel.QueueBindAsync(
            queue: retryQueueName, exchange: retryExchangeName, routingKey: topicPattern,
            cancellationToken: cancellationToken).ConfigureAwait(false);

        await _channel.BasicQosAsync(0, _options.PrefetchCount, false, cancellationToken)
            .ConfigureAwait(false);

        // Main Queue (on failure, goes to retry exchange, not directly to final DLQ)
        var mainQueueArguments = new Dictionary<string, object?>
        {
            ["x-dead-letter-exchange"] = retryExchangeName
        };

        bool exclusive = string.IsNullOrWhiteSpace(_options.QueueName);
        var declareResult = await _channel.QueueDeclareAsync(
            queue: exclusive ? string.Empty : baseQueueName,
            durable: !exclusive && _options.DurableQueue,
            exclusive: exclusive,
            autoDelete: exclusive,
            arguments: mainQueueArguments,
            cancellationToken: cancellationToken).ConfigureAwait(false);

        string queueName = declareResult.QueueName;

        await _channel.QueueBindAsync(
            queue: queueName, exchange: _options.ExchangeName, routingKey: topicPattern,
            cancellationToken: cancellationToken).ConfigureAwait(false);

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += async (_, args) =>
        {
            T? payload = default;
            try
            {
                payload = JsonSerializer.Deserialize<T>(args.Body.Span);
                if (payload is not null)
                    await handler(payload, args.RoutingKey, cancellationToken).ConfigureAwait(false);

                await _channel.BasicAckAsync(args.DeliveryTag, multiple: false, cancellationToken)
                    .ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                int retryCount = GetRetryCount(args.BasicProperties, retryQueueName);

                if (retryCount >= _options.MaxRetryCount && !string.IsNullOrWhiteSpace(finalDlxName))
                {
                    if (payload is not null && onRetriesExhausted is not null)
                    {
                        try
                        {
                            await onRetriesExhausted(payload, exception, cancellationToken).ConfigureAwait(false);
                        }
                        catch
                        {
                            // Compensation is best-effort; the message still goes to the DLQ below.
                        }
                    }

                    // BasicPublishAsync requires a concrete IAmqpHeader-implementing type,
                    // so we copy the read-only incoming properties into a mutable BasicProperties instance.
                    var publishProperties = new BasicProperties(args.BasicProperties);

                    await _channel.BasicPublishAsync(
                        exchange: finalDlxName, routingKey: args.RoutingKey, mandatory: false,
                        basicProperties: publishProperties, body: args.Body,
                        cancellationToken: cancellationToken).ConfigureAwait(false);

                    await _channel.BasicAckAsync(args.DeliveryTag, multiple: false, cancellationToken)
                        .ConfigureAwait(false);
                }
                else
                {
                    await _channel.BasicNackAsync(args.DeliveryTag, multiple: false, requeue: false, cancellationToken)
                        .ConfigureAwait(false);
                }
            }
        };

        await _channel.BasicConsumeAsync(queueName, autoAck: false, consumer, cancellationToken)
            .ConfigureAwait(false);
    }
    /// <summary>
    /// Reads the number of times a message has been dead-lettered from the given
    /// retry queue, using RabbitMQ's "x-death" header array.
    /// </summary>
    /// <param name="properties">The incoming message's basic properties (may contain the "x-death" header).</param>
    /// <param name="retryQueueName">
    /// The retry queue name to match against each x-death entry's "queue" field,
    /// so counts from unrelated queues are ignored.
    /// </param>
    /// <returns>
    /// The recorded death/retry count for <paramref name="retryQueueName"/>, or 0 if the
    /// header is missing, malformed, or no entry matches that queue.
    /// </returns>
    private static int GetRetryCount(IReadOnlyBasicProperties? properties, string retryQueueName)
    {
        if (properties?.Headers is null || !properties.Headers.TryGetValue("x-death", out var xDeathObj))
            return 0;

        if (xDeathObj is not List<object> deaths)
            return 0;

        foreach (var entry in deaths)
        {
            if (entry is not Dictionary<string, object> death)
                continue;

            string? deathQueueName = death.TryGetValue("queue", out var q) && q is byte[] bytes
                ? Encoding.UTF8.GetString(bytes)
                : null;

            if (deathQueueName == retryQueueName && death.TryGetValue("count", out var countObj))
                return Convert.ToInt32(countObj);
        }

        return 0;
    }
    /// <summary>
    /// Disposes the channel and connection, if they were created.
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        if (_channel is not null)
            await _channel.DisposeAsync().ConfigureAwait(false);
        if (_connection is not null)
            await _connection.DisposeAsync().ConfigureAwait(false);
    }
}