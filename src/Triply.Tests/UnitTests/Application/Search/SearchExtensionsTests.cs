using Triply.Application.Extensions;
using Triply.Application.Features.Search.Queries.SearchHotels;
using Triply.Domain.Enums.Hotels;

namespace Triply.Tests.UnitTests.Application.Search;

public class SearchExtensionsTests
{
    [Fact]
    public void ToCriteria_EmptyRequest_AppliesDefaults()
    {
        var criteria = new SearchHotelsRequest().ToCriteria();

        Assert.Null(criteria.Q);
        Assert.Null(criteria.CityId);
        Assert.Equal(SearchExtensions.Today(), criteria.CheckIn);
        Assert.Equal(SearchExtensions.Today().AddDays(1), criteria.CheckOut);
        Assert.Equal(1, criteria.Nights);
        Assert.Equal(2, criteria.Adults);
        Assert.Equal(0, criteria.Children);
        Assert.Equal(1, criteria.Rooms);
        Assert.Equal(SearchSorts.Recommended, criteria.Sort);
        Assert.Equal(1, criteria.Page);
        Assert.Equal(10, criteria.PageSize);
        Assert.Empty(criteria.Stars);
        Assert.Empty(criteria.Types);
        Assert.Empty(criteria.Amenities);
    }

    [Fact]
    public void ToCriteria_OnlyCheckIn_DefaultsCheckOutToNextDay()
    {
        var checkIn = SearchExtensions.Today().AddDays(10);

        var criteria = new SearchHotelsRequest { CheckIn = checkIn }.ToCriteria();

        Assert.Equal(checkIn.AddDays(1), criteria.CheckOut);
    }

    [Theory]
    [InlineData("  Amman  ", "Amman")]
    [InlineData("   ", null)]
    [InlineData("", null)]
    public void ToCriteria_Query_IsTrimmedOrNull(string input, string? expected)
    {
        var criteria = new SearchHotelsRequest { Q = input }.ToCriteria();

        Assert.Equal(expected, criteria.Q);
    }

    [Fact]
    public void ToCriteria_TypesAndSort_AreCaseInsensitiveAndDeduplicated()
    {
        var criteria = new SearchHotelsRequest { Types = ["luxury", "LUXURY", "Budget"], Sort = "PRICE_DESC" }
            .ToCriteria();

        Assert.Equal([HotelType.Luxury, HotelType.Budget], criteria.Types);
        Assert.Equal(SearchSorts.PriceDesc, criteria.Sort);
    }

    [Fact]
    public void ToCriteria_StarsAndAmenities_AreDeduplicated()
    {
        var amenity = Guid.NewGuid();

        var criteria = new SearchHotelsRequest { Stars = [5, 5, 4], Amenities = [amenity, amenity] }.ToCriteria();

        Assert.Equal(new byte[] { 5, 4 }, criteria.Stars);
        Assert.Single(criteria.Amenities);
    }

    [Theory]
    [InlineData(2, 0, 1, 2, 0)]
    [InlineData(4, 2, 2, 2, 1)]
    [InlineData(3, 1, 2, 2, 1)]
    [InlineData(5, 0, 3, 2, 0)]
    public void ToCriteria_GuestsAreSplitAcrossRooms_RoundingUp(int adults, int children, int rooms,
        int expectedAdultsPerRoom, int expectedChildrenPerRoom)
    {
        var criteria = new SearchHotelsRequest { Adults = adults, Children = children, Rooms = rooms }.ToCriteria();

        Assert.Equal(expectedAdultsPerRoom, criteria.AdultsPerRoom);
        Assert.Equal(expectedChildrenPerRoom, criteria.ChildrenPerRoom);
    }

    [Fact]
    public void Nights_IsDifferenceBetweenDates()
    {
        var checkIn = SearchExtensions.Today().AddDays(3);

        var criteria = new SearchHotelsRequest { CheckIn = checkIn, CheckOut = checkIn.AddDays(7) }.ToCriteria();

        Assert.Equal(7, criteria.Nights);
    }
}
