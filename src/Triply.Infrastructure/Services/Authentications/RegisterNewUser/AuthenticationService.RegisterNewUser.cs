using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.WebUtilities;
using Triply.Application.DTOs.Authentications;
using Triply.Application.Extensions;
using Triply.Application.Features.Authentications.Commands.Register;
using Triply.Domain.Contracts;
using Triply.Domain.Contracts.Enums;
using Triply.Domain.Entities.Identity;
using Triply.Domain.Exceptions;
using Triply.Domain.Results;
using Triply.Domain.Results.Enums;

namespace Triply.Infrastructure.Services.Authentications;
public partial class AuthenticationService
{
    /// <inheritdoc />
    public async Task<Result<RegisterUserResponse>> RegisterNewUserAsync(RegisterUserRequest request,
        CancellationToken cancellationToken = default)
    {
        await using var transaction = await userRepository.BeginTransactionAsync();
        try
        {
            var user = request.ToTriplyUser();
            var createResult = await userManager.CreateAsync(user, request.Password);
            if (!createResult.Succeeded)
            {
                await transaction.RollbackAsync(cancellationToken);
                logger.LogWarning("Registration failed for {UserName} ({Email}): {Errors}",
                    request.Username, request.Email, createResult.Errors.Select(e => e.Code).ToArray());
                var fields = createResult.Errors
                    .GroupBy(_ => "General")
                    .ToDictionary(
                        g => g.Key, 
                        g => g.Select(e => e.Description).ToList());
                return Result<RegisterUserResponse>.Failure(fields);
            }

            var roleExists = await roleManager.RoleExistsAsync(DefaultRole);
            if (!roleExists)
            {
                await transaction.RollbackAsync(cancellationToken);
                logger.LogError(
                    "Registration failed: the role {Role} does not exist, check the role seeding", DefaultRole);
                throw new NotFoundException(
                    $"Role '{DefaultRole}' does not exist. Check role seeding.",
                    "ROLE_NOT_FOUND");
            }
            
            var roleResult = await userManager.AddToRoleAsync(user, DefaultRole);
            if (!roleResult.Succeeded)
            {
                await transaction.RollbackAsync(cancellationToken);
                logger.LogError("Registration failed: could not assign the role {Role} to {UserName}: {Errors}",
                    DefaultRole, user.UserName, roleResult.Errors.Select(e => e.Code).ToArray());
                return Result<RegisterUserResponse>.Failure(
                    "ROLE_ASSIGNMENT_FAILED", "Failed to assign role to the new user.");
            }

            await transaction.CommitAsync(cancellationToken);
            logger.LogInformation("User {UserId} ({UserName}) registered with the role {Role}", 
                user.Id, user.UserName, DefaultRole);
            await PublishConfirmationEmail(user);
            
            var response = new RegisterUserResponse(
                $"{user.FirstName} {user.LastName}",
                user.Id, user.Email, user.UserName);
            return Result<RegisterUserResponse>.Success(response, ResultSuccessType.Created, new(
                ResultResponseMessages.Authentication.Api.RegisterSucceeded.Code,
                ResultResponseMessages.Authentication.Api.RegisterSucceeded.Message));
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    private async Task<string> GenerateConfirmationLink(TriplyUser user)
    {
        var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
        var frontendUrl = configuration["FrontendUrl"]?.TrimEnd('/');

        // The link must open the SPA, not the API endpoint.  The API can be hosted on a
        // different origin (or behind a reverse proxy), in which case deriving its host
        // from the registration request sends users to a route that does not exist.
        if (string.IsNullOrWhiteSpace(frontendUrl))
        {
            var request = httpContextAccessor.HttpContext?.Request
                ?? throw new InvalidOperationException("Cannot generate a confirmation link without an HTTP request.");
            frontendUrl = $"{request.Scheme}://{request.Host}";
        }

        return QueryHelpers.AddQueryString($"{frontendUrl}/confirm-email", new Dictionary<string, string?>
        {
            ["email"] = user.Email,
            ["token"] = token
        });
    }

    private async Task PublishConfirmationEmail(TriplyUser user)
    {
        await publisher.PublishAsync("email.send", new EmailMessage
        {
            Type = EmailType.ConfirmationEmail,
            To = user.Email,
            TemplateData = new Dictionary<string, string>
            {
                ["user_name"] = user.UserName,
                ["confirmation_link"] = await GenerateConfirmationLink(user)
            }
        });
        logger.LogInformation("Confirmation email queued for user {UserId} ({UserName})", user.Id, user.UserName);
    }
}
