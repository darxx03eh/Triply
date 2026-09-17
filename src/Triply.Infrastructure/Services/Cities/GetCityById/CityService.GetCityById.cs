using Triply.Application.DTOs.Cities;
using Triply.Application.Extensions;
using Triply.Domain.Results;
using Triply.Domain.Results.Enums;

namespace Triply.Infrastructure.Services.Cities;

public partial class CityService
{
    public async Task<Result<CityResponse>> GetByIdAsync(Guid cityId, CancellationToken cancellationToken = default)
    {
        var city = await cityRepository.GetByIdAsync(cityId, cancellationToken);
        if (city is null || city.IsDeleted)
            return Result<CityResponse>.Failure(
                "CITY_NOT_FOUND", 
                $"The requested city with id: {cityId.ToString()} was not found.", 
                ResultErrorType.NotFound);

        return Result<CityResponse>.Success(city.ToCityResponse(), success: new(
            "CITY_FOUND", "The requested city was found."));
    }
}