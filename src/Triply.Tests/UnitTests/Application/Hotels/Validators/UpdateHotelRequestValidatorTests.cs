using FluentValidation.TestHelper;
using Moq;
using Triply.Application.Features.Hotels.Commands.UpdateHotel;
using Triply.Application.Interfaces.Repositories;
using Triply.Domain.Enums.Hotels;
using Triply.Domain.Results;
using Triply.Tests.UnitTests.Common.Builders;

namespace Triply.Tests.UnitTests.Application.Hotels.Validators;

public class UpdateHotelRequestValidatorTests
{
    private readonly Mock<IHotelRepository> _hotelRepository = new();
    private readonly Mock<ICityRepository> _cityRepository = new();
    private readonly UpdateHotelRequestValidator _validator;
    private readonly Guid _hotelId = Guid.NewGuid();
    private readonly Guid _cityId = Guid.NewGuid();

    public UpdateHotelRequestValidatorTests()
    {
        _hotelRepository.Setup(r => r.IsHotelExistsExcludeId(It.IsAny<string>(), 
                It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _hotelRepository.Setup(r => r.IsLocationExistsExcludeIdAsync(It.IsAny<decimal?>(), 
                It.IsAny<decimal?>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _cityRepository.Setup(r => r.IsCityIdExistsAsync(It.IsAny<Guid>(), 
            It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _validator = new UpdateHotelRequestValidator(_hotelRepository.Object, _cityRepository.Object);
    }

    private UpdateHotelRequest ValidRequest() => new()
    {
        HotelId = _hotelId,
        Name = "Darawsheh Hotel",
        CityId = _cityId,
        StarRating = 4,
        HotelType = HotelType.Boutique,
        Address = "Rafidia Street 100",
        Latitude = 32.2m,
        Longitude = 35.2m,
        RowVersion = TestData.RowVersion
    };

    [Fact]
    public async Task Validate_ValidRequest_HasNoErrors()
    {
        var result = await _validator.TestValidateAsync(ValidRequest());

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Validate_UniquenessChecks_ExcludeTheHotelBeingUpdated()
    {
        await _validator.TestValidateAsync(ValidRequest());

        _hotelRepository.Verify(r => 
            r.IsHotelExistsExcludeId("Darawsheh Hotel", _cityId, _hotelId, 
                It.IsAny<CancellationToken>()), Times.Once);
        _hotelRepository.Verify(r => 
            r.IsLocationExistsExcludeIdAsync(32.2m, 35.2m, _hotelId, 
                It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Validate_NameUsedByAnotherHotelInCity_HasErrorOnName()
    {
        _hotelRepository.Setup(r => 
                r.IsHotelExistsExcludeId("Darawsheh Hotel", _cityId, _hotelId, 
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _validator.TestValidateAsync(ValidRequest());

        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage(ResultResponseMessages.Hotels.Validation.HotelAlreadyExists.Message);
    }

    [Fact]
    public async Task Validate_LocationUsedByAnotherHotel_HasErrorOnLatitude()
    {
        _hotelRepository.Setup(r => 
                r.IsLocationExistsExcludeIdAsync(32.2m, 35.2m, _hotelId, 
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _validator.TestValidateAsync(ValidRequest());

        result.ShouldHaveValidationErrorFor(x => x.Latitude);
    }

    [Fact]
    public async Task Validate_CityDoesNotExist_HasError()
    {
        _cityRepository.Setup(r => r.IsCityIdExistsAsync(_cityId, 
            It.IsAny<CancellationToken>())).ReturnsAsync(false);

        var result = await _validator.TestValidateAsync(ValidRequest());

        result.ShouldHaveValidationErrorFor(x => x.CityId);
    }

    [Fact]
    public async Task Validate_MissingRowVersion_HasRequiredError()
    {
        var request = ValidRequest();
        request.RowVersion = [];

        var result = await _validator.TestValidateAsync(request);

        result.ShouldHaveValidationErrorFor(x => x.RowVersion)
            .WithErrorMessage(ResultResponseMessages.Hotels.Validation.RowVersionRequired.Message);
    }

    [Fact]
    public async Task Validate_RowVersionWrongLength_HasLengthError()
    {
        var request = ValidRequest();
        request.RowVersion = [1, 2, 3];

        var result = await _validator.TestValidateAsync(request);

        result.ShouldHaveValidationErrorFor(x => x.RowVersion)
            .WithErrorMessage(ResultResponseMessages.Hotels.Validation.RowVersionInvalidLength.Message);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(6)]
    public async Task Validate_StarRatingOutOfRange_HasError(byte stars)
    {
        var request = ValidRequest();
        request.StarRating = stars;

        var result = await _validator.TestValidateAsync(request);

        result.ShouldHaveValidationErrorFor(x => x.StarRating);
    }

    [Fact]
    public async Task Validate_MissingAddress_HasError()
    {
        var request = ValidRequest();
        request.Address = "";

        var result = await _validator.TestValidateAsync(request);

        result.ShouldHaveValidationErrorFor(x => x.Address);
    }
}
