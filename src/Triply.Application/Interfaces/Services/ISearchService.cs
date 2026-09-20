using Triply.Application.DTOs.Search;
using Triply.Application.Features.Search.Queries.SearchHotels;
using Triply.Domain.Results;

namespace Triply.Application.Interfaces.Services;

/// <summary>Defines the search operations.</summary>
public interface ISearchService
{
    /// <summary>Searches the hotels.</summary>
    Task<Result<SearchHotelsResponse>> SearchHotelsAsync(SearchHotelsRequest request,
        CancellationToken cancellationToken = default);
}
