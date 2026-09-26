using Microsoft.Extensions.Logging;
using Triply.Application.DTOs.Rooms;
using Triply.Application.Extensions;
using Triply.Application.Features.Rooms.Commands.UploadImage;
using Triply.Domain.Contracts;
using Triply.Domain.Contracts.Enums;
using Triply.Domain.Entities;
using Triply.Domain.Enums.Images;
using Triply.Domain.Results;
using Triply.Domain.Results.Enums;
using Triply.Infrastructure.Helpers;

namespace Triply.Infrastructure.Services.Rooms;

public partial class RoomService
{
    private const string ImageUploadTopic = "image.upload";
    /// <summary>Initiates the upload.</summary>
    public async Task<Result<RoomImageResponse>> InitiateUploadAsync(Guid roomId, UploadRoomImageRequest request,
        CancellationToken cancellationToken = default)

    {
        var room = await roomRepository.GetByIdAsync(roomId, cancellationToken);
        if (room is null)
        {
            logger.LogWarning("Upload image failed: room {RoomId} was not found", room);
            return Result<RoomImageResponse>.Failure(
                "ROOM_NOT_FOUND", "The specified room does not exist.", ResultErrorType.NotFound);
        }

        string extension = Path.GetExtension(request.File.FileName).ToLowerInvariant();

        await using var stream = request.File.OpenReadStream();
        if (!ImageFileSignature.IsValid(stream, extension))
        {
            logger.LogWarning(
                "Upload image for room {RoomId} rejected: the content of {FileName} does not match its extension",
                roomId, request.File.FileName);
            return Result<RoomImageResponse>.Failure(
                "FILE_CONTENT_INVALID",
                "The file content does not match its extension.",
                ResultErrorType.Validation);
        }

        var image = new RoomImage()
        {
            RoomId = roomId,
            DisplayOrder = request.DisplayOrder
                           ?? await imageRepository.GetNextDisplayOrderAsync(roomId, cancellationToken),
            Status = ImageStatus.Pending
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
                    Id = roomId,
                    Target = ImageTarget.RoomImage,
                    FilePath = filePath,
                    OriginalFileName = request.File.FileName,
                },
                cancellationToken: cancellationToken);
            logger.LogInformation("Image {ImageId} of room {RoomId} " +
                                  "saved to {FilePath} ({FileSize} bytes) and queued for upload",
                image.ImageId, roomId, filePath, request.File.Length);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Failed to queue upload of image {ImageId} for room {RoomId}",
                image.ImageId, roomId);

            image.Status = ImageStatus.Failed;
            await imageRepository.SaveChangesAsync(CancellationToken.None);
            TryDeleteFile(filePath);

            return Result<RoomImageResponse>.Failure(
                "ROOM_IMAGE_QUEUE_FAILED",
                "The image could not be queued for processing. Please try again later.",
                ResultErrorType.BusinessRule);
        }

        return Result<RoomImageResponse>.Success(
            image.ToRoomImageResponse(),
            ResultSuccessType.Accepted,
            new ResultSuccess("ROOM_IMAGE_UPLOAD_QUEUED",
                "Image upload has been queued for processing."));
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
