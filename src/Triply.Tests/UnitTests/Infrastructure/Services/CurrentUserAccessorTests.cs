using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Moq;
using Triply.Domain.Constants;
using Triply.Infrastructure.Services;

namespace Triply.Tests.UnitTests.Infrastructure.Services;

public class CurrentUserAccessorTests
{
    private static CurrentUserAccessor Create(ClaimsPrincipal? user)
    {
        var accessor = new Mock<IHttpContextAccessor>();
        accessor.Setup(a => a.HttpContext).Returns(user is null ? null : 
            new DefaultHttpContext { User = user });
        return new CurrentUserAccessor(accessor.Object);
    }

    private static ClaimsPrincipal User(Guid id, params string[] roles)
        => new(new ClaimsIdentity(
            [new Claim(TokenClaims.Id, id.ToString()), .. roles.Select(r => 
                new Claim(TokenClaims.Role, r))],
            "Bearer", TokenClaims.Username, TokenClaims.Role));

    [Fact]
    public void UserId_Authenticated_ReturnsIdClaim()
    {
        var id = Guid.NewGuid();

        Assert.Equal(id, Create(User(id)).UserId);
    }

    [Fact]
    public void UserId_Anonymous_ReturnsEmpty()
    {
        Assert.Equal(Guid.Empty, Create(new ClaimsPrincipal(new ClaimsIdentity())).UserId);
    }

    [Fact]
    public void UserId_NoHttpContext_ReturnsEmpty()
    {
        Assert.Equal(Guid.Empty, Create(null).UserId);
    }

    [Theory]
    [InlineData(new[] { Roles.Admin }, true)]
    [InlineData(new[] { Roles.User, Roles.Admin }, true)]
    [InlineData(new[] { Roles.User }, false)]
    [InlineData(new string[0], false)]
    public void IsAdmin_DependsOnRoleClaim(string[] roles, bool expected)
    {
        Assert.Equal(expected, Create(User(Guid.NewGuid(), roles)).IsAdmin);
    }

    [Fact]
    public void IsAdmin_NoHttpContext_ReturnsFalse()
    {
        Assert.False(Create(null).IsAdmin);
    }
}
