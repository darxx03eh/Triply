using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.IdentityModel.Tokens;
using Moq;
using Triply.Domain.Constants;
using Triply.Domain.Entities.Identity;
using Triply.Infrastructure.Repositories;
using Triply.Infrastructure.Services.Tokens;
using Triply.Tests.UnitTests.Common.Builders;
using Triply.Tests.UnitTests.Common.Database;
using Triply.Tests.UnitTests.Common.Identity;
using Triply.XUnitTests.Common.Identity;

namespace Triply.Tests.UnitTests.Infrastructure.Services.Tokens;

public class TokenServiceTests : IDisposable
{
    private readonly Triply.Infrastructure.Db.TriplyDbContext _db = TestDbContextFactory.Create();
    private readonly Mock<UserManager<TriplyUser>> _userManager = IdentityMocks.UserManager();
    private readonly TokenService _service;
    private readonly TriplyUser _user = TestData.User();

    public TokenServiceTests()
    {
        _userManager.Setup(m => 
            m.GetRolesAsync(_user)).ReturnsAsync([Roles.Admin, Roles.User]);
        _service = 
            new TokenService(Options.Create(TestTokens.Settings), new RefreshTokenRepository(_db), 
                _userManager.Object, NullLogger<TokenService>.Instance);
    }

    private static JwtSecurityToken Validate(string token)
    {
        new JwtSecurityTokenHandler { MapInboundClaims = false }.ValidateToken(token, new TokenValidationParameters
        {
            ValidIssuer = TestTokens.Settings.Issuer,
            ValidAudience = TestTokens.Settings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(TestTokens.Settings.SecretKey)),
            ClockSkew = TimeSpan.Zero
        }, out var validated);
        return (JwtSecurityToken)validated;
    }

    [Fact]
    public async Task GenerateAccessTokenAsync_AccessToken_IsSignedAndCarriesUserClaimsAndRoles()
    {
        var tokens = await _service.GenerateAccessTokenAsync(_user, flag: false);

        var jwt = Validate(tokens.Access);
        Assert.Equal("access", jwt.Claims.Single(c => c.Type == TokenClaims.Type).Value);
        Assert.Equal(_user.Id.ToString(), jwt.Claims.Single(c => c.Type == TokenClaims.Id).Value);
        Assert.Equal("mahmoud", jwt.Claims.Single(c => c.Type == TokenClaims.Username).Value);
        Assert.Equal("mahmoud@triply.com", jwt.Claims.Single(c => c.Type == TokenClaims.Email).Value);
        Assert.Equal([Roles.Admin, Roles.User], jwt.Claims.Where(c => c.Type == TokenClaims.Role)
            .Select(c => c.Value));
        Assert.False(string.IsNullOrEmpty(jwt.Claims.Single(c => c.Type == TokenClaims.Jti).Value));
    }

    [Fact]
    public async Task GenerateAccessTokenAsync_AccessToken_ExpiresAfterConfiguredMinutes()
    {
        var tokens = await _service.GenerateAccessTokenAsync(_user, flag: false);

        var expected = DateTime.UtcNow.AddMinutes(TestTokens.Settings.ExpiryMinutes);
        Assert.InRange(Validate(tokens.Access).ValidTo, expected.AddSeconds(-5), expected.AddSeconds(5));
    }

    [Fact]
    public async Task GenerateAccessTokenAsync_WithoutFlag_DoesNotCreateRefreshToken()
    {
        var tokens = await _service.GenerateAccessTokenAsync(_user, flag: false);

        Assert.Equal("", tokens.Refresh);
        Assert.Empty(_db.RefreshTokens);
    }

    [Fact]
    public async Task GenerateAccessTokenAsync_WithFlag_StoresRefreshTokenMatchingItsJti()
    {
        var tokens = await _service.GenerateAccessTokenAsync(_user, flag: true);

        var refresh = Validate(tokens.Refresh);
        var stored = _db.RefreshTokens.Single();
        Assert.Equal("refresh", refresh.Claims.Single(c => c.Type == TokenClaims.Type).Value);
        Assert.Equal(refresh.Claims.Single(c => c.Type == TokenClaims.Jti).Value, stored.Jti);
        Assert.Equal(tokens.Refresh, stored.Token);
        Assert.Equal(_user.Id, stored.UserId);
        Assert.True(stored.IsActive);
        Assert.False(stored.IsRevoked);
        var expected = DateTime.UtcNow.AddDays(TestTokens.Settings.RefreshTokenExpiryDays);
        Assert.InRange(stored.ExpiryDate, expected.AddSeconds(-5), expected.AddSeconds(5));
    }

    [Fact]
    public async Task GenerateAccessTokenAsync_EachCall_UsesNewJti()
    {
        var first = Validate((await _service.GenerateAccessTokenAsync(_user, false)).Access);
        var second = Validate((await _service.GenerateAccessTokenAsync(_user, false)).Access);

        Assert.NotEqual(first.Id, second.Id);
    }

    [Fact]
    public async Task ReadJwtTokenAsync_ValidToken_ReturnsClaims()
    {
        var tokens = await _service.GenerateAccessTokenAsync(_user, flag: true);

        var jwt = await _service.ReadJwtTokenAsync(tokens.Refresh);

        Assert.Equal(_user.Id.ToString(), jwt.Claims.Single(c => c.Type == TokenClaims.Id).Value);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task ReadJwtTokenAsync_Empty_Throws(string? token)
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() => _service.ReadJwtTokenAsync(token!));
    }

    [Fact]
    public async Task ReadJwtTokenAsync_Garbage_Throws()
    {
        await Assert.ThrowsAnyAsync<Exception>(() => _service.ReadJwtTokenAsync("not.a.jwt"));
    }

    public void Dispose() => _db.Dispose();
}
