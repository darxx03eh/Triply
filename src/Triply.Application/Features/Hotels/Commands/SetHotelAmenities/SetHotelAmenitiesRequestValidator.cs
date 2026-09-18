using FluentValidation;
using Triply.Application.Interfaces.Repositories;
using Triply.Domain.Results;

namespace Triply.Application.Features.Hotels.Commands.SetHotelAmenities;

public class SetHotelAmenitiesRequestValidator : AbstractValidator<SetHotelAmenitiesRequest>
{
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
            .When(x => x.AmenityIds is not null);
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
