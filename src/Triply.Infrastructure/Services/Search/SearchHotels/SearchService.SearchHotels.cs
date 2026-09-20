using Microsoft.Extensions.Logging;
using Triply.Application.DTOs.Search;
using Triply.Application.Extensions;
using Triply.Application.Features.Search.Queries.SearchHotels;
using Triply.Domain.Results;

namespace Triply.Infrastructure.Services.Search;

/// <summary>Implements the search operations.</summary>
public partial class SearchService
{
    /// <summary>Searches the hotels.</summary>
    public async Task<Result<SearchHotelsResponse>> SearchHotelsAsync(SearchHotelsRequest request,
        CancellationToken cancellationToken = default)
    {
        var criteria = request.ToCriteria();
        var (hotels, totalCount) = await searchRepository
            .SearchHotelsAsync(criteria, cancellationToken);
        logger.LogInformation(
            "Search {Query} in city {CityId} from {CheckIn} to {CheckOut} for {Adults} adults, " +
            "{Children} children, {Rooms} rooms returned {TotalCount} hotels",
            criteria.Q, criteria.CityId, criteria.CheckIn, criteria.CheckOut, criteria.Adults, criteria.Children,
            criteria.Rooms, totalCount);

        var response = criteria.ToSearchHotelsResponse(hotels, totalCount);

        return Result<SearchHotelsResponse>.Success(response, success: totalCount == 0
            ? new("SEARCH_NO_RESULTS", "No hotels are available for the given criteria.")
            : new("SEARCH_RESULTS_FOUND", "Available hotels were found successfully."));
    }
}
