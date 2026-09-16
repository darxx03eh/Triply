using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Triply.Domain.Constants;
using Triply.Domain.Entities.Identity;

namespace Triply.Infrastructure.Services.Tokens;
public partial class TokenService
{
    private async Task<string> GenerateJwtTokenAsync(TriplyUser user, CancellationToken cancellationToken = default)
    {
        var claims = await GenerateUserClaimsAsync(user, cancellationToken);

        var tokenDescriptor = new SecurityTokenDescriptor()
        {
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience,
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey)),
                SecurityAlgorithms.HmacSha256)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
    private async Task<List<Claim>> GenerateUserClaimsAsync(TriplyUser user,
        CancellationToken cancellationToken = default)
    {
        var roles = await userManager.GetRolesAsync(user);
        var claims = new List<Claim>()
        {
            new(TokenClaims.Type, "access"),
            new(TokenClaims.Id, user.Id.ToString()),
            new(TokenClaims.Username,  user.UserName!),
            new(TokenClaims.Email, user.Email!)
        };
        claims.AddRange(roles.Select(role => new Claim(TokenClaims.Role, role)));
        return claims;
    }
}