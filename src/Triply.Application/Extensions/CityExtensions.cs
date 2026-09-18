using Triply.Application.DTOs.Cities;
using Triply.Domain.Entities;

namespace Triply.Application.Extensions;

public static class CityExtensions
{
    public static CityResponse ToCityResponse(this City city)
        => new CityResponse()
        {
            CityId =  city.CityId,
            Name = city.Name,
            Country = city.Country,
            PostOffice = city.PostOffice,
            IsDeleted =  city.IsDeleted,
            CreatedAt = city.CreatedAt,
            ModifiedAt = city.ModifiedAt,
        };
}