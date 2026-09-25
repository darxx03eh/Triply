using FluentValidation.TestHelper;
using Moq;
using Triply.Application.Features.Authentications.Commands.EmailConfirmation;
using Triply.Application.Interfaces.Repositories;
using Triply.Domain.Results;

namespace Triply.Tests.UnitTests.Application.Authentications.Validators;

public class ConfirmEmailRequestValidatorTests
{
    private readonly Mock<IUserRepository> _userRepository = new();
    private readonly ConfirmEmailRequestValidator _validator;

    public ConfirmEmailRequestValidatorTests()
    {
        _userRepository.Setup(r => r.IsEmailExistsAsync(It.IsAny<string>(), 
            It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _validator = new ConfirmEmailRequestValidator(_userRepository.Object);
    }

    [Fact]
    public async Task Validate_ValidRequest_HasNoErrors()
    {
        var result = await _validator.TestValidateAsync(new ConfirmEmailRequest { Email = "a@triply.com", Token = "token" });

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Validate_UnknownEmail_HasError()
    {
        _userRepository.Setup(r => r.IsEmailExistsAsync("ghost@triply.com", 
            It.IsAny<CancellationToken>())).ReturnsAsync(false);

        var result = await _validator.TestValidateAsync(new ConfirmEmailRequest 
            { Email = "ghost@triply.com", Token = "token" });

        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage(ResultResponseMessages.Authentication.Validation.EmailNotExists.Message);
    }

    [Theory]
    [InlineData("")]
    [InlineData("invalid")]
    public async Task Validate_InvalidEmail_HasError(string email)
    {
        var result = await _validator.TestValidateAsync(new ConfirmEmailRequest { Email = email, Token = "token" });

        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public async Task Validate_EmptyToken_HasError()
    {
        var result = await _validator.TestValidateAsync(new ConfirmEmailRequest { Email = "a@triply.com", Token = "" });

        result.ShouldHaveValidationErrorFor(x => x.Token)
            .WithErrorMessage(ResultResponseMessages.Authentication.Validation.TokenRequired.Message);
    }
}
