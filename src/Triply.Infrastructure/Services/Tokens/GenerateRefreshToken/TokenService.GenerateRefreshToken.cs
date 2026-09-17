using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Triply.Domain.Constants;
using Triply.Domain.Entities.Identity;

namespace Triply.Infrastructure.Services.Tokens;

public partial class TokenService
{
    private async Task<(string Token, string Jti)> GenerateRefreshTokenAsync(TriplyUser user,
        CancellationToken cancellationToken = default)
    {
        var jti = Guid.NewGuid().ToString();
        var claims = await GenerateRefreshTokenClaimsAsync(user, jti, cancellationToken);
        
        var tokenDescriptor = new SecurityTokenDescriptor()
        {
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience,
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpiryDays),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey)),
                SecurityAlgorithms.HmacSha256)
        };
        
        var tokenHandler = new JwtSecurityTokenHandler();
        var token =  tokenHandler.CreateToken(tokenDescriptor);
        return (tokenHandler.WriteToken(token), jti);
    }

    private async Task<List<Claim>> GenerateRefreshTokenClaimsAsync(TriplyUser user,
        string jti,
        CancellationToken cancellationToken = default)
    {
        return
        [
            new(TokenClaims.Jti, jti),
            new(TokenClaims.Id, user.Id.ToString()),
            new(TokenClaims.Type, "refresh")
        ];
    }
}