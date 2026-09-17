using System.Security.Claims;
using Triply.Application.DTOs.Authentications;
using Triply.Application.Features.Authentications.Commands.EmailConfirmation;
using Triply.Application.Features.Authentications.Commands.Login;
using Triply.Application.Features.Authentications.Commands.Register;
using Triply.Domain.Results;

namespace Triply.Application.Interfaces.Services;

/// <summary>Coordinates user registration, email confirmation, login, token refresh, and logout.</summary>
public interface IAuthenticationService
{
    /// <summary>Registers a new user account.</summary>
    /// <param name="request">The registration details.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>The created user's public registration details.</returns>
    Task<Result<RegisterUserResponse>> RegisterNewUserAsync(RegisterUserRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>Confirms a user's email address using the supplied verification token.</summary>
    /// <param name="request">The email address and confirmation token.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A result describing the confirmation outcome.</returns>
    Task<Result<string>> ConfirmationEmailAsync(ConfirmEmailRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>Authenticates a user and creates access and refresh tokens.</summary>
    /// <param name="request">The user's identifier and password.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>The authenticated user's name and tokens.</returns>
    Task<Result<LoginResponse>> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);

    /// <summary>Invalidates the current access token and refresh token.</summary>
    /// <param name="user">The authenticated principal.</param>
    /// <param name="refresh">The refresh token to revoke.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A result describing the logout outcome.</returns>
    Task<Result<string>> Logout(ClaimsPrincipal user,
        string refresh, 
        CancellationToken cancellationToken = default);

    /// <summary>Validates a refresh token and issues a replacement access token.</summary>
    /// <param name="refresh">The refresh token.</param>
    /// <param name="userClaims">Claims from the current access token.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>The user's name and newly issued access token.</returns>
    Task<Result<LoginResponse>> GenerateAccessTokenFromRefreshToken(string refresh,
        ClaimsPrincipal userClaims,
        CancellationToken cancellationToken = default);
}