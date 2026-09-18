using Triply.Domain.Results;
using Triply.Domain.Results.Enums;

namespace Triply.Infrastructure.Services.Rooms;

public partial class RoomService
{
    public async Task<Result<bool>> DeleteAsync(Guid roomId, CancellationToken cancellationToken = default)
    {
        var room = await roomRepository.GetByIdAsync(roomId, cancellationToken);
        if (room is null)
            return Result<bool>.Failure(
                "ROOM_NOT_FOUND",
                $"The requested room with id: {roomId.ToString()} was not found.",
                ResultErrorType.NotFound);

        room.IsDeleted = true;
        room.ModifiedAt = DateTime.UtcNow;
        await roomRepository.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(
            true, ResultSuccessType.NoContent,
            new ("ROOM_DELETED", "Room deleted successfully."));
    }
}
