using Moq;
using Triply.Domain.Entities.Identity;
using Triply.Domain.Results.Enums;
using Triply.Tests.UnitTests.Common.Assertions;
using Triply.XUnitTests.Common.Identity;
using Triply.XUnitTests.Infrastructure.Services.Authentications;

namespace Triply.Tests.UnitTests.Infrastructure.Services.Authentications;

public class LogoutTests : AuthenticationServiceTestBase
{
    private readonly Guid _userId = Guid.NewGuid();
    private readonly DateTimeOffset _accessExpiry = DateTimeOffset.UtcNow.AddMinutes(10);

    private RefreshToken StoreRefresh(string jti, DateTime expiry, bool revoked = false)
    {
        var token = 
            new RefreshToken { Jti = jti, UserId = _userId, Token = "raw", ExpiryDate = expiry, IsRevoked = revoked };
        Db.RefreshTokens.Add(token);
        Db.SaveChanges();
        Db.ChangeTracker.Clear();
        TokenService.Setup(t => t.ReadJwtTokenAsync("refresh", 
            It.IsAny<CancellationToken>())).ReturnsAsync(TestTokens.Refresh(jti, _userId));
        return token;
    }

    [Fact]
    public async Task Logout_ValidRefresh_BlacklistsAccessTokenAndRevokesRefresh()
    {
        StoreRefresh("refresh-jti", DateTime.UtcNow.AddDays(5));

        var result = await Service.Logout(TestTokens.AccessPrincipal("access-jti", _accessExpiry), 
            "refresh");

        result.AssertSuccess(ResultSuccessType.NoContent);
        Blacklist.Verify(b => b.BlacklistTokenAsync("access-jti",
            It.Is<DateTime>(d => Math.Abs((d - _accessExpiry.UtcDateTime).TotalSeconds) < 1), It.IsAny<CancellationToken>()), Times.Once);
        var stored = Db.RefreshTokens.Single(r => r.Jti == "refresh-jti");
        Assert.True(stored.IsRevoked);
        Assert.False(stored.IsActive);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task Logout_MissingRefreshCookie_StillBlacklistsAndReturnsUnauthorized(string? refresh)
    {
        var result = await Service.Logout(TestTokens.AccessPrincipal("access-jti", _accessExpiry), refresh!);

        result.AssertFailure("REFRESH_TOKEN_MISSING", ResultErrorType.Unauthorized);
        Blacklist.Verify(b => b.BlacklistTokenAsync("access-jti", It.IsAny<DateTime>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Logout_UnknownRefresh_ReturnsNotFound()
    {
        TokenService.Setup(t => t.ReadJwtTokenAsync("refresh", 
            It.IsAny<CancellationToken>())).ReturnsAsync(TestTokens.Refresh("nope", _userId));

        var result = await Service.Logout(
            TestTokens.AccessPrincipal("access-jti", _accessExpiry), "refresh");

        result.AssertFailure("REFRESH_TOKEN_INVALID", ResultErrorType.NotFound);
    }

    [Fact]
    public async Task Logout_ExpiredRefresh_ReturnsExpired()
    {
        StoreRefresh("old-jti", DateTime.UtcNow.AddDays(-1));

        var result = await Service.Logout(TestTokens.AccessPrincipal("access-jti", _accessExpiry), 
            "refresh");

        result.AssertFailure("REFRESH_TOKEN_EXPIRED", ResultErrorType.Unauthorized);
    }

    [Fact]
    public async Task Logout_AlreadyRevokedRefresh_ReturnsRevoked()
    {
        StoreRefresh("revoked-jti", DateTime.UtcNow.AddDays(3), revoked: true);

        var result = await Service.Logout(TestTokens.AccessPrincipal("access-jti", _accessExpiry), 
            "refresh");

        result.AssertFailure("REFRESH_TOKEN_REVOKED", ResultErrorType.BusinessRule);
    }

    [Fact]
    public async Task Logout_AccessTokenWithoutJti_Throws()
    {
        var principal = new System.Security.Claims.ClaimsPrincipal(new System.Security.Claims.ClaimsIdentity());

        await Assert.ThrowsAsync<InvalidOperationException>(() => Service.Logout(principal, "refresh"));
    }
}
