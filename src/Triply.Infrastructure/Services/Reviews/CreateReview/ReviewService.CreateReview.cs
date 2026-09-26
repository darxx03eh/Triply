using Microsoft.Extensions.Logging;
using Triply.Application.DTOs.Reviews;
using Triply.Application.Extensions;
using Triply.Application.Features.Reviews.Commands.CreateReview;
using Triply.Domain.Entities;
using Triply.Domain.Results;
using Triply.Domain.Results.Enums;

namespace Triply.Infrastructure.Services.Reviews;

/// <summary>Implements the review operations.</summary>
public partial class ReviewService
{
    /// <summary>Creates a new review.</summary>
    public async Task<Result<ReviewResponse>> CreateAsync(CreateReviewRequest request,
        CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByIdAsync(request.UserId.ToString());
        if (user is null)
        {
            logger.LogWarning("Create review failed: user {UserId} was not found", request.UserId);
            return Result<ReviewResponse>.Failure(
                "USER_NOT_FOUND", "The current user was not found.", ResultErrorType.Unauthorized);
        }

        var hasCompletedBooking = await bookingRepository
            .HasCompletedBookingAtHotelAsync(request.UserId, request.HotelId, cancellationToken);
        if (!hasCompletedBooking)
        {
            logger.LogWarning(
                "Create review failed: user {UserId} has no completed booking at hotel {HotelId}",
                request.UserId, request.HotelId);
            return Result<ReviewResponse>.Failure(
                "REVIEW_REQUIRES_COMPLETED_BOOKING",
                "You can only review hotels where you have completed a stay.",
                ResultErrorType.Forbidden);
        }

        var review = request.ToReview();

        await reviewRepository.AddAsync(review, cancellationToken);
        await reviewRepository.SaveChangesAsync(cancellationToken);
        await RefreshHotelRatingAsync(review.HotelId, cancellationToken);
        logger.LogInformation("Review {ReviewId} ({Rating} stars) added to hotel {HotelId} by user {UserId}",
            review.ReviewId, review.Rating, review.HotelId, review.UserId);

        return Result<ReviewResponse>.Success(
            review.ToReviewResponse($"{user.FirstName} {user.LastName}"), ResultSuccessType.Created,
            new ResultSuccess("REVIEW_CREATED", "Review added successfully."));
    }
}
