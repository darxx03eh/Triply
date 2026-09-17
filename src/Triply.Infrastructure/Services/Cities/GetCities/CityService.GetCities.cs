using Triply.Application.Common.Models;
using Triply.Application.DTOs.Cities;
using Triply.Application.Extensions;
using Triply.Application.Features.Cities.Queries.GetCitiesRequest;
using Triply.Domain.Results;
using Triply.Domain.Results.Enums;

namespace Triply.Infrastructure.Services.Cities;

public partial class CityService
{
    public async Task<Result<PagedResult<CityResponse>>> GetPagedAsync(GetCitiesRequest request,
        bool isAdmin,
        CancellationToken cancellationToken = default)
    {
        var (cities, totalCount) = await cityRepository.GetPagedAsync(request, isAdmin, cancellationToken);
        if (!cities.Any())
            return Result<PagedResult<CityResponse>>.Failure(
                "CITIES_NOT_FOUND", "There is no cities were found.",
                ResultErrorType.NotFound);

        var pagedResult = new PagedResult<CityResponse>
        {
            Items = cities.Select(c => c.ToCityResponse()).ToList(),
            Page = request.Page ?? 1,
            PageSize = request.PageSize ?? 10,
            TotalCount = totalCount
        };

        return Result<PagedResult<CityResponse>>.Success(pagedResult, success: new(
            "CITIES_FOUND", "Cities were found successfully."));
    }
}