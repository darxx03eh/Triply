using FluentValidation.TestHelper;
using Triply.Application.Features.Authentications.Commands.Login;

namespace Triply.Tests.UnitTests.Application.Authentications.Validators;

public class LoginRequestValidatorTests
{
    private readonly LoginRequestValidator _validator = new();

    [Theory]
    [InlineData("admin")]
    [InlineData("admin@triply.com")]
    [InlineData("+970591234567")]
    public void Validate_ValidRequest_HasNoErrors(string identifier)
    {
        var result = _validator.TestValidate(new LoginRequest { Identifier = identifier, Password = "Password123" });

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_EmptyIdentifier_HasError()
    {
        var result = _validator.TestValidate(new LoginRequest { Identifier = "", Password = "Password123" });

        result.ShouldHaveValidationErrorFor(x => x.Identifier);
    }

    [Theory]
    [InlineData("")]
    [InlineData("1234567")]
    public void Validate_InvalidPassword_HasError(string password)
    {
        var result = _validator.TestValidate(new LoginRequest { Identifier = "admin", Password = password });

        result.ShouldHaveValidationErrorFor(x => x.Password);
    }
}
