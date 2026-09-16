using EmailWorker.Contracts;
using EmailWorker.IProviders;
using EmailWorker.ISenders;
using MessageQueue.IRabbitMQ;

namespace EmailWorker;

public class EmailConsumerHostedService(
    IMessageConsumer consumer,
    IEmailTemplateProvider templateProvider,
    IEmailSender emailSender,
    ILogger<EmailConsumerHostedService> logger
    ) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await consumer.SubscribeAsync<EmailMessage>("email.send", HandleAsync, stoppingToken);
    }
    private async Task HandleAsync(EmailMessage message, string routingKey, CancellationToken cancellationToken)
    {
        logger.LogInformation("Processing {EmailType} email for {To}", message.Type, message.To);

        var (subject, body) = await templateProvider.
            RenderAsync(message.Type, message.TemplateData, cancellationToken);
        await emailSender.SendAsync(message.To, subject, body, cancellationToken);
        
        logger.LogInformation("Sent {EmailType} email to {To}", message.Type, message.To);
    }
}
