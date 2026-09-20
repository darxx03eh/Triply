using Triply.Application.DTOs.Reviews;
using Triply.Application.Features.Reviews.Commands.CreateReview;
using Triply.Domain.Entities;

namespace Triply.Application.Extensions;

/// <summary>Extension methods for review.</summary>
public static class ReviewExtensions
{
    /// <summary>Maps the review to a review response.</summary>
    public static ReviewResponse ToReviewResponse(this Review review, string authorName)
        => new ReviewResponse()
        {
            ReviewId = review.ReviewId,
            HotelId = review.HotelId,
            UserId = review.UserId,
            AuthorName = authorName,
            Rating = review.Rating,
            Title = review.Title,
            Comment = review.Comment,
            CreatedAt = review.CreatedAt,
            ModifiedAt = review.ModifiedAt
        };

    /// <summary>Maps the review to a review.</summary>
    public static Review ToReview(this CreateReviewRequest request)
        => new Review()
        {
            HotelId =  request.HotelId,
            UserId = request.UserId,
            Rating =  request.Rating,
            Title = string.IsNullOrWhiteSpace(request.Title) ? null : request.Title.Trim(),
            Comment = request.Comment.Trim()
        };
}