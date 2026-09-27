using Triply.Api.Extensions;
using Triply.Api.Responses;
using Triply.Application.Common.Models;
using Triply.Application.DTOs.Reviews;
using Triply.Application.Features.Reviews.Commands.CreateReview;
using Triply.Application.Features.Reviews.Commands.UpdateReview;
using Triply.Application.Features.Reviews.Queries.GetReviews;
using Triply.Application.Interfaces.Services;
using Triply.Infrastructure.Routes;

namespace Triply.Api.Endpoints;

/// <summary>Maps the review endpoints.</summary>
public static class ReviewEndpoints
{
    extension(IEndpointRouteBuilder app)
    {
        /// <summary>Maps the review endpoints.</summary>
        public void MapReviewEndpoints()
        {
            var group = app.MapGroup("")
                .WithTags("Reviews");

            group.MapGet(Router.HotelRoutes.Reviews, async (
                    Guid id,
                    [AsParameters] GetReviewsRequest request,
                    IReviewService reviewService,
                    CancellationToken cancellationToken) =>
                {
                    var result = await reviewService.GetHotelReviewsAsync(id, request, cancellationToken);
                    return result.ToMinimalApiResult();
                })
                .WithName("GetHotelReviews")
                .WithDisplayName("Get Hotel Reviews")
                .WithSummary("Retrieves the guest reviews of a hotel")
                .WithDescription("""
                                 Retrieves a paginated list of guest reviews for a specific hotel,
                                 newest first by default (sorts=-Rating, filters=Rating>=4 are supported).
                                 Returns a not found response when the hotel does not exist.
                                 """)
                .Produces<ApiResponse<PagedResult<ReviewResponse>>>(StatusCodes.Status200OK)
                .Produces<ApiResponse<object>>(StatusCodes.Status404NotFound)
                .WithRateLimit(
                    "get-hotel-reviews",
                    60,
                    TimeSpan.FromMinutes(1),
                    ApiResponseMessages.Review.GetHotelReviewsRateLimited);

            group.MapPost(Router.HotelRoutes.Reviews, async (
                    Guid id,
                    CreateReviewRequest request,
                    ICurrentUserAccessor user,
                    IReviewService reviewService,
                    CancellationToken cancellationToken) =>
                {
                    request.HotelId = id;
                    request.UserId = user.UserId;
                    var result = await reviewService.CreateAsync(request, cancellationToken);
                    return result.ToMinimalApiResult();
                }).RequireAuthorization()
                .WithName("CreateReview")
                .WithDisplayName("Create Review")
                .WithSummary("Adds a review to a hotel")
                .WithDescription("""
                                 Adds a rating (1-5) and a comment to a hotel on behalf of the signed-in user.
                                 Each user can review a hotel only once; use the update endpoint to change it.
                                 """)
                .Produces<ApiResponse<ReviewResponse>>(StatusCodes.Status201Created)
                .Produces<ApiResponse<object>>(StatusCodes.Status422UnprocessableEntity)
                .WithRateLimit(
                    "create-review",
                    10,
                    TimeSpan.FromMinutes(1),
                    ApiResponseMessages.Review.CreateRateLimited);

            group.MapPut(Router.ReviewRoutes.Update, async (
                    Guid id,
                    UpdateReviewRequest request,
                    ICurrentUserAccessor user,
                    IReviewService reviewService,
                    CancellationToken cancellationToken) =>
                {
                    var result = await reviewService.UpdateAsync(id, user.UserId, request, cancellationToken);
                    return result.ToMinimalApiResult();
                }).RequireAuthorization()
                .WithName("UpdateReview")
                .WithDisplayName("Update Review")
                .WithSummary("Updates your review")
                .WithDescription("""
                                 Updates the rating, title and comment of a review written by the signed-in user.
                                 Returns forbidden when the review belongs to someone else.
                                 """)
                .Produces<ApiResponse<ReviewResponse>>(StatusCodes.Status200OK)
                .Produces<ApiResponse<object>>(StatusCodes.Status403Forbidden)
                .Produces<ApiResponse<object>>(StatusCodes.Status404NotFound)
                .WithRateLimit(
                    "update-review",
                    10,
                    TimeSpan.FromMinutes(1),
                    ApiResponseMessages.Review.UpdateRateLimited);

            group.MapDelete(Router.ReviewRoutes.Delete, async (
                    Guid id,
                    ICurrentUserAccessor user,
                    IReviewService reviewService,
                    CancellationToken cancellationToken) =>
                {
                    var result = await reviewService.DeleteAsync(id, user.UserId, user.IsAdmin, cancellationToken);
                    return result.ToMinimalApiResult();
                }).RequireAuthorization()
                .WithName("DeleteReview")
                .WithDisplayName("Delete Review")
                .WithSummary("Deletes a review")
                .WithDescription("""
                                 Deletes a review. Users can delete their own reviews and administrators
                                 can delete any review.
                                 """)
                .Produces(StatusCodes.Status204NoContent)
                .Produces<ApiResponse<object>>(StatusCodes.Status403Forbidden)
                .Produces<ApiResponse<object>>(StatusCodes.Status404NotFound)
                .WithRateLimit(
                    "delete-review",
                    10,
                    TimeSpan.FromMinutes(1),
                    ApiResponseMessages.Review.DeleteRateLimited);
        }
    }
}