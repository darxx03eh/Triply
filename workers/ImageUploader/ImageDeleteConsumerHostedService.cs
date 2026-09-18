using ImageUploader.IProviders;
using MessageQueue.IRabbitMQ;
using Triply.Domain.Contracts;

namespace ImageUploader;

/// <summary>Removes deleted hotel images from Cloudinary.</summary>
/// <remarks>
/// Uses its own <see cref="IMessageConsumer"/> (and so its own queue) because a consumer instance
/// holds a single channel and is bound to a single queue.
/// </remarks>
public class ImageDeleteConsumerHostedService(
    IMessageConsumer consumer,
    ICloudinaryUploader cloudinaryUploader,
    ILogger<ImageDeleteConsumerHostedService> logger) : BackgroundService
{
    private const string ImageDeleteTopic = "image.delete";

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        => await consumer.SubscribeAsync<ImageDeleteMessage>(
            ImageDeleteTopic, HandleAsync, OnRetriesExhaustedAsync, stoppingToken);

    private async Task HandleAsync(ImageDeleteMessage message, string routingKey, CancellationToken cancellationToken)
    {
        logger.LogInformation("Deleting asset {PublicId} of image {ImageId}", message.PublicId, message.ImageId);

        await cloudinaryUploader.DeleteAsync(message.PublicId, cancellationToken);

        logger.LogInformation("Asset {PublicId} of image {ImageId} deleted", message.PublicId, message.ImageId);
    }

    private Task OnRetriesExhaustedAsync(ImageDeleteMessage message, Exception exception,
        CancellationToken cancellationToken)
    {
        // The message is kept in the dead-letter queue, so the asset can still be removed manually.
        logger.LogError(exception, "Giving up on deleting asset {PublicId} of image {ImageId}",
            message.PublicId, message.ImageId);
        return Task.CompletedTask;
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await base.StopAsync(cancellationToken);
        await consumer.DisposeAsync();
    }
}
