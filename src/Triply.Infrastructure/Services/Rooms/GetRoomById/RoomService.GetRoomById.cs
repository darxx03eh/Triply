using Triply.Application.DTOs.Rooms;
using Triply.Application.Extensions;
using Triply.Domain.Results;
using Triply.Domain.Results.Enums;

namespace Triply.Infrastructure.Services.Rooms;

public partial class RoomService
{
    public async Task<Result<RoomResponse>> GetByIdAsync(Guid roomId, CancellationToken cancellationToken = default)
    {
        var room = await roomRepository.GetByIdWithHotelAsync(roomId, cancellationToken);
        if (room is null)
            return Result<RoomResponse>.Failure(
                "ROOM_NOT_FOUND",
                $"The requested room with id: {roomId.ToString()} was not found.",
                ResultErrorType.NotFound);

        return Result<RoomResponse>.Success(room.ToRoomResponse(room.Hotel.Name), success: new(
            "ROOM_FOUND", $"The requested room with id: {roomId.ToString()} was found."));
    }
}
