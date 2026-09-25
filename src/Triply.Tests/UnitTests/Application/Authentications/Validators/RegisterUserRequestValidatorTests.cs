using FluentValidation.TestHelper;
using Moq;
using Triply.Application.Features.Authentications.Commands.Register;
using Triply.Application.Interfaces.Repositories;
using Triply.Domain.Results;

namespace Triply.Tests.UnitTests.Application.Authentications.Validators;

public class RegisterUserRequestValidatorTests
{
    private readonly Mock<IUserRepository> _userRepository = new();
    private readonly RegisterUserRequestValidator _validator;

    public RegisterUserRequestValidatorTests()
    {
        _userRepository.Setup(r => r.IsEmailExistsAsync(It.IsAny<string>(), 
            It.IsAny<CancellationToken>())).ReturnsAsync(false);
        _userRepository.Setup(r => r.IsUsernameExistsAsync(It.IsAny<string>(), 
            It.IsAny<CancellationToken>())).ReturnsAsync(false);
        _validator = new RegisterUserRequestValidator(_userRepository.Object);
    }

    private static RegisterUserRequest ValidRequest() => new()
    {
        FirstName = "Mahmoud",
        LastName = "Darawsheh",
        Email = "mahmoud@triply.com",
        Username = "mahmoud.d",
        Password = "Password123",
        ConfirmPassword = "Password123",
        PhoneNumber = "+970591234567",
        DateOfBirth = new DateTime(2000, 1, 1)
    };

    [Fact]
    public async Task Validate_ValidRequest_HasNoErrors()
    {
        var result = await _validator.TestValidateAsync(ValidRequest());

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Validate_WithoutDateOfBirth_IsValid()
    {
        var request = ValidRequest();
        request.DateOfBirth = null;

        var result = await _validator.TestValidateAsync(request);

        result.ShouldNotHaveValidationErrorFor(x => x.DateOfBirth);
    }

    [Theory]
    [InlineData("")]
    [InlineData("Mo")]
    public async Task Validate_InvalidFirstName_HasError(string firstName)
    {
        var request = ValidRequest();
        request.FirstName = firstName;

        var result = await _validator.TestValidateAsync(request);

        result.ShouldHaveValidationErrorFor(x => x.FirstName);
    }

    [Fact]
    public async Task Validate_FirstNameTooLong_HasMaxLengthError()
    {
        var request = ValidRequest();
        request.FirstName = new string('a', 101);

        var result = await _validator.TestValidateAsync(request);

        result.ShouldHaveValidationErrorFor(x => x.FirstName)
            .WithErrorMessage(ResultResponseMessages.Authentication.Validation.FirstNameMaxLength.Message);
    }

    [Theory]
    [InlineData("")]
    [InlineData("Da")]
    public async Task Validate_InvalidLastName_HasError(string lastName)
    {
        var request = ValidRequest();
        request.LastName = lastName;

        var result = await _validator.TestValidateAsync(request);

        result.ShouldHaveValidationErrorFor(x => x.LastName);
    }

    [Theory]
    [InlineData("")]
    [InlineData("not-an-email")]
    public async Task Validate_InvalidEmail_HasError(string email)
    {
        var request = ValidRequest();
        request.Email = email;

        var result = await _validator.TestValidateAsync(request);

        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public async Task Validate_EmailAlreadyExists_HasError()
    {
        _userRepository.Setup(r => r.IsEmailExistsAsync("mahmoud@triply.com", It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var result = await _validator.TestValidateAsync(ValidRequest());

        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage(ResultResponseMessages.Authentication.Validation.EmailAlreadyExists.Message);
    }

    [Fact]
    public async Task Validate_UsernameAlreadyExists_HasError()
    {
        _userRepository.Setup(r => r.IsUsernameExistsAsync("mahmoud.d", It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var result = await _validator.TestValidateAsync(ValidRequest());

        result.ShouldHaveValidationErrorFor(x => x.Username)
            .WithErrorMessage(ResultResponseMessages.Authentication.Validation.UsernameAlreadyExists.Message);
    }

    [Theory]
    [InlineData("")]
    [InlineData("ab")]
    [InlineData("mahmoud@d")]
    [InlineData("has space")]
    [InlineData("dash-name")]
    public async Task Validate_InvalidUsername_HasError(string username)
    {
        var request = ValidRequest();
        request.Username = username;

        var result = await _validator.TestValidateAsync(request);

        result.ShouldHaveValidationErrorFor(x => x.Username);
    }

    [Theory]
    [InlineData("")]
    [InlineData("short")]
    public async Task Validate_InvalidPassword_HasError(string password)
    {
        var request = ValidRequest();
        request.Password = password;
        request.ConfirmPassword = password;

        var result = await _validator.TestValidateAsync(request);

        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public async Task Validate_PasswordsDoNotMatch_HasError()
    {
        var request = ValidRequest();
        request.ConfirmPassword = "Different123";

        var result = await _validator.TestValidateAsync(request);

        result.ShouldHaveValidationErrorFor(x => x.ConfirmPassword)
            .WithErrorMessage(ResultResponseMessages.Authentication.Validation.PasswordsDoNotMatch.Message);
    }

    [Theory]
    [InlineData("")]
    [InlineData("0591234567")]
    [InlineData("+0591234567")]
    [InlineData("+97")]
    [InlineData("+9705912345678901")]
    [InlineData("+97059abc567")]
    public async Task Validate_InvalidPhoneNumber_HasError(string phone)
    {
        var request = ValidRequest();
        request.PhoneNumber = phone;

        var result = await _validator.TestValidateAsync(request);

        result.ShouldHaveValidationErrorFor(x => x.PhoneNumber);
    }

    [Fact]
    public async Task Validate_UserYoungerThan18_HasMinimumAgeError()
    {
        var request = ValidRequest();
        request.DateOfBirth = DateTime.UtcNow.AddYears(-17);

        var result = await _validator.TestValidateAsync(request);

        result.ShouldHaveValidationErrorFor(x => x.DateOfBirth)
            .WithErrorMessage(ResultResponseMessages.Authentication.Validation.MinimumAge.Message);
    }
}
