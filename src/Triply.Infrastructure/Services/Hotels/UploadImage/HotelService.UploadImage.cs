using Microsoft.Extensions.Logging;
using Triply.Application.DTOs.Hotels;
using Triply.Application.Extensions;
using Triply.Application.Features.Hotels.Commands.UploadImage;
using Triply.Domain.Contracts;
using Triply.Domain.Entities;
using Triply.Domain.Enums.HotleImages;
using Triply.Domain.Results;
using Triply.Domain.Results.Enums;
using Triply.Infrastructure.Helpers;

namespace Triply.Infrastructure.Services.Hotels;

public partial class HotelService
{
    private const string ImageUploadTopic = "image.upload";

    /// <summary>Initiates the upload.</summary>
    public async Task<Result<HotelImageResponse>> InitiateUploadAsync(
        Guid hotelId, UploadHotelImageRequest request, CancellationToken cancellationToken = default)
    {
        var hotel = await hotelRepository.GetByIdAsync(hotelId, cancellationToken);
        if (hotel is null)
        {
            logger.LogWarning("Upload image failed: hotel {HotelId} was not found", hotelId);
            return Result<HotelImageResponse>.Failure(
                "HOTEL_NOT_FOUND", "The specified hotel does not exist.", ResultErrorType.NotFound);
        }

        string extension = Path.GetExtension(request.File.FileName).ToLowerInvariant();

        await using var stream = request.File.OpenReadStream();
        if (!ImageFileSignature.IsValid(stream, extension))
        {
            logger.LogWarning(
                "Upload image for hotel {HotelId} rejected: the content of {FileName} does not match its extension",
                hotelId, request.File.FileName);
            return Result<HotelImageResponse>.Failure(
                "FILE_CONTENT_INVALID",
                "The file content does not match its extension.",
                ResultErrorType.Validation);
        }

        var image = new HotelImage
        {
            HotelId = hotelId,
            DisplayOrder = request.DisplayOrder
                           ?? await imageRepository.GetNextDisplayOrderAsync(hotelId, cancellationToken),
            Status = HotelImageStatus.Pending
        };

        // Stage the file first so a DB row never points to a file that was never written.
        string storageDirectory = uploadOptions.Value.SharedStoragePath;
        Directory.CreateDirectory(storageDirectory);
        string filePath = Path.Combine(storageDirectory, $"{image.ImageId}{extension}");

        await using (var fileStream = File.Create(filePath))
            await stream.CopyToAsync(fileStream, cancellationToken);

        try
        {
            await imageRepository.AddAsync(image, cancellationToken);
            await imageRepository.SaveChangesAsync(cancellationToken);
        }
        catch
        {
            TryDeleteFile(filePath);
            throw;
        }

        try
        {
            await publisher.PublishAsync(ImageUploadTopic,
                new ImageUploadMessage
                {
                    ImageId = image.ImageId,
                    HotelId = hotelId,
                    FilePath = filePath,
                    OriginalFileName = request.File.FileName,
                },
                cancellationToken: cancellationToken);
            logger.LogInformation("Image {ImageId} of hotel {HotelId} " +
                                  "saved to {FilePath} ({FileSize} bytes) and queued for upload",
                image.ImageId, hotelId, filePath, request.File.Length);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Failed to queue upload of image {ImageId} for hotel {HotelId}",
                image.ImageId, hotelId);

            image.Status = HotelImageStatus.Failed;
            await imageRepository.SaveChangesAsync(CancellationToken.None);
            TryDeleteFile(filePath);

            return Result<HotelImageResponse>.Failure(
                "HOTEL_IMAGE_QUEUE_FAILED",
                "The image could not be queued for processing. Please try again later.",
                ResultErrorType.BusinessRule);
        }

        return Result<HotelImageResponse>.Success(
            image.ToHotelImageResponse(),
            ResultSuccessType.Accepted,
            new ResultSuccess("HOTEL_IMAGE_UPLOAD_QUEUED", "Image upload has been queued for processing."));
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
