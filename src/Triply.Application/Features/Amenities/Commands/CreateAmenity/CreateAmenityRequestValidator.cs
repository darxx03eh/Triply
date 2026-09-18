using FluentValidation;
using Triply.Application.Interfaces.Repositories;
using Triply.Domain.Results;

namespace Triply.Application.Features.Amenities.Commands.CreateAmenity;

public class CreateAmenityRequestValidator : AbstractValidator<CreateAmenityRequest>
{
    public CreateAmenityRequestValidator(IAmenityRepository amenityRepository)
    {
        ApplyValidationRules();
        ApplyCustomValidationRules(amenityRepository);
    }

    private void ApplyValidationRules()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage(ResultResponseMessages.Amenities.Validation.NameRequired.Message)
            .Must(name => !string.IsNullOrWhiteSpace(name))
            .WithMessage(ResultResponseMessages.Amenities.Validation.NameWhitespace.Message)
            .MaximumLength(100).WithMessage(ResultResponseMessages.Amenities.Validation.NameMaxLength.Message);
    }

    private void ApplyCustomValidationRules(IAmenityRepository amenityRepository)
    {
        RuleFor(x => x.Name)
            .MustAsync(async (name, cancellationToken) =>
            {
                return !await amenityRepository.IsAmenityExistsAsync(name.Trim(), cancellationToken);
            }).WithMessage(ResultResponseMessages.Amenities.Validation.AmenityAlreadyExists.Message)
            .When(x => !string.IsNullOrWhiteSpace(x.Name));
    }
}
