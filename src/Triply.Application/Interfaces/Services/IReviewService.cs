using Triply.Application.Common.Models;
using Triply.Application.DTOs.Reviews;
using Triply.Application.Features.Reviews.Commands.CreateReview;
using Triply.Application.Features.Reviews.Commands.UpdateReview;
using Triply.Application.Features.Reviews.Queries.GetReviews;
using Triply.Domain.Results;

namespace Triply.Application.Interfaces.Services;

public interface IReviewService
{
    Task<Result<ReviewResponse>> CreateAsync(CreateReviewRequest request, CancellationToken cancellationToken = default);

    Task<Result<PagedResult<ReviewResponse>>> GetHotelReviewsAsync(Guid hotelId, GetReviewsRequest request,
        CancellationToken cancellationToken = default);

    Task<Result<ReviewResponse>> UpdateAsync(Guid reviewId, Guid userId, UpdateReviewRequest request,
        CancellationToken cancellationToken = default);

    Task<Result<bool>> DeleteAsync(Guid reviewId, Guid userId, bool isAdmin,
        CancellationToken cancellationToken = default);
}