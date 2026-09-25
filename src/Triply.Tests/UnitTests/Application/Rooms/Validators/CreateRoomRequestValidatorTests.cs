using FluentValidation.TestHelper;
using Moq;
using Triply.Application.Features.Rooms.Commands.CreateRoom;
using Triply.Application.Interfaces.Repositories;
using Triply.Domain.Enums.Rooms;
using Triply.Domain.Results;

namespace Triply.Tests.UnitTests.Application.Rooms.Validators;

public class CreateRoomRequestValidatorTests
{
    private readonly Mock<IRoomRepository> _roomRepository = new();
    private readonly Mock<IHotelRepository> _hotelRepository = new();
    private readonly CreateRoomRequestValidator _validator;
    private readonly Guid _hotelId = Guid.NewGuid();

    public CreateRoomRequestValidatorTests()
    {
        _hotelRepository.Setup(r => r.IsHotelIdExistsAsync(It.IsAny<Guid>(), 
            It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _roomRepository.Setup(r => r.IsRoomNumberExistsAsync(It.IsAny<Guid>(), 
                It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _validator = new CreateRoomRequestValidator(_roomRepository.Object, _hotelRepository.Object);
    }

    private CreateRoomRequest ValidRequest() => new()
    {
        HotelId = _hotelId,
        Number = "101",
        RoomType = RoomType.Double,
        AdultCapacity = 2,
        ChildCapacity = 1,
        PricePerNight = 120.50m,
        IsAvailable = true,
        Description = "Sea view"
    };

    [Fact]
    public async Task Validate_ValidRequest_HasNoErrors()
    {
        var result = await _validator.TestValidateAsync(ValidRequest());

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Validate_EmptyHotelId_HasRequiredErrorAndSkipsDatabase()
    {
        var request = ValidRequest();
        request.HotelId = Guid.Empty;

        var result = await _validator.TestValidateAsync(request);

        result.ShouldHaveValidationErrorFor(x => x.HotelId)
            .WithErrorMessage(ResultResponseMessages.Rooms.Validation.HotelIdRequired.Message);
        _hotelRepository.Verify(r => r.IsHotelIdExistsAsync(It.IsAny<Guid>(), 
            It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Validate_HotelDoesNotExist_HasError()
    {
        _hotelRepository.Setup(r => r.IsHotelIdExistsAsync(_hotelId, 
            It.IsAny<CancellationToken>())).ReturnsAsync(false);

        var result = await _validator.TestValidateAsync(ValidRequest());

        result.ShouldHaveValidationErrorFor(x => x.HotelId)
            .WithErrorMessage(ResultResponseMessages.Rooms.Validation.HotelNotFound.Message);
    }

    [Fact]
    public async Task Validate_NumberAlreadyExistsInHotel_HasErrorOnNumber()
    {
        _roomRepository.Setup(r => r.IsRoomNumberExistsAsync(_hotelId, "101", 
            It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var result = await _validator.TestValidateAsync(ValidRequest());

        result.ShouldHaveValidationErrorFor(x => x.Number)
            .WithErrorMessage(ResultResponseMessages.Rooms.Validation.RoomAlreadyExists.Message);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Validate_BlankNumber_HasErrorAndSkipsDuplicateCheck(string number)
    {
        var request = ValidRequest();
        request.Number = number;

        var result = await _validator.TestValidateAsync(request);

        result.ShouldHaveValidationErrorFor(x => x.Number);
        _roomRepository.Verify(r => r.IsRoomNumberExistsAsync(It.IsAny<Guid>(), 
            It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Validate_NumberTooLong_HasError()
    {
        var request = ValidRequest();
        request.Number = new string('1', 21);

        var result = await _validator.TestValidateAsync(request);

        result.ShouldHaveValidationErrorFor(x => x.Number)
            .WithErrorMessage(ResultResponseMessages.Rooms.Validation.NumberMaxLength.Message);
    }

    [Fact]
    public async Task Validate_UndefinedRoomType_HasError()
    {
        var request = ValidRequest();
        request.RoomType = (RoomType)42;

        var result = await _validator.TestValidateAsync(request);

        result.ShouldHaveValidationErrorFor(x => x.RoomType);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(11)]
    public async Task Validate_AdultCapacityOutOfRange_HasError(short adults)
    {
        var request = ValidRequest();
        request.AdultCapacity = adults;

        var result = await _validator.TestValidateAsync(request);

        result.ShouldHaveValidationErrorFor(x => x.AdultCapacity);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(11)]
    public async Task Validate_ChildCapacityOutOfRange_HasError(short children)
    {
        var request = ValidRequest();
        request.ChildCapacity = children;

        var result = await _validator.TestValidateAsync(request);

        result.ShouldHaveValidationErrorFor(x => x.ChildCapacity);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    public async Task Validate_PriceNotPositive_HasError(decimal price)
    {
        var request = ValidRequest();
        request.PricePerNight = price;

        var result = await _validator.TestValidateAsync(request);

        result.ShouldHaveValidationErrorFor(x => x.PricePerNight)
            .WithErrorMessage(ResultResponseMessages.Rooms.Validation.PricePerNightInvalid.Message);
    }

    [Theory]
    [InlineData(10.555)]
    [InlineData(123456789)]
    public async Task Validate_PriceExceedsPrecision_HasError(decimal price)
    {
        var request = ValidRequest();
        request.PricePerNight = price;

        var result = await _validator.TestValidateAsync(request);

        result.ShouldHaveValidationErrorFor(x => x.PricePerNight)
            .WithErrorMessage(ResultResponseMessages.Rooms.Validation.PricePerNightPrecision.Message);
    }

    [Fact]
    public async Task Validate_DescriptionTooLong_HasError()
    {
        var request = ValidRequest();
        request.Description = new string('a', 1001);

        var result = await _validator.TestValidateAsync(request);

        result.ShouldHaveValidationErrorFor(x => x.Description);
    }
}
