using FluentValidation;
using Triply.Application.Interfaces.Repositories;
using Triply.Domain.Results;

namespace Triply.Application.Features.Cities.Commands.CreateCity;

/// <summary>Validates city creation requests.</summary>
public class CreateCityRequestValidator : AbstractValidator<CreateCityRequest>
{
    /// <summary>Initializes a new instance of the create city request validator.</summary>
    public CreateCityRequestValidator(ICityRepository cityRepository)
    {
        ApplyValidationRules();
        ApplyCustomValidationRules(cityRepository);
    }

    /// <summary>Initializes the city creation validation rules.</summary>
    private void ApplyValidationRules()
    {
        RuleFor(request => request.Name)
            .NotEmpty().WithMessage(ResultResponseMessages.Cities.Validation.NameRequired.Message)
            .Must(name => !string.IsNullOrWhiteSpace(name))
            .WithMessage(ResultResponseMessages.Cities.Validation.NameWhitespace.Message)
            .MaximumLength(100)
            .WithMessage(ResultResponseMessages.Cities.Validation.NameMaxLength.Message);

        RuleFor(request => request.Country)
            .NotEmpty().WithMessage(ResultResponseMessages.Cities.Validation.CountryRequired.Message)
            .Must(country => !string.IsNullOrWhiteSpace(country))
            .WithMessage(ResultResponseMessages.Cities.Validation.CountryWhitespace.Message)
            .MaximumLength(100)
            .WithMessage(ResultResponseMessages.Cities.Validation.CountryMaxLength.Message);

        RuleFor(request => request.PostOffice)
            .MaximumLength(20)
            .When(request => request.PostOffice is not null)
            .WithMessage(ResultResponseMessages.Cities.Validation.PostOfficeMaxLength.Message);
    }

    private void ApplyCustomValidationRules(ICityRepository cityRepository)
    {
        RuleFor(request => request)
            .MustAsync(async (city, cancellation) =>
            {
                return !await cityRepository.IsCityExistsAsync(city.Name, city.Country, cancellation);
            }).WithMessage(ResultResponseMessages.Cities.Validation.CityAlreadExistsInThisCountry.Message)
            .OverridePropertyName(nameof(CreateCityRequest.Name));
    }
}