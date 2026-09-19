using Triply.Domain.Results;
using Triply.Domain.Results.Enums;

namespace Triply.Infrastructure.Services.Reviews;

public partial class ReviewService
{
    public async Task<Result<bool>> DeleteAsync(Guid reviewId, Guid userId, bool isAdmin,
        CancellationToken cancellationToken = default)
    {
        var review = await reviewRepository.GetByIdAsync(reviewId, cancellationToken);
        if (review is null)
            return Result<bool>.Failure(
                "REVIEW_NOT_FOUND",
                $"The requested review with id: {reviewId.ToString()} was not found.",
                ResultErrorType.NotFound);

        if (review.UserId != userId && !isAdmin)
            return Result<bool>.Failure(
                "REVIEW_NOT_OWNED", "You can only delete your own reviews.", ResultErrorType.Forbidden);

        await reviewRepository.DeleteAsync(review);
        await reviewRepository.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(
            true, ResultSuccessType.NoContent,
            new ("REVIEW_DELETED", "Review deleted successfully."));
    }
}