using Triply.Application.Common.Models;
using Triply.Application.DTOs.Rooms;
using Triply.Application.Extensions;
using Triply.Application.Features.Rooms.Queries.GetRooms;
using Triply.Domain.Results;
using Triply.Domain.Results.Enums;

namespace Triply.Infrastructure.Services.Rooms;

public partial class RoomService
{
    public async Task<Result<PagedResult<RoomResponse>>> GetHotelRoomsAsync(Guid hotelId, GetRoomsRequest request,
        CancellationToken cancellationToken = default)
    {
        if (!await hotelRepository.IsHotelIdExistsAsync(hotelId, cancellationToken))
            return Result<PagedResult<RoomResponse>>.Failure(
                "HOTEL_NOT_FOUND",
                $"The requested hotel with id: {hotelId.ToString()} was not found.",
                ResultErrorType.NotFound);

        var (rooms, totalCount) = await roomRepository.GetPagedAsync(request, isAdmin: false, hotelId,
            cancellationToken);

        var pagedResult = new PagedResult<RoomResponse>
        {
            Items = rooms.Select(r => r.ToRoomResponse(r.Hotel.Name)).ToList(),
            Page = request.Page ?? 1,
            PageSize = request.PageSize ?? 10,
            TotalCount = totalCount
        };

        return Result<PagedResult<RoomResponse>>.Success(pagedResult, success: rooms.Count == 0
            ? new("ROOMS_EMPTY", "This hotel has no rooms matching the given criteria.")
            : new("ROOMS_FOUND", "Rooms were found successfully."));
    }
}
