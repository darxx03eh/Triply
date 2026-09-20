using Microsoft.Extensions.Logging;
using Triply.Domain.Contracts;
using Triply.Domain.Results;
using Triply.Domain.Results.Enums;

namespace Triply.Infrastructure.Services.Cities;

public partial class CityService
{
    private const string ImageDeleteTopic = "image.delete";

    /// <summary>Deletes the thumbnail.</summary>
    public async Task<Result<bool>> DeleteThumbnailAsync(Guid cityId, CancellationToken cancellationToken = default)
    {
        var city = await cityRepository.GetByIdAsync(cityId, cancellationToken);
        if (city is null)
        {
            logger.LogWarning("Delete thumbnail failed: city {CityId} was not found", cityId);
            return Result<bool>.Failure(
                "CITY_NOT_FOUND",
                $"The requested city with id: {cityId.ToString()} was not found.",
                ResultErrorType.NotFound);
        }

        if (city.ThumbnailUrl is null)
        {
            logger.LogWarning("Delete thumbnail failed: city {CityId} has no thumbnail", cityId);
            return Result<bool>.Failure(
                "CITY_THUMBNAIL_NOT_FOUND",
                $"The city with id: {cityId.ToString()} has no thumbnail.",
                ResultErrorType.NotFound);
        }

        string? publicId = city.ThumbnailPublicId;

        city.ThumbnailUrl = null;
        city.ThumbnailPublicId = null;
        city.ModifiedAt = DateTime.UtcNow;
        await cityRepository.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Thumbnail of city {CityId} removed", cityId);

        if (publicId is not null)
        {
            try
            {
                await publisher.PublishAsync(ImageDeleteTopic,
                    new ImageDeleteMessage
                    {
                        ImageId = cityId, 
                        PublicId = publicId
                    },
                    cancellationToken: cancellationToken);
                logger.LogInformation("Cloudinary asset {PublicId} of city {CityId} queued for deletion", publicId,
                    cityId);
            }
            catch (Exception exception)
            {
                logger.LogError(exception,
                    "Thumbnail of city {CityId} was removed but asset {PublicId} could not be queued for deletion",
                    cityId, publicId);
            }
        }

        return Result<bool>.Success(
            true, ResultSuccessType.NoContent,
            new("CITY_THUMBNAIL_DELETED", "City thumbnail deleted successfully."));
    }
}
