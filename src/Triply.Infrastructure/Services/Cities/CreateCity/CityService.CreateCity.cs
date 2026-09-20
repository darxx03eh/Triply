using Microsoft.Extensions.Logging;
using Triply.Application.DTOs.Cities;
using Triply.Application.Extensions;
using Triply.Application.Features.Cities.Commands.CreateCity;
using Triply.Domain.Entities;
using Triply.Domain.Results;
using Triply.Domain.Results.Enums;

namespace Triply.Infrastructure.Services.Cities;

public partial class CityService
{
    /// <summary>Creates a new city.</summary>
    public async Task<Result<CityResponse>> CreateAsync(CreateCityRequest request,
        CancellationToken cancellationToken = default)
    {
        var city = request.ToCity();

        await cityRepository.AddAsync(city, cancellationToken);
        await cityRepository.SaveChangesAsync(cancellationToken);
        logger.LogInformation("City {CityId} ({CityName}, {Country}) created", city.CityId, city.Name, city.Country);

        return Result<CityResponse>.Success(
            city.ToCityResponse(), ResultSuccessType.Created,
            new ResultSuccess("CITY_CREATED", "City created successfully."));
    }
}