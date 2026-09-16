using Triply.Application.DTOs.Authentications;
using Triply.Application.Extensions;
using Triply.Application.Features.Authentications.Commands.Register;
using Triply.Domain.Exceptions;
using Triply.Domain.Results;
using Triply.Domain.Results.Enums;

namespace Triply.Infrastructure.Services.Authentications;
public partial class AuthenticationService
{
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
                throw new NotFoundException($"Role '{DefaultRole}' does not exist. Check role seeding.",
                    "ROLE_NOT_FOUND");
            }
            
            var roleResult = await userManager.AddToRoleAsync(user, DefaultRole);
            if (!roleResult.Succeeded)
            {
                await transaction.RollbackAsync(cancellationToken);
                return Result<RegisterUserResponse>.Failure(
                    "ROLE_ASSIGNMENT_FAILED", "Failed to assign role to the new user.");
            }

            await transaction.CommitAsync(cancellationToken);
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
}