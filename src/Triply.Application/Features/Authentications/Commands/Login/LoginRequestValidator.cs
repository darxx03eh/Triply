using FluentValidation;
using Triply.Domain.Results;

namespace Triply.Application.Features.Authentications.Commands.Login;

/// <summary>Validates the login request.</summary>
public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    /// <summary>Initializes a new instance of the login request validator.</summary>
    public LoginRequestValidator() => ApplyValidationRules();
    private void ApplyValidationRules()
    {
        RuleFor(x => x.Identifier)
            .NotEmpty().WithMessage(ResultResponseMessages.Authentication.Validation.Identifier.Message);

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage(ResultResponseMessages.Authentication.Validation.PasswordRequired.Message)
            .MinimumLength(8).WithMessage(ResultResponseMessages.Authentication.Validation.PasswordMinLength.Message);
    }
}