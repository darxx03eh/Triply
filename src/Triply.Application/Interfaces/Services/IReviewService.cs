using Triply.Application.Common.Models;
using Triply.Application.DTOs.Reviews;
using Triply.Application.Features.Reviews.Commands.CreateReview;
using Triply.Application.Features.Reviews.Commands.UpdateReview;
using Triply.Application.Features.Reviews.Queries.GetReviews;
using Triply.Domain.Results;

namespace Triply.Application.Interfaces.Services;

/// <summary>Defines the review operations.</summary>
public interface IReviewService
{
    /// <summary>Creates a new review.</summary>
    Task<Result<ReviewResponse>> CreateAsync(CreateReviewRequest request, CancellationToken cancellationToken = default);

    /// <summary>Gets the hotel reviews.</summary>
    Task<Result<PagedResult<ReviewResponse>>> GetHotelReviewsAsync(Guid hotelId, GetReviewsRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>Updates an existing review.</summary>
    Task<Result<ReviewResponse>> UpdateAsync(Guid reviewId, Guid userId, UpdateReviewRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>Deletes the review.</summary>
    Task<Result<bool>> DeleteAsync(Guid reviewId, Guid userId, bool isAdmin,
        CancellationToken cancellationToken = default);
}