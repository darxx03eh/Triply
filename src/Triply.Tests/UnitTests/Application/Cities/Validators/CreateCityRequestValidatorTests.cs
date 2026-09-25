using FluentValidation.TestHelper;
using Moq;
using Triply.Application.Features.Cities.Commands.CreateCity;
using Triply.Application.Interfaces.Repositories;
using Triply.Domain.Results;

namespace Triply.Tests.UnitTests.Application.Cities.Validators;

public class CreateCityRequestValidatorTests
{
    private readonly Mock<ICityRepository> _cityRepository = new();
    private readonly CreateCityRequestValidator _validator;

    public CreateCityRequestValidatorTests()
    {
        _cityRepository.Setup(r => r.IsCityExistsAsync(It.IsAny<string>(), 
                It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _validator = new CreateCityRequestValidator(_cityRepository.Object);
    }

    private static CreateCityRequest ValidRequest() => new() 
        { Name = "Nablus", Country = "Palestine", PostOffice = "P400" };

    [Fact]
    public async Task Validate_ValidRequest_HasNoErrors()
    {
        var result = await _validator.TestValidateAsync(ValidRequest());

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Validate_WithoutPostOffice_IsValid()
    {
        var request = ValidRequest();
        request.PostOffice = null;

        var result = await _validator.TestValidateAsync(request);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Validate_EmptyName_HasError(string name)
    {
        var request = ValidRequest();
        request.Name = name;

        var result = await _validator.TestValidateAsync(request);

        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public async Task Validate_NameTooLong_HasError()
    {
        var request = ValidRequest();
        request.Name = new string('a', 101);

        var result = await _validator.TestValidateAsync(request);

        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage(ResultResponseMessages.Cities.Validation.NameMaxLength.Message);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Validate_EmptyCountry_HasError(string country)
    {
        var request = ValidRequest();
        request.Country = country;

        var result = await _validator.TestValidateAsync(request);

        result.ShouldHaveValidationErrorFor(x => x.Country);
    }

    [Fact]
    public async Task Validate_PostOfficeTooLong_HasError()
    {
        var request = ValidRequest();
        request.PostOffice = new string('1', 21);

        var result = await _validator.TestValidateAsync(request);

        result.ShouldHaveValidationErrorFor(x => x.PostOffice);
    }

    [Fact]
    public async Task Validate_CityAlreadyExistsInCountry_HasErrorOnName()
    {
        _cityRepository.Setup(r => r.IsCityExistsAsync("Nablus", 
            "Palestine", It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var result = await _validator.TestValidateAsync(ValidRequest());

        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage(ResultResponseMessages.Cities.Validation.CityAlreadyExistsInThisCountry.Message);
    }
}
