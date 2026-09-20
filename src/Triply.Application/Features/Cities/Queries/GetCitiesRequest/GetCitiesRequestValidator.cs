using FluentValidation;
using Triply.Domain.Results;

namespace Triply.Application.Features.Cities.Queries.GetCitiesRequest;

/// <summary>Validates city filtering and pagination options.</summary>
public class GetCitiesRequestValidator : AbstractValidator<GetCitiesRequest>
{
    /// <summary>Initializes the city query validation rules.</summary>
    public GetCitiesRequestValidator() => ApplyValidationRules();

    /// <summary>Applies the validation rules.</summary>
    public void ApplyValidationRules()
    {
        RuleFor(request => request.Page)
            .GreaterThanOrEqualTo(1)
            .When(request => request.Page.HasValue)
            .WithMessage(ResultResponseMessages.Cities.Validation.PageInvalid.Message);

        RuleFor(request => request.PageSize)
            .InclusiveBetween(1, 50)
            .When(request => request.PageSize.HasValue)
            .WithMessage(ResultResponseMessages.Cities.Validation.PageSizeInvalid.Message);
    }
}