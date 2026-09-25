using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Triply.Domain.Constants;
using Triply.Infrastructure.Settings;

namespace Triply.XUnitTests.Common.Identity;

public static class TestTokens
{
    public static readonly JwtSettings Settings = new()
    {
        SecretKey = "unit-test-secret-key-that-is-long-enough-for-hs256",
        Issuer = "triply-tests",
        Audience = "triply-tests",
        ExpiryMinutes = 15,
        RefreshTokenExpiryDays = 7
    };

    /// <summary>A parsed refresh token (what ITokenService.ReadJwtTokenAsync returns).</summary>
    public static JwtSecurityToken Refresh(string jti, Guid userId, string algorithm = SecurityAlgorithms.HmacSha256)
    {
        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Settings.SecretKey)), algorithm);
        return new JwtSecurityToken(
            new JwtHeader(credentials),
            new JwtPayload([new Claim(TokenClaims.Jti, jti), 
                new Claim(TokenClaims.Id, userId.ToString()), new Claim(TokenClaims.Type, "refresh")]));
    }

    /// <summary>The authenticated user of the current access token (jti + exp claims).</summary>
    public static ClaimsPrincipal AccessPrincipal(string jti, DateTimeOffset expiresAt)
        => new(new ClaimsIdentity([
            new Claim(TokenClaims.Jti, jti),
            new Claim("exp", expiresAt.ToUnixTimeSeconds().ToString())
        ], "Bearer"));
}
