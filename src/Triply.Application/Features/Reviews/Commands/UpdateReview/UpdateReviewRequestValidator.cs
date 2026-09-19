using FluentValidation;
using Triply.Domain.Results;

namespace Triply.Application.Features.Reviews.Commands.UpdateReview;

public class UpdateReviewRequestValidator : AbstractValidator<UpdateReviewRequest>
{
    public UpdateReviewRequestValidator() => ApplyValidationRules();

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
}