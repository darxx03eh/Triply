using FluentValidation;
using Triply.Application.Interfaces.Repositories;
using Triply.Domain.Results;

namespace Triply.Application.Features.Cities.Commands.UpdateCity;

/// <summary>Validates city update requests, including the concurrency version.</summary>
public class UpdateCityRequestValidator : AbstractValidator<UpdateCityRequest>
{
    /// <summary>Initializes the city update validation rules.</summary>
    public UpdateCityRequestValidator() => ApplyValidationRules();

    private void ApplyValidationRules()
    {
        RuleFor(request => request.Name)
            .NotEmpty()
            .WithMessage(ResultResponseMessages.Cities.Validation.NameRequired.Message)
            .Must(name => !string.IsNullOrWhiteSpace(name))
            .WithMessage(ResultResponseMessages.Cities.Validation.NameWhitespace.Message)
            .MaximumLength(100)
            .WithMessage(ResultResponseMessages.Cities.Validation.NameMaxLength.Message);

        RuleFor(request => request.Country)
            .NotEmpty()
            .WithMessage(ResultResponseMessages.Cities.Validation.CountryRequired.Message)
            .Must(country => !string.IsNullOrWhiteSpace(country))
            .WithMessage(ResultResponseMessages.Cities.Validation.CountryWhitespace.Message)
            .MaximumLength(100)
            .WithMessage(ResultResponseMessages.Cities.Validation.CountryMaxLength.Message);

        RuleFor(request => request.PostOffice)
            .MaximumLength(20)
            .When(request => request.PostOffice is not null)
            .WithMessage(ResultResponseMessages.Cities.Validation.PostOfficeMaxLength.Message);

        RuleFor(request => request.RowVersion)
            .NotEmpty()
            .WithMessage(ResultResponseMessages.Cities.Validation.RowVersionRequired.Message);
    }

    private void ApplyCustomValidationRules(ICityRepository cityRepository)
    {
        RuleFor(request => request)
            .MustAsync(async (city, cancellationTone) =>
            {
                return !await cityRepository.IsCityExistsExcludeId(
                    city.Name, city.Country, city.CityId,
                    cancellationTone);
            }).WithMessage(ResultResponseMessages.Cities.Validation.CityAlreadExistsInThisCountry.Message);
    }
}