using Triply.Application.DTOs.Amenities;
using Triply.Domain.Entities;

namespace Triply.Application.Extensions;

public static class AmenityExtensions
{
    public static AmenityResponse ToAmenityResponse(this Amenity amenity)
        => new AmenityResponse()
        {
            AmenityId = amenity.AmenityId,
            Name = amenity.Name,
            CreatedAt = amenity.CreatedAt,
            ModifiedAt = amenity.ModifiedAt
        };
}
