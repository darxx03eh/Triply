using Microsoft.Extensions.Logging;
using Triply.Application.DTOs.Cities;
using Triply.Application.Extensions;
using Triply.Domain.Results;
using Triply.Domain.Results.Enums;

namespace Triply.Infrastructure.Services.Cities;

public partial class CityService
{
    /// <summary>Gets the city by its identifier.</summary>
    public async Task<Result<CityResponse>> GetByIdAsync(Guid cityId, CancellationToken cancellationToken = default)
    {
        var city = await cityRepository.GetByIdAsync(cityId, cancellationToken);
        if (city is null || city.IsDeleted)
        {
            logger.LogWarning("City {CityId} was not found", cityId);
            return Result<CityResponse>.Failure(
                "CITY_NOT_FOUND",
                $"The requested city with id: {cityId.ToString()} was not found.",
                ResultErrorType.NotFound);
        }

        var hotelsCount = await cityRepository.GetHotelsCountAsync([cityId], cancellationToken);
        return Result<CityResponse>.Success(city.ToCityResponse(hotelsCount.GetValueOrDefault(cityId)), success: new(
            "CITY_FOUND", $"The requested city with id: {cityId} was found."));
    }
}