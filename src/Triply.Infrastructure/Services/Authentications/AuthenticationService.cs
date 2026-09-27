using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Triply.Application.Interfaces.Repositories;
using Triply.Application.Interfaces.Services;
using Triply.Domain.Constants;
using Triply.Domain.Entities.Identity;
using MessageQueue.IRabbitMQ;
using Microsoft.AspNetCore.Http;

namespace Triply.Infrastructure.Services.Authentications;

/// <summary>Implements the application's authentication workflows.</summary>
public partial class AuthenticationService(
    IUserRepository userRepository,
    IRefreshTokenRepository refreshTokenRepository,
    ITokenService tokenService,
    UserManager<TriplyUser> userManager,
    SignInManager<TriplyUser> signInManager,
    RoleManager<TriplyRole> roleManager,
    IMessagePublisher publisher,
    ITokenBlacklistService tokenBlacklistService,
    IHttpContextAccessor httpContextAccessor,
    IConfiguration configuration,
    ILogger<AuthenticationService> logger,
    IFailedLoginRateLimitService failedLoginRateLimitService
    ) : IAuthenticationService
{
    private const string DefaultRole = Roles.User;
}
