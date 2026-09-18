using Microsoft.Extensions.Logging;
using Triply.Application.DTOs.Cities;
using Triply.Application.Extensions;
using Triply.Application.Features.Cities.Commands.UploadCityThumbnail;
using Triply.Domain.Contracts;
using Triply.Domain.Contracts.Enums;
using Triply.Domain.Results;
using Triply.Domain.Results.Enums;
using Triply.Infrastructure.Helpers;

namespace Triply.Infrastructure.Services.Cities;

public partial class CityService
{
    private const string ImageUploadTopic = "image.upload";

    public async Task<Result<CityResponse>> UploadThumbnailAsync(Guid cityId, UploadCityThumbnailRequest request,
        CancellationToken cancellationToken = default)
    {
        var city = await cityRepository.GetByIdAsync(cityId, cancellationToken);
        if (city is null)
            return Result<CityResponse>.Failure(
                "CITY_NOT_FOUND",
                $"The requested city with id: {cityId.ToString()} was not found.",
                ResultErrorType.NotFound);

        string extension = Path.GetExtension(request.File.FileName).ToLowerInvariant();

        await using var stream = request.File.OpenReadStream();
        if (!ImageFileSignature.IsValid(stream, extension))
            return Result<CityResponse>.Failure(
                "FILE_CONTENT_INVALID",
                "The file content does not match its extension.",
                ResultErrorType.Validation);

        var uploadId = Guid.NewGuid();
        string storageDirectory = uploadOptions.Value.SharedStoragePath;
        Directory.CreateDirectory(storageDirectory);
        string filePath = Path.Combine(storageDirectory, $"{uploadId}{extension}");

        await using (var fileStream = File.Create(filePath))
            await stream.CopyToAsync(fileStream, cancellationToken);

        try
        {
            await publisher.PublishAsync(ImageUploadTopic,
                new ImageUploadMessage
                {
                    ImageId = uploadId,
                    CityId = cityId,
                    Target = ImageTarget.CityThumbnail,
                    FilePath = filePath,
                    OriginalFileName = request.File.FileName,
                },
                cancellationToken: cancellationToken);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Failed to queue thumbnail upload for city {CityId}", cityId);
            File.Delete(filePath);

            return Result<CityResponse>.Failure(
                "CITY_THUMBNAIL_QUEUE_FAILED",
                "The image could not be queued for processing. Please try again later.",
                ResultErrorType.BusinessRule);
        }

        var hotelsCount = await cityRepository.GetHotelsCountAsync([cityId], cancellationToken);
        return Result<CityResponse>.Success(
            city.ToCityResponse(hotelsCount.GetValueOrDefault(cityId)),
            ResultSuccessType.Accepted,
            new ResultSuccess("CITY_THUMBNAIL_UPLOAD_QUEUED", "Thumbnail upload has been queued for processing."));
    }
}
