using MessageQueue.IRabbitMQ;
using Moq;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client.Extensions.Msal;
using Triply.Application.Interfaces.Repositories;
using Triply.Application.Options;
using Triply.Domain.Entities;
using Triply.Infrastructure.Services.Rooms;
using Triply.Tests.UnitTests.Common.Builders;
using Triply.Tests.UnitTests.Common.Fakes;

namespace Triply.Tests.UnitTests.Infrastructure.Services.Rooms;

public abstract class RoomServiceTestBase
{
    protected readonly Mock<IRoomRepository> RoomRepository = new();
    protected readonly Mock<IHotelRepository> HotelRepository = new();
    protected readonly Mock<IRoomImageRepository> ImageRepository = new();
    protected readonly Mock<IMessagePublisher> Publisher = new();
    protected readonly TempStorage Storage = new();


    protected readonly RoomService Service;
    protected readonly Hotel Hotel = TestData.Hotel(TestData.City(), "Red Sea Resort");

    protected RoomServiceTestBase() => Service = 
        new RoomService(RoomRepository.Object, HotelRepository.Object, ImageRepository.Object, 
            Publisher.Object,
            Options.Create(new ImageUploadOptions { SharedStoragePath = Storage.Path }),
            NullLogger<RoomService>.Instance);

    protected Room ExistingRoom()
    {
        var room = TestData.Room(Hotel);
        RoomRepository.Setup(r => r.GetByIdAsync(room.RoomId, 
            It.IsAny<CancellationToken>())).ReturnsAsync(room);
        RoomRepository.Setup(r => r.GetByIdWithHotelAndImagesAsync(room.RoomId, 
            It.IsAny<CancellationToken>())).ReturnsAsync(room);
        return room;
    }
}
