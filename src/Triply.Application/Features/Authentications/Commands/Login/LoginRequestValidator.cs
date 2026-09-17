using FluentValidation;
using Triply.Domain.Results;

namespace Triply.Application.Features.Authentications.Commands.Login;

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
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