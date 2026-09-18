using Triply.Application.DTOs.Search;
using Triply.Application.Features.Search.Queries.SearchHotels;

namespace Triply.Application.Interfaces.Repositories;

public interface ISearchRepository
{
    Task<(List<HotelSearchItemResponse> Hotels, int TotalCount)> SearchHotelsAsync(SearchHotelsCriteria criteria,
        CancellationToken cancellationToken = default);
}
