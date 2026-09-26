using Microsoft.Extensions.Logging;
using Triply.Domain.Results;
using Triply.Domain.Results.Enums;

namespace Triply.Infrastructure.Services.Reviews;

public partial class ReviewService
{
    /// <summary>Deletes the review.</summary>
    public async Task<Result<bool>> DeleteAsync(Guid reviewId, Guid userId, bool isAdmin,
        CancellationToken cancellationToken = default)
    {
        var review = await reviewRepository.GetByIdAsync(reviewId, cancellationToken);
        if (review is null)
        {
            logger.LogWarning("Delete review failed: review {ReviewId} was not found", reviewId);
            return Result<bool>.Failure(
                "REVIEW_NOT_FOUND",
                $"The requested review with id: {reviewId.ToString()} was not found.",
                ResultErrorType.NotFound);
        }

        if (review.UserId != userId && !isAdmin)
        {
            logger.LogWarning("User {UserId} tried to delete review {ReviewId} of user {OwnerId}", 
                userId, reviewId, review.UserId);
            return Result<bool>.Failure(
                "REVIEW_NOT_OWNED", "You can only delete your own reviews.", ResultErrorType.Forbidden);
        }

        var hotelId = review.HotelId;
        await reviewRepository.DeleteAsync(review);
        await reviewRepository.SaveChangesAsync(cancellationToken);
        await RefreshHotelRatingAsync(hotelId, cancellationToken);
        logger.LogInformation("Review {ReviewId} of hotel {HotelId} deleted by user {UserId} (admin: {IsAdmin})",
            reviewId, review.HotelId, userId, isAdmin);

        return Result<bool>.Success(
            true, ResultSuccessType.NoContent,
            new ("REVIEW_DELETED", "Review deleted successfully."));
    }
}
