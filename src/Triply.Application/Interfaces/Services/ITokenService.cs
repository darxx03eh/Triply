using System.IdentityModel.Tokens.Jwt;
using Triply.Application.DTOs.Tokens;
using Triply.Domain.Entities.Identity;

namespace Triply.Application.Interfaces.Services;

public interface ITokenService
{
    Task<TokenResponse> GenerateAccessTokenAsync(TriplyUser user, CancellationToken cancellationToken = default);
    Task<JwtSecurityToken> ReadJwtTokenAsync(string token, CancellationToken cancellationToken = default);
}