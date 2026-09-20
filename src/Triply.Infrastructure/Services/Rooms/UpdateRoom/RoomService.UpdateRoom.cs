using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Triply.Application.DTOs.Rooms;
using Triply.Application.Extensions;
using Triply.Application.Features.Rooms.Commands.UpdateRoom;
using Triply.Domain.Results;
using Triply.Domain.Results.Enums;

namespace Triply.Infrastructure.Services.Rooms;

public partial class RoomService
{
    /// <summary>Updates an existing room.</summary>
    public async Task<Result<RoomResponse>> UpdateAsync(Guid roomId, UpdateRoomRequest request,
        CancellationToken cancellationToken = default)
    {
        var room = await roomRepository.GetByIdWithHotelAsync(roomId, cancellationToken);
        if (room is null)
        {
            logger.LogWarning("Update room failed: room {RoomId} was not found", roomId);
            return Result<RoomResponse>.Failure("ROOM_NOT_FOUND",
                $"The requested room with id: {roomId.ToString()} was not found.",
                ResultErrorType.NotFound);
        }

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
            logger.LogWarning("Update room {RoomId} rejected: it was modified by someone else (concurrency conflict)",
                roomId);
            return Result<RoomResponse>.Failure(
                "ROOM_CONCURRENCY_CONFLICT",
                $"This room with id: {roomId.ToString()} was modified by someone else. Refresh and try again.",
                ResultErrorType.Conflict);
        }

        logger.LogInformation("Room {RoomId} (#{RoomNumber}) of hotel {HotelId} updated, price {PricePerNight}/night, available: {IsAvailable}",
            roomId, room.Number, room.HotelId, room.PricePerNight, room.IsAvailable);
        return Result<RoomResponse>.Success(
            room.ToRoomResponse(room.Hotel.Name), ResultSuccessType.Ok,
            new ResultSuccess("ROOM_UPDATED", "Room updated successfully."));
    }
}
