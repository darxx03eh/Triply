using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Triply.Domain.Constants;
using Triply.Domain.Entities.Identity;

namespace Triply.Infrastructure.Seeders;

public static class UserSeeder
{
    public static async Task SeedAsync(UserManager<TriplyUser> userManager, CancellationToken cancellationToken)
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
            await userManager.AddToRoleAsync(@default, Roles.Admin);
        }
    }
}