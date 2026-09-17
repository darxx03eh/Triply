using Microsoft.AspNetCore.Identity;
using Triply.Application.Interfaces.Repositories;
using Triply.Application.Interfaces.Services;
using Triply.Domain.Constants;
using Triply.Domain.Entities.Identity;
using MessageQueue.IRabbitMQ;
using Microsoft.AspNetCore.Http;

namespace Triply.Infrastructure.Services.Authentications;

public partial class AuthenticationService(
    IUserRepository userRepository,
    IRefreshTokenRepository refreshTokenRepository,
    ITokenService tokenService,
    UserManager<TriplyUser> userManager,
    SignInManager<TriplyUser> signInManager,
    RoleManager<TriplyRole> roleManager,
    IMessagePublisher publisher,
    IHttpContextAccessor httpContextAccessor,
    ITokenBlacklistService tokenBlacklistService
    ) : IAuthenticationService
{
    private const string DefaultRole = Roles.User;
}