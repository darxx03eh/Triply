using Microsoft.Extensions.Logging;
using Triply.Application.DTOs.Tokens;
using Triply.Domain.Entities.Identity;

namespace Triply.Infrastructure.Services.Tokens;
public partial class TokenService
{
    /// <inheritdoc />
    public async Task<TokenResponse> GenerateAccessTokenAsync(TriplyUser user, bool flag = true,
        CancellationToken cancellationToken = default)

    {
        var access = await GenerateJwtTokenAsync(user, cancellationToken);
        (string Token, string Jti) refresh = ("", "");
        if (flag)
        {
            refresh = await GenerateRefreshTokenAsync(user, cancellationToken);

            var refreshToken = new RefreshToken()
            {
                AddedDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpiryDays),
                IsActive = true,
                IsRevoked = false,
                Token = refresh.Token,
                Jti = refresh.Jti,
                UserId = user.Id
            };

            await refreshTokenRepository.AddAsync(refreshToken, cancellationToken);
            await refreshTokenRepository.SaveChangesAsync(cancellationToken);
            logger.LogInformation("Refresh token {Jti} issued for user {UserId}, expires at {ExpiryDate}",
                refresh.Jti, user.Id, refreshToken.ExpiryDate);
        }

        logger.LogDebug("Access token issued for user {UserId}", user.Id);

        return new TokenResponse(access, refresh.Token);
    }
}