using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Triply.Application.DTOs.Cities;
using Triply.Application.Extensions;
using Triply.Application.Features.Cities.Commands.UpdateCity;
using Triply.Domain.Results;
using Triply.Domain.Results.Enums;

namespace Triply.Infrastructure.Services.Cities;

public partial class CityService
{
    /// <summary>Updates an existing city.</summary>
    public async Task<Result<CityResponse>> UpdateAsync(Guid cityId, UpdateCityRequest request,
        CancellationToken cancellationToken = default)
    {
        var city = await cityRepository.GetByIdAsync(cityId, cancellationToken);
        if (city is null || city.IsDeleted)
        {
            logger.LogWarning("Update city failed: city {CityId} was not found", cityId);
            return Result<CityResponse>.Failure("CITY_NOT_FOUND", 
                $"The requested city with id: {cityId.ToString()} was not found.",
                ResultErrorType.NotFound);
        }

        city.Name = request.Name;
        city.Country = request.Country;
        city.PostOffice = request.PostOffice;
        city.ModifiedAt = DateTime.UtcNow;

        cityRepository.SetOriginalRowVersion(city, request.RowVersion);

        await cityRepository.UpdateAsync(city);

        try
        {
            await cityRepository.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            logger.LogWarning("Update city {CityId} rejected: it was modified by someone else (concurrency conflict)"
                , cityId);
            return Result<CityResponse>.Failure(
                "CITY_CONCURRENCY_CONFLICT",
                $"This city with id: {cityId.ToString()} was modified by someone else. Refresh and try again.",
                ResultErrorType.Conflict);
        }

        logger.LogInformation("City {CityId} updated to {CityName}, {Country}", cityId, city.Name, city.Country);
        var hotelsCount = await cityRepository.GetHotelsCountAsync([cityId], cancellationToken);
        return Result<CityResponse>.Success(
            city.ToCityResponse(hotelsCount.GetValueOrDefault(cityId)), 
            ResultSuccessType.Ok, 
            new ("CITY_UPDATED", "City updated successfully."));
    }
}