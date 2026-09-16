using System.Text;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Triply.Application.Features.Authentications.Commands.Register;
using Triply.Application.Interfaces.Repositories.General;
using Triply.Application.Interfaces.Services;
using Triply.Application.Services;
using Triply.Domain.Constants;
using Triply.Domain.Entities.Identity;
using Triply.Infrastructure.Db;
using Triply.Infrastructure.Repositories;
using Triply.Infrastructure.Repositories.General;
using Triply.Infrastructure.Seeders;
using Triply.Infrastructure.Services.Authentications;
using Triply.Infrastructure.Settings;

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
                    options.SignIn.RequireConfirmedEmail = true;
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
        
        public IServiceCollection AddJwtAuthentication(IConfiguration configuration)
        {
            services.AddOptions<JwtSettings>()
                .Bind(configuration.GetSection(nameof(JwtSettings)))
                .ValidateDataAnnotations()
                .ValidateOnStart();

            var jwtSettings = configuration.GetSection(nameof(JwtSettings)).Get<JwtSettings>()
                              ?? throw new InvalidOperationException(
                                  $"{nameof(JwtSettings)} section is missing from configuration.");
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.MapInboundClaims = false;
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = jwtSettings.ValidateIssuer,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidateAudience = jwtSettings.ValidateAudience,
                    ValidAudience = jwtSettings.Audience,
                    ValidateIssuerSigningKey = jwtSettings.ValidateIssuerSigningKey,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
                    ValidateLifetime = jwtSettings.ValidateLifetime,
                    RoleClaimType = TokenClaims.Role,
                    NameClaimType = TokenClaims.Username,
                    ClockSkew = TimeSpan.Zero
                };
            });
            return services;
        }
        
        public IServiceCollection AddInfrastructureDependencies()
        {
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.Scan(scan => scan
                .FromAssemblyOf<RefreshTokenRepository>()
                .AddClasses(@class 
                    => @class.Where(type => type.Name.EndsWith("Repository") && !type.IsGenericType))
                .AsMatchingInterface()
                .WithScopedLifetime());

            services.Scan(scan => scan
                .FromAssemblyOf<AuthenticationService>()
                .AddClasses(@class 
                    => @class.Where(type => type.Name.EndsWith("Service") && !type.IsGenericType))
                .AsMatchingInterface()
                .WithScopedLifetime());
            
            return services;
        }

        public IServiceCollection ApplyDecorators()
        {
            services.AddValidatorsFromAssemblyContaining<RegisterUserRequestValidator>();
            
            services.Decorate<IAuthenticationService, ValidatedAuthenticationService>();
            return services;
        }
    }
    extension(IServiceProvider services)
    {
        public async Task SeedAsync()
        {
            using var scope = services.CreateScope();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<TriplyRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<TriplyUser>>();
            var context = scope.ServiceProvider.GetRequiredService<TriplyDbContext>();

            await RoleSeeder.SeedAsync(roleManager, CancellationToken.None);
            await UserSeeder.SeedAsync(userManager, CancellationToken.None);
        }
    }
}