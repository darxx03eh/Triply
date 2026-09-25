using FluentValidation.TestHelper;
using Moq;
using Triply.Application.Features.Rooms.Commands.UpdateRoom;
using Triply.Application.Interfaces.Repositories;
using Triply.Domain.Enums.Rooms;
using Triply.Domain.Results;
using Triply.Tests.UnitTests.Common.Builders;

namespace Triply.Tests.UnitTests.Application.Rooms.Validators;

public class UpdateRoomRequestValidatorTests
{
    private readonly Mock<IRoomRepository> _roomRepository = new();
    private readonly UpdateRoomRequestValidator _validator;
    private readonly Guid _roomId = Guid.NewGuid();

    public UpdateRoomRequestValidatorTests()
    {
        _roomRepository.Setup(r => r.IsRoomNumberExistsExcludeIdAsync(It.IsAny<string>(), 
                It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _validator = new UpdateRoomRequestValidator(_roomRepository.Object);
    }

    private UpdateRoomRequest ValidRequest() => new()
    {
        RoomId = _roomId,
        Number = "101",
        RoomType = RoomType.Suite,
        AdultCapacity = 3,
        ChildCapacity = 1,
        PricePerNight = 250m,
        IsAvailable = false,
        RowVersion = TestData.RowVersion
    };

    [Fact]
    public async Task Validate_ValidRequest_HasNoErrors()
    {
        var result = await _validator.TestValidateAsync(ValidRequest());

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Validate_DuplicateCheck_ExcludesTheRoomBeingUpdated()
    {
        await _validator.TestValidateAsync(ValidRequest());

        _roomRepository.Verify(r => r.IsRoomNumberExistsExcludeIdAsync("101", _roomId, 
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Validate_NumberUsedByAnotherRoom_HasErrorOnNumber()
    {
        _roomRepository.Setup(r => r.IsRoomNumberExistsExcludeIdAsync("101", _roomId, 
            It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var result = await _validator.TestValidateAsync(ValidRequest());

        result.ShouldHaveValidationErrorFor(x => x.Number)
            .WithErrorMessage(ResultResponseMessages.Rooms.Validation.RoomAlreadyExists.Message);
    }

    [Fact]
    public async Task Validate_MissingRowVersion_HasRequiredError()
    {
        var request = ValidRequest();
        request.RowVersion = [];

        var result = await _validator.TestValidateAsync(request);

        result.ShouldHaveValidationErrorFor(x => x.RowVersion)
            .WithErrorMessage(ResultResponseMessages.Rooms.Validation.RowVersionRequired.Message);
    }

    [Fact]
    public async Task Validate_RowVersionWrongLength_HasLengthError()
    {
        var request = ValidRequest();
        request.RowVersion = [9];

        var result = await _validator.TestValidateAsync(request);

        result.ShouldHaveValidationErrorFor(x => x.RowVersion)
            .WithErrorMessage(ResultResponseMessages.Rooms.Validation.RowVersionInvalidLength.Message);
    }

    [Theory]
    [InlineData(0, 0, 100)]
    [InlineData(2, 11, 100)]
    [InlineData(2, 0, 0)]
    public async Task Validate_InvalidCapacityOrPrice_HasErrors(short adults, short children, decimal price)
    {
        var request = ValidRequest();
        request.AdultCapacity = adults;
        request.ChildCapacity = children;
        request.PricePerNight = price;

        var result = await _validator.TestValidateAsync(request);

        Assert.False(result.IsValid);
    }
}
