using Triply.Application.DTOs.Search;
using Triply.Application.Extensions;
using Triply.Application.Features.Search.Queries.SearchHotels;
using Triply.Domain.Results;

namespace Triply.Infrastructure.Services.Search;

public partial class SearchService
{
    public async Task<Result<SearchHotelsResponse>> SearchHotelsAsync(SearchHotelsRequest request,
        CancellationToken cancellationToken = default)
    {
        var criteria = request.ToCriteria();
        var (hotels, totalCount) = await searchRepository.SearchHotelsAsync(criteria, cancellationToken);

        var response = new SearchHotelsResponse
        {
            Items = hotels.Select(h => h with { TotalPrice = h.MinPricePerNight * criteria.Nights * criteria.Rooms })
                .ToList(),
            Page = criteria.Page,
            PageSize = criteria.PageSize,
            TotalCount = totalCount,
            CheckIn = criteria.CheckIn,
            CheckOut = criteria.CheckOut,
            Nights = criteria.Nights,
            Adults = criteria.Adults,
            Children = criteria.Children,
            Rooms = criteria.Rooms
        };

        return Result<SearchHotelsResponse>.Success(response, success: totalCount == 0
            ? new("SEARCH_NO_RESULTS", "No hotels are available for the given criteria.")
            : new("SEARCH_RESULTS_FOUND", "Available hotels were found successfully."));
    }
}
