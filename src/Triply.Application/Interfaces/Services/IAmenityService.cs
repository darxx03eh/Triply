using Triply.Application.DTOs.Amenities;
using Triply.Application.Features.Amenities.Commands.CreateAmenity;
using Triply.Application.Features.Amenities.Commands.UpdateAmenity;
using Triply.Application.Features.Hotels.Commands.SetHotelAmenities;
using Triply.Domain.Results;

namespace Triply.Application.Interfaces.Services;

/// <summary>Defines the amenity operations.</summary>
public interface IAmenityService
{
    /// <summary>Creates a new amenity.</summary>
    Task<Result<AmenityResponse>> CreateAsync(CreateAmenityRequest request,
        CancellationToken cancellationToken = default);
    /// <summary>Gets all amenities.</summary>
    Task<Result<IReadOnlyList<AmenityResponse>>> GetAllAsync(CancellationToken cancellationToken = default);
    /// <summary>Gets the amenity by its identifier.</summary>
    Task<Result<AmenityResponse>> GetByIdAsync(Guid amenityId, CancellationToken cancellationToken = default);

    /// <summary>Updates an existing amenity.</summary>
    Task<Result<AmenityResponse>> UpdateAsync(Guid amenityId, UpdateAmenityRequest request,
        CancellationToken cancellationToken = default);
    /// <summary>Deletes the amenity.</summary>
    Task<Result<bool>> DeleteAsync(Guid amenityId, CancellationToken cancellationToken = default);

    /// <summary>Gets the hotel amenities.</summary>
    Task<Result<IReadOnlyList<AmenityResponse>>> GetHotelAmenitiesAsync(Guid hotelId,
        CancellationToken cancellationToken = default);

    /// <summary>Sets the hotel amenities.</summary>
    Task<Result<IReadOnlyList<AmenityResponse>>> SetHotelAmenitiesAsync(Guid hotelId,
        SetHotelAmenitiesRequest request, CancellationToken cancellationToken = default);
}
