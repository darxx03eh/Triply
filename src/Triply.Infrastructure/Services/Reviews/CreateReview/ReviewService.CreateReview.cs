using Triply.Application.DTOs.Reviews;
using Triply.Application.Extensions;
using Triply.Application.Features.Reviews.Commands.CreateReview;
using Triply.Domain.Entities;
using Triply.Domain.Results;
using Triply.Domain.Results.Enums;

namespace Triply.Infrastructure.Services.Reviews;

public partial class ReviewService
{
    public async Task<Result<ReviewResponse>> CreateAsync(CreateReviewRequest request,
        CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
            return Result<ReviewResponse>.Failure(
                "USER_NOT_FOUND", "The current user was not found.", ResultErrorType.Unauthorized);

        var review = request.ToReview();

        await reviewRepository.AddAsync(review, cancellationToken);
        await reviewRepository.SaveChangesAsync(cancellationToken);

        return Result<ReviewResponse>.Success(
            review.ToReviewResponse($"{user.FirstName} {user.LastName}"), ResultSuccessType.Created,
            new ResultSuccess("REVIEW_CREATED", "Review added successfully."));
    }
}