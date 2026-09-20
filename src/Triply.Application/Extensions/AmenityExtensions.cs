using Triply.Application.DTOs.Amenities;
using Triply.Application.Features.Amenities.Commands.CreateAmenity;
using Triply.Domain.Entities;

namespace Triply.Application.Extensions;

/// <summary>Extension methods for amenity.</summary>
public static class AmenityExtensions
{
    /// <summary>Maps the amenity to an amenity response.</summary>
    public static AmenityResponse ToAmenityResponse(this Amenity amenity)
        => new AmenityResponse()
        {
            AmenityId = amenity.AmenityId,
            Name = amenity.Name,
            CreatedAt = amenity.CreatedAt,
            ModifiedAt = amenity.ModifiedAt
        };
    
    /// <summary>Maps the Create Amenity Request to an amenity.</summary>
    public static Amenity ToAmenity(this CreateAmenityRequest request)
        => new Amenity() { Name = request.Name.Trim() };
}
