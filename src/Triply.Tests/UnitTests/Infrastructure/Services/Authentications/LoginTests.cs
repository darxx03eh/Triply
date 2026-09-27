using Microsoft.AspNetCore.Identity;
using Moq;
using Triply.Application.DTOs.Tokens;
using Triply.Application.Features.Authentications.Commands.Login;
using Triply.Domain.Contracts;
using Triply.Domain.Entities.Identity;
using Triply.Domain.Exceptions;
using Triply.Domain.Results.Enums;
using Triply.Tests.UnitTests.Common.Assertions;
using Triply.Tests.UnitTests.Common.Builders;
using Triply.XUnitTests.Infrastructure.Services.Authentications;

namespace Triply.Tests.UnitTests.Infrastructure.Services.Authentications;

public class LoginTests : AuthenticationServiceTestBase
{
    private readonly TriplyUser _user = TestData.User();

    public LoginTests()
    {
        UserManager.Setup(m => m.FindByNameAsync("mahmoud")).ReturnsAsync(_user);
        UserManager.Setup(m => m.FindByEmailAsync("mahmoud@triply.com")).ReturnsAsync(_user);
        UserRepository.Setup(r => r.GetByPhoneNumberAsync("+970591234567", 
            It.IsAny<CancellationToken>())).ReturnsAsync(_user);
        SignInManager.Setup(s => s.CheckPasswordSignInAsync(_user, "Password123", true)).ReturnsAsync(SignInResult.Success);
        TokenService.Setup(t => t.GenerateAccessTokenAsync(_user, true, 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new TokenResponse("access-token", "refresh-token"));
    }

    private static LoginRequest Request(string identifier = "mahmoud", string password = "Password123")
        => new() { Identifier = identifier, Password = password };

    [Theory]
    [InlineData("mahmoud")]
    [InlineData("mahmoud@triply.com")]
    [InlineData("+970591234567")]
    public async Task LoginAsync_ValidCredentials_ReturnsTokensAndStampsLastLogin(string identifier)
    {
        var result = await Service.LoginAsync(Request(identifier), "unknown");

        var response = result.AssertSuccess();
        Assert.Equal("Mahmoud Darawsheh", response.Name);
        Assert.Equal("access-token", response.Access);
        Assert.Equal("refresh-token", response.Refresh);
        Assert.NotNull(_user.LastLoginAt);
        UserManager.Verify(m => m.UpdateAsync(_user), Times.Once);
    }

    [Fact]
    public async Task LoginAsync_UnknownUser_ReturnsInvalidCredentials()
    {
        var result = await Service.LoginAsync(Request("ghost"), "unknown");

        result.AssertFailure("INVALID_CREDENTIALS", ResultErrorType.Unauthorized);
        TokenService.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task LoginAsync_WrongPassword_ReturnsInvalidCredentials()
    {
        SignInManager.Setup(s => s.CheckPasswordSignInAsync(_user, 
            "WrongPass1", true)).ReturnsAsync(SignInResult.Failed);

        var result = await Service.LoginAsync(Request(password: "WrongPass1"), "unknown");

        result.AssertFailure("INVALID_CREDENTIALS", ResultErrorType.Unauthorized);
    }

    [Fact]
    public async Task LoginAsync_AlreadyLockedOut_ReturnsAccountLockedWithoutCheckingPassword()
    {
        _user.LockoutEnd = DateTimeOffset.UtcNow.AddMinutes(5);
        UserManager.Setup(m => m.IsLockedOutAsync(_user)).ReturnsAsync(true);

        var result = await Service.LoginAsync(Request(), "");

        result.AssertFailure("ACCOUNT_LOCKED", ResultErrorType.Forbidden);
        SignInManager.Verify(s => s.CheckPasswordSignInAsync(It.IsAny<TriplyUser>(), 
            It.IsAny<string>(), It.IsAny<bool>()), Times.Never);
    }

    [Fact]
    public async Task LoginAsync_FailureThatLocksAccount_ReturnsAccountLocked()
    {
        _user.LockoutEnd = DateTimeOffset.UtcNow.AddMinutes(5);
        SignInManager.Setup(s => s.CheckPasswordSignInAsync(_user, "WrongPass1",
            true)).ReturnsAsync(SignInResult.LockedOut);

        var result = await Service.LoginAsync(Request(password: "WrongPass1"),"unknown");

        result.AssertFailure("ACCOUNT_LOCKED", ResultErrorType.Forbidden);
    }

    [Fact]
    public async Task LoginAsync_EmailNotConfirmed_ResendsConfirmationEmail()
    {
        _user.EmailConfirmed = false;
        SignInManager.Setup(s => s.CheckPasswordSignInAsync(_user, "Password123",
            true)).ReturnsAsync(SignInResult.NotAllowed);

        var result = await Service.LoginAsync(Request(), "unknown");

        result.AssertFailure("EMAIL_NOT_CONFIRMED", ResultErrorType.Forbidden);
        Publisher.Verify(p => p.PublishAsync("email.send", 
            It.Is<EmailMessage>(m => m.To == _user.Email), null, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task LoginAsync_InactiveAccount_ReturnsForbidden()
    {
        _user.IsActive = false;

        var result = await Service.LoginAsync(Request(), "unknown");

        result.AssertFailure("ACCOUNT_NOT_ACTIVE", ResultErrorType.Forbidden);
        TokenService.VerifyNoOtherCalls();
    }

    [Theory]
    [InlineData("bad@email")]
    [InlineData("+123")]
    [InlineData("user name")]
    [InlineData("user!name")]
    public async Task LoginAsync_MalformedIdentifier_ThrowsInvalidFormat(string identifier)
    {
        await Assert.ThrowsAsync<InvalidFormatException>(() => Service.LoginAsync(Request(identifier), "unknown"));
    }
}
