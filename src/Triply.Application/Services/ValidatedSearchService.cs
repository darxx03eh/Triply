using FluentValidation;
using Triply.Application.DTOs.Search;
using Triply.Application.Extensions;
using Triply.Application.Features.Search.Queries.SearchHotels;
using Triply.Application.Interfaces.Services;
using Triply.Domain.Results;

namespace Triply.Application.Services;

public class ValidatedSearchService(
    ISearchService inner,
    IEnumerable<IValidator<SearchHotelsRequest>> searchValidators) : ISearchService
{
    public async Task<Result<SearchHotelsResponse>> SearchHotelsAsync(SearchHotelsRequest request,
        CancellationToken cancellationToken = default)
    {
        await searchValidators.ValidateAndThrowAsync(request, cancellationToken);
        return await inner.SearchHotelsAsync(request, cancellationToken);
    }
}
