using FluentValidation;
using Triply.Application.Interfaces.Repositories;
using Triply.Domain.Results;

namespace Triply.Application.Features.Amenities.Commands.UpdateAmenity;

/// <summary>Validates the update amenity request.</summary>
public class UpdateAmenityRequestValidator : AbstractValidator<UpdateAmenityRequest>
{
    /// <summary>Initializes a new instance of the update amenity request validator.</summary>
    public UpdateAmenityRequestValidator(IAmenityRepository amenityRepository)
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
        RuleFor(x => x)
            .MustAsync(async (amenity, cancellationToken) =>
            {
                return !await amenityRepository.IsAmenityExistsExcludeIdAsync(
                    amenity.Name.Trim(), amenity.AmenityId, cancellationToken);
            }).WithMessage(ResultResponseMessages.Amenities.Validation.AmenityAlreadyExists.Message)
            .OverridePropertyName(nameof(UpdateAmenityRequest.Name))
            .When(x => !string.IsNullOrWhiteSpace(x.Name));
    }
}
