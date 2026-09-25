using FluentValidation.TestHelper;
using Moq;
using Triply.Application.Features.Hotels.Commands.SetHotelAmenities;
using Triply.Application.Interfaces.Repositories;
using Triply.Domain.Results;

namespace Triply.Tests.UnitTests.Application.Hotels.Validators;

public class SetHotelAmenitiesRequestValidatorTests
{
    private readonly Mock<IAmenityRepository> _amenityRepository = new();
    private readonly SetHotelAmenitiesRequestValidator _validator;

    public SetHotelAmenitiesRequestValidatorTests()
    {
        _amenityRepository.Setup(r => r.CountExistingAsync(It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((IEnumerable<Guid> ids, CancellationToken _) => ids.Distinct().Count());
        _validator = new SetHotelAmenitiesRequestValidator(_amenityRepository.Object);
    }

    [Fact]
    public async Task Validate_ExistingAmenities_HasNoErrors()
    {
        var result = await _validator.TestValidateAsync(new SetHotelAmenitiesRequest 
            { AmenityIds = [Guid.NewGuid(), Guid.NewGuid()] });

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Validate_EmptyList_IsValidAndSkipsDatabase()
    {
        var result = await _validator.TestValidateAsync(new SetHotelAmenitiesRequest { AmenityIds = [] });

        result.ShouldNotHaveAnyValidationErrors();
        _amenityRepository.Verify(r => r.CountExistingAsync(It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Validate_NullList_HasRequiredError()
    {
        var result = await _validator.TestValidateAsync(new SetHotelAmenitiesRequest { AmenityIds = null! });

        result.ShouldHaveValidationErrorFor(x => x.AmenityIds)
            .WithErrorMessage(ResultResponseMessages.Hotels.Validation.AmenityIdsRequired.Message);
    }

    [Fact]
    public async Task Validate_DuplicateIds_HasError()
    {
        var id = Guid.NewGuid();

        var result = await _validator.TestValidateAsync(new SetHotelAmenitiesRequest { AmenityIds = [id, id] });

        result.ShouldHaveValidationErrorFor(x => x.AmenityIds)
            .WithErrorMessage(ResultResponseMessages.Hotels.Validation.AmenityIdsDuplicated.Message);
    }

    [Fact]
    public async Task Validate_UnknownAmenity_HasError()
    {
        _amenityRepository.Setup(r => r.CountExistingAsync(It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var result = await _validator.TestValidateAsync(new SetHotelAmenitiesRequest 
            { AmenityIds = [Guid.NewGuid(), Guid.NewGuid()] });

        result.ShouldHaveValidationErrorFor(x => x.AmenityIds)
            .WithErrorMessage(ResultResponseMessages.Hotels.Validation.AmenityNotFound.Message);
    }
}
