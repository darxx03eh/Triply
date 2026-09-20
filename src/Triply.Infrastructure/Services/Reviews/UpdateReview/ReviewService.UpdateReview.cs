using Microsoft.Extensions.Logging;
using Triply.Application.DTOs.Reviews;
using Triply.Application.Extensions;
using Triply.Application.Features.Reviews.Commands.UpdateReview;
using Triply.Domain.Results;
using Triply.Domain.Results.Enums;

namespace Triply.Infrastructure.Services.Reviews;

public partial class ReviewService
{
    /// <summary>Updates an existing review.</summary>
    public async Task<Result<ReviewResponse>> UpdateAsync(Guid reviewId, Guid userId, UpdateReviewRequest request,
        CancellationToken cancellationToken = default)
    {
        var review = await reviewRepository.GetByIdWithUserAsync(reviewId, cancellationToken);
        if (review is null)
        {
            logger.LogWarning("Update review failed: review {ReviewId} was not found", reviewId);
            return Result<ReviewResponse>.Failure(
                "REVIEW_NOT_FOUND",
                $"The requested review with id: {reviewId.ToString()} was not found.",
                ResultErrorType.NotFound);
        }

        if (review.UserId != userId)
        {
            logger.LogWarning("User {UserId} tried to edit review {ReviewId} of user {OwnerId}", 
                userId, reviewId, review.UserId);
            return Result<ReviewResponse>.Failure(
                "REVIEW_NOT_OWNED", "You can only edit your own reviews.", ResultErrorType.Forbidden);
        }

        review.Rating = request.Rating;
        review.Title = string.IsNullOrWhiteSpace(request.Title) ? null : request.Title.Trim();
        review.Comment = request.Comment.Trim();
        review.ModifiedAt = DateTime.UtcNow;
        await reviewRepository.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Review {ReviewId} of hotel {HotelId} updated by user {UserId}, rating {Rating}",
            reviewId, review.HotelId, userId, review.Rating);

        return Result<ReviewResponse>.Success(
            review.ToReviewResponse($"{review.User.FirstName} {review.User.LastName}"), ResultSuccessType.Ok,
            new ResultSuccess("REVIEW_UPDATED", "Review updated successfully."));
    }
}