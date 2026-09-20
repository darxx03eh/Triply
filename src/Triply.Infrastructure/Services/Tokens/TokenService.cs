using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Triply.Application.Interfaces.Repositories;
using Triply.Application.Interfaces.Services;
using Triply.Domain.Entities.Identity;
using Triply.Infrastructure.Settings;

namespace Triply.Infrastructure.Services.Tokens;

/// <summary>Creates and parses the application's JWT access and refresh tokens.</summary>
public partial class TokenService(
    IOptions<JwtSettings> options,
    IRefreshTokenRepository refreshTokenRepository,
    UserManager<TriplyUser> userManager,
    ILogger<TokenService> logger
    ) : ITokenService
{
    private readonly JwtSettings _jwtSettings = options.Value;
}