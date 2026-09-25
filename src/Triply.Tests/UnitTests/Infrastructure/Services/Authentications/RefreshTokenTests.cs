using Microsoft.IdentityModel.Tokens;
using Moq;
using Triply.Application.DTOs.Tokens;
using Triply.Domain.Entities.Identity;
using Triply.Domain.Exceptions;
using Triply.Domain.Results.Enums;
using Triply.Tests.UnitTests.Common.Assertions;
using Triply.Tests.UnitTests.Common.Builders;
using Triply.XUnitTests.Common.Identity;
using Triply.XUnitTests.Infrastructure.Services.Authentications;

namespace Triply.Tests.UnitTests.Infrastructure.Services.Authentications;

public class RefreshTokenTests : AuthenticationServiceTestBase
{
    private readonly TriplyUser _user = TestData.User();
    private readonly System.Security.Claims.ClaimsPrincipal _principal =
        TestTokens.AccessPrincipal("old-access-jti", DateTimeOffset.UtcNow.AddMinutes(1));

    public RefreshTokenTests()
    {
        UserManager.Setup(m => m.FindByIdAsync(_user.Id.ToString())).ReturnsAsync(_user);
        TokenService.Setup(t => t.GenerateAccessTokenAsync(_user, false, 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new TokenResponse("new-access", ""));
    }

    private void StoreRefresh(string jti, DateTime expiry, bool revoked = false, 
        bool active = true, string algorithm = SecurityAlgorithms.HmacSha256)
    {
        Db.RefreshTokens.Add(new RefreshToken
        {
            Jti = jti, UserId = _user.Id, Token = "raw", ExpiryDate = expiry, IsRevoked = revoked, IsActive = active
        });
        Db.SaveChanges();
        Db.ChangeTracker.Clear();
        TokenService.Setup(t => t.ReadJwtTokenAsync("refresh", It.IsAny<CancellationToken>()))
            .ReturnsAsync(TestTokens.Refresh(jti, _user.Id, algorithm));
    }

    [Fact]
    public async Task Refresh_ValidToken_IssuesNewAccessAndBlacklistsOldOne()
    {
        StoreRefresh("r1", DateTime.UtcNow.AddDays(3));

        var result = await Service.GenerateAccessTokenFromRefreshToken("refresh", _principal);

        var response = result.AssertSuccess();
        Assert.Equal("new-access", response.Access);
        Assert.Equal("refresh", response.Refresh);
        Assert.NotNull(_user.LastLoginAt);
        Blacklist.Verify(b => b.BlacklistTokenAsync("old-access-jti", 
            It.IsAny<DateTime>(), It.IsAny<CancellationToken>()), Times.Once);
        TokenService.Verify(t => t.GenerateAccessTokenAsync(_user, false, 
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Refresh_MissingCookie_ReturnsUnauthorized(string? refresh)
    {
        var result = await Service.GenerateAccessTokenFromRefreshToken(refresh!, _principal);

        result.AssertFailure("REFRESH_TOKEN_MISSING", ResultErrorType.Unauthorized);
    }

    [Fact]
    public async Task Refresh_WrongAlgorithm_ReturnsInvalid()
    {
        StoreRefresh("r1", DateTime.UtcNow.AddDays(3), algorithm: SecurityAlgorithms.HmacSha512);

        var result = await Service.GenerateAccessTokenFromRefreshToken("refresh", _principal);

        result.AssertFailure("REFRESH_TOKEN_INVALID", ResultErrorType.Unauthorized);
    }

    [Fact]
    public async Task Refresh_UnknownToken_ReturnsNotFound()
    {
        TokenService.Setup(t => t.ReadJwtTokenAsync("refresh", 
            It.IsAny<CancellationToken>())).ReturnsAsync(TestTokens.Refresh("ghost", _user.Id));

        var result = await Service.GenerateAccessTokenFromRefreshToken("refresh", _principal);

        result.AssertFailure("REFRESH_TOKEN_INVALID", ResultErrorType.NotFound);
    }

    [Fact]
    public async Task Refresh_RevokedToken_ReturnsRevoked()
    {
        StoreRefresh("r1", DateTime.UtcNow.AddDays(3), revoked: true);

        var result = await Service.GenerateAccessTokenFromRefreshToken("refresh", _principal);

        result.AssertFailure("REFRESH_TOKEN_REVOKED", ResultErrorType.BusinessRule);
    }

    [Fact]
    public async Task Refresh_InactiveToken_ReturnsInvalid()
    {
        StoreRefresh("r1", DateTime.UtcNow.AddDays(3), active: false);

        var result = await Service.GenerateAccessTokenFromRefreshToken("refresh", _principal);

        result.AssertFailure("REFRESH_TOKEN_INVALID", ResultErrorType.BusinessRule);
    }

    [Fact]
    public async Task Refresh_ExpiredToken_RevokesItAndReturnsExpired()
    {
        StoreRefresh("r1", DateTime.UtcNow.AddMinutes(-1));

        var result = await Service.GenerateAccessTokenFromRefreshToken("refresh", _principal);

        result.AssertFailure("REFRESH_TOKEN_EXPIRED", ResultErrorType.Unauthorized);
        var stored = Db.RefreshTokens.Single(r => r.Jti == "r1");
        Assert.True(stored.IsRevoked);
        Assert.False(stored.IsActive);
    }

    [Fact]
    public async Task Refresh_InactiveUser_ReturnsForbidden()
    {
        StoreRefresh("r1", DateTime.UtcNow.AddDays(3));
        _user.IsActive = false;

        var result = await Service.GenerateAccessTokenFromRefreshToken("refresh", _principal);

        result.AssertFailure("ACCOUNT_NOT_ACTIVE", ResultErrorType.Forbidden);
        TokenService.Verify(t => t.GenerateAccessTokenAsync(It.IsAny<TriplyUser>(), 
            It.IsAny<bool>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Refresh_UserDeleted_ThrowsNotFound()
    {
        StoreRefresh("r1", DateTime.UtcNow.AddDays(3));
        UserManager.Setup(m => m.FindByIdAsync(_user.Id.ToString())).ReturnsAsync((TriplyUser?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => 
            Service.GenerateAccessTokenFromRefreshToken("refresh", _principal));
    }
}
