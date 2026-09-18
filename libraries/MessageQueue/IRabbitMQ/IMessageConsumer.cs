namespace MessageQueue.IRabbitMQ;

public interface IMessageConsumer : IAsyncDisposable
{
    Task SubscribeAsync<T>(string topicPattern, Func<T, string, CancellationToken, Task> handler,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Same as <see cref="SubscribeAsync{T}(string, Func{T, string, CancellationToken, Task}, CancellationToken)"/>,
    /// but invokes <paramref name="onRetriesExhausted"/> once a message is moved to the final dead-letter queue,
    /// so the consumer can compensate (e.g. mark the related record as failed).
    /// </summary>
    Task SubscribeAsync<T>(string topicPattern, Func<T, string, CancellationToken, Task> handler,
        Func<T, Exception, CancellationToken, Task>? onRetriesExhausted,
        CancellationToken cancellationToken = default);
}