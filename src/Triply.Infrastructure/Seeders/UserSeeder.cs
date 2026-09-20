using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Triply.Domain.Constants;
using Triply.Domain.Entities.Identity;

namespace Triply.Infrastructure.Seeders;

/// <summary>Seeds the default user data.</summary>
public static class UserSeeder
{
    /// <summary>Seeds the default user data when it is missing.</summary>
    public static async Task SeedAsync(UserManager<TriplyUser> userManager, ILogger logger,
        CancellationToken cancellationToken)
    {
        var users = await userManager.Users.CountAsync(cancellationToken);
        if (users == 0)
        {
            var @default = new TriplyUser()
            {
                FirstName = "Mahmoud",
                LastName = "Darawsheh",
                Email = "admin@triply.com",
                UserName = "admin",
                DateOfBirth = new DateTime(2003, 2, 18),
                EmailConfirmed = true
            };

            var user = await userManager.CreateAsync(@default, "admin003+-");
            if (!user.Succeeded)
            {
                logger.LogError("Could not seed the default admin {UserName}: {Errors}", @default.UserName,
                    user.Errors.Select(e => e.Description).ToArray());
                return;
            }

            await userManager.AddToRoleAsync(@default, Roles.Admin);
            logger.LogInformation("Seeded the default admin {UserName} ({Email})", @default.UserName, @default.Email);
        }
        else logger.LogDebug("Users already seeded ({UsersCount} users)", users);
            
    }
}