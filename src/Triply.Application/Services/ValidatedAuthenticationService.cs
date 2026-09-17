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

public class ValidatedAuthenticationService(
    IAuthenticationService inner,
    IEnumerable<IValidator<RegisterUserRequest>> registerValidator,
    IEnumerable<IValidator<ConfirmEmailRequest>> confirmEmailValidator,
    IEnumerable<IValidator<LoginRequest>> loginValidator) : IAuthenticationService
{
    public async Task<Result<RegisterUserResponse>> RegisterNewUserAsync(RegisterUserRequest request, CancellationToken cancellationToken = default)
    {
        await registerValidator.ValidateAndThrowAsync(request, cancellationToken);
        return await inner.RegisterNewUserAsync(request, cancellationToken);
    }

    public async Task<Result<string>> ConfirmationEmailAsync(ConfirmEmailRequest request,
        CancellationToken cancellationToken = default)
    {
        await confirmEmailValidator.ValidateAndThrowAsync(request, cancellationToken);
        return await inner.ConfirmationEmailAsync(request, cancellationToken);
    }

    public async Task<Result<LoginResponse>> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        await loginValidator.ValidateAndThrowAsync(request, cancellationToken);
        return await inner.LoginAsync(request, cancellationToken);
    }

    public async Task<Result<string>> Logout(ClaimsPrincipal user, CancellationToken cancellationToken = default)
        => await inner.Logout(user, cancellationToken);
}