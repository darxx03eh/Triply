using System.Security.Claims;
using FluentValidation;
using Triply.Application.DTOs.Authentications;
using Triply.Application.Extensions;
using Triply.Application.Features.Authentications.Commands.EmailConfirmation;
using Triply.Application.Features.Authentications.Commands.Login;
using Triply.Application.Features.Authentications.Commands.Register;
using Triply.Application.Interfaces.Services;
using Triply.Domain.Results;

namespace Triply.Application.Services;

/// <summary>Gets or sets the validated authentication service.</summary>
/// <summary>Validates the requests before delegating to the authentication service.</summary>
public class ValidatedAuthenticationService(
    IAuthenticationService inner,
    IEnumerable<IValidator<RegisterUserRequest>> registerValidator,
    IEnumerable<IValidator<ConfirmEmailRequest>> confirmEmailValidator,
    IEnumerable<IValidator<LoginRequest>> loginValidator) : IAuthenticationService
{
    /// <summary>Registers the new user.</summary>
    public async Task<Result<RegisterUserResponse>> RegisterNewUserAsync(RegisterUserRequest request, CancellationToken cancellationToken = default)
    {
        await registerValidator.ValidateAndThrowAsync(request, cancellationToken);
        return await inner.RegisterNewUserAsync(request, cancellationToken);
    }

    /// <summary>Confirmations the email.</summary>
    public async Task<Result<string>> ConfirmationEmailAsync(ConfirmEmailRequest request,
        CancellationToken cancellationToken = default)
    {
        await confirmEmailValidator.ValidateAndThrowAsync(request, cancellationToken);
        return await inner.ConfirmationEmailAsync(request, cancellationToken);
    }

    /// <summary>Logs the user in and issues the tokens.</summary>
    public async Task<Result<LoginResponse>> LoginAsync(LoginRequest request, string ip,
        CancellationToken cancellationToken = default)
    {
        await loginValidator.ValidateAndThrowAsync(request, cancellationToken);
        return await inner.LoginAsync(request, ip, cancellationToken);
    }

    /// <summary>Logs the user out and revokes the tokens.</summary>
    public async Task<Result<string>> Logout(ClaimsPrincipal user,
        string refresh, CancellationToken cancellationToken = default)
        => await inner.Logout(user, refresh, cancellationToken);

    /// <summary>Generates the access token from refresh token.</summary>
    public async Task<Result<LoginResponse>> GenerateAccessTokenFromRefreshToken(string refresh,
        ClaimsPrincipal userClaims,
        CancellationToken cancellationToken = default)
            => await inner.GenerateAccessTokenFromRefreshToken(refresh, userClaims, cancellationToken);
}