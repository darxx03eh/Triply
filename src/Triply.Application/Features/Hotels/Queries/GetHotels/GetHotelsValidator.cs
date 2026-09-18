using FluentValidation;
using Triply.Domain.Results;

namespace Triply.Application.Features.Hotels.Queries.GetHotels;

public class GetHotelsValidator : AbstractValidator<GetHotelsRequest>
{
    public GetHotelsValidator() => ApplyValidationRules();
    public void ApplyValidationRules()
    {
        RuleFor(request => request.Page)
            .GreaterThanOrEqualTo(1)
            .When(request => request.Page.HasValue)
            .WithMessage(ResultResponseMessages.Hotels.Validation.PageInvalid.Message);

        RuleFor(request => request.PageSize)
            .InclusiveBetween(1, 50)
            .When(request => request.PageSize.HasValue)
            .WithMessage(ResultResponseMessages.Hotels.Validation.PageSizeInvalid.Message);
    }
}