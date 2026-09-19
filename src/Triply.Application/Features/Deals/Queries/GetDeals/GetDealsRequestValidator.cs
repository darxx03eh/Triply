using FluentValidation;
using Triply.Domain.Results;

namespace Triply.Application.Features.Deals.Queries.GetDeals;

public class GetDealsRequestValidator : AbstractValidator<GetDealsRequest>
{
    public GetDealsRequestValidator() => ApplyValidationRules();

    private void ApplyValidationRules()
    {
        RuleFor(request => request.Page)
            .GreaterThanOrEqualTo(1)
            .When(request => request.Page.HasValue)
            .WithMessage(ResultResponseMessages.Deals.Validation.PageInvalid.Message);

        RuleFor(request => request.PageSize)
            .InclusiveBetween(1, 50)
            .When(request => request.PageSize.HasValue)
            .WithMessage(ResultResponseMessages.Deals.Validation.PageSizeInvalid.Message);
    }
}