using Microsoft.Extensions.Logging;
using Triply.Application.Common.Models;
using Triply.Application.DTOs.Cities;
using Triply.Application.Extensions;
using Triply.Application.Features.Cities.Queries.GetCitiesRequest;
using Triply.Domain.Results;

namespace Triply.Infrastructure.Services.Cities;

public partial class CityService
{
    /// <summary>Gets a paginated list of cities.</summary>
    public async Task<Result<PagedResult<CityResponse>>> GetPagedAsync(GetCitiesRequest request,
        bool isAdmin,
        CancellationToken cancellationToken = default)
    {
        // An empty page is a valid result, so it returns 200 with no items instead of 404.
        var (cities, totalCount) = await cityRepository.GetPagedAsync(request, isAdmin, cancellationToken);
        var hotelsCount = await cityRepository.
            GetHotelsCountAsync(cities.Select(c => c.CityId), cancellationToken);

        logger.LogDebug("Cities page {Page} returned {Count} of {TotalCount} (admin: {IsAdmin})",
            request.Page ?? 1, cities.Count, totalCount, isAdmin);
        var pagedResult = new PagedResult<CityResponse>
        {
            Items = cities.Select(c => c.ToCityResponse(hotelsCount.GetValueOrDefault(c.CityId))).ToList(),
            Page = request.Page ?? 1,
            PageSize = request.PageSize ?? 10,
            TotalCount = totalCount
        };

        return Result<PagedResult<CityResponse>>.Success(pagedResult, success: cities.Count == 0
            ? new("CITIES_EMPTY", "No cities match the given criteria.")
            : new("CITIES_FOUND", "Cities were found successfully."));
    }
}