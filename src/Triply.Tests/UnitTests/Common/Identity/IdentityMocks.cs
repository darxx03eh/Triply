using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Triply.Domain.Entities.Identity;

namespace Triply.Tests.UnitTests.Common.Identity;

/// <summary>UserManager / SignInManager / RoleManager have no interfaces, so they are mocked through their stores.</summary>
public static class IdentityMocks
{
    public static Mock<UserManager<TriplyUser>> UserManager()
        => new(Mock.Of<IUserStore<TriplyUser>>(), null!, null!, null!, null!, null!, null!, null!, null!);

    public static Mock<RoleManager<TriplyRole>> RoleManager()
        => new(Mock.Of<IRoleStore<TriplyRole>>(), null!, null!, null!, null!);

    public static Mock<SignInManager<TriplyUser>> SignInManager(UserManager<TriplyUser> userManager)
        => new(userManager,
            Mock.Of<IHttpContextAccessor>(),
            Mock.Of<IUserClaimsPrincipalFactory<TriplyUser>>(),
            Options.Create(new IdentityOptions()),
            Mock.Of<ILogger<SignInManager<TriplyUser>>>(),
            Mock.Of<Microsoft.AspNetCore.Authentication.IAuthenticationSchemeProvider>(),
            Mock.Of<IUserConfirmation<TriplyUser>>());
}
