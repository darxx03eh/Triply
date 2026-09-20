using FluentValidation;
using Triply.Application.DTOs.Amenities;
using Triply.Application.Extensions;
using Triply.Application.Features.Amenities.Commands.CreateAmenity;
using Triply.Application.Features.Amenities.Commands.UpdateAmenity;
using Triply.Application.Features.Hotels.Commands.SetHotelAmenities;
using Triply.Application.Interfaces.Services;
using Triply.Domain.Results;

namespace Triply.Application.Services;

/// <summary>Gets or sets the validated amenity service.</summary>
/// <summary>Validates the requests before delegating to the amenity service.</summary>
public class ValidatedAmenityService(
    IAmenityService inner,
    IEnumerable<IValidator<CreateAmenityRequest>> createValidators,
    IEnumerable<IValidator<UpdateAmenityRequest>> updateValidators,
    IEnumerable<IValidator<SetHotelAmenitiesRequest>> setHotelAmenitiesValidators) : IAmenityService
{
    /// <summary>Creates a new amenity.</summary>
    public async Task<Result<AmenityResponse>> CreateAsync(CreateAmenityRequest request,
        CancellationToken cancellationToken = default)
    {
        await createValidators.ValidateAndThrowAsync(request, cancellationToken);
        return await inner.CreateAsync(request, cancellationToken);
    }

    /// <summary>Gets all amenities.</summary>
    public async Task<Result<IReadOnlyList<AmenityResponse>>> GetAllAsync(
        CancellationToken cancellationToken = default)
        => await inner.GetAllAsync(cancellationToken);

    /// <summary>Gets the amenity by its identifier.</summary>
    public async Task<Result<AmenityResponse>> GetByIdAsync(Guid amenityId,
        CancellationToken cancellationToken = default)
        => await inner.GetByIdAsync(amenityId, cancellationToken);

    /// <summary>Updates an existing amenity.</summary>
    public async Task<Result<AmenityResponse>> UpdateAsync(Guid amenityId, UpdateAmenityRequest request,
        CancellationToken cancellationToken = default)
    {
        await updateValidators.ValidateAndThrowAsync(request, cancellationToken);
        return await inner.UpdateAsync(amenityId, request, cancellationToken);
    }

    /// <summary>Deletes the amenity.</summary>
    public async Task<Result<bool>> DeleteAsync(Guid amenityId, CancellationToken cancellationToken = default)
        => await inner.DeleteAsync(amenityId, cancellationToken);

    /// <summary>Gets the hotel amenities.</summary>
    public async Task<Result<IReadOnlyList<AmenityResponse>>> GetHotelAmenitiesAsync(Guid hotelId,
        CancellationToken cancellationToken = default)
        => await inner.GetHotelAmenitiesAsync(hotelId, cancellationToken);

    /// <summary>Sets the hotel amenities.</summary>
    public async Task<Result<IReadOnlyList<AmenityResponse>>> SetHotelAmenitiesAsync(Guid hotelId,
        SetHotelAmenitiesRequest request, CancellationToken cancellationToken = default)
    {
        await setHotelAmenitiesValidators.ValidateAndThrowAsync(request, cancellationToken);
        return await inner.SetHotelAmenitiesAsync(hotelId, request, cancellationToken);
    }
}
