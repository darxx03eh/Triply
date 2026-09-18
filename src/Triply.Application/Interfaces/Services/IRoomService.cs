using Triply.Application.Common.Models;
using Triply.Application.DTOs.Rooms;
using Triply.Application.Features.Rooms.Commands.CreateRoom;
using Triply.Application.Features.Rooms.Commands.UpdateRoom;
using Triply.Application.Features.Rooms.Queries.GetRooms;
using Triply.Domain.Results;

namespace Triply.Application.Interfaces.Services;

public interface IRoomService
{
    Task<Result<RoomResponse>> CreateAsync(CreateRoomRequest request, CancellationToken cancellationToken = default);
    Task<Result<RoomResponse>> GetByIdAsync(Guid roomId, CancellationToken cancellationToken = default);

    Task<Result<PagedResult<RoomResponse>>> GetPagedAsync(GetRoomsRequest request,
        bool isAdmin,
        CancellationToken cancellationToken = default);

    Task<Result<PagedResult<RoomResponse>>> GetHotelRoomsAsync(Guid hotelId, GetRoomsRequest request,
        CancellationToken cancellationToken = default);

    Task<Result<RoomResponse>> UpdateAsync(Guid roomId, UpdateRoomRequest request,
        CancellationToken cancellationToken = default);

    Task<Result<bool>> DeleteAsync(Guid roomId, CancellationToken cancellationToken = default);
}
