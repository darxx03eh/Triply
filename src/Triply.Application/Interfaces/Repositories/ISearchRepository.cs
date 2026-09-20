using Triply.Application.DTOs.Search;
using Triply.Application.Features.Search.Queries.SearchHotels;

namespace Triply.Application.Interfaces.Repositories;

/// <summary>Defines the persistence operations for search.</summary>
public interface ISearchRepository
{
    /// <summary>Searches the hotels.</summary>
    Task<(List<HotelSearchItemResponse> Hotels, int TotalCount)> SearchHotelsAsync(SearchHotelsCriteria criteria,
        CancellationToken cancellationToken = default);
}
