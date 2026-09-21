using FluentValidation;
using Triply.Domain.Results;

namespace Triply.Application.Features.Bookings.Commands.Checkout;

/// <summary>Validates check-out requests.</summary>
public class CheckoutRequestValidator : AbstractValidator<CheckoutRequest>
{
    /// <summary>Initializes a new instance of the check-out request validator.</summary>
    public CheckoutRequestValidator() => ApplyValidationRules();

    /// <summary>Initializes the check-out validation rules.</summary>
    private void ApplyValidationRules()
    {
        RuleFor(x => x.GuestFullName)
            .NotEmpty().WithMessage(ResultResponseMessages.Bookings.Validation.GuestFullNameRequired.Message)
            .MaximumLength(100).WithMessage(ResultResponseMessages.Bookings.Validation.GuestFullNameMaxLength.Message);

        RuleFor(x => x.GuestEmail)
            .NotEmpty().WithMessage(ResultResponseMessages.Bookings.Validation.GuestEmailRequired.Message)
            .EmailAddress().WithMessage(ResultResponseMessages.Bookings.Validation.GuestEmailInvalid.Message)
            .MaximumLength(256).WithMessage(ResultResponseMessages.Bookings.Validation.GuestEmailInvalid.Message);

        RuleFor(x => x.GuestPhoneNumber)
            .Matches(@"^\+[1-9][0-9]{7,14}$")
            .WithMessage(ResultResponseMessages.Bookings.Validation.GuestPhoneNumberInvalid.Message)
            .When(x => !string.IsNullOrWhiteSpace(x.GuestPhoneNumber));

        RuleFor(x => x.SpecialRequests)
            .MaximumLength(2000).WithMessage(ResultResponseMessages.Bookings.Validation.SpecialRequestsMaxLength.Message)
            .When(x => x.SpecialRequests is not null);
    }
}