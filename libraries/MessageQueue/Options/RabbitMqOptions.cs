namespace MessageQueue.Options;
public class RabbitMqOptions
{
    public string HostName { get; set; } = "localhost";
    public int Port { get; set; } = 5672;
    public string UserName { get; set; } = "guest";
    public string Password { get; set; } = "guest";
    public string VirtualHost { get; set; } = "/";
    public string ExchangeName { get; set; } = "server.events";
    public bool DurableExchange { get; set; } = true;
    public string? QueueName { get; set; }
    public bool DurableQueue { get; set; } = true;
    public ushort PrefetchCount { get; set; } = 10;

    // Retry
    public int MaxRetryCount { get; set; } = 3;
    public int RetryDelayMilliseconds { get; set; } = 5000;
    public string? RetryExchangeName { get; set; }
    public string? RetryQueueName { get; set; }

    // Final Dead-letter 
    public string? DeadLetterExchangeName { get; set; }
    public string? DeadLetterQueueName { get; set; }
    public string? DeadLetterRoutingKey { get; set; }
}