using MessageQueue.IRabbitMQ;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using Triply.Application.Interfaces.Repositories;
using Triply.Application.Options;
using Triply.Domain.Entities;
using Triply.Infrastructure.Services.Hotels;
using Triply.Tests.UnitTests.Common.Builders;
using Triply.Tests.UnitTests.Common.Fakes;

namespace Triply.Tests.UnitTests.Infrastructure.Services.Hotels;

public abstract class HotelServiceTestBase : IDisposable
{
    protected readonly Mock<IHotelRepository> HotelRepository = new();
    protected readonly Mock<IHotelImageRepository> ImageRepository = new();
    protected readonly Mock<ICityRepository> CityRepository = new();
    protected readonly Mock<IMessagePublisher> Publisher = new();
    protected readonly TempStorage Storage = new();
    protected readonly HotelService Service;

    protected readonly City City = TestData.City();

    protected HotelServiceTestBase()
    {
        Service = new HotelService(
            HotelRepository.Object,
            ImageRepository.Object,
            Options.Create(new ImageUploadOptions { SharedStoragePath = Storage.Path }),
            Publisher.Object,
            CityRepository.Object,
            NullLogger<HotelService>.Instance);
    }

    protected Hotel ExistingHotel(bool withDetails = false)
    {
        var hotel = TestData.Hotel(City);
        HotelRepository.Setup(r => r.GetByIdAsync(hotel.HotelId, 
            It.IsAny<CancellationToken>())).ReturnsAsync(hotel);
        if (withDetails)
            HotelRepository.Setup(r => r.GetByIdWithImagesAsync(hotel.HotelId, 
                It.IsAny<CancellationToken>())).ReturnsAsync(hotel);
        return hotel;
    }

    public void Dispose() => Storage.Dispose();
}
