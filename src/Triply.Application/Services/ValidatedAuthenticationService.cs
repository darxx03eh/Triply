using FluentValidation;
using Triply.Application.DTOs.Authentications;
using Triply.Application.Extensions;
using Triply.Application.Features.Authentications.Commands.Register;
using Triply.Application.Interfaces.Services;
using Triply.Domain.Results;

namespace Triply.Application.Services;

public class ValidatedAuthenticationService(
    IAuthenticationService inner,
    IEnumerable<IValidator<RegisterUserRequest>> registerValidator) : IAuthenticationService
{
    public async Task<Result<RegisterUserResponse>> RegisterNewUserAsync(RegisterUserRequest request, CancellationToken cancellationToken = default)
    {
        await registerValidator.ValidateAndThrowAsync(request, cancellationToken);
        return await inner.RegisterNewUserAsync(request, cancellationToken);
    }
}