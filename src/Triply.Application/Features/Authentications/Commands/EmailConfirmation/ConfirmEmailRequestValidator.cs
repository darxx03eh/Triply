using FluentValidation;
using Triply.Application.Interfaces.Repositories;
using Triply.Domain.Results;

namespace Triply.Application.Features.Authentications.Commands.EmailConfirmation;

public class ConfirmEmailRequestValidator : AbstractValidator<ConfirmEmailRequest>
{
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