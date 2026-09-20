using Microsoft.Extensions.Logging;
using Triply.Application.DTOs.Hotels;
using Triply.Application.Extensions;
using Triply.Domain.Results;
using Triply.Domain.Results.Enums;

namespace Triply.Infrastructure.Services.Hotels;

public partial class HotelService
{
    /// <summary>Gets the hotel by its identifier.</summary>
    public async Task<Result<HotelResponse>> GetByIdAsync(Guid hotelId, CancellationToken cancellationToken = default)
    {
        var hotel = await hotelRepository.GetByIdWithImagesAsync(hotelId, cancellationToken);
        if (hotel is null)
        {
            logger.LogWarning("Hotel {HotelId} was not found", hotelId);
            return Result<HotelResponse>.Failure(
                "HOTEL_NOT_FOUND",
                $"The requested hotel with id: {hotelId.ToString()} was not found.",
                ResultErrorType.NotFound);
        }

        var imageUrls = hotel.Images
            .OrderBy(i => i.DisplayOrder)
            .Select(i => i.Url!)
            .ToList();

        var stats = await hotelRepository.GetReviewStatsAsync(hotelId, cancellationToken);
        logger.LogDebug("Hotel {HotelId} loaded with {ImagesCount} images and {ReviewsCount} reviews",
            hotelId, imageUrls.Count, stats.ReviewsCount);
        return Result<HotelResponse>.Success(hotel.ToHotelResponse(
                hotel.City.Name, imageUrls,
                stats.AverageRating, stats.ReviewsCount),
            success: new(
                "HOTEL_FOUND", 
                $"The requested hotel with id: {hotelId.ToString()} was found."));
    }
}