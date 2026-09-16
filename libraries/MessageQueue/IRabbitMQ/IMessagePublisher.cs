namespace MessageQueue.IRabbitMQ;

public interface IMessagePublisher : IAsyncDisposable
{
    Task PublishAsync<T>(string topic, T message, string? exchange = null,
        CancellationToken cancellationToken = default);
}