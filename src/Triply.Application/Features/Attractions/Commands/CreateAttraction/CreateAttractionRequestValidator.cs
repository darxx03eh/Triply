using FluentValidation;
using Triply.Application.Interfaces.Repositories;
using Triply.Domain.Results;

namespace Triply.Application.Features.Attractions.Commands.CreateAttraction;

public class CreateAttractionRequestValidator : AbstractValidator<CreateAttractionRequest>
{
    public CreateAttractionRequestValidator(IHotelRepository hotelRepository)
    {
        ApplyValidationRules();
        ApplyCustomValidationRules(hotelRepository);
    }

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

    private void ApplyCustomValidationRules(IHotelRepository hotelRepository)
    {
        RuleFor(x => x.HotelId)
            .MustAsync(async (hotelId, cancellationToken) =>
            {
                return await hotelRepository.IsHotelIdExistsAsync(hotelId, cancellationToken);
            }).WithMessage(ResultResponseMessages.Reviews.Validation.HotelNotFound.Message);
    }
}