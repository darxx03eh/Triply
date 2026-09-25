using Microsoft.EntityFrameworkCore;
using Moq;
using Triply.Application.Features.Hotels.Commands.UpdateHotel;
using Triply.Domain.Enums.Hotels;
using Triply.Domain.Results.Enums;
using Triply.Tests.UnitTests.Common.Assertions;
using Triply.Tests.UnitTests.Common.Builders;

namespace Triply.Tests.UnitTests.Infrastructure.Services.Hotels;

public class UpdateHotelTests : HotelServiceTestBase
{
    private static UpdateHotelRequest Request(Guid cityId) => new()
    {
        Name = "Renamed Hotel",
        CityId = cityId,
        StarRating = 3,
        HotelType = HotelType.Budget,
        Address = " New Address ",
        RowVersion = [7, 7, 7, 7, 7, 7, 7, 7]
    };

    [Fact]
    public async Task UpdateAsync_SameCity_UpdatesWithoutLoadingCityAgain()
    {
        var hotel = ExistingHotel(withDetails: true);

        var result = await Service.UpdateAsync(hotel.HotelId, Request(City.CityId));

        var response = result.AssertSuccess();
        Assert.Equal("Renamed Hotel", response.Name);
        Assert.Equal("Nablus", response.CityName);
        Assert.Equal("New Address", hotel.Address);
        Assert.Equal(HotelType.Budget, hotel.HotelType);
        Assert.NotNull(hotel.ModifiedAt);
        CityRepository.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), 
            It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_NewCity_MovesHotelToCity()
    {
        var hotel = ExistingHotel(withDetails: true);
        var amman = TestData.City("Amman", "Jordan");
        CityRepository.Setup(r => r.GetByIdAsync(amman.CityId, 
            It.IsAny<CancellationToken>())).ReturnsAsync(amman);

        var result = await Service.UpdateAsync(hotel.HotelId, Request(amman.CityId));

        Assert.Equal("Amman", result.AssertSuccess().CityName);
        Assert.Equal(amman.CityId, hotel.CityId);
        Assert.Same(amman, hotel.City);
    }

    [Fact]
    public async Task UpdateAsync_NewCityMissing_ReturnsNotFound()
    {
        var hotel = ExistingHotel(withDetails: true);

        var result = await Service.UpdateAsync(hotel.HotelId, Request(Guid.NewGuid()));

        result.AssertFailure("CITY_NOT_FOUND", ResultErrorType.NotFound);
        HotelRepository.Verify(r => r.SaveChangesAsync(
            It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_Missing_ReturnsNotFound()
    {
        var result = await Service.UpdateAsync(Guid.NewGuid(), Request(City.CityId));

        result.AssertFailure("HOTEL_NOT_FOUND", ResultErrorType.NotFound);
    }

    [Fact]
    public async Task UpdateAsync_UsesClientRowVersion_AndReturnsImages()
    {
        var hotel = ExistingHotel(withDetails: true);
        hotel.Images.Add(TestData.Image(hotel, url: "a.png"));
        var request = Request(City.CityId);

        var result = await Service.UpdateAsync(hotel.HotelId, request);

        Assert.Equal(["a.png"], result.AssertSuccess().ImageUrls);
        HotelRepository.Verify(r => r.SetOriginalRowVersion(hotel, request.RowVersion), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ConcurrentEdit_ReturnsConflict()
    {
        var hotel = ExistingHotel(withDetails: true);
        HotelRepository.Setup(r => r.SaveChangesAsync(
            It.IsAny<CancellationToken>())).ThrowsAsync(new DbUpdateConcurrencyException());

        var result = await Service.UpdateAsync(hotel.HotelId, Request(City.CityId));

        result.AssertFailure("HOTEL_CONCURRENCY_CONFLICT", ResultErrorType.Conflict);
    }
}
