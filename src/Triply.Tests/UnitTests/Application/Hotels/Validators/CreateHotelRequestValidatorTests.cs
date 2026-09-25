using FluentValidation.TestHelper;
using Moq;
using Triply.Application.Features.Hotels.Commands.CreateHotel;
using Triply.Application.Interfaces.Repositories;
using Triply.Domain.Enums.Hotels;
using Triply.Domain.Results;

namespace Triply.Tests.UnitTests.Application.Hotels.Validators;

public class CreateHotelRequestValidatorTests
{
    private readonly Mock<IHotelRepository> _hotelRepository = new();
    private readonly Mock<ICityRepository> _cityRepository = new();
    private readonly CreateHotelRequestValidator _validator;
    private readonly Guid _cityId = Guid.NewGuid();

    public CreateHotelRequestValidatorTests()
    {
        _hotelRepository.Setup(r => r.IsHotelExistsAsync(It.IsAny<string>(), 
            It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);
        _hotelRepository.Setup(r => r.IsLocationsExistsAsync(It.IsAny<decimal?>(), 
            It.IsAny<decimal?>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);
        _cityRepository.Setup(r => r.IsCityIdExistsAsync(It.IsAny<Guid>(), 
            It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _validator = new CreateHotelRequestValidator(_hotelRepository.Object, _cityRepository.Object);
    }

    private CreateHotelRequest ValidRequest() => new()
    {
        Name = "Darawsheh Hotel",
        CityId = _cityId,
        StarRating = 5,
        HotelType = HotelType.Luxury,
        Address = "Rafidia Street 100",
        Description = "Nice",
        Latitude = 32.2m,
        Longitude = 35.2m
    };

    [Fact]
    public async Task Validate_ValidRequest_HasNoErrors()
    {
        var result = await _validator.TestValidateAsync(ValidRequest());

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Validate_WithoutCoordinates_SkipsLocationCheck()
    {
        var request = ValidRequest();
        request.Latitude = null;
        request.Longitude = null;

        var result = await _validator.TestValidateAsync(request);

        result.ShouldNotHaveAnyValidationErrors();
        _hotelRepository.Verify(r => r.IsLocationsExistsAsync(It.IsAny<decimal?>(), 
            It.IsAny<decimal?>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public async Task Validate_MissingName_HasError(string? name)
    {
        var request = ValidRequest();
        request.Name = name!;

        var result = await _validator.TestValidateAsync(request);

        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public async Task Validate_NameTooLong_HasError()
    {
        var request = ValidRequest();
        request.Name = new string('a', 151);

        var result = await _validator.TestValidateAsync(request);

        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage(ResultResponseMessages.Hotels.Validation.NameMaxLength.Message);
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
    public async Task Validate_UndefinedHotelType_HasError()
    {
        var request = ValidRequest();
        request.HotelType = (HotelType)99;

        var result = await _validator.TestValidateAsync(request);

        result.ShouldHaveValidationErrorFor(x => x.HotelType);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public async Task Validate_MissingAddress_HasError(string? address)
    {
        var request = ValidRequest();
        request.Address = address!;

        var result = await _validator.TestValidateAsync(request);

        result.ShouldHaveValidationErrorFor(x => x.Address)
            .WithErrorMessage(ResultResponseMessages.Hotels.Validation.AddressRequired.Message);
    }

    [Fact]
    public async Task Validate_DescriptionTooLong_HasError()
    {
        var request = ValidRequest();
        request.Description = new string('a', 2001);

        var result = await _validator.TestValidateAsync(request);

        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Theory]
    [InlineData(-91, 0)]
    [InlineData(91, 0)]
    public async Task Validate_LatitudeOutOfRange_HasError(decimal latitude, decimal longitude)
    {
        var request = ValidRequest();
        request.Latitude = latitude;
        request.Longitude = longitude;

        var result = await _validator.TestValidateAsync(request);

        result.ShouldHaveValidationErrorFor(x => x.Latitude);
    }

    [Theory]
    [InlineData(0, -181)]
    [InlineData(0, 181)]
    public async Task Validate_LongitudeOutOfRange_HasError(decimal latitude, decimal longitude)
    {
        var request = ValidRequest();
        request.Latitude = latitude;
        request.Longitude = longitude;

        var result = await _validator.TestValidateAsync(request);

        result.ShouldHaveValidationErrorFor(x => x.Longitude);
    }

    [Fact]
    public async Task Validate_EmptyCityId_HasError()
    {
        var request = ValidRequest();
        request.CityId = Guid.Empty;

        var result = await _validator.TestValidateAsync(request);

        result.ShouldHaveValidationErrorFor(x => x.CityId);
    }

    [Fact]
    public async Task Validate_CityDoesNotExist_HasError()
    {
        _cityRepository.Setup(r => r.IsCityIdExistsAsync(_cityId, 
            It.IsAny<CancellationToken>())).ReturnsAsync(false);

        var result = await _validator.TestValidateAsync(ValidRequest());

        result.ShouldHaveValidationErrorFor(x => x.CityId)
            .WithErrorMessage(ResultResponseMessages.Cities.Validation.CityNotFound.Message);
    }

    [Fact]
    public async Task Validate_HotelNameExistsInCity_HasErrorOnName()
    {
        _hotelRepository.Setup(r => r.IsHotelExistsAsync("Darawsheh Hotel", _cityId, 
            It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var result = await _validator.TestValidateAsync(ValidRequest());

        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage(ResultResponseMessages.Hotels.Validation.HotelAlreadyExists.Message);
    }

    [Fact]
    public async Task Validate_LocationAlreadyUsed_HasErrorOnLatitude()
    {
        _hotelRepository.Setup(r => r.IsLocationsExistsAsync(32.2m, 35.2m, 
            It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var result = await _validator.TestValidateAsync(ValidRequest());

        result.ShouldHaveValidationErrorFor(x => x.Latitude)
            .WithErrorMessage(ResultResponseMessages.Hotels.Validation.LocationAlreadyExists.Message);
    }
}
