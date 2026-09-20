using Microsoft.Extensions.Logging;
using Triply.Application.Common.Models;
using Triply.Application.DTOs.Hotels;
using Triply.Application.Extensions;
using Triply.Application.Features.Hotels.Queries.GetHotels;
using Triply.Domain.Results;

namespace Triply.Infrastructure.Services.Hotels;

public partial class HotelService
{
    /// <summary>Gets a paginated list of hotels.</summary>
    public async Task<Result<PagedResult<HotelSummaryResponse>>> GetPagedAsync(GetHotelsRequest request, bool isAdmin,
        CancellationToken cancellationToken = default)
    {
        // An empty page is a valid result (e.g. a search with no matches or scrolling past the end),
        // so it returns 200 with no items instead of 404.
        var (hotels, totalCount) = await hotelRepository.GetPagedAsync(request, isAdmin, cancellationToken);
        var hotelIds = hotels.Select(h => h.HotelId).ToList();
        var roomsCount = await hotelRepository.GetRoomsCountAsync(hotelIds, cancellationToken);
        var thumbnails = await hotelRepository.GetThumbnailsAsync(hotelIds, cancellationToken);
        logger.LogDebug("Hotels page {Page} returned {Count} of {TotalCount} (admin: {IsAdmin})",
            request.Page ?? 1, hotels.Count, totalCount, isAdmin);
        var pagedResult = new PagedResult<HotelSummaryResponse>
        {
            Items = hotels.Select(h => h.ToHotelSummaryResponse(h.City.Name,
                roomsCount.GetValueOrDefault(h.HotelId), thumbnails.GetValueOrDefault(h.HotelId))).ToList(),
            Page = request.Page ?? 1,
            PageSize = request.PageSize ?? 10,
            TotalCount = totalCount
        };

        return Result<PagedResult<HotelSummaryResponse>>.Success(pagedResult, success: hotels.Count == 0
            ? new("HOTELS_EMPTY", "No hotels match the given criteria.")
            : new("HOTELS_FOUND", "Hotels were found successfully."));
    }
}