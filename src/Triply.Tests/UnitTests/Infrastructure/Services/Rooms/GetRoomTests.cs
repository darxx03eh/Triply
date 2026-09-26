using Moq;
using Sieve.Models;
using Triply.Application.Features.Rooms.Queries.GetRooms;
using Triply.Domain.Entities;
using Triply.Domain.Results.Enums;
using Triply.Tests.UnitTests.Common.Assertions;
using Triply.Tests.UnitTests.Common.Builders;

namespace Triply.Tests.UnitTests.Infrastructure.Services.Rooms;

public class GetRoomTests : RoomServiceTestBase
{
    [Fact]
    public async Task GetByIdAsync_Existing_ReturnsRoomWithHotelName()
    {
        var room = ExistingRoom();

        var response = (await Service.GetByIdAsync(room.RoomId)).AssertSuccess();

        Assert.Equal(room.RoomId, response.RoomId);
        Assert.Equal("Red Sea Resort", response.HotelName);
    }

    [Fact]
    public async Task GetByIdAsync_Existing_ReturnsBookingStateAndUploadedImageUrls()
    {
        var room = ExistingRoom();
        room.Images.Add(new RoomImage { RoomId = room.RoomId, Url = "https://cdn.test/room.jpg", DisplayOrder = 1 });
        HotelRepository.Setup(repository => repository.IsHotelIdExistsAsync(Hotel.HotelId,
            It.IsAny<CancellationToken>())).ReturnsAsync(true);
        RoomRepository.Setup(repository => repository.GetPagedAsync(It.IsAny<SieveModel>(), false, Hotel.HotelId,
            It.IsAny<CancellationToken>())).ReturnsAsync((new List<Room> { room }, 1));
        RoomRepository.Setup(r => r.IsBookedAsync(room.RoomId, It.IsAny<DateTime>(), It.IsAny<DateTime>(),
            It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var response = (await Service.GetHotelRoomsAsync(Hotel.HotelId,
            new GetRoomsRequest { CheckIn = new DateOnly(2026, 10, 10), CheckOut = new DateOnly(2026, 10, 12) }))
            .AssertSuccess().Items.Single();

        Assert.True(response.IsBooked);
        Assert.Equal(["https://cdn.test/room.jpg"], response.ImageUrls);
    }

    [Fact]
    public async Task GetByIdAsync_Missing_ReturnsNotFound()
    {
        var result = await Service.GetByIdAsync(Guid.NewGuid());

        result.AssertFailure("ROOM_NOT_FOUND", ResultErrorType.NotFound);
    }

    [Fact]
    public async Task GetPagedAsync_ReturnsPageForAllHotels()
    {
        RoomRepository.Setup(r => r.GetPagedAsync(It.IsAny<SieveModel>(), true, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync((new List<Room> { TestData.Room(Hotel, "1"), TestData.Room(Hotel, "2") }, 30));

        var result = await Service.GetPagedAsync(new GetRoomsRequest { Page = 3, PageSize = 2 }, isAdmin: true);

        var page = result.AssertSuccess();
        Assert.Equal(2, page.Items.Count);
        Assert.Equal(30, page.TotalCount);
        Assert.Equal(15, page.TotalPages);
        Assert.Equal("ROOMS_FOUND", result.SuccessObject!.Code);
    }

    [Fact]
    public async Task GetPagedAsync_Empty_ReturnsEmptyMessage()
    {
        RoomRepository.Setup(r => r.GetPagedAsync(It.IsAny<SieveModel>(), It.IsAny<bool>(), It.IsAny<Guid?>(), 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((new List<Room>(), 0));

        var result = await Service.GetPagedAsync(new GetRoomsRequest(), false);

        Assert.Equal("ROOMS_EMPTY", result.SuccessObject!.Code);
    }

    [Fact]
    public async Task GetHotelRoomsAsync_ExistingHotel_FiltersByHotelAndHidesDeleted()
    {
        HotelRepository.Setup(r => r.IsHotelIdExistsAsync(Hotel.HotelId, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        RoomRepository.Setup(r => r.GetPagedAsync(It.IsAny<SieveModel>(), false, Hotel.HotelId, 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((new List<Room> { TestData.Room(Hotel) }, 1));

        var result = await Service.GetHotelRoomsAsync(Hotel.HotelId, new GetRoomsRequest());

        Assert.Single(result.AssertSuccess().Items);
        RoomRepository.Verify(r => r.GetPagedAsync(It.IsAny<SieveModel>(), false, Hotel.HotelId, 
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetHotelRoomsAsync_MissingHotel_ReturnsNotFound()
    {
        var result = await Service.GetHotelRoomsAsync(Guid.NewGuid(), new GetRoomsRequest());

        result.AssertFailure("HOTEL_NOT_FOUND", ResultErrorType.NotFound);
        RoomRepository.Verify(r => r.GetPagedAsync(It.IsAny<SieveModel>(), It.IsAny<bool>(), 
            It.IsAny<Guid?>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
