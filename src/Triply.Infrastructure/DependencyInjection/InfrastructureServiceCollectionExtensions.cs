using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Triply.Domain.Entities.Identity;
using Triply.Infrastructure.Db;

namespace Triply.Infrastructure.DependencyInjection;

public static class InfrastructureServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddTriplyDbContext(IConfiguration configuration)
        {
            services.AddDbContext<TriplyDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("TriplyDbLocalConnection"));
            });
            return services;
        }

        public IServiceCollection AddIdentityServices()
        {
            services.AddIdentity<TriplyUser, TriplyRole>(options =>
                {
                    // Sign in settings
                    options.SignIn.RequireConfirmedEmail = false;
                    // password settings
                    options.Password.RequireDigit = false;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequiredLength = 8;
                    options.Password.RequiredUniqueChars = 0;
                    // lockout settings
                    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                    options.Lockout.MaxFailedAccessAttempts = 5;
                    options.Lockout.AllowedForNewUsers = false;
                    // User settings
                    options.User.RequireUniqueEmail = true;
                    options.User.AllowedUserNameCharacters =
                        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789._";
                }).AddEntityFrameworkStores<TriplyDbContext>()
                .AddDefaultTokenProviders();
            return services;
        }
    }
}