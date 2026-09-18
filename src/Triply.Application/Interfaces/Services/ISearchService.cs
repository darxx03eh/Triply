using Triply.Application.DTOs.Search;
using Triply.Application.Features.Search.Queries.SearchHotels;
using Triply.Domain.Results;

namespace Triply.Application.Interfaces.Services;

public interface ISearchService
{
    Task<Result<SearchHotelsResponse>> SearchHotelsAsync(SearchHotelsRequest request,
        CancellationToken cancellationToken = default);
}
