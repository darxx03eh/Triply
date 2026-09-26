namespace Triply.Infrastructure.Services.Reviews;

public partial class ReviewService
{
    /// <summary>Persists the denormalized guest-rating summary after a review changes.</summary>
    private async Task RefreshHotelRatingAsync(Guid hotelId, CancellationToken cancellationToken)
    {
        var hotel = await hotelRepository.GetByIdAsync(hotelId, cancellationToken);
        if (hotel is null)
            return;

        var stats = await hotelRepository.GetReviewStatsAsync(hotelId, cancellationToken);
        hotel.AverageRating = stats.AverageRating;
        hotel.ReviewsCount = stats.ReviewsCount;
        hotel.ModifiedAt = DateTime.UtcNow;

        await hotelRepository.SaveChangesAsync(cancellationToken);
    }
}
