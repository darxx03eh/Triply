using Triply.Domain.Results.Enums;
using Triply.Tests.UnitTests.Common.Assertions;
using Triply.Tests.UnitTests.Common.Builders;

namespace Triply.Tests.UnitTests.Infrastructure.Services.Hotels;

public class GetHotelByIdTests : HotelServiceTestBase
{
    [Fact]
    public async Task GetByIdAsync_Existing_ReturnsImagesOrderedByDisplayOrder()
    {
        var hotel = ExistingHotel(withDetails: true);
        hotel.Images.Add(TestData.Image(hotel, order: 2, url: "second.png"));
        hotel.Images.Add(TestData.Image(hotel, order: 1, url: "first.png"));

        var result = await Service.GetByIdAsync(hotel.HotelId);

        var response = result.AssertSuccess();
        Assert.Equal(["first.png", "second.png"], response.ImageUrls);
        Assert.Equal("Nablus", response.CityName);
        Assert.Equal(TestData.RowVersion, response.RowVersion);
    }

    [Fact]
    public async Task GetByIdAsync_Missing_ReturnsNotFound()
    {
        var result = await Service.GetByIdAsync(Guid.NewGuid());

        result.AssertFailure("HOTEL_NOT_FOUND", ResultErrorType.NotFound);
    }
}
