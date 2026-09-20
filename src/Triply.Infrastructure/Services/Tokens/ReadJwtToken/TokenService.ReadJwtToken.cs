using Microsoft.Extensions.Logging;
using System.IdentityModel.Tokens.Jwt;

namespace Triply.Infrastructure.Services.Tokens;
public partial class TokenService
{
    /// <inheritdoc />
    public async Task<JwtSecurityToken> ReadJwtTokenAsync(string token, CancellationToken cancellationToken = default)
    {

        if (string.IsNullOrEmpty(token))
            throw new ArgumentNullException(nameof(token));

        var handler = new JwtSecurityTokenHandler();
        try
        {
            return handler.ReadJwtToken(token);
        }
        catch (ArgumentException exception)
        {
            logger.LogWarning(exception, "Could not read a malformed JWT token");
            throw;
        }
    }
}