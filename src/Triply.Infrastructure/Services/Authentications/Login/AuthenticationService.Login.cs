using System.Text.RegularExpressions;
using Triply.Application.DTOs.Authentications;
using Triply.Application.Features.Authentications.Commands.Login;
using Triply.Domain.Entities.Identity;
using Triply.Domain.Exceptions;
using Triply.Domain.Results;
using Triply.Domain.Results.Enums;

namespace Triply.Infrastructure.Services.Authentications;
public partial class AuthenticationService
{
    /// <inheritdoc />
    public async Task<Result<LoginResponse>> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var user = await FindUserByIdentifierAsync(request.Identifier, cancellationToken);
        
        if(user is null)
            return Result<LoginResponse>.Failure(
                "INVALID_CREDENTIALS", "Invalid credentials",
                ResultErrorType.Unauthorized);

        if (await userManager.IsLockedOutAsync(user))
            return Result<LoginResponse>.Failure(
                "ACCOUNT_LOCKED",
                $"This account is temporarily locked due to multiple failed login attempts. Please try again after {user.LockoutEnd.Value:u}.",
                ResultErrorType.Forbidden);
        
        var signInResult = await signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true);
        if (!signInResult.Succeeded)
        {
            if (!user.EmailConfirmed)
            {
                await PublishConfirmationEmail(user);
                return Result<LoginResponse>.Failure(
                    "EMAIL_NOT_CONFIRMED",
                    "Your email address has not been confirmed yet. Please check your inbox for the confirmation email.",
                    ResultErrorType.Forbidden);
            }
            if (signInResult.IsLockedOut)
                return Result<LoginResponse>.Failure(
                    "ACCOUNT_LOCKED",
                    $"This account is temporarily locked due to multiple failed login attempts. Please try again after {user.LockoutEnd.Value:u}.",
                    ResultErrorType.Forbidden);
            return Result<LoginResponse>.Failure(
                "INVALID_CREDENTIALS", "Invalid credentials", ResultErrorType.Unauthorized);
        }

        if (!user.IsActive)
            return Result<LoginResponse>.Failure(
                "ACCOUNT_NOT_ACTIVE",
                "Your account is not active. Please contact support for assistance.",
                ResultErrorType.Forbidden);
        var tokens = await tokenService.GenerateAccessTokenAsync(user, flag: true, cancellationToken);

        user.LastLoginAt = DateTime.UtcNow;
        await userManager.UpdateAsync(user);

        var response = new LoginResponse($"{user.FirstName} {user.LastName}", tokens.Access, tokens.Refresh);
        return Result<LoginResponse>.Success(response, success: new(
            ResultResponseMessages.Authentication.Api.LoginSucceeded.Code,
            ResultResponseMessages.Authentication.Api.LoginSucceeded.Message));
    }
    private async Task<TriplyUser?> FindUserByIdentifierAsync(string identifier,
        CancellationToken cancellationToken = default)
    {
        if (identifier.Contains('@'))
        {
            if(IsValidEmail(identifier))
                return await userManager.FindByEmailAsync(identifier);
            throw new InvalidFormatException(ResultResponseMessages.Authentication.Validation.InvalidEmail.Message,
                ResultResponseMessages.Authentication.Validation.InvalidEmail.Code);
        }

        if (IsPhoneNumber(identifier))
        {
            if(IsValidPhoneNumber(identifier))
                return await userRepository.GetByPhoneNumberAsync(identifier, cancellationToken);
            throw new InvalidFormatException(
                ResultResponseMessages.Authentication.Validation.InvalidPhoneNumber.Message,
                ResultResponseMessages.Authentication.Validation.InvalidPhoneNumber.Code);
        }
        
        if(IsValidUsername(identifier))
            return await userManager.FindByNameAsync(identifier);
        throw new InvalidFormatException(ResultResponseMessages.Authentication.Validation.InvalidUsername.Message,
            ResultResponseMessages.Authentication.Validation.InvalidUsername.Code);
    }

    private static bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        return Regex.Match(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$").Success;
    }

    private static bool IsValidUsername(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
            return false;

        return Regex.Match(username, @"^[a-zA-Z0-9._]+$").Success;
    }

    private static bool IsValidPhoneNumber(string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
            return false;
        
        return Regex.Match(phoneNumber, @"^\+[1-9][0-9]{7,14}$").Success;
    }

    private static bool IsPhoneNumber(string identifier)
    {
        var trimmed = identifier.StartsWith('+') ? identifier[1..] : identifier;
        return trimmed.Length > 0 && trimmed.All(char.IsDigit);
    }
}