using Microsoft.Extensions.Logging;
using Triply.Application.Common.Models;
using Triply.Application.DTOs.Reviews;
using Triply.Application.Extensions;
using Triply.Application.Features.Reviews.Queries.GetReviews;
using Triply.Domain.Results;
using Triply.Domain.Results.Enums;

namespace Triply.Infrastructure.Services.Reviews;

public partial class ReviewService
{
    /// <summary>Gets the hotel reviews.</summary>
    public async Task<Result<PagedResult<ReviewResponse>>> GetHotelReviewsAsync(Guid hotelId, GetReviewsRequest request,
        CancellationToken cancellationToken = default)
    {
        if (!await hotelRepository.IsHotelIdExistsAsync(hotelId, cancellationToken))
        {
            logger.LogWarning("Get reviews failed: hotel {HotelId} was not found", hotelId);
            return Result<PagedResult<ReviewResponse>>.Failure(
                "HOTEL_NOT_FOUND",
                $"The requested hotel with id: {hotelId.ToString()} was not found.",
                ResultErrorType.NotFound);
        }

        var (reviews, totalCount) = await reviewRepository
            .GetHotelReviewsAsync(hotelId, request, cancellationToken);

        logger.LogDebug("Hotel {HotelId} reviews page {Page} returned {Count} of {TotalCount}",
            hotelId, request.Page ?? 1, reviews.Count, totalCount);
        var pagedResult = new PagedResult<ReviewResponse>
        {
            Items = reviews.Select(r => r.ToReviewResponse($"{r.User.FirstName} {r.User.LastName}"))
                .ToList(),
            Page = request.Page ?? 1,
            PageSize = request.PageSize ?? 10,
            TotalCount = totalCount
        };

        return Result<PagedResult<ReviewResponse>>.Success(pagedResult, success: reviews.Count == 0
            ? new("REVIEWS_EMPTY", "This hotel has no reviews yet.")
            : new("REVIEWS_FOUND", "Reviews were found successfully."));
    }
}