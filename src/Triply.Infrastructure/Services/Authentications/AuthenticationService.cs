using Microsoft.AspNetCore.Identity;
using Triply.Application.Interfaces.Repositories;
using Triply.Application.Interfaces.Services;
using Triply.Domain.Constants;
using Triply.Domain.Entities.Identity;
using MessageQueue.IRabbitMQ;

namespace Triply.Infrastructure.Services.Authentications;

public partial class AuthenticationService(
    IUserRepository userRepository,
    ITokenService tokenService,
    UserManager<TriplyUser> userManager,
    SignInManager<TriplyUser> signInManager,
    RoleManager<TriplyRole> roleManager,
    IMessagePublisher publisher
    ) : IAuthenticationService
{
    private const string DefaultRole = Roles.User;
}