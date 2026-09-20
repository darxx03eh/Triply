using Microsoft.Extensions.Logging;
using Triply.Domain.Results;
using Triply.Domain.Results.Enums;

namespace Triply.Infrastructure.Services.Rooms;

public partial class RoomService
{
    /// <summary>Deletes the room.</summary>
    public async Task<Result<bool>> DeleteAsync(Guid roomId, CancellationToken cancellationToken = default)
    {
        var room = await roomRepository.GetByIdAsync(roomId, cancellationToken);
        if (room is null)
        {
            logger.LogWarning("Delete room failed: room {RoomId} was not found", roomId);
            return Result<bool>.Failure(
                "ROOM_NOT_FOUND",
                $"The requested room with id: {roomId.ToString()} was not found.",
                ResultErrorType.NotFound);
        }

        room.IsDeleted = true;
        room.ModifiedAt = DateTime.UtcNow;
        await roomRepository.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Room {RoomId} (#{RoomNumber}) of hotel {HotelId} soft deleted", 
            roomId, room.Number, room.HotelId);

        return Result<bool>.Success(
            true, ResultSuccessType.NoContent,
            new ("ROOM_DELETED", "Room deleted successfully."));
    }
}
