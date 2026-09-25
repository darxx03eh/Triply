using Moq;
using Sieve.Models;
using Triply.Application.Features.Hotels.Queries.GetHotels;
using Triply.Domain.Entities;
using Triply.Tests.UnitTests.Common.Assertions;
using Triply.Tests.UnitTests.Common.Builders;

namespace Triply.Tests.UnitTests.Infrastructure.Services.Hotels;

public class GetHotelsTests : HotelServiceTestBase
{
    [Fact]
    public async Task GetPagedAsync_CombinesRoomsCountAndThumbnailPerHotel()
    {
        var first = TestData.Hotel(City, "First");
        var second = TestData.Hotel(City, "Second");
        HotelRepository.Setup(r => r.GetPagedAsync(It.IsAny<SieveModel>(), true, It.IsAny<CancellationToken>()))
            .ReturnsAsync((new List<Hotel> { first, second }, 2));
        HotelRepository.Setup(r => r.GetRoomsCountAsync(It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Dictionary<Guid, int> { [first.HotelId] = 5 });
        HotelRepository.Setup(r => r.GetThumbnailsAsync(It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Dictionary<Guid, string> { [second.HotelId] = "second.png" });

        var result = await Service.GetPagedAsync(new GetHotelsRequest(), isAdmin: true);

        var page = result.AssertSuccess();
        Assert.Equal("HOTELS_FOUND", result.SuccessObject!.Code);
        var firstItem = page.Items.Single(h => h.Name == "First");
        var secondItem = page.Items.Single(h => h.Name == "Second");
        Assert.Equal(5, firstItem.RoomsCount);
        Assert.Null(firstItem.ThumbnailUrl);
        Assert.Equal(0, secondItem.RoomsCount);
        Assert.Equal("second.png", secondItem.ThumbnailUrl);
    }

    [Fact]
    public async Task GetPagedAsync_Empty_ReturnsOkWithEmptyMessage()
    {
        HotelRepository.Setup(r => r.GetPagedAsync(It.IsAny<SieveModel>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((new List<Hotel>(), 0));
        HotelRepository.Setup(r => r.GetRoomsCountAsync(It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Dictionary<Guid, int>());
        HotelRepository.Setup(r => r.GetThumbnailsAsync(It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Dictionary<Guid, string>());

        var result = await Service.GetPagedAsync(new GetHotelsRequest(), false);

        Assert.Empty(result.AssertSuccess().Items);
        Assert.Equal("HOTELS_EMPTY", result.SuccessObject!.Code);
    }
}
