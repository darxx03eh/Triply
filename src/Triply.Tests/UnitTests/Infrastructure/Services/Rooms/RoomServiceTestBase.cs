using Moq;
using Microsoft.Extensions.Logging.Abstractions;
using Triply.Application.Interfaces.Repositories;
using Triply.Domain.Entities;
using Triply.Infrastructure.Services.Rooms;
using Triply.Tests.UnitTests.Common.Builders;

namespace Triply.Tests.UnitTests.Infrastructure.Services.Rooms;

public abstract class RoomServiceTestBase
{
    protected readonly Mock<IRoomRepository> RoomRepository = new();
    protected readonly Mock<IHotelRepository> HotelRepository = new();
    protected readonly RoomService Service;
    protected readonly Hotel Hotel = TestData.Hotel(TestData.City(), "Red Sea Resort");

    protected RoomServiceTestBase() => Service = 
        new RoomService(RoomRepository.Object, HotelRepository.Object, NullLogger<RoomService>.Instance);

    protected Room ExistingRoom()
    {
        var room = TestData.Room(Hotel);
        RoomRepository.Setup(r => r.GetByIdAsync(room.RoomId, 
            It.IsAny<CancellationToken>())).ReturnsAsync(room);
        RoomRepository.Setup(r => r.GetByIdWithHotelAsync(room.RoomId, 
            It.IsAny<CancellationToken>())).ReturnsAsync(room);
        return room;
    }
}
