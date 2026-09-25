using Moq;
using Triply.Application.Features.Rooms.Commands.CreateRoom;
using Triply.Domain.Entities;
using Triply.Domain.Enums.Rooms;
using Triply.Domain.Results.Enums;
using Triply.Tests.UnitTests.Common.Assertions;

namespace Triply.Tests.UnitTests.Infrastructure.Services.Rooms;

public class CreateRoomTests : RoomServiceTestBase
{
    private CreateRoomRequest Request() => new()
    {
        HotelId = Hotel.HotelId,
        Number = "  T-101 ",
        RoomType = RoomType.Suite,
        AdultCapacity = 3,
        ChildCapacity = 2,
        PricePerNight = 250m,
        IsAvailable = false,
        Description = "Sea view"
    };

    [Fact]
    public async Task CreateAsync_ExistingHotel_SavesTrimmedRoomAndReturnsCreated()
    {
        Room? saved = null;
        HotelRepository.Setup(r => r.GetByIdAsync(Hotel.HotelId, It.IsAny<CancellationToken>())).ReturnsAsync(Hotel);
        RoomRepository.Setup(r => r.AddAsync(It.IsAny<Room>(), It.IsAny<CancellationToken>()))
            .Callback<Room, CancellationToken>((room, _) => saved = room);

        var result = await Service.CreateAsync(Request());

        var response = result.AssertSuccess(ResultSuccessType.Created);
        Assert.Equal("T-101", saved!.Number);
        Assert.Equal(RoomType.Suite, saved.RoomType);
        Assert.False(saved.IsAvailable);
        Assert.Equal("Red Sea Resort", response.HotelName);
        RoomRepository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_MissingHotel_ReturnsNotFound()
    {
        var result = await Service.CreateAsync(Request());

        result.AssertFailure("HOTEL_NOT_FOUND", ResultErrorType.NotFound);
        RoomRepository.Verify(r => r.AddAsync(It.IsAny<Room>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
