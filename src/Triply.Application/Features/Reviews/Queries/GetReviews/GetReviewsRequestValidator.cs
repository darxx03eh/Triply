using FluentValidation;
using Triply.Domain.Results;

namespace Triply.Application.Features.Reviews.Queries.GetReviews;

public class GetReviewsRequestValidator : AbstractValidator<GetReviewsRequest>
{
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