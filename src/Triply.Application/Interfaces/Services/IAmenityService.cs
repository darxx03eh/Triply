using Triply.Application.DTOs.Amenities;
using Triply.Application.Features.Amenities.Commands.CreateAmenity;
using Triply.Application.Features.Amenities.Commands.UpdateAmenity;
using Triply.Application.Features.Hotels.Commands.SetHotelAmenities;
using Triply.Domain.Results;

namespace Triply.Application.Interfaces.Services;

public interface IAmenityService
{
    Task<Result<AmenityResponse>> CreateAsync(CreateAmenityRequest request,
        CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyList<AmenityResponse>>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Result<AmenityResponse>> GetByIdAsync(Guid amenityId, CancellationToken cancellationToken = default);

    Task<Result<AmenityResponse>> UpdateAsync(Guid amenityId, UpdateAmenityRequest request,
        CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteAsync(Guid amenityId, CancellationToken cancellationToken = default);

    Task<Result<IReadOnlyList<AmenityResponse>>> GetHotelAmenitiesAsync(Guid hotelId,
        CancellationToken cancellationToken = default);

    Task<Result<IReadOnlyList<AmenityResponse>>> SetHotelAmenitiesAsync(Guid hotelId,
        SetHotelAmenitiesRequest request, CancellationToken cancellationToken = default);
}
