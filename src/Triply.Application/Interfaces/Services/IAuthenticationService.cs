using System.Security.Claims;
using Triply.Application.DTOs.Authentications;
using Triply.Application.Features.Authentications.Commands.EmailConfirmation;
using Triply.Application.Features.Authentications.Commands.Login;
using Triply.Application.Features.Authentications.Commands.Register;
using Triply.Domain.Results;

namespace Triply.Application.Interfaces.Services;

public interface IAuthenticationService
{
    Task<Result<RegisterUserResponse>> RegisterNewUserAsync(RegisterUserRequest request,
        CancellationToken cancellationToken = default);

    Task<Result<string>> ConfirmationEmailAsync(ConfirmEmailRequest request,
        CancellationToken cancellationToken = default);
    Task<Result<LoginResponse>> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
    Task<Result<string>> Logout(ClaimsPrincipal user,
        string refresh, 
        CancellationToken cancellationToken = default);

    Task<Result<LoginResponse>> GenerateAccessTokenFromRefreshToken(string refresh,
        ClaimsPrincipal userClaims,
        CancellationToken cancellationToken = default);
}