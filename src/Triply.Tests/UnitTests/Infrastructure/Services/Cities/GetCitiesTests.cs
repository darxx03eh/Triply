using Moq;
using Sieve.Models;
using Triply.Application.Features.Cities.Queries.GetCitiesRequest;
using Triply.Domain.Entities;
using Triply.Tests.UnitTests.Common.Assertions;
using Triply.Tests.UnitTests.Common.Builders;

namespace Triply.Tests.UnitTests.Infrastructure.Services.Cities;

public class GetCitiesTests : CityServiceTestBase
{
    [Fact]
    public async Task GetPagedAsync_ReturnsPageWithCounts()
    {
        var nablus = TestData.City("Nablus");
        var amman = TestData.City("Amman", "Jordan");
        CityRepository.Setup(r => r.GetPagedAsync(It.IsAny<SieveModel>(), false, 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((new List<City> { nablus, amman }, 12));
        CityRepository.Setup(r => r.GetHotelsCountAsync(It.IsAny<IEnumerable<Guid>>(), 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Dictionary<Guid, int> { [amman.CityId] = 3 });

        var result = await Service.GetPagedAsync(new GetCitiesRequest { Page = 2, PageSize = 2 }, isAdmin: false);

        var page = result.AssertSuccess();
        Assert.Equal("CITIES_FOUND", result.SuccessObject!.Code);
        Assert.Equal(2, page.Page);
        Assert.Equal(2, page.PageSize);
        Assert.Equal(12, page.TotalCount);
        Assert.Equal(6, page.TotalPages);
        Assert.Equal(0, page.Items.Single(c => c.Name == "Nablus").HotelsCount);
        Assert.Equal(3, page.Items.Single(c => c.Name == "Amman").HotelsCount);
    }

    [Fact]
    public async Task GetPagedAsync_NoPagingValues_UsesDefaults()
    {
        CityRepository.Setup(r => r.GetPagedAsync(It.IsAny<SieveModel>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((new List<City>(), 0));

        var page = (await Service.GetPagedAsync(new GetCitiesRequest(), true))
            .AssertSuccess();

        Assert.Equal(1, page.Page);
        Assert.Equal(10, page.PageSize);
    }

    [Fact]
    public async Task GetPagedAsync_Empty_ReturnsOkWithEmptyList()
    {
        CityRepository.Setup(r => r.GetPagedAsync(It.IsAny<SieveModel>(), 
                It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((new List<City>(), 0));

        var result = await Service.GetPagedAsync(new GetCitiesRequest(), false);

        var page = result.AssertSuccess();
        Assert.Empty(page.Items);
        Assert.Equal("CITIES_EMPTY", result.SuccessObject!.Code);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task GetPagedAsync_PassesAdminFlagToRepository(bool isAdmin)
    {
        CityRepository.Setup(r => r.GetPagedAsync(It.IsAny<SieveModel>(), 
                It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((new List<City>(), 0));

        await Service.GetPagedAsync(new GetCitiesRequest(), isAdmin);

        CityRepository.Verify(r => r.GetPagedAsync(It.IsAny<SieveModel>(), 
            isAdmin, It.IsAny<CancellationToken>()), Times.Once);
    }
}
