using FluentValidation;
using Triply.Domain.Results;

namespace Triply.Application.Features.Deals.Commands.UpdateDeal;

/// <summary>Validates the update deal request.</summary>
public class UpdateDealRequestValidator : AbstractValidator<UpdateDealRequest>
{
    /// <summary>Initializes a new instance of the update deal request validator.</summary>
    public UpdateDealRequestValidator() => ApplyValidationRules();

    private void ApplyValidationRules()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage(ResultResponseMessages.Deals.Validation.TitleRequired.Message)
            .MaximumLength(100).WithMessage(ResultResponseMessages.Deals.Validation.TitleMaxLength.Message);

        RuleFor(x => x.DiscountPercentage)
            .GreaterThan(0)
            .LessThanOrEqualTo(90)
            .PrecisionScale(5, 2, true)
            .WithMessage(ResultResponseMessages.Deals.Validation.DiscountInvalid.Message);

        RuleFor(x => x.EndsAt)
            .GreaterThan(x => x.StartsAt).WithMessage(ResultResponseMessages.Deals.Validation.EndsBeforeStarts.Message)
            .GreaterThan(_ => DateTime.UtcNow).WithMessage(ResultResponseMessages.Deals.Validation.EndsInPast.Message);
    }
}