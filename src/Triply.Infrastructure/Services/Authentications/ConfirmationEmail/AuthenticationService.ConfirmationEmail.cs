using Microsoft.Extensions.Logging;
using Triply.Application.Features.Authentications.Commands.EmailConfirmation;
using Triply.Domain.Exceptions;
using Triply.Domain.Results;
using Triply.Domain.Results.Enums;

namespace Triply.Infrastructure.Services.Authentications;

public partial class AuthenticationService
{
    /// <inheritdoc />
    public async Task<Result<string>> ConfirmationEmailAsync(ConfirmEmailRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await userManager.FindByEmailAsync(request.Email);
            if (user is null)
            {
                logger.LogWarning("Email confirmation failed: no user with the email {Email}", request.Email);
                throw new NotFoundException($"User with email {request.Email} not found", "USER_NOT_FOUND");
            }

            if (user.EmailConfirmed)
            {
                logger.LogInformation("Email confirmation skipped: user {UserId} is already confirmed", user.Id);
                return Result<string>.Failure(
                    "EMAIL_ALREADY_VERIFIED", "This email address has already been verified.");
            }

            var result = await userManager.ConfirmEmailAsync(user, request.Token);
            if (!result.Succeeded)
            {
                logger.LogWarning("Email confirmation failed for user {UserId}: {Errors}",
                    user.Id, result.Errors.Select(e => e.Code).ToArray());
                return Result<string>.Failure(
                    "EMAIL_VERIFICATION_EXPIRED",
                    "The verification link has expired. Please request a new one.");
            }

            logger.LogInformation("User {UserId} ({UserName}) confirmed the email", user.Id, user.UserName);

            return Result<string>.Success(null, ResultSuccessType.Ok, new(
                ResultResponseMessages.Authentication.Api.ConfirmationSucceeded.Code,
                ResultResponseMessages.Authentication.Api.ConfirmationSucceeded.Message));
        }
        catch (Exception exp) when (exp is not NotFoundException)
        {
            logger.LogError(exp, "Email confirmation failed for {Email}", request.Email);
            throw;
        }
    }
}