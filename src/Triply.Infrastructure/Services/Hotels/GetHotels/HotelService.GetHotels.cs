using Triply.Application.Common.Models;
using Triply.Application.DTOs.Hotels;
using Triply.Application.Extensions;
using Triply.Application.Features.Hotels.Queries.GetHotels;
using Triply.Domain.Results;
using Triply.Domain.Results.Enums;

namespace Triply.Infrastructure.Services.Hotels;

public partial class HotelService
{
    public async Task<Result<PagedResult<HotelSummaryResponse>>> GetPagedAsync(GetHotelsRequest request, bool isAdmin,
        CancellationToken cancellationToken = default)
    {
        var (hotels, totalCount) = await hotelRepository.GetPagedAsync(request, isAdmin, cancellationToken);
        if (!hotels.Any())
            return Result<PagedResult<HotelSummaryResponse>>.Failure(
                "HOTELS_NOT_FOUND", "There is no hotels were found.",
                ResultErrorType.NotFound);
        var pagedResult = new PagedResult<HotelSummaryResponse>
        {
            Items = hotels.Select(h => h.ToHotelSummaryResponse(h.City.Name)).ToList(),
            Page = request.Page ?? 1,
            PageSize = request.PageSize ?? 10,
            TotalCount = totalCount
        };

        return Result<PagedResult<HotelSummaryResponse>>.Success(pagedResult, success: new(
            "HOTELS_FOUND", "Hotels were found successfully."));
    }
}