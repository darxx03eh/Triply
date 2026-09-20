using Triply.Application.DTOs.Amenities;
using Triply.Domain.Entities;

namespace Triply.Application.Extensions;

/// <summary>Extension methods for amenity.</summary>
public static class AmenityExtensions
{
    /// <summary>Maps the amenity to a amenity response.</summary>
    public static AmenityResponse ToAmenityResponse(this Amenity amenity)
        => new AmenityResponse()
        {
            AmenityId = amenity.AmenityId,
            Name = amenity.Name,
            CreatedAt = amenity.CreatedAt,
            ModifiedAt = amenity.ModifiedAt
        };
}
