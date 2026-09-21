using FluentValidation;
using Triply.Domain.Results;

namespace Triply.Application.Features.Bookings.Queries.GetBookings;

/// <summary>Validates booking filtering and pagination options.</summary>
public class GetBookingsRequestValidator : AbstractValidator<GetBookingsRequest>
{
    /// <summary>Initializes the booking query validation rules.</summary>
    public GetBookingsRequestValidator() => ApplyValidationRules();

    /// <summary>Applies the validation rules.</summary>
    private void ApplyValidationRules()
    {
        RuleFor(request => request.Page)
            .GreaterThanOrEqualTo(1)
            .When(request => request.Page.HasValue)
            .WithMessage(ResultResponseMessages.Bookings.Validation.PageInvalid.Message);

        RuleFor(request => request.PageSize)
            .InclusiveBetween(1, 50)
            .When(request => request.PageSize.HasValue)
            .WithMessage(ResultResponseMessages.Bookings.Validation.PageSizeInvalid.Message);
    }
}