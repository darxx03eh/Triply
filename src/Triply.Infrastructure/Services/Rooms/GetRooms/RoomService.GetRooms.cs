using Microsoft.Extensions.Logging;
using Triply.Application.Common.Models;
using Triply.Application.DTOs.Rooms;
using Triply.Application.Extensions;
using Triply.Application.Features.Rooms.Queries.GetRooms;
using Triply.Domain.Results;

namespace Triply.Infrastructure.Services.Rooms;

public partial class RoomService
{
    /// <summary>Gets a paginated list of rooms.</summary>
    public async Task<Result<PagedResult<RoomResponse>>> GetPagedAsync(GetRoomsRequest request,
        bool isAdmin,
        CancellationToken cancellationToken = default)
    {
        var (rooms, totalCount) = await roomRepository.GetPagedAsync(request, isAdmin,
            cancellationToken: cancellationToken);

        logger.LogDebug("Rooms page {Page} returned {Count} of {TotalCount} (admin: {IsAdmin})",
            request.Page ?? 1, rooms.Count, totalCount, isAdmin);
        var pagedResult = new PagedResult<RoomResponse>
        {
            Items = rooms.Select(r => r.ToRoomResponse(r.Hotel.Name)).ToList(),
            Page = request.Page ?? 1,
            PageSize = request.PageSize ?? 10,
            TotalCount = totalCount
        };

        return Result<PagedResult<RoomResponse>>.Success(pagedResult, success: rooms.Count == 0
            ? new("ROOMS_EMPTY", "No rooms match the given criteria.")
            : new("ROOMS_FOUND", "Rooms were found successfully."));
    }
}
