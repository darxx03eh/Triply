namespace MessageQueue.IRabbitMQ;

/// <summary>Defines the message publisher operations.</summary>
public interface IMessagePublisher : IAsyncDisposable
{
    /// <summary>Publishes a message to the exchange with the given routing key.</summary>
    Task PublishAsync<T>(string topic, T message, string? exchange = null,
        CancellationToken cancellationToken = default);
}