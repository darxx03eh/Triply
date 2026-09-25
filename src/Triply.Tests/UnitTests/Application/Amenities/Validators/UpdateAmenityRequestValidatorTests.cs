using FluentValidation.TestHelper;
using Moq;
using Triply.Application.Features.Amenities.Commands.UpdateAmenity;
using Triply.Application.Interfaces.Repositories;
using Triply.Domain.Results;

namespace Triply.Tests.UnitTests.Application.Amenities.Validators;

public class UpdateAmenityRequestValidatorTests
{
    private readonly Mock<IAmenityRepository> _amenityRepository = new();
    private readonly UpdateAmenityRequestValidator _validator;
    private readonly Guid _amenityId = Guid.NewGuid();

    public UpdateAmenityRequestValidatorTests()
    {
        _amenityRepository.Setup(r => r.IsAmenityExistsExcludeIdAsync(
                It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _validator = new UpdateAmenityRequestValidator(_amenityRepository.Object);
    }

    [Fact]
    public async Task Validate_ValidName_HasNoErrors()
    {
        var result = await _validator.TestValidateAsync(new UpdateAmenityRequest 
            { AmenityId = _amenityId, Name = "Sky Lounge" });

        result.ShouldNotHaveAnyValidationErrors();
        _amenityRepository.Verify(r => r.IsAmenityExistsExcludeIdAsync("Sky Lounge", 
            _amenityId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Validate_NameUsedByAnotherAmenity_HasErrorOnName()
    {
        _amenityRepository.Setup(r => r.IsAmenityExistsExcludeIdAsync("Spa", _amenityId, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var result = await _validator.TestValidateAsync(new UpdateAmenityRequest { AmenityId = _amenityId, Name = "Spa" });

        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage(ResultResponseMessages.Amenities.Validation.AmenityAlreadyExists.Message);
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    public async Task Validate_BlankName_HasError(string name)
    {
        var result = await _validator.TestValidateAsync(new UpdateAmenityRequest { AmenityId = _amenityId, Name = name });

        result.ShouldHaveValidationErrorFor(x => x.Name);
    }
}
