using Microsoft.Extensions.Logging;
using System.Security.Claims;
using System.Text;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using QuestPDF.Infrastructure;
using Sieve.Models;
using Sieve.Services;
using Triply.Application.Features.Authentications.Commands.Register;
using Triply.Application.Interfaces.Payments;
using Triply.Application.Interfaces.Repositories.General;
using Triply.Application.Interfaces.Services;
using Triply.Application.Options;
using Triply.Application.Services;
using Triply.Domain.Constants;
using Triply.Domain.Entities.Identity;
using Triply.Infrastructure.Db;
using Triply.Infrastructure.Filtering;
using Triply.Infrastructure.Interceptors;
using Triply.Infrastructure.Payments;
using Triply.Infrastructure.Repositories;
using Triply.Infrastructure.Repositories.General;
using Triply.Infrastructure.Seeders;
using Triply.Infrastructure.Services.Authentications;
using Triply.Infrastructure.Services;
using Triply.Infrastructure.Settings;

namespace Triply.Infrastructure.DependencyInjection;

/// <summary>Extension methods for infrastructure service collection.</summary>
public static class InfrastructureServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        /// <summary>Registers the generate invoice configurations.</summary>
        public IServiceCollection AddGenerateInvoiceConfigurations()
        {
            QuestPDF.Settings.License = LicenseType.Community;
            return services;
        }
        /// <summary>Registers the payment service services.</summary>
        public IServiceCollection AddPaymentServices(IConfiguration configuration)
        {
            services.Configure<PaymentOptions>(configuration.GetSection(nameof(PaymentOptions)));
            
            var provider = configuration.GetSection(nameof(PaymentOptions))[nameof(PaymentOptions.Provider)];
            if (string.Equals(provider, "Stripe", StringComparison.OrdinalIgnoreCase))
                services.AddScoped<IPaymentGateway, StripePaymentGateway>();
            else services.AddScoped<IPaymentGateway, MockPaymentGateway>();
            
            return services;
        }
        /// <summary>Register Booking options.</summary>
        public IServiceCollection AddBookingOptions(IConfiguration configuration)
        {
            services.Configure<BookingOptions>(configuration.GetSection(nameof(BookingOptions)));
            return  services;
        }
        /// <summary>Registers the Sieve service services.</summary>
        public IServiceCollection AddSieveService(IConfiguration configuration)
        {
            services.Configure<SieveOptions>(configuration.GetSection("Sieve"));
            services.AddScoped<ISieveProcessor, ApplicationSieveProcessor>();

            return services;
        }
        /// <summary>Registers the Triply database context services.</summary>
        public IServiceCollection AddTriplyDbContext(IConfiguration configuration)
        {
            services.Configure<DatabaseLoggingSettings>(configuration.GetSection(nameof(DatabaseLoggingSettings)));
            services.AddSingleton<SlowQueryInterceptor>();
            services.AddDbContext<TriplyDbContext>((provider, options) =>
            {
                var logging = provider.GetRequiredService<IOptions<DatabaseLoggingSettings>>().Value;
                options.UseSqlServer(configuration.GetConnectionString("TriplyDbLocalConnection"))
                    .AddInterceptors(provider.GetRequiredService<SlowQueryInterceptor>())
                    .EnableSensitiveDataLogging(logging.EnableSensitiveDataLogging);
            });
            return services;
        }

        /// <summary>Registers the identity services services.</summary>
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
        
        /// <summary>Registers the JWT authentication services.</summary>
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

                options.Events = new JwtBearerEvents()
                {
                    OnTokenValidated = async context =>
                    {
                        var blacklistService = context.HttpContext.RequestServices
                            .GetRequiredService<ITokenBlacklistService>();

                        string? jti = context.Principal?.FindFirstValue(TokenClaims.Jti);

                        if (string.IsNullOrEmpty(jti) ||
                            await blacklistService.IsBlacklistedAsync(jti, context.HttpContext.RequestAborted))
                            context.Fail("Token has been revoked.");
                    },
                    OnAuthenticationFailed = context =>
                    {
                        context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>()
                            .CreateLogger("Triply.Authentication")
                            .LogWarning("JWT authentication failed for " +
                                        "{RequestMethod} {RequestPath}: {FailureType} {FailureMessage}",
                                context.Request.Method, context.Request.Path,
                                context.Exception.GetType().Name, context.Exception.Message);
                        return Task.CompletedTask;
                    }
                };
            });
            return services;
        }
        
        /// <summary>Registers the infrastructure dependencies services.</summary>
        public IServiceCollection AddInfrastructureDependencies()
        {
            services.AddHttpContextAccessor();
            services.AddScoped<ICurrentUserAccessor, CurrentUserAccessor>();
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

        /// <summary>Applies the decorators.</summary>
        public IServiceCollection ApplyDecorators()
        {
            services.AddValidatorsFromAssemblyContaining<RegisterUserRequestValidator>();
            
            services.Decorate<IAuthenticationService, ValidatedAuthenticationService>();
            services.Decorate<ICityService, ValidatedCityService>();
            services.Decorate<IHotelService, ValidatedHotelService>();
            services.Decorate<IRoomService, ValidatedRoomService>();
            services.Decorate<IAmenityService, ValidatedAmenityService>();
            services.Decorate<ISearchService, ValidatedSearchService>();
            services.Decorate<IAttractionService, ValidatedAttractionService>();
            services.Decorate<IReviewService, ValidatedReviewService>();
            services.Decorate<IDealService, ValidatedDealService>();
            services.Decorate<ICartService, ValidatedCartService>();
            services.Decorate<IBookingService, ValidatedBookingService>();
            return services;
        }
    }
    extension(IServiceProvider services)
    {
        /// <summary>Seeds the default infrastructure service collection data when it is missing.</summary>
        public async Task SeedAsync()
        {
            using var scope = services.CreateScope();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<TriplyRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<TriplyUser>>();
            var context = scope.ServiceProvider.GetRequiredService<TriplyDbContext>();

            var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>()
                .CreateLogger("Triply.Infrastructure.Seeders");

            await RoleSeeder.SeedAsync(roleManager, logger, CancellationToken.None);
            await UserSeeder.SeedAsync(userManager, logger, CancellationToken.None);
        }
    }
}