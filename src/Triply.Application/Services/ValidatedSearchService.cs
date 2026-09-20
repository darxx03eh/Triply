using FluentValidation;
using Triply.Application.DTOs.Search;
using Triply.Application.Extensions;
using Triply.Application.Features.Search.Queries.SearchHotels;
using Triply.Application.Interfaces.Services;
using Triply.Domain.Results;

namespace Triply.Application.Services;

/// <summary>Gets or sets the validated search service.</summary>
/// <summary>Validates the requests before delegating to the search service.</summary>
public class ValidatedSearchService(
    ISearchService inner,
    IEnumerable<IValidator<SearchHotelsRequest>> searchValidators) : ISearchService
{
    /// <summary>Searches the hotels.</summary>
    public async Task<Result<SearchHotelsResponse>> SearchHotelsAsync(SearchHotelsRequest request,
        CancellationToken cancellationToken = default)
    {
        await searchValidators.ValidateAndThrowAsync(request, cancellationToken);
        return await inner.SearchHotelsAsync(request, cancellationToken);
    }
}
