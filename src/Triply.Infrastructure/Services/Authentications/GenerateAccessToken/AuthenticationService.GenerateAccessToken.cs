using Microsoft.Extensions.Logging;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Triply.Application.DTOs.Authentications;
using Triply.Domain.Constants;
using Triply.Domain.Exceptions;
using Triply.Domain.Results;
using Triply.Domain.Results.Enums;

namespace Triply.Infrastructure.Services.Authentications;

public partial class AuthenticationService
{
    /// <inheritdoc />
    public async Task<Result<LoginResponse>> GenerateAccessTokenFromRefreshToken(string refresh,
        ClaimsPrincipal userClaims,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(refresh))
        {
            logger.LogWarning("Token refresh failed: the refresh token cookie is missing");
            return Result<LoginResponse>.Failure(
                "REFRESH_TOKEN_MISSING",
                "Your session has expired. Please log in again.",
                type: ResultErrorType.Unauthorized);
        }
        
        var jwtToken = await tokenService.ReadJwtTokenAsync(refresh, cancellationToken);
        var (isValid, reason) = await ValidateDetails(jwtToken, refresh, cancellationToken);
        if (!isValid)
        {
            var (code, message, error) = reason switch
            {
                "WRONG_ALGORITHM" =>
                    ("REFRESH_TOKEN_INVALID","Your session is no longer valid. Please log in again."
                        ,ResultErrorType.Unauthorized),
                "EXPIRED" => 
                    ("REFRESH_TOKEN_EXPIRED", "Your session has expired. Please log in again."
                        , ResultErrorType.Unauthorized),
                "REVOKED" => 
                    ("REFRESH_TOKEN_REVOKED", "Your session was terminated. Please log in again."
                        , ResultErrorType.BusinessRule),
                "NOT_FOUND" => 
                    ("REFRESH_TOKEN_INVALID", "Your session is no longer valid. Please log in again."
                        , ResultErrorType.NotFound),
                "NOT_ACTIVE" =>
                    ("REFRESH_TOKEN_INVALID", "Your session is no longer valid. Please log in again."
                        ,ResultErrorType.BusinessRule),
                _ => 
                    ("REFRESH_TOKEN_INVALID", "Something went wrong. Please log in again."
                        , ResultErrorType.Unauthorized)
            };

            logger.LogWarning("Token refresh rejected: the refresh token is {Reason}", reason);
            return  Result<LoginResponse>.Failure(code, message, type: error);
        }
        var userId = jwtToken.Claims.FirstOrDefault(claim => claim.Type.Equals(TokenClaims.Id))?.Value;
        var user = await userManager.FindByIdAsync(userId);
        if (user is null)
        {
            logger.LogWarning("Token refresh failed: user {UserId} from the refresh token no longer exists", userId);
            throw new NotFoundException($"User with email {userId} not found", "USER_NOT_FOUND");
        }
        
        if (!user.IsActive)
        {
            logger.LogWarning(
                "Token refresh blocked: user {UserId} ({UserName}) is not active", user.Id, user.UserName);
            return Result<LoginResponse>.Failure(
                "ACCOUNT_NOT_ACTIVE",
                "Your account is not active. Please contact support for assistance.",
                ResultErrorType.Forbidden);
        }

        var token = await tokenService.GenerateAccessTokenAsync(user, flag: false, cancellationToken);
        
        user.LastLoginAt = DateTime.UtcNow;
        await userManager.UpdateAsync(user);
        
        string? jti = userClaims.FindFirstValue(TokenClaims.Jti);
        string? expClaim = userClaims.FindFirstValue("exp");

        if (string.IsNullOrEmpty(jti) || string.IsNullOrEmpty(expClaim))
            throw new InvalidOperationException("Token is missing required claims (jti/exp).");

        var expiryUtc = DateTimeOffset.FromUnixTimeSeconds(long.Parse(expClaim)).UtcDateTime;

        await tokenBlacklistService.BlacklistTokenAsync(jti, expiryUtc, cancellationToken);
        logger.LogInformation("Access token refreshed for user {UserId} ({UserName}), old token {Jti} blacklisted",
            user.Id, user.UserName, jti);

        var response = new LoginResponse($"{user.FirstName} {user.LastName}", token.Access, refresh);
        return Result<LoginResponse>.Success(response, success: new(
            ResultResponseMessages.Authentication.Api.TokenRegenerated.Code,
            ResultResponseMessages.Authentication.Api.TokenRegenerated.Message));
    }

    private async Task<(bool Success, string? Reason)> ValidateDetails(JwtSecurityToken jwtToken,
        string refresh, CancellationToken cancellationToken)
    {

        if (jwtToken is null || !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256))
            return (false, "WRONG_ALGORITHM");

        var jti = jwtToken.Claims.FirstOrDefault(claim => claim.Type.Equals(TokenClaims.Jti))?.Value;

        var refreshToken = await refreshTokenRepository.GetTableNoTracking()
            .Where(r => r.Jti.Equals(jti)).FirstOrDefaultAsync(cancellationToken);
        if (refreshToken is null)
            return (false, "NOT_FOUND");

        if (refreshToken.IsRevoked)
            return (false, "REVOKED");

        if (!refreshToken.IsActive)
            return (false, "NOT_ACTIVE");

        if (refreshToken.ExpiryDate < DateTime.UtcNow)
        {
            logger.LogInformation(
                "Refresh token {Jti} of user {UserId} expired and was revoked",
                jti, refreshToken.UserId);
            refreshToken.IsRevoked = true;
            refreshToken.IsActive = false;
            await refreshTokenRepository.UpdateAsync(refreshToken);
            await refreshTokenRepository.SaveChangesAsync(cancellationToken);
            return (false, "EXPIRED");
        }

        return (true, null);
    }
}