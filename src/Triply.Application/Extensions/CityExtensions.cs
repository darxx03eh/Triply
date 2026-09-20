using Triply.Application.DTOs.Cities;
using Triply.Application.Features.Cities.Commands.CreateCity;
using Triply.Domain.Entities;

namespace Triply.Application.Extensions;

/// <summary>Extension methods for city.</summary>
public static class CityExtensions
{
    /// <summary>Maps the city to a city response.</summary>
    public static CityResponse ToCityResponse(this City city, int hotelsCount = 0)
        => new CityResponse()
        {
            CityId =  city.CityId,
            Name = city.Name,
            Country = city.Country,
            PostOffice = city.PostOffice,
            HotelsCount = hotelsCount,
            ThumbnailUrl = city.ThumbnailUrl,
            IsDeleted =  city.IsDeleted,
            CreatedAt = city.CreatedAt,
            ModifiedAt = city.ModifiedAt,
            RowVersion = city.RowVersion,
        };

    /// <summary>Maps the Create city request to a city.</summary>
    public static City ToCity(this CreateCityRequest request)
        => new City()
        {
            Name = request.Name.Trim(),
            Country = request.Country.Trim(),
            PostOffice = request.PostOffice.Trim(),
        };
}