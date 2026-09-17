using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Triply.Domain.Constants;
using Triply.Domain.Results;
using Triply.Domain.Results.Enums;

namespace Triply.Infrastructure.Services.Authentications;

public partial class AuthenticationService
{
    public async Task<Result<string>> Logout(ClaimsPrincipal user, CancellationToken cancellationToken = default)
    {
        string? jti = user.FindFirstValue(TokenClaims.Jti);
        string? expClaim = user.FindFirstValue("exp");

        if (string.IsNullOrEmpty(jti) || string.IsNullOrEmpty(expClaim))
            throw new InvalidOperationException("Token is missing required claims (jti/exp).");

        var expiryUtc = DateTimeOffset.FromUnixTimeSeconds(long.Parse(expClaim)).UtcDateTime;

        await tokenBlacklistService.BlacklistTokenAsync(jti, expiryUtc, cancellationToken);
        
        var refresh = httpContextAccessor.HttpContext?.Request.Cookies["refresh"];
        if (string.IsNullOrWhiteSpace(refresh))
            return Result<string>.Failure(
                "REFRESH_TOKEN_MISSING",
                "Your session has expired. Please log in again.",
                type: ResultErrorType.Unauthorized);
        
        var (isRevoked, reason) = await RevokeRefreshTokenAsync(refresh, cancellationToken);
        if (!isRevoked)
        {
            var (code, message, error) = reason switch
            {
                "EXPIRED"   
                    => ("REFRESH_TOKEN_EXPIRED", "Your session has expired. Please log in again.", ResultErrorType.Unauthorized),
                "REVOKED"   
                    => ("REFRESH_TOKEN_REVOKED", "Your session was terminated. Please log in again.", ResultErrorType.BusinessRule),
                "NOT_FOUND" 
                    => ("REFRESH_TOKEN_INVALID", "Your session is no longer valid. Please log in again.", ResultErrorType.NotFound),
                _           
                    => ("REFRESH_TOKEN_INVALID", "Something went wrong. Please log in again.", ResultErrorType.Unauthorized)
            };
            return  Result<string>.Failure(
                code, message, type: error);
        }

        return Result<string>.Success(
            null, ResultSuccessType.NoContent, new(
                ResultResponseMessages.Success.NoContent.Code,
                ResultResponseMessages.Success.NoContent.Message));
    }

    private async Task<(bool Success, string Reason)> RevokeRefreshTokenAsync(
        string refresh,
        CancellationToken cancellationToken)
    {
        var jwtToken = await tokenService.ReadJwtTokenAsync(refresh);
        if (jwtToken is null)
            return (false, "EXPIRED");

        var jti = jwtToken.Claims
            .FirstOrDefault(c => c.Type == TokenClaims.Jti)?.Value;
        
        if (string.IsNullOrEmpty(jti))
            return (false, "EXPIRED");

        var token = await refreshTokenRepository.GetTableNoTracking()
            .FirstOrDefaultAsync(r => r.Jti == jti, cancellationToken);

        if (token is null)
            return (false, "NOT_FOUND");

        if (token.ExpiryDate <= DateTime.UtcNow)
            return (false, "EXPIRED");

        if (token.IsRevoked)
            return (false, "REVOKED");

        token.IsRevoked = true;
        token.IsActive = false;
        token.ExpiryDate = DateTime.UtcNow;

        await refreshTokenRepository.UpdateAsync(token);
        await refreshTokenRepository.SaveChangesAsync(cancellationToken);
        return (true, null);
    }
}