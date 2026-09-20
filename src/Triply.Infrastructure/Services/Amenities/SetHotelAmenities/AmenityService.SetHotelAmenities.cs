using Microsoft.Extensions.Logging;
using Triply.Application.DTOs.Amenities;
using Triply.Application.Extensions;
using Triply.Application.Features.Hotels.Commands.SetHotelAmenities;
using Triply.Domain.Results;
using Triply.Domain.Results.Enums;

namespace Triply.Infrastructure.Services.Amenities;

public partial class AmenityService
{
    /// <summary>Sets the hotel amenities.</summary>
    public async Task<Result<IReadOnlyList<AmenityResponse>>> SetHotelAmenitiesAsync(Guid hotelId,
        SetHotelAmenitiesRequest request, CancellationToken cancellationToken = default)
    {
        var isExists = await hotelRepository.IsHotelIdExistsAsync(hotelId, cancellationToken);
        if (!isExists)
        {
            logger.LogWarning("Set amenities failed: hotel {HotelId} was not found", hotelId);
            return Result<IReadOnlyList<AmenityResponse>>.Failure(
                "HOTEL_NOT_FOUND",
                $"The requested hotel with id: {hotelId.ToString()} was not found.",
                ResultErrorType.NotFound);
        }

        await amenityRepository.ReplaceHotelAmenitiesAsync(hotelId, request.AmenityIds, cancellationToken);
        await amenityRepository.SaveChangesAsync(cancellationToken);

        var amenities = await amenityRepository.GetByHotelIdAsync(hotelId, cancellationToken);
        logger.LogInformation("Amenities of hotel {HotelId} replaced with {AmenitiesCount} amenities: {Amenities}",
            hotelId, amenities.Count, amenities.Select(a => a.Name).ToArray());

        return Result<IReadOnlyList<AmenityResponse>>.Success(
            amenities.Select(a => a.ToAmenityResponse()).ToList(), ResultSuccessType.Ok,
            new ResultSuccess("HOTEL_AMENITIES_UPDATED", "Hotel amenities updated successfully."));
    }
}
