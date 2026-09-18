using FluentValidation;
using Triply.Application.DTOs.Amenities;
using Triply.Application.Extensions;
using Triply.Application.Features.Amenities.Commands.CreateAmenity;
using Triply.Application.Features.Amenities.Commands.UpdateAmenity;
using Triply.Application.Features.Hotels.Commands.SetHotelAmenities;
using Triply.Application.Interfaces.Services;
using Triply.Domain.Results;

namespace Triply.Application.Services;

public class ValidatedAmenityService(
    IAmenityService inner,
    IEnumerable<IValidator<CreateAmenityRequest>> createValidators,
    IEnumerable<IValidator<UpdateAmenityRequest>> updateValidators,
    IEnumerable<IValidator<SetHotelAmenitiesRequest>> setHotelAmenitiesValidators) : IAmenityService
{
    public async Task<Result<AmenityResponse>> CreateAsync(CreateAmenityRequest request,
        CancellationToken cancellationToken = default)
    {
        await createValidators.ValidateAndThrowAsync(request, cancellationToken);
        return await inner.CreateAsync(request, cancellationToken);
    }

    public async Task<Result<IReadOnlyList<AmenityResponse>>> GetAllAsync(
        CancellationToken cancellationToken = default)
        => await inner.GetAllAsync(cancellationToken);

    public async Task<Result<AmenityResponse>> GetByIdAsync(Guid amenityId,
        CancellationToken cancellationToken = default)
        => await inner.GetByIdAsync(amenityId, cancellationToken);

    public async Task<Result<AmenityResponse>> UpdateAsync(Guid amenityId, UpdateAmenityRequest request,
        CancellationToken cancellationToken = default)
    {
        await updateValidators.ValidateAndThrowAsync(request, cancellationToken);
        return await inner.UpdateAsync(amenityId, request, cancellationToken);
    }

    public async Task<Result<bool>> DeleteAsync(Guid amenityId, CancellationToken cancellationToken = default)
        => await inner.DeleteAsync(amenityId, cancellationToken);

    public async Task<Result<IReadOnlyList<AmenityResponse>>> GetHotelAmenitiesAsync(Guid hotelId,
        CancellationToken cancellationToken = default)
        => await inner.GetHotelAmenitiesAsync(hotelId, cancellationToken);

    public async Task<Result<IReadOnlyList<AmenityResponse>>> SetHotelAmenitiesAsync(Guid hotelId,
        SetHotelAmenitiesRequest request, CancellationToken cancellationToken = default)
    {
        await setHotelAmenitiesValidators.ValidateAndThrowAsync(request, cancellationToken);
        return await inner.SetHotelAmenitiesAsync(hotelId, request, cancellationToken);
    }
}
