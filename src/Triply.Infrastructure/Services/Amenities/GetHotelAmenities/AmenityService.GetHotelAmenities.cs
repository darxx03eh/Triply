using Triply.Application.DTOs.Amenities;
using Triply.Application.Extensions;
using Triply.Domain.Results;
using Triply.Domain.Results.Enums;

namespace Triply.Infrastructure.Services.Amenities;

public partial class AmenityService
{
    public async Task<Result<IReadOnlyList<AmenityResponse>>> GetHotelAmenitiesAsync(Guid hotelId,
        CancellationToken cancellationToken = default)
    {
        if (!await hotelRepository.IsHotelIdExistsAsync(hotelId, cancellationToken))
            return Result<IReadOnlyList<AmenityResponse>>.Failure(
                "HOTEL_NOT_FOUND",
                $"The requested hotel with id: {hotelId.ToString()} was not found.",
                ResultErrorType.NotFound);

        var amenities = await amenityRepository.GetByHotelIdAsync(hotelId, cancellationToken);

        return Result<IReadOnlyList<AmenityResponse>>.Success(
            amenities.Select(a => a.ToAmenityResponse()).ToList(),
            success: new("HOTEL_AMENITIES_FOUND", "Hotel amenities were retrieved successfully."));
    }
}
