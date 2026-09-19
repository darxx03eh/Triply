using FluentValidation;
using Triply.Domain.Results;

namespace Triply.Application.Features.Attractions.Commands.UpdateAttraction;

public class UpdateAttractionRequestValidator : AbstractValidator<UpdateAttractionRequest>
{
    public UpdateAttractionRequestValidator() => ApplyValidationRules();

    private void ApplyValidationRules()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage(ResultResponseMessages.Attractions.Validation.NameRequired.Message)
            .MaximumLength(150).WithMessage(ResultResponseMessages.Attractions.Validation.NameMaxLength.Message);

        RuleFor(x => x.Category)
            .NotEmpty().WithMessage(ResultResponseMessages.Attractions.Validation.CategoryRequired.Message)
            .MaximumLength(50).WithMessage(ResultResponseMessages.Attractions.Validation.CategoryMaxLength.Message);

        RuleFor(x => x.DistanceKm)
            .InclusiveBetween(0, 100)
            .PrecisionScale(5, 2, true)
            .WithMessage(ResultResponseMessages.Attractions.Validation.DistanceInvalid.Message);
    }
}