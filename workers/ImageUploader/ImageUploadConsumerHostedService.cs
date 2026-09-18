using ImageUploader.IProviders;
using MessageQueue.IRabbitMQ;
using Triply.Application.Interfaces.Repositories;
using Triply.Domain.Contracts;
using Triply.Domain.Enums.HotleImages;

namespace ImageUploader;

public class ImageUploadConsumerHostedService(
    IMessageConsumer consumer,
    ICloudinaryUploader cloudinaryUploader,
    IServiceScopeFactory scopeFactory,
    ILogger<ImageUploadConsumerHostedService> logger) : BackgroundService
{
    private const string ImageUploadTopic = "image.upload";

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        => await consumer.SubscribeAsync<ImageUploadMessage>(
            ImageUploadTopic, HandleAsync, OnRetriesExhaustedAsync, stoppingToken);

    private async Task HandleAsync(ImageUploadMessage message, string routingKey, CancellationToken cancellationToken)
    {
        // DbContext is scoped: create a fresh scope per message instead of holding one for the worker's lifetime.
        await using var scope = scopeFactory.CreateAsyncScope();
        var imageRepository = scope.ServiceProvider.GetRequiredService<IHotelImageRepository>();

        var image = await imageRepository.GetByIdAsync(message.ImageId, cancellationToken);
        if (image is null)
        {
            logger.LogWarning("Image {ImageId} no longer exists; discarding staged file", message.ImageId);
            TryDeleteFile(message.FilePath);
            return;
        }

        // Redelivered message after a successful upload: nothing left to do.
        if (image.Status == HotelImageStatus.Uploaded)
        {
            TryDeleteFile(message.FilePath);
            return;
        }

        if (!File.Exists(message.FilePath))
        {
            // Retrying cannot bring the file back, so fail fast instead of going through the retry queue.
            logger.LogError("Staged file {FilePath} for image {ImageId} was not found",
                message.FilePath, message.ImageId);
            image.Status = HotelImageStatus.Failed;
            await imageRepository.SaveChangesAsync(cancellationToken);
            return;
        }

        logger.LogInformation("Uploading image {ImageId} for hotel {HotelId}", message.ImageId, message.HotelId);

        // Transient failures (network, Cloudinary) throw here and are retried by the consumer.
        string cloudinaryUrl = await cloudinaryUploader.UploadAsync(
            message.FilePath, message.OriginalFileName, cancellationToken);

        image.Url = cloudinaryUrl;
        image.Status = HotelImageStatus.Uploaded;
        await imageRepository.SaveChangesAsync(cancellationToken);

        TryDeleteFile(message.FilePath);

        logger.LogInformation("Image {ImageId} uploaded successfully", message.ImageId);
    }

    private async Task OnRetriesExhaustedAsync(ImageUploadMessage message, Exception exception,
        CancellationToken cancellationToken)
    {
        logger.LogError(exception, "Giving up on image {ImageId} for hotel {HotelId} after all retries",
            message.ImageId, message.HotelId);

        await using var scope = scopeFactory.CreateAsyncScope();
        var imageRepository = scope.ServiceProvider.GetRequiredService<IHotelImageRepository>();

        var image = await imageRepository.GetByIdAsync(message.ImageId, cancellationToken);
        if (image is not null && image.Status != HotelImageStatus.Uploaded)
        {
            image.Status = HotelImageStatus.Failed;
            await imageRepository.SaveChangesAsync(cancellationToken);
        }

        TryDeleteFile(message.FilePath);
    }

    private void TryDeleteFile(string filePath)
    {
        try
        {
            File.Delete(filePath);
        }
        catch (Exception exception)
        {
            logger.LogWarning(exception, "Could not delete staged image file {FilePath}", filePath);
        }
    }
}
