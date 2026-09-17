using Triply.Application.DTOs.Cities;
using Triply.Application.Extensions;
using Triply.Application.Features.Cities.Commands.CreateCity;
using Triply.Domain.Entities;
using Triply.Domain.Results;
using Triply.Domain.Results.Enums;

namespace Triply.Infrastructure.Services.Cities;

public partial class CityService
{
    public async Task<Result<CityResponse>> CreateAsync(CreateCityRequest request, CancellationToken cancellationToken = default)
    {
        var city = new City
        {
            Name = request.Name,
            Country = request.Country,
            PostOffice = request.PostOffice
        };

        await cityRepository.AddAsync(city, cancellationToken);
        await cityRepository.SaveChangesAsync(cancellationToken);

        return Result<CityResponse>.Success(
            city.ToCityResponse(), ResultSuccessType.Created,
            new ResultSuccess("CITY_CREATED", "City created successfully."));
    }
}