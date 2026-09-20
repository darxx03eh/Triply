using Microsoft.Extensions.Logging;
using Triply.Application.DTOs.Hotels;
using Triply.Application.Extensions;
using Triply.Domain.Results;
using Triply.Domain.Results.Enums;

namespace Triply.Infrastructure.Services.Hotels;

public partial class HotelService
{
    /// <summary>Gets the images.</summary>
    public async Task<Result<IReadOnlyList<HotelImageResponse>>> GetImagesAsync(
        Guid hotelId, CancellationToken cancellationToken = default)
    {
        var hotel = await hotelRepository.GetByIdAsync(hotelId, cancellationToken);
        if (hotel is null)
        {
            logger.LogWarning("Get images failed: hotel {HotelId} was not found", hotelId);
            return Result<IReadOnlyList<HotelImageResponse>>.Failure(
                "HOTEL_NOT_FOUND", "The specified hotel does not exist.", ResultErrorType.NotFound);
        }

        var images = await imageRepository.GetByHotelIdAsync(hotelId, cancellationToken);

        return Result<IReadOnlyList<HotelImageResponse>>.Success(
            images.Select(i => i.ToHotelImageResponse()).ToList(),
            success: new ResultSuccess("HOTEL_IMAGES_FOUND", "Hotel images were retrieved successfully."));
    }
}
