using Microsoft.Extensions.Logging;
using Triply.Application.DTOs.Rooms;
using Triply.Application.Extensions;
using Triply.Domain.Results;
using Triply.Domain.Results.Enums;

namespace Triply.Infrastructure.Services.Rooms;

public partial class RoomService
{
    /// <summary>Gets the images.</summary>
    public async Task<Result<IReadOnlyList<RoomImageResponse>>> GetImagesAsync(Guid roomId,
        CancellationToken cancellationToken = default)

    {
        var room = await roomRepository.GetByIdAsync(roomId, cancellationToken);
        if (room is null)
        {
            logger.LogWarning("Get images failed: room {RoomId} was not found", roomId);
            return Result<IReadOnlyList<RoomImageResponse>>.Failure(
                "ROOM_NOT_FOUND", "The specified room does not exist.", ResultErrorType.NotFound);
        }

        var images = await imageRepository.GetByRoomIdAsync(roomId, cancellationToken);

        return Result<IReadOnlyList<RoomImageResponse>>.Success(
            images.Select(i => i.ToRoomImageResponse()).ToList(),
            success: new ResultSuccess("ROOM_IMAGES_FOUND", "Room images were retrieved successfully."));
    }
}
