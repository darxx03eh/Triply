using FluentValidation;
using Triply.Application.Interfaces.Repositories;
using Triply.Domain.Results;

namespace Triply.Application.Features.Authentications.Commands.Register;

/// <summary>Validates the register user request.</summary>
public class RegisterUserRequestValidator : AbstractValidator<RegisterUserRequest>
{
    /// <summary>Initializes a new instance of the register user request validator.</summary>
    public RegisterUserRequestValidator(IUserRepository userRepository)
    {
        ApplyValidationRules();
        ApplyCustomValidationRules(userRepository);
    }

    private void ApplyValidationRules()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage(ResultResponseMessages.Authentication.Validation.FirstNameRequired.Message)
            .MinimumLength(3).WithMessage(ResultResponseMessages.Authentication.Validation.FirstNameMinLength.Message)
            .MaximumLength(100)
            .WithMessage(ResultResponseMessages.Authentication.Validation.FirstNameMaxLength.Message);

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage(ResultResponseMessages.Authentication.Validation.LastNameRequired.Message)
            .MinimumLength(3).WithMessage(ResultResponseMessages.Authentication.Validation.LastNameMinLength.Message)
            .MaximumLength(100).WithMessage(ResultResponseMessages.Authentication.Validation.LastNameMaxLength.Message);

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage(ResultResponseMessages.Authentication.Validation.EmailRequired.Message)
            .EmailAddress().WithMessage(ResultResponseMessages.Authentication.Validation.InvalidEmail.Message);

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage(ResultResponseMessages.Authentication.Validation.PasswordRequired.Message)
            .MinimumLength(8).WithMessage(ResultResponseMessages.Authentication.Validation.PasswordMinLength.Message);

        RuleFor(x => x.ConfirmPassword)
            .NotEmpty().WithMessage(ResultResponseMessages.Authentication.Validation.ConfirmPasswordRequired.Message)
            .Equal(x => x.Password)
            .WithMessage(ResultResponseMessages.Authentication.Validation.PasswordsDoNotMatch.Message);

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage(ResultResponseMessages.Authentication.Validation.PhoneNumberRequired.Message)
            .Matches(@"^\+[1-9][0-9]{7,14}$")
            .WithMessage(ResultResponseMessages.Authentication.Validation.InvalidPhoneNumber.Message);

        RuleFor(x => x.Username)
            .NotEmpty().WithMessage(ResultResponseMessages.Authentication.Validation.UsernameRequired.Message)
            .MinimumLength(3).WithMessage(ResultResponseMessages.Authentication.Validation.UsernameMinLength.Message)
            .Matches(@"^[a-zA-Z0-9._]+$")
            .WithMessage(ResultResponseMessages.Authentication.Validation.UsernameCannotContainAtSymbol.Message);

        RuleFor(x => x.DateOfBirth)
            .Must(dob => dob is null || dob.Value <= DateTime.UtcNow.AddYears(-18))
            .WithMessage(ResultResponseMessages.Authentication.Validation.MinimumAge.Message);
    }
    private void ApplyCustomValidationRules(IUserRepository userRepository)
    {
        RuleFor(x => x.Email)
            .MustAsync(async (email, cancellationToken) =>
            {
                return !await userRepository.IsEmailExistsAsync(email, cancellationToken);
            }).WithMessage(ResultResponseMessages.Authentication.Validation.EmailAlreadyExists.Message);

        RuleFor(x => x.Username)
            .MustAsync(async (username, cancellationToken) =>
            {
                return !await userRepository.IsUsernameExistsAsync(username, cancellationToken);
            }).WithMessage(ResultResponseMessages.Authentication.Validation.UsernameAlreadyExists.Message);
    }
}