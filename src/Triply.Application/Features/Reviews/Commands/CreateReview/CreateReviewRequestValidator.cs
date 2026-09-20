using FluentValidation;
using Triply.Application.Interfaces.Repositories;
using Triply.Domain.Results;

namespace Triply.Application.Features.Reviews.Commands.CreateReview;

/// <summary>Validates the create review request.</summary>
public class CreateReviewRequestValidator : AbstractValidator<CreateReviewRequest>
{
    /// <summary>Initializes a new instance of the create review request validator.</summary>
    public CreateReviewRequestValidator(IReviewRepository reviewRepository, IHotelRepository hotelRepository)
    {
        ApplyValidationRules();
        ApplyCustomValidationRules(reviewRepository, hotelRepository);
    }

    private void ApplyValidationRules()
    {
        RuleFor(x => x.Rating)
            .InclusiveBetween((byte)1, (byte)5)
            .WithMessage(ResultResponseMessages.Reviews.Validation.RatingInvalid.Message);

        RuleFor(x => x.Title)
            .MaximumLength(100).WithMessage(ResultResponseMessages.Reviews.Validation.TitleMaxLength.Message)
            .When(x => x.Title is not null);

        RuleFor(x => x.Comment)
            .NotEmpty().WithMessage(ResultResponseMessages.Reviews.Validation.CommentRequired.Message)
            .Length(10, 1000).WithMessage(ResultResponseMessages.Reviews.Validation.CommentLength.Message);
    }

    private void ApplyCustomValidationRules(IReviewRepository reviewRepository, IHotelRepository hotelRepository)
    {
        RuleFor(x => x.HotelId)
            .MustAsync(async (hotelId, cancellationToken) =>
            {
                return await hotelRepository.IsHotelIdExistsAsync(hotelId, cancellationToken);
            }).WithMessage(ResultResponseMessages.Reviews.Validation.HotelNotFound.Message);

        RuleFor(x => x)
            .MustAsync(async (review, cancellationToken) =>
            {
                return !await reviewRepository.IsReviewExistsAsync(review.HotelId, review.UserId,
                    cancellationToken);
            }).WithMessage(ResultResponseMessages.Reviews.Validation.AlreadyReviewed.Message)
            .OverridePropertyName(nameof(CreateReviewRequest.HotelId));
    }
}