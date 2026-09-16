using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Triply.Domain.Constants;
using Triply.Domain.Entities.Identity;

namespace Triply.Infrastructure.Seeders;

public static class RoleSeeder
{
    public static async Task SeedAsync(RoleManager<TriplyRole> roleManager, CancellationToken cancellationToken)
    {
        var roles = await roleManager.Roles.CountAsync(cancellationToken);
        if (roles == 0)
        {
            await roleManager.CreateAsync(new TriplyRole() { Name = Roles.Admin });
            await roleManager.CreateAsync(new TriplyRole() { Name = Roles.User });
        }
    }
}