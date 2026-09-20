using Microsoft.Extensions.Logging;
using Triply.Application.DTOs.Rooms;
using Triply.Application.Extensions;
using Triply.Application.Features.Rooms.Commands.CreateRoom;
using Triply.Domain.Results;
using Triply.Domain.Results.Enums;

namespace Triply.Infrastructure.Services.Rooms;

/// <summary>Implements the room operations.</summary>
public partial class RoomService
{
    /// <summary>Creates a new room.</summary>
    public async Task<Result<RoomResponse>> CreateAsync(CreateRoomRequest request,
        CancellationToken cancellationToken = default)
    {
        var hotel = await hotelRepository.GetByIdAsync(request.HotelId, cancellationToken);
        if (hotel is null)
        {
            logger.LogWarning("Create room {RoomNumber} failed: hotel {HotelId} does not exist", 
                request.Number, request.HotelId);
            return Result<RoomResponse>.Failure(
                "HOTEL_NOT_FOUND", 
                $"The specified hotel with id: {request.HotelId.ToString()} does not exist.",
                ResultErrorType.NotFound);
        }

        var room = request.ToRoom();

        await roomRepository.AddAsync(room, cancellationToken);
        await roomRepository.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Room {RoomId} (#{RoomNumber}, {RoomType}, " +
                              "{PricePerNight}/night) created in hotel {HotelId} ({HotelName})",
            room.RoomId, room.Number, room.RoomType, room.PricePerNight, hotel.HotelId, hotel.Name);

        return Result<RoomResponse>.Success(
            room.ToRoomResponse(hotel.Name), ResultSuccessType.Created,
            new ResultSuccess("ROOM_CREATED", "Room created successfully."));
    }
}
