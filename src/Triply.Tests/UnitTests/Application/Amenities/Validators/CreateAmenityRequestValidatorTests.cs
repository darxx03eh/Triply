using FluentValidation.TestHelper;
using Moq;
using Triply.Application.Features.Amenities.Commands.CreateAmenity;
using Triply.Application.Interfaces.Repositories;
using Triply.Domain.Results;

namespace Triply.Tests.UnitTests.Application.Amenities.Validators;

public class CreateAmenityRequestValidatorTests
{
    private readonly Mock<IAmenityRepository> _amenityRepository = new();
    private readonly CreateAmenityRequestValidator _validator;

    public CreateAmenityRequestValidatorTests()
    {
        _amenityRepository.Setup(r => r.IsAmenityExistsAsync(
            It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);
        _validator = new CreateAmenityRequestValidator(_amenityRepository.Object);
    }

    [Fact]
    public async Task Validate_ValidName_HasNoErrors()
    {
        var result = await _validator.TestValidateAsync(new CreateAmenityRequest { Name = "Rooftop Bar" });

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Validate_DuplicateCheck_UsesTrimmedName()
    {
        await _validator.TestValidateAsync(new CreateAmenityRequest { Name = "  Rooftop Bar  " });

        _amenityRepository.Verify(r => 
            r.IsAmenityExistsAsync("Rooftop Bar", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Validate_NameAlreadyExists_HasError()
    {
        _amenityRepository.Setup(r => r.IsAmenityExistsAsync("Spa", 
            It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var result = await _validator.TestValidateAsync(new CreateAmenityRequest { Name = "Spa" });

        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage(ResultResponseMessages.Amenities.Validation.AmenityAlreadyExists.Message);
    }

    [Theory]
    [InlineData("")]
    [InlineData("    ")]
    public async Task Validate_BlankName_HasErrorAndSkipsDuplicateCheck(string name)
    {
        var result = await _validator.TestValidateAsync(new CreateAmenityRequest { Name = name });

        result.ShouldHaveValidationErrorFor(x => x.Name);
        _amenityRepository.Verify(r => r.IsAmenityExistsAsync(
            It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Validate_NameTooLong_HasError()
    {
        var result = await _validator.TestValidateAsync(new CreateAmenityRequest { Name = new string('a', 101) });

        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage(ResultResponseMessages.Amenities.Validation.NameMaxLength.Message);
    }
}
