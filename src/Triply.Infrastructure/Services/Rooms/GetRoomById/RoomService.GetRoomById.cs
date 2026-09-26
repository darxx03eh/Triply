using Microsoft.Extensions.Logging;
using Triply.Application.DTOs.Rooms;
using Triply.Application.Extensions;
using Triply.Domain.Results;
using Triply.Domain.Results.Enums;

namespace Triply.Infrastructure.Services.Rooms;

public partial class RoomService
{
    /// <summary>Gets the room by its identifier.</summary>
    public async Task<Result<RoomResponse>> GetByIdAsync(Guid roomId, CancellationToken cancellationToken = default)
    {
        var room = await roomRepository.GetByIdWithHotelAndImagesAsync(roomId, cancellationToken);
        if (room is null)
        {
            logger.LogWarning("Room {RoomId} was not found", roomId);
            return Result<RoomResponse>.Failure(
                "ROOM_NOT_FOUND",
                $"The requested room with id: {roomId.ToString()} was not found.",
                ResultErrorType.NotFound);
        }

        var imageUrls = room.Images
            .OrderBy(i => i.DisplayOrder)
            .Select(i => i.Url!)
            .ToList();

        return Result<RoomResponse>.Success(room.ToRoomResponse(room.Hotel.Name, false, imageUrls), success: new(
            "ROOM_FOUND", $"The requested room with id: {roomId.ToString()} was found."));
    }
}
