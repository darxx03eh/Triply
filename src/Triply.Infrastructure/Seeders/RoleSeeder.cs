using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Triply.Domain.Constants;
using Triply.Domain.Entities.Identity;

namespace Triply.Infrastructure.Seeders;

/// <summary>Seeds the default role data.</summary>
public static class RoleSeeder
{
    /// <summary>Seeds the default role data when it is missing.</summary>
    public static async Task SeedAsync(RoleManager<TriplyRole> roleManager, ILogger logger,
        CancellationToken cancellationToken)
    {
        var roles = await roleManager.Roles.CountAsync(cancellationToken);
        if (roles == 0)
        {
            foreach (var role in new[] { Roles.Admin, Roles.User })
            {
                var result = await roleManager.CreateAsync(new TriplyRole() { Name = role });
                if (result.Succeeded)
                    logger.LogInformation("Seeded the role {Role}", role);
                else
                    logger.LogError("Could not seed the role {Role}: {Errors}", role,
                        result.Errors.Select(e => e.Description).ToArray());
            }
        }
        else logger.LogDebug("Roles already seeded ({RolesCount} roles)", roles);
            
    }
}