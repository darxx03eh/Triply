using System.Text;
using System.Text.Json;
using MessageQueue.IRabbitMQ;
using MessageQueue.Options;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace MessageQueue.RabbitMQ;

public class RabbitMqConsumer(IOptions<RabbitMqOptions> options) : IMessageConsumer
{
    private readonly RabbitMqOptions _options
        = options.Value ?? throw new ArgumentNullException(nameof(options));
    private IConnection? _connection;
    private IChannel? _channel;

    public async Task SubscribeAsync<T>(string topicPattern, Func<T, string, CancellationToken, Task> handler,
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
            try
            {
                T? payload = JsonSerializer.Deserialize<T>(args.Body.Span);
                if (payload is not null)
                    await handler(payload, args.RoutingKey, cancellationToken).ConfigureAwait(false);

                await _channel.BasicAckAsync(args.DeliveryTag, multiple: false, cancellationToken)
                    .ConfigureAwait(false);
            }
            catch
            {
                int retryCount = GetRetryCount(args.BasicProperties, retryQueueName);

                if (retryCount >= _options.MaxRetryCount && !string.IsNullOrWhiteSpace(finalDlxName))
                {
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

    public async ValueTask DisposeAsync()
    {
        if (_channel is not null)
            await _channel.DisposeAsync().ConfigureAwait(false);
        if (_connection is not null)
            await _connection.DisposeAsync().ConfigureAwait(false);
    }
}