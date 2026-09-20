using FluentValidation;
using Triply.Application.Interfaces.Repositories;
using Triply.Domain.Results;

namespace Triply.Application.Features.Authentications.Commands.EmailConfirmation;

/// <summary>Validates the confirm email request.</summary>
public class ConfirmEmailRequestValidator : AbstractValidator<ConfirmEmailRequest>
{
    /// <summary>Initializes a new instance of the confirm email request validator.</summary>
    public ConfirmEmailRequestValidator(IUserRepository userRepository)
    {
        ApplyValidationRules();
        ApplyCustomValidationRules(userRepository);
    }
    private void ApplyValidationRules()
    {

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage(ResultResponseMessages.Authentication.Validation.EmailRequired.Message)
            .EmailAddress().WithMessage(ResultResponseMessages.Authentication.Validation.InvalidEmail.Message);

        RuleFor(x => x.Token)
            .NotEmpty().WithMessage(ResultResponseMessages.Authentication.Validation.TokenRequired.Message);
    }
    private void ApplyCustomValidationRules(IUserRepository userRepository)
    {
        RuleFor(x => x.Email)
            .MustAsync(async (email, cancellationToken) =>
            {
                return await userRepository.IsEmailExistsAsync(email, cancellationToken);
            }).WithMessage(ResultResponseMessages.Authentication.Validation.EmailNotExists.Message);
    }
}