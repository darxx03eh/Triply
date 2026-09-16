using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Triply.Application.Interfaces.Repositories;
using Triply.Application.Interfaces.Services;
using Triply.Domain.Entities.Identity;
using Triply.Infrastructure.Settings;

namespace Triply.Infrastructure.Services.Tokens;

public partial class TokenService(
    IOptions<JwtSettings> options,
    IRefreshTokenRepository refreshTokenRepository,
    UserManager<TriplyUser> userManager
    ) : ITokenService
{
    private readonly JwtSettings _jwtSettings = options.Value;
}