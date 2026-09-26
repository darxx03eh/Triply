using ImageUploader.IProviders;
using MessageQueue.IRabbitMQ;
using Triply.Application.Interfaces.Repositories;
using Triply.Domain.Contracts;
using Triply.Domain.Contracts.Enums;
using Triply.Domain.Enums.Images;

namespace ImageUploader;

/// <summary>Gets or sets the image upload consumer hosted service.</summary>
/// <summary>Defines the mage upload consumer hosted operations.</summary>
public class ImageUploadConsumerHostedService(
    IMessageConsumer consumer,
    ICloudinaryUploader cloudinaryUploader,
    IServiceScopeFactory scopeFactory,
    ILogger<ImageUploadConsumerHostedService> logger) : BackgroundService
{
    private const string ImageUploadTopic = "image.upload";
    private const string HotelsFolder = "triply/hotels";
    private const string CitiesFolder = "triply/cities";
    private const string RoomsFolder = "triply/rooms";

    /// <summary>Executes.</summary>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        => await consumer.SubscribeAsync<ImageUploadMessage>(
            ImageUploadTopic, HandleAsync, OnRetriesExhaustedAsync, stoppingToken);

    private async Task HandleAsync(ImageUploadMessage message, string routingKey, CancellationToken cancellationToken)
    {
        await using var scope = scopeFactory.CreateAsyncScope();

        switch (message.Target)
        {
            case ImageTarget.CityThumbnail:
                await HandleCityThumbnailAsync(message, scope.ServiceProvider, cancellationToken);
                break;
            case ImageTarget.HotelImage:
                await HandleHotelImageAsync(message, scope.ServiceProvider, cancellationToken);
                break;
            case ImageTarget.RoomImage:
                await HandleRoomImageAsync(message, scope.ServiceProvider, cancellationToken);
                break;
            default:
                logger.LogWarning("Image {ImageId} has an unsupported target {Target}; discarding staged file",
                    message.ImageId, message.Target);
                TryDeleteFile(message.FilePath);
                break;
        }
    }

    private async Task HandleHotelImageAsync(ImageUploadMessage message, IServiceProvider services,
        CancellationToken cancellationToken)
    {
        var imageRepository = services.GetRequiredService<IHotelImageRepository>();

        var image = await imageRepository.GetByIdAsync(message.ImageId, cancellationToken);
        if (image is null)
        {
            logger.LogWarning("Image {ImageId} no longer exists; discarding staged file", message.ImageId);
            TryDeleteFile(message.FilePath);
            return;
        }

        if (image.Status == ImageStatus.Uploaded)
        {
            TryDeleteFile(message.FilePath);
            return;
        }

        if (!File.Exists(message.FilePath))
        {
            logger.LogError("Staged file {FilePath} for image {ImageId} was not found",
                message.FilePath, message.ImageId);
            image.Status = ImageStatus.Failed;
            await imageRepository.SaveChangesAsync(cancellationToken);
            return;
        }

        logger.LogInformation("Uploading image {ImageId} for hotel {HotelId}", message.ImageId, message.Id);

        var uploaded = await cloudinaryUploader.UploadAsync(
            message.FilePath, message.OriginalFileName, HotelsFolder, cancellationToken);

        image.Url = uploaded.Url;
        image.PublicId = uploaded.PublicId;
        image.Status = ImageStatus.Uploaded;
        await imageRepository.SaveChangesAsync(cancellationToken);

        TryDeleteFile(message.FilePath);

        logger.LogInformation("Image {ImageId} uploaded successfully", message.ImageId);
    }
    private async Task HandleRoomImageAsync(ImageUploadMessage message, IServiceProvider services,
        CancellationToken cancellationToken)
    {
        var imageRepository = services.GetRequiredService<IRoomImageRepository>();

        var image = await imageRepository.GetByIdAsync(message.ImageId, cancellationToken);
        if (image is null)
        {
            logger.LogWarning("Image {ImageId} no longer exists; discarding staged file", message.ImageId);
            TryDeleteFile(message.FilePath);
            return;
        }

        if (image.Status == ImageStatus.Uploaded)
        {
            TryDeleteFile(message.FilePath);
            return;
        }

        if (!File.Exists(message.FilePath))
        {
            logger.LogError("Staged file {FilePath} for image {ImageId} was not found",
                message.FilePath, message.ImageId);
            image.Status = ImageStatus.Failed;
            await imageRepository.SaveChangesAsync(cancellationToken);
            return;
        }

        logger.LogInformation("Uploading image {ImageId} for room {RoomId}", message.ImageId, message.Id);

        var uploaded = await cloudinaryUploader.UploadAsync(
            message.FilePath, message.OriginalFileName, RoomsFolder, cancellationToken);

        image.Url = uploaded.Url;
        image.PublicId = uploaded.PublicId;
        image.Status = ImageStatus.Uploaded;
        await imageRepository.SaveChangesAsync(cancellationToken);

        TryDeleteFile(message.FilePath);

        logger.LogInformation("Image {ImageId} uploaded successfully", message.ImageId);
    }

    private async Task HandleCityThumbnailAsync(ImageUploadMessage message, IServiceProvider services,
        CancellationToken cancellationToken)
    {
        var cityRepository = services.GetRequiredService<ICityRepository>();

        var city = message.CityId is { } cityId
            ? await cityRepository.GetByIdAsync(cityId, cancellationToken)
            : null;
        if (city is null)
        {
            logger.LogWarning("City {CityId} no longer exists; discarding staged file", message.CityId);
            TryDeleteFile(message.FilePath);
            return;
        }

        if (!File.Exists(message.FilePath))
        {
            logger.LogError("Staged file {FilePath} for city {CityId} was not found",
                message.FilePath, message.CityId);
            return;
        }

        logger.LogInformation("Uploading thumbnail for city {CityId}", message.CityId);

        var uploaded = await cloudinaryUploader.UploadAsync(
            message.FilePath, message.OriginalFileName, CitiesFolder, cancellationToken);

        string? oldPublicId = city.ThumbnailPublicId;
        city.ThumbnailUrl = uploaded.Url;
        city.ThumbnailPublicId = uploaded.PublicId;
        city.ModifiedAt = DateTime.UtcNow;
        await cityRepository.SaveChangesAsync(cancellationToken);

        TryDeleteFile(message.FilePath);

        if (oldPublicId is not null)
        {
            try
            {
                await cloudinaryUploader.DeleteAsync(oldPublicId, cancellationToken);
            }
            catch (Exception exception)
            {
                logger.LogWarning(exception, "Could not delete the previous thumbnail {PublicId} of city {CityId}",
                    oldPublicId, message.CityId);
            }
        }

        logger.LogInformation("Thumbnail for city {CityId} uploaded successfully", message.CityId);
    }

    private async Task OnRetriesExhaustedAsync(ImageUploadMessage message, Exception exception,
        CancellationToken cancellationToken)
    {
        logger.LogError(exception, "Giving up on upload {ImageId} ({Target}) after all retries",
            message.ImageId, message.Target);

        await using var scope = scopeFactory.CreateAsyncScope();
        if (message.Target == ImageTarget.HotelImage)
        {
            var imageRepository = scope.ServiceProvider.GetRequiredService<IHotelImageRepository>();

            var image = await imageRepository.GetByIdAsync(message.ImageId, cancellationToken);
            if (image is not null && image.Status != ImageStatus.Uploaded)
            {
                image.Status = ImageStatus.Failed;
                await imageRepository.SaveChangesAsync(cancellationToken);
            }
        }
        else if (message.Target == ImageTarget.RoomImage)
        {
            var imageRepository = scope.ServiceProvider.GetRequiredService<IRoomImageRepository>();

            var image = await imageRepository.GetByIdAsync(message.ImageId, cancellationToken);
            if (image is not null && image.Status != ImageStatus.Uploaded)
            {
                image.Status = ImageStatus.Failed;
                await imageRepository.SaveChangesAsync(cancellationToken);
            }
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
