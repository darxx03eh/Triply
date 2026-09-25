using FluentValidation;
using Triply.Application.Interfaces.Repositories;
using Triply.Domain.Results;

namespace Triply.Application.Features.Hotels.Commands.SetHotelAmenities;

/// <summary>Validates the set hotel amenities request.</summary>
public class SetHotelAmenitiesRequestValidator : AbstractValidator<SetHotelAmenitiesRequest>
{
    /// <summary>Initializes a new instance of the set hotel amenities request validator.</summary>
    public SetHotelAmenitiesRequestValidator(IAmenityRepository amenityRepository)
    {
        ApplyValidationRules();
        ApplyCustomValidationRules(amenityRepository);
    }

    private void ApplyValidationRules()
    {
        RuleFor(x => x.AmenityIds)
            .NotNull().WithMessage(ResultResponseMessages.Hotels.Validation.AmenityIdsRequired.Message)
            .Must(ids => ids.Distinct().Count() == ids.Count)
            .WithMessage(ResultResponseMessages.Hotels.Validation.AmenityIdsDuplicated.Message)
            .When(x => x.AmenityIds is not null, ApplyConditionTo.CurrentValidator);
    }

    private void ApplyCustomValidationRules(IAmenityRepository amenityRepository)
    {
        RuleFor(x => x.AmenityIds)
            .MustAsync(async (ids, cancellationToken) =>
            {
                return await amenityRepository.CountExistingAsync(ids, cancellationToken) == ids.Distinct().Count();
            }).WithMessage(ResultResponseMessages.Hotels.Validation.AmenityNotFound.Message)
            .When(x => x.AmenityIds is { Count: > 0 });
    }
}
