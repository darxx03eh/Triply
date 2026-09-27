using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Moq;
using Triply.Application.Features.Authentications.Commands.Register;
using Triply.Domain.Constants;
using Triply.Domain.Contracts;
using Triply.Domain.Contracts.Enums;
using Triply.Domain.Entities.Identity;
using Triply.Domain.Exceptions;
using Triply.Domain.Results.Enums;
using Triply.Tests.UnitTests.Common.Assertions;
using Triply.XUnitTests.Infrastructure.Services.Authentications;

namespace Triply.Tests.UnitTests.Infrastructure.Services.Authentications;

public class RegisterTests : AuthenticationServiceTestBase
{
    private EmailMessage? _email;

    public RegisterTests()
    {
        UserManager.Setup(m => m.CreateAsync(It.IsAny<TriplyUser>(), 
            It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);
        UserManager.Setup(m => m.AddToRoleAsync(It.IsAny<TriplyUser>(), 
            Roles.User)).ReturnsAsync(IdentityResult.Success);
        RoleManager.Setup(m => m.RoleExistsAsync(Roles.User)).ReturnsAsync(true);
        Publisher.Setup(p => p.PublishAsync("email.send", 
                It.IsAny<EmailMessage>(), null, It.IsAny<CancellationToken>()))
            .Callback<string, EmailMessage, string?, CancellationToken>((_, m, _, _) => _email = m);
    }

    private static RegisterUserRequest Request() => new()
    {
        FirstName = "Lina",
        LastName = "Haddad",
        Email = "lina@triply.com",
        Username = "lina",
        Password = "Password123",
        ConfirmPassword = "Password123",
        PhoneNumber = "+962791234567"
    };

    [Fact]
    public async Task RegisterNewUserAsync_Success_CommitsAssignsUserRoleAndSendsConfirmationEmail()
    {
        var result = await Service.RegisterNewUserAsync(Request());

        var response = result.AssertSuccess(ResultSuccessType.Created);
        Assert.Equal("Lina Haddad", response.Name);
        Assert.Equal("lina@triply.com", response.Email);
        Assert.Equal("lina", response.Username);
        UserManager.Verify(m => 
            m.AddToRoleAsync(It.Is<TriplyUser>(u => u.UserName == "lina"), Roles.User), Times.Once);
        Transaction.Verify(t => 
            t.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        Transaction.Verify(t => 
            t.RollbackAsync(It.IsAny<CancellationToken>()), Times.Never);
        Assert.Equal(EmailType.ConfirmationEmail, _email!.Type);
        Assert.Equal("lina@triply.com", _email.To);
    }

    [Fact]
    public async Task RegisterNewUserAsync_Success_BuildsConfirmationLinkForFrontend()
    {
        await Service.RegisterNewUserAsync(Request());

        var link = _email!.TemplateData["confirmation_link"];
        var confirmationUri = new Uri(link);
        var query = QueryHelpers.ParseQuery(confirmationUri.Query);

        Assert.Equal("https", confirmationUri.Scheme);
        Assert.Equal("triply.example", confirmationUri.Host);
        Assert.Equal("/confirm-email", confirmationUri.AbsolutePath);
        Assert.Equal("lina@triply.com", query["email"].Single());
        Assert.Equal("confirm-token", query["token"].Single());
    }

    [Fact]
    public async Task RegisterNewUserAsync_IdentityRejectsUser_RollsBackAndReturnsFieldErrors()
    {
        UserManager.Setup(m => m.CreateAsync(It.IsAny<TriplyUser>(), 
                It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Passwords must have a digit." }));

        var result = await Service.RegisterNewUserAsync(Request());

        Assert.False(result.IsSuccess);
        Assert.Equal("Passwords must have a digit.", result.Fields["General"].Single());
        Transaction.Verify(t => t.RollbackAsync(
            It.IsAny<CancellationToken>()), Times.Once);
        Transaction.Verify(t => t.CommitAsync(
            It.IsAny<CancellationToken>()), Times.Never);
        Publisher.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task RegisterNewUserAsync_RoleMissing_RollsBackAndThrows()
    {
        RoleManager.Setup(m => m.RoleExistsAsync(Roles.User)).ReturnsAsync(false);

        await Assert.ThrowsAsync<NotFoundException>(() => Service.RegisterNewUserAsync(Request()));

        Transaction.Verify(t => t.RollbackAsync(
            It.IsAny<CancellationToken>()), Times.AtLeastOnce);
        Transaction.Verify(t => t.CommitAsync(
            It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task RegisterNewUserAsync_RoleAssignmentFails_RollsBackAndReturnsFailure()
    {
        UserManager.Setup(m => 
            m.AddToRoleAsync(It.IsAny<TriplyUser>(), Roles.User)).ReturnsAsync(IdentityResult.Failed());

        var result = await Service.RegisterNewUserAsync(Request());

        result.AssertFailure("ROLE_ASSIGNMENT_FAILED", ResultErrorType.BusinessRule);
        Transaction.Verify(t => 
            t.RollbackAsync(It.IsAny<CancellationToken>()), Times.Once);
        Publisher.VerifyNoOtherCalls();
    }
}
