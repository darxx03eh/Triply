namespace MessageQueue.Options;
/// <summary>Configuration options of RabbitMQ MQ.</summary>
public class RabbitMqOptions
{
    /// <summary>Gets or sets the host name.</summary>
    public string HostName { get; set; } = "localhost";
    /// <summary>Gets or sets the port.</summary>
    public int Port { get; set; } = 5672;
    /// <summary>Gets or sets the user name.</summary>
    public string UserName { get; set; } = "guest";
    /// <summary>Gets or sets the password.</summary>
    public string Password { get; set; } = "guest";
    /// <summary>Gets or sets the virtual host.</summary>
    public string VirtualHost { get; set; } = "/";
    /// <summary>Gets or sets the exchange name.</summary>
    public string ExchangeName { get; set; } = "server.events";
    /// <summary>Gets or sets the durable exchange.</summary>
    public bool DurableExchange { get; set; } = true;
    /// <summary>Gets or sets the queue name.</summary>
    public string? QueueName { get; set; }
    /// <summary>Gets or sets the durable queue.</summary>
    public bool DurableQueue { get; set; } = true;
    /// <summary>Gets or sets the number of prefetch.</summary>
    public ushort PrefetchCount { get; set; } = 10;

    // Retry
    /// <summary>Gets or sets the number of max retry.</summary>
    public int MaxRetryCount { get; set; } = 3;
    /// <summary>Gets or sets the retry delay milliseconds.</summary>
    public int RetryDelayMilliseconds { get; set; } = 5000;
    /// <summary>Gets or sets the retry exchange name.</summary>
    public string? RetryExchangeName { get; set; }
    /// <summary>Gets or sets the retry queue name.</summary>
    public string? RetryQueueName { get; set; }

    // Final Dead-letter 
    /// <summary>Gets or sets the dead letter exchange name.</summary>
    public string? DeadLetterExchangeName { get; set; }
    /// <summary>Gets or sets the dead letter queue name.</summary>
    public string? DeadLetterQueueName { get; set; }
    /// <summary>Gets or sets the dead letter routing key.</summary>
    public string? DeadLetterRoutingKey { get; set; }
}