using Triply.Application.Features.Cities.Queries.GetCitiesRequest;
using Triply.Application.Features.Search.Queries.SearchHotels;
using Triply.Infrastructure.Caching;

namespace Triply.Tests.UnitTests.Infrastructure.Caching;

public class CacheKeyBuilderTests
{
    [Fact]
    public void BuildCitiesKey_EquivalentDefaultAndWhitespaceQueryValues_ReturnsSameKey()
    {
        var omittedDefaults = new GetCitiesRequest
        {
            Filters = " Country==Jordan , Name@=Amman ",
            Sorts = " Name "
        };
        var explicitDefaults = new GetCitiesRequest
        {
            Page = 1,
            PageSize = 10,
            Filters = "Country==Jordan,Name@=Amman",
            Sorts = "Name"
        };

        Assert.Equal(
            CacheKeyBuilder.BuildCitiesKey(omittedDefaults, isAdmin: false),
            CacheKeyBuilder.BuildCitiesKey(explicitDefaults, isAdmin: false));
    }

    [Fact]
    public void BuildCitiesKey_ResultAffectingValues_ReturnDifferentKeys()
    {
        var request = new GetCitiesRequest { Page = 1, PageSize = 10 };

        Assert.NotEqual(
            CacheKeyBuilder.BuildCitiesKey(request, isAdmin: false),
            CacheKeyBuilder.BuildCitiesKey(request, isAdmin: true));
        Assert.NotEqual(
            CacheKeyBuilder.BuildCitiesKey(request, isAdmin: false),
            CacheKeyBuilder.BuildCitiesKey(new GetCitiesRequest { Page = 2, PageSize = 10 }, isAdmin: false));
    }

    [Fact]
    public void BuildPagedSieveKey_EquivalentDefaults_ReturnsSameKey()
    {
        Assert.Equal(
            CacheKeyBuilder.BuildPagedSieveKey(null, null, null, null, isAdmin: false),
            CacheKeyBuilder.BuildPagedSieveKey("", "", 1, 10, isAdmin: false));
    }

    [Fact]
    public void BuildHomeCountKey_OmittedAndDefaultCount_ReturnSameKey()
    {
        Assert.Equal(CacheKeyBuilder.BuildHomeCountKey(null), CacheKeyBuilder.BuildHomeCountKey(5));
    }

    [Fact]
    public void BuildSearchKey_EquivalentDefaultsAndUnorderedFilters_ReturnSameKey()
    {
        var amenityA = Guid.NewGuid();
        var amenityB = Guid.NewGuid();
        var today = Triply.Application.Extensions.SearchExtensions.Today();
        var implicitValues = new SearchHotelsRequest
        {
            Q = "  Amman  ",
            Stars = [5, 3, 5],
            Types = ["Luxury", "Boutique", "Luxury"],
            Amenities = [amenityB, amenityA, amenityB]
        };
        var explicitValues = new SearchHotelsRequest
        {
            Q = "Amman",
            CheckIn = today,
            CheckOut = today.AddDays(1),
            Adults = 2,
            Children = 0,
            Rooms = 1,
            Sort = "recommended",
            Page = 1,
            PageSize = 10,
            Stars = [3, 5],
            Types = ["Boutique", "Luxury"],
            Amenities = [amenityA, amenityB]
        };

        Assert.Equal(CacheKeyBuilder.BuildSearchKey(implicitValues), CacheKeyBuilder.BuildSearchKey(explicitValues));
    }
}
