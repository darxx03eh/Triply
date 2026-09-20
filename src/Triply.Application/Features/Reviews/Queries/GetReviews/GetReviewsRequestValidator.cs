using FluentValidation;
using Triply.Domain.Results;

namespace Triply.Application.Features.Reviews.Queries.GetReviews;

/// <summary>Validates the get reviews request.</summary>
public class GetReviewsRequestValidator : AbstractValidator<GetReviewsRequest>
{
    /// <summary>Initializes a new instance of the get reviews request validator.</summary>
    public GetReviewsRequestValidator() => ApplyValidationRules();

    private void ApplyValidationRules()
    {
        RuleFor(request => request.Page)
            .GreaterThanOrEqualTo(1)
            .When(request => request.Page.HasValue)
            .WithMessage(ResultResponseMessages.Reviews.Validation.PageInvalid.Message);

        RuleFor(request => request.PageSize)
            .InclusiveBetween(1, 50)
            .When(request => request.PageSize.HasValue)
            .WithMessage(ResultResponseMessages.Reviews.Validation.PageSizeInvalid.Message);
    }
}