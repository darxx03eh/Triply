using MessageQueue.IRabbitMQ;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Triply.Application.Interfaces.Repositories;
using Triply.Application.Interfaces.Services;
using Triply.Domain.Entities.Identity;
using Triply.Infrastructure.Db;
using Triply.Infrastructure.Repositories;
using Triply.Infrastructure.Services.Authentications;
using Triply.Tests.UnitTests.Common.Database;
using Triply.Tests.UnitTests.Common.Identity;
using Triply.XUnitTests.Common.Identity;

namespace Triply.XUnitTests.Infrastructure.Services.Authentications;

public abstract class AuthenticationServiceTestBase : IDisposable
{
    protected readonly Mock<IUserRepository> UserRepository = new();
    protected readonly Mock<ITokenService> TokenService = new();
    protected readonly Mock<UserManager<TriplyUser>> UserManager = IdentityMocks.UserManager();
    protected readonly Mock<RoleManager<TriplyRole>> RoleManager = IdentityMocks.RoleManager();
    protected readonly Mock<SignInManager<TriplyUser>> SignInManager;
    protected readonly Mock<IMessagePublisher> Publisher = new();
    protected readonly Mock<ITokenBlacklistService> Blacklist = new();
    protected readonly Mock<IDbContextTransaction> Transaction = new();
    protected readonly TriplyDbContext Db = TestDbContextFactory.Create();
    protected readonly AuthenticationService Service;

    protected AuthenticationServiceTestBase()
    {
        SignInManager = IdentityMocks.SignInManager(UserManager.Object);
        UserRepository.Setup(r => r.BeginTransactionAsync()).ReturnsAsync(Transaction.Object);
        UserManager.Setup(m => m.GenerateEmailConfirmationTokenAsync(
            It.IsAny<TriplyUser>())).ReturnsAsync("confirm-token");

        var httpContext = new DefaultHttpContext();
        httpContext.Request.Scheme = "http";
        httpContext.Request.Host = new HostString("localhost:3000");
        var accessor = new Mock<IHttpContextAccessor>();
        accessor.Setup(a => a.HttpContext).Returns(httpContext);

        Service = new AuthenticationService(
            UserRepository.Object,
            new RefreshTokenRepository(Db),
            TokenService.Object,
            UserManager.Object,
            SignInManager.Object,
            RoleManager.Object,
            Publisher.Object,
            Blacklist.Object,
            accessor.Object,
            NullLogger<AuthenticationService>.Instance);
    }

    public void Dispose() => Db.Dispose();
}
