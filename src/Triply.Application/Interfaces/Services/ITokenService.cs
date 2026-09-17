using System.IdentityModel.Tokens.Jwt;
using Triply.Application.DTOs.Tokens;
using Triply.Domain.Entities.Identity;

namespace Triply.Application.Interfaces.Services;

/// <summary>Creates and reads JWT access and refresh tokens.</summary>
public interface ITokenService
{
    /// <summary>Generates an access token and, optionally, a persisted refresh token.</summary>
    /// <param name="user">The user for whom the tokens are created.</param>
    /// <param name="flag">Whether a refresh token should also be generated and stored.</param>
    /// <param name="cancellationToken">Token used to cancel persistence.</param>
    /// <returns>The generated access and refresh token values.</returns>
    Task<TokenResponse> GenerateAccessTokenAsync(TriplyUser user, bool flag = true,
        CancellationToken cancellationToken = default);

    /// <summary>Reads the claims and header from a serialized JWT without validating it.</summary>
    /// <param name="token">The serialized JWT.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>The parsed JWT.</returns>
    Task<JwtSecurityToken> ReadJwtTokenAsync(string token, CancellationToken cancellationToken = default);
}