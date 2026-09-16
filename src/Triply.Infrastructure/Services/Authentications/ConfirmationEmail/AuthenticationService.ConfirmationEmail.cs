using Triply.Application.Features.Authentications.Commands.EmailConfirmation;
using Triply.Domain.Exceptions;
using Triply.Domain.Results;
using Triply.Domain.Results.Enums;

namespace Triply.Infrastructure.Services.Authentications;

public partial class AuthenticationService
{
    public async Task<Result<string>> ConfirmationEmailAsync(ConfirmEmailRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await userManager.FindByEmailAsync(request.Email);
            if (user is null)
                throw new NotFoundException($"User with email {request.Email} not found", "USER_NOT_FOUND");

            if (user.EmailConfirmed)
                return Result<string>.Failure(
                    "EMAIL_ALREADY_VERIFIED", "This email address has already been verified.");

            var result = await userManager.ConfirmEmailAsync(user, request.Token);
            if (!result.Succeeded)
                return Result<string>.Failure(
                    "EMAIL_VERIFICATION_EXPIRED", "The verification link has expired. Please request a new one.");
           
            return Result<string>.Success(null, ResultSuccessType.Ok, new(
                ResultResponseMessages.Authentication.Api.ConfirmationSucceeded.Code,
                ResultResponseMessages.Authentication.Api.ConfirmationSucceeded.Message));
        }
        catch (Exception exp)
        {
            throw;
        }
    }
}