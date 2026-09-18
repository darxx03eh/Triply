using Microsoft.Extensions.Logging;
using Triply.Application.DTOs.Hotels;
using Triply.Application.Extensions;
using Triply.Application.Features.Hotels.Commands.UploadImage;
using Triply.Domain.Contracts;
using Triply.Domain.Entities;
using Triply.Domain.Enums.HotleImages;
using Triply.Domain.Results;
using Triply.Domain.Results.Enums;

namespace Triply.Infrastructure.Services.Hotels;

public partial class HotelService
{
    private const string ImageUploadTopic = "image.upload";

    public async Task<Result<HotelImageResponse>> InitiateUploadAsync(
        Guid hotelId, UploadHotelImageRequest request, CancellationToken cancellationToken = default)
    {
        var hotel = await hotelRepository.GetByIdAsync(hotelId, cancellationToken);
        if (hotel is null)
            return Result<HotelImageResponse>.Failure(
                "HOTEL_NOT_FOUND", "The specified hotel does not exist.", ResultErrorType.NotFound);

        string extension = Path.GetExtension(request.File.FileName).ToLowerInvariant();

        await using var stream = request.File.OpenReadStream();
        if (!IsValidImage(stream, extension))
            return Result<HotelImageResponse>.Failure(
                "FILE_CONTENT_INVALID",
                "The file content does not match its extension.",
                ResultErrorType.Validation);

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

    private static readonly Dictionary<string, byte[]> Signatures = new()
    {
        [".jpg"] = [0xFF, 0xD8, 0xFF],
        [".jpeg"] = [0xFF, 0xD8, 0xFF],
        [".png"] = [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A],
        [".webp"] = [0x52, 0x49, 0x46, 0x46] // "RIFF", followed by "WEBP" at offset 8
    };

    private static readonly byte[] WebpMarker = "WEBP"u8.ToArray();

    public static bool IsValidImage(Stream stream, string extension)
    {
        if (!Signatures.TryGetValue(extension.ToLowerInvariant(), out var signature))
            return false;

        var header = new byte[12];
        stream.Position = 0;
        int bytesRead = stream.ReadAtLeast(header, header.Length, throwOnEndOfStream: false);
        stream.Position = 0;

        if (bytesRead < signature.Length || !header.AsSpan(0, signature.Length).SequenceEqual(signature))
            return false;

        // RIFF is a generic container (WAV, AVI, ...), so also check the WEBP form type.
        return extension != ".webp"
               || (bytesRead >= 12 && header.AsSpan(8, 4).SequenceEqual(WebpMarker));
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
