using Moq;
using Triply.Application.Features.Hotels.Commands.SetHotelAmenities;
using Triply.Domain.Results.Enums;
using Triply.Tests.UnitTests.Common.Assertions;
using Triply.Tests.UnitTests.Common.Builders;

namespace Triply.Tests.UnitTests.Infrastructure.Services.Amenities;

public class HotelAmenitiesTests : AmenityServiceTestBase
{
    private readonly Guid _hotelId = Guid.NewGuid();

    [Fact]
    public async Task GetHotelAmenitiesAsync_ExistingHotel_ReturnsAmenities()
    {
        HotelRepository.Setup(r => r.IsHotelIdExistsAsync(_hotelId, 
            It.IsAny<CancellationToken>())).ReturnsAsync(true);
        AmenityRepository.Setup(r => r.GetByHotelIdAsync(_hotelId, 
            It.IsAny<CancellationToken>())).ReturnsAsync([TestData.Amenity("Spa")]);

        var amenities = (await Service.GetHotelAmenitiesAsync(_hotelId)).AssertSuccess();

        Assert.Equal("Spa", amenities.Single().Name);
    }

    [Fact]
    public async Task GetHotelAmenitiesAsync_MissingHotel_ReturnsNotFound()
    {
        (await Service.GetHotelAmenitiesAsync(_hotelId)).AssertFailure("HOTEL_NOT_FOUND", ResultErrorType.NotFound);
    }

    [Fact]
    public async Task SetHotelAmenitiesAsync_ExistingHotel_ReplacesSavesAndReturnsCurrentList()
    {
        var ids = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
        HotelRepository.Setup(r => r.IsHotelIdExistsAsync(_hotelId, 
            It.IsAny<CancellationToken>())).ReturnsAsync(true);
        AmenityRepository.Setup(r => r.GetByHotelIdAsync(_hotelId, 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync([TestData.Amenity("Free WiFi"), TestData.Amenity("Spa")]);

        var result = await Service.SetHotelAmenitiesAsync(_hotelId, new SetHotelAmenitiesRequest { AmenityIds = ids });

        Assert.Equal(2, result.AssertSuccess().Count);
        AmenityRepository.Verify(r => r.ReplaceHotelAmenitiesAsync(_hotelId, ids, 
            It.IsAny<CancellationToken>()), Times.Once);
        AmenityRepository.Verify(r => r.SaveChangesAsync(
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SetHotelAmenitiesAsync_MissingHotel_ReturnsNotFoundAndChangesNothing()
    {
        var result = await Service.SetHotelAmenitiesAsync(_hotelId, new SetHotelAmenitiesRequest 
            { AmenityIds = [Guid.NewGuid()] });

        result.AssertFailure("HOTEL_NOT_FOUND", ResultErrorType.NotFound);
        AmenityRepository.Verify(r => r.ReplaceHotelAmenitiesAsync(It.IsAny<Guid>(), 
            It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
