using FluentValidation;
using Triply.Application.Common.Models;
using Triply.Application.DTOs.Reviews;
using Triply.Application.Extensions;
using Triply.Application.Features.Reviews.Commands.CreateReview;
using Triply.Application.Features.Reviews.Commands.UpdateReview;
using Triply.Application.Features.Reviews.Queries.GetReviews;
using Triply.Application.Interfaces.Services;
using Triply.Domain.Results;

namespace Triply.Application.Services;

/// <summary>Gets or sets the validated review service.</summary>
/// <summary>Validates the requests before delegating to the review service.</summary>
public class ValidatedReviewService(
    IReviewService inner,
    IEnumerable<IValidator<CreateReviewRequest>> createValidators,
    IEnumerable<IValidator<UpdateReviewRequest>> updateValidators,
    IEnumerable<IValidator<GetReviewsRequest>> getReviewsValidators) : IReviewService
{
    /// <summary>Creates a new review.</summary>
    public async Task<Result<ReviewResponse>> CreateAsync(CreateReviewRequest request,
        CancellationToken cancellationToken = default)
    {
        await createValidators.ValidateAndThrowAsync(request, cancellationToken);
        return await inner.CreateAsync(request, cancellationToken);
    }

    /// <summary>Gets the hotel reviews.</summary>
    public async Task<Result<PagedResult<ReviewResponse>>> GetHotelReviewsAsync(Guid hotelId, GetReviewsRequest request,
        CancellationToken cancellationToken = default)
    {
        await getReviewsValidators.ValidateAndThrowAsync(request, cancellationToken);
        return await inner.GetHotelReviewsAsync(hotelId, request, cancellationToken);
    }

    /// <summary>Updates an existing review.</summary>
    public async Task<Result<ReviewResponse>> UpdateAsync(Guid reviewId, Guid userId, UpdateReviewRequest request,
        CancellationToken cancellationToken = default)
    {
        await updateValidators.ValidateAndThrowAsync(request, cancellationToken);
        return await inner.UpdateAsync(reviewId, userId, request, cancellationToken);
    }

    /// <summary>Deletes the review.</summary>
    public async Task<Result<bool>> DeleteAsync(Guid reviewId, Guid userId, bool isAdmin,
        CancellationToken cancellationToken = default)
        => await inner.DeleteAsync(reviewId, userId, isAdmin, cancellationToken);
}
