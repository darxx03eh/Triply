using System.IdentityModel.Tokens.Jwt;

namespace Triply.Infrastructure.Services.Tokens;
public partial class TokenService
{
    public async Task<JwtSecurityToken> ReadJwtTokenAsync(string token, CancellationToken cancellationToken = default)
    {

        if (string.IsNullOrEmpty(token))
            throw new ArgumentNullException(nameof(token));

        var handler = new JwtSecurityTokenHandler();
        var response = handler.ReadJwtToken(token);
        
        return response;
    }
}