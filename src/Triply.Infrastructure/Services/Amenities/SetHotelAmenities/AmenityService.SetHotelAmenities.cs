using Triply.Application.DTOs.Amenities;
using Triply.Application.Extensions;
using Triply.Application.Features.Hotels.Commands.SetHotelAmenities;
using Triply.Domain.Results;
using Triply.Domain.Results.Enums;

namespace Triply.Infrastructure.Services.Amenities;

public partial class AmenityService
{
    public async Task<Result<IReadOnlyList<AmenityResponse>>> SetHotelAmenitiesAsync(Guid hotelId,
        SetHotelAmenitiesRequest request, CancellationToken cancellationToken = default)
    {
        if (!await hotelRepository.IsHotelIdExistsAsync(hotelId, cancellationToken))
            return Result<IReadOnlyList<AmenityResponse>>.Failure(
                "HOTEL_NOT_FOUND",
                $"The requested hotel with id: {hotelId.ToString()} was not found.",
                ResultErrorType.NotFound);

        await amenityRepository.ReplaceHotelAmenitiesAsync(hotelId, request.AmenityIds, cancellationToken);
        await amenityRepository.SaveChangesAsync(cancellationToken);

        var amenities = await amenityRepository.GetByHotelIdAsync(hotelId, cancellationToken);

        return Result<IReadOnlyList<AmenityResponse>>.Success(
            amenities.Select(a => a.ToAmenityResponse()).ToList(), ResultSuccessType.Ok,
            new ResultSuccess("HOTEL_AMENITIES_UPDATED", "Hotel amenities updated successfully."));
    }
}
