using Triply.Application.Features.Rooms.Queries.GetRooms;

namespace Triply.Infrastructure.Services.Rooms;

public partial class RoomService
{
    private Task<bool> IsBookedForRequestedStayAsync(Guid roomId, GetRoomsRequest request,
        CancellationToken cancellationToken)
    {
        if (request.CheckIn is not { } checkIn || request.CheckOut is not { } checkOut)
            return Task.FromResult(false);

        return roomRepository.IsBookedAsync(roomId,
            checkIn.ToDateTime(TimeOnly.MinValue), checkOut.ToDateTime(TimeOnly.MinValue), cancellationToken);
    }
}
