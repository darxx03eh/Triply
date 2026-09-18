using Microsoft.Extensions.Logging;
using Triply.Domain.Contracts;
using Triply.Domain.Results;
using Triply.Domain.Results.Enums;

namespace Triply.Infrastructure.Services.Hotels;

public partial class HotelService
{
    private const string ImageDeleteTopic = "image.delete";

    public async Task<Result<bool>> DeleteImageAsync(Guid hotelId, Guid imageId,
        CancellationToken cancellationToken = default)
    {
        var image = await imageRepository.GetByIdAsync(imageId, cancellationToken);
        if (image is null || image.HotelId != hotelId)
            return Result<bool>.Failure(
                "HOTEL_IMAGE_NOT_FOUND",
                $"The image with id: {imageId.ToString()} was not found for this hotel.",
                ResultErrorType.NotFound);

        string? publicId = image.PublicId;

        await imageRepository.DeleteAsync(image);
        await imageRepository.SaveChangesAsync(cancellationToken);

        // Pending/Failed images were never stored in Cloudinary. A Pending image's staged file is
        // cleaned up by the upload worker once it sees the row is gone.
        if (publicId is not null)
        {
            try
            {
                await publisher.PublishAsync(ImageDeleteTopic,
                    new ImageDeleteMessage { ImageId = imageId, HotelId = hotelId, PublicId = publicId },
                    cancellationToken: cancellationToken);
            }
            catch (Exception exception)
            {
                // The image is already gone from the gallery; only the remote asset is left behind.
                logger.LogError(exception,
                    "Image {ImageId} was deleted but its Cloudinary asset {PublicId} could not be queued for removal",
                    imageId, publicId);
            }
        }

        return Result<bool>.Success(
            true, ResultSuccessType.NoContent,
            new ResultSuccess("HOTEL_IMAGE_DELETED", "Image deleted successfully."));
    }
}
