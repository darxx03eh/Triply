using Triply.Application.DTOs.Rooms;
using Triply.Application.Extensions;
using Triply.Application.Features.Rooms.Commands.CreateRoom;
using Triply.Domain.Entities;
using Triply.Domain.Results;
using Triply.Domain.Results.Enums;

namespace Triply.Infrastructure.Services.Rooms;

public partial class RoomService
{
    public async Task<Result<RoomResponse>> CreateAsync(CreateRoomRequest request,
        CancellationToken cancellationToken = default)
    {
        var hotel = await hotelRepository.GetByIdAsync(request.HotelId, cancellationToken);
        if (hotel is null)
            return Result<RoomResponse>.Failure(
                "HOTEL_NOT_FOUND", $"The specified hotel with id: {request.HotelId.ToString()} does not exist.",
                ResultErrorType.NotFound);

        var room = new Room
        {
            HotelId = request.HotelId,
            Number = request.Number.Trim(),
            RoomType = request.RoomType,
            AdultCapacity = request.AdultCapacity,
            ChildCapacity = request.ChildCapacity,
            PricePerNight = request.PricePerNight,
            IsAvailable = request.IsAvailable,
            Description = request.Description
        };

        await roomRepository.AddAsync(room, cancellationToken);
        await roomRepository.SaveChangesAsync(cancellationToken);

        return Result<RoomResponse>.Success(
            room.ToRoomResponse(hotel.Name), ResultSuccessType.Created,
            new ResultSuccess("ROOM_CREATED", "Room created successfully."));
    }
}
