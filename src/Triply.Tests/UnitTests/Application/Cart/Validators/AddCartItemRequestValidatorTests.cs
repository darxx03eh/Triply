using FluentValidation.TestHelper;
using Moq;
using Triply.Application.Features.Cart.Commands.AddCartItem;
using Triply.Application.Interfaces.Repositories;
using Triply.Domain.Entities;
using Triply.Domain.Results;

namespace Triply.Tests.UnitTests.Application.Cart.Validators;

public class AddCartItemRequestValidatorTests
{
    private readonly Mock<IRoomRepository> _rooms = new();
    private readonly Mock<ICartRepository> _cart = new();
    private readonly AddCartItemRequestValidator _validator;

    public AddCartItemRequestValidatorTests()
    {
        _validator = new AddCartItemRequestValidator(_rooms.Object, _cart.Object);
        _rooms.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Room());
        _cart.Setup(x => x.IsItemExistsAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<DateTime>(),
                It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
    }

    [Fact]
    public async Task Validate_ValidRequest_HasNoErrors()
    {
        var result = await _validator.TestValidateAsync(ValidRequest());

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Validate_EmptyRoomId_HasRequiredError()
    {
        var result = await _validator.TestValidateAsync(ValidRequest(roomId: Guid.Empty));

        result.ShouldHaveValidationErrorFor(x => x.RoomId)
            .WithErrorMessage(ResultResponseMessages.Cart.Validation.RoomIdRequired.Message);
    }

    [Fact]
    public async Task Validate_PastCheckIn_HasCheckInError()
    {
        var result = await _validator.TestValidateAsync(ValidRequest(checkIn: DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1))));

        result.ShouldHaveValidationErrorFor(x => x.CheckIn)
            .WithErrorMessage(ResultResponseMessages.Cart.Validation.CheckInInPast.Message);
    }

    [Fact]
    public async Task Validate_CheckOutBeforeCheckIn_HasDateRangeError()
    {
        var request = ValidRequest();
        var result = await _validator.TestValidateAsync(ValidRequest(checkIn: request.CheckIn, checkOut: request.CheckIn));

        result.ShouldHaveValidationErrorFor(x => x.CheckOut)
            .WithErrorMessage(ResultResponseMessages.Cart.Validation.CheckOutBeforeCheckIn.Message);
    }

    [Fact]
    public async Task Validate_StayLongerThanThirtyDays_HasStayLengthError()
    {
        var request = ValidRequest();
        var result = await _validator.TestValidateAsync(ValidRequest(checkIn: request.CheckIn, checkOut: request.CheckIn.AddDays(31)));

        result.ShouldHaveValidationErrorFor(x => x.CheckOut)
            .WithErrorMessage(ResultResponseMessages.Cart.Validation.StayTooLong.Message);
    }

    [Theory]
    [InlineData((short)0)]
    [InlineData((short)21)]
    public async Task Validate_InvalidAdults_HasAdultsError(short adults)
    {
        var result = await _validator.TestValidateAsync(ValidRequest(adults: adults));

        result.ShouldHaveValidationErrorFor(x => x.Adults)
            .WithErrorMessage(ResultResponseMessages.Cart.Validation.AdultsInvalid.Message);
    }

    [Theory]
    [InlineData((short)-1)]
    [InlineData((short)21)]
    public async Task Validate_InvalidChildren_HasChildrenError(short children)
    {
        var result = await _validator.TestValidateAsync(ValidRequest(children: children));

        result.ShouldHaveValidationErrorFor(x => x.Children)
            .WithErrorMessage(ResultResponseMessages.Cart.Validation.ChildrenInvalid.Message);
    }

    [Fact]
    public async Task Validate_MissingRoom_HasRoomNotFoundError()
    {
        _rooms.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((Room)null!);

        var result = await _validator.TestValidateAsync(ValidRequest());

        result.ShouldHaveValidationErrorFor(x => x.RoomId)
            .WithErrorMessage(ResultResponseMessages.Cart.Validation.RoomNotFound.Message);
    }

    [Fact]
    public async Task Validate_OverlappingCartItem_HasAlreadyInCartError()
    {
        _cart.Setup(x => x.IsItemExistsAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<DateTime>(),
                It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _validator.TestValidateAsync(ValidRequest());

        result.ShouldHaveValidationErrorFor(x => x.RoomId)
            .WithErrorMessage(ResultResponseMessages.Cart.Validation.AlreadyInCart.Message);
    }

    private static AddCartItemRequest ValidRequest(Guid? roomId = null, DateOnly? checkIn = null,
        DateOnly? checkOut = null, short adults = 2, short children = 1) => new()
    {
        UserId = Guid.NewGuid(), RoomId = roomId ?? Guid.NewGuid(),
        CheckIn = checkIn ?? DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)),
        CheckOut = checkOut ?? DateOnly.FromDateTime(DateTime.UtcNow.AddDays(3)), Adults = adults, Children = children
    };
}
