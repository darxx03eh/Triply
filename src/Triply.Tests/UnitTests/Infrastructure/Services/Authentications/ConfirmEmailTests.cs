using Microsoft.AspNetCore.Identity;
using Moq;
using Triply.Application.Features.Authentications.Commands.EmailConfirmation;
using Triply.Domain.Entities.Identity;
using Triply.Domain.Exceptions;
using Triply.Domain.Results.Enums;
using Triply.Tests.UnitTests.Common.Assertions;
using Triply.Tests.UnitTests.Common.Builders;
using Triply.XUnitTests.Infrastructure.Services.Authentications;

namespace Triply.Tests.UnitTests.Infrastructure.Services.Authentications;

public class ConfirmEmailTests : AuthenticationServiceTestBase
{
    private readonly TriplyUser _user = TestData.User(emailConfirmed: false);
    private readonly ConfirmEmailRequest _request = new() { Email = "mahmoud@triply.com", Token = "token" };

    public ConfirmEmailTests() => UserManager.Setup(m => 
        m.FindByEmailAsync(_request.Email)).ReturnsAsync(_user);

    [Fact]
    public async Task ConfirmationEmailAsync_ValidToken_Confirms()
    {
        UserManager.Setup(m => m.ConfirmEmailAsync(_user, "token"))
            .ReturnsAsync(IdentityResult.Success);

        var result = await Service.ConfirmationEmailAsync(_request);

        result.AssertSuccess();
        UserManager.Verify(m => m.ConfirmEmailAsync(_user, "token"), Times.Once);
    }

    [Fact]
    public async Task ConfirmationEmailAsync_AlreadyConfirmed_ReturnsFailureWithoutConfirming()
    {
        _user.EmailConfirmed = true;

        var result = await Service.ConfirmationEmailAsync(_request);

        result.AssertFailure("EMAIL_ALREADY_VERIFIED", ResultErrorType.BusinessRule);
        UserManager.Verify(m => m.ConfirmEmailAsync(It.IsAny<TriplyUser>(), 
            It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task ConfirmationEmailAsync_InvalidOrExpiredToken_ReturnsFailure()
    {
        UserManager.Setup(m => m.ConfirmEmailAsync(_user, "token"))
            .ReturnsAsync(IdentityResult.Failed());

        var result = await Service.ConfirmationEmailAsync(_request);

        result.AssertFailure("EMAIL_VERIFICATION_EXPIRED", ResultErrorType.BusinessRule);
    }

    [Fact]
    public async Task ConfirmationEmailAsync_UnknownEmail_ThrowsNotFound()
    {
        await Assert.ThrowsAsync<NotFoundException>(
            () => Service.ConfirmationEmailAsync(new ConfirmEmailRequest { Email = "ghost@triply.com", Token = "t" }));
    }
}
