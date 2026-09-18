using Microsoft.EntityFrameworkCore;
using Triply.Application.DTOs.Rooms;
using Triply.Application.Extensions;
using Triply.Application.Features.Rooms.Commands.UpdateRoom;
using Triply.Domain.Results;
using Triply.Domain.Results.Enums;

namespace Triply.Infrastructure.Services.Rooms;

public partial class RoomService
{
    public async Task<Result<RoomResponse>> UpdateAsync(Guid roomId, UpdateRoomRequest request,
        CancellationToken cancellationToken = default)
    {
        var room = await roomRepository.GetByIdWithHotelAsync(roomId, cancellationToken);
        if (room is null)
            return Result<RoomResponse>.Failure("ROOM_NOT_FOUND",
                $"The requested room with id: {roomId.ToString()} was not found.",
                ResultErrorType.NotFound);

        room.Number = request.Number.Trim();
        room.RoomType = request.RoomType;
        room.AdultCapacity = request.AdultCapacity;
        room.ChildCapacity = request.ChildCapacity;
        room.PricePerNight = request.PricePerNight;
        room.IsAvailable = request.IsAvailable;
        room.Description = request.Description;
        room.ModifiedAt = DateTime.UtcNow;

        roomRepository.SetOriginalRowVersion(room, request.RowVersion);

        try
        {
            await roomRepository.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            return Result<RoomResponse>.Failure(
                "ROOM_CONCURRENCY_CONFLICT",
                $"This room with id: {roomId.ToString()} was modified by someone else. Refresh and try again.",
                ResultErrorType.Conflict);
        }

        return Result<RoomResponse>.Success(
            room.ToRoomResponse(room.Hotel.Name), ResultSuccessType.Ok,
            new ResultSuccess("ROOM_UPDATED", "Room updated successfully."));
    }
}
