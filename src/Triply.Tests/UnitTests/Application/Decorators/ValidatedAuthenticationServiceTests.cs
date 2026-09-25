using System.Security.Claims;
using Moq;
using Triply.Application.Exceptions;
using Triply.Application.Features.Authentications.Commands.EmailConfirmation;
using Triply.Application.Features.Authentications.Commands.Login;
using Triply.Application.Features.Authentications.Commands.Register;
using Triply.Application.Interfaces.Services;
using Triply.Application.Services;
using Triply.Tests.UnitTests.Common.Fakes;

namespace Triply.Tests.UnitTests.Application.Decorators;

public class ValidatedAuthenticationServiceTests
{
    private readonly Mock<IAuthenticationService> _inner = new();

    private ValidatedAuthenticationService Create(bool valid = true) => new(
        _inner.Object,
        valid ? FakeValidators.Passing<RegisterUserRequest>() : FakeValidators.Failing<RegisterUserRequest>(),
        valid ? FakeValidators.Passing<ConfirmEmailRequest>() : FakeValidators.Failing<ConfirmEmailRequest>(),
        valid ? FakeValidators.Passing<LoginRequest>() : FakeValidators.Failing<LoginRequest>());

    [Fact]
    public async Task ValidatedMethods_Valid_CallInner()
    {
        var service = Create();

        await service.RegisterNewUserAsync(new RegisterUserRequest());
        await service.ConfirmationEmailAsync(new ConfirmEmailRequest());
        await service.LoginAsync(new LoginRequest());

        _inner.Verify(s => s.RegisterNewUserAsync(It.IsAny<RegisterUserRequest>(), 
            It.IsAny<CancellationToken>()), Times.Once);
        _inner.Verify(s => s.ConfirmationEmailAsync(It.IsAny<ConfirmEmailRequest>(), It.IsAny<CancellationToken>()), Times.Once);
        _inner.Verify(s => s.LoginAsync(It.IsAny<LoginRequest>(), 
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ValidatedMethods_Invalid_ThrowAndSkipInner()
    {
        var service = Create(false);

        await Assert.ThrowsAsync<UnprocessableEntityException>(() => 
            service.RegisterNewUserAsync(new RegisterUserRequest()));
        await Assert.ThrowsAsync<UnprocessableEntityException>(() => 
            service.ConfirmationEmailAsync(new ConfirmEmailRequest()));
        await Assert.ThrowsAsync<UnprocessableEntityException>(() => 
            service.LoginAsync(new LoginRequest()));

        _inner.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task LogoutAndRefresh_AreNotValidated_AndCallInner()
    {
        var principal = new ClaimsPrincipal();
        var service = Create(false);

        await service.Logout(principal, "refresh");
        await service.GenerateAccessTokenFromRefreshToken("refresh", principal);

        _inner.Verify(s => s.Logout(principal, "refresh", 
            It.IsAny<CancellationToken>()), Times.Once);
        _inner.Verify(s => s.GenerateAccessTokenFromRefreshToken("refresh", principal,
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
