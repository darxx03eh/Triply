using Moq;
using Triply.Domain.Results.Enums;
using Triply.Tests.UnitTests.Common.Assertions;
using Triply.Tests.UnitTests.Common.Builders;

namespace Triply.Tests.UnitTests.Infrastructure.Services.Cities;

public class GetCityByIdTests : CityServiceTestBase
{
    [Fact]
    public async Task GetByIdAsync_Existing_ReturnsCityWithHotelsCount()
    {
        var city = TestData.City();
        CityRepository.Setup(r => 
            r.GetByIdAsync(city.CityId, It.IsAny<CancellationToken>())).ReturnsAsync(city);
        CityRepository.Setup(r => 
                r.GetHotelsCountAsync(It.IsAny<IEnumerable<Guid>>(), 
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Dictionary<Guid, int> { [city.CityId] = 4 });

        var result = await Service.GetByIdAsync(city.CityId);

        var response = result.AssertSuccess();
        Assert.Equal("Nablus", response.Name);
        Assert.Equal(4, response.HotelsCount);
    }

    [Fact]
    public async Task GetByIdAsync_Missing_ReturnsNotFound()
    {
        var result = await Service.GetByIdAsync(Guid.NewGuid());

        result.AssertFailure("CITY_NOT_FOUND", ResultErrorType.NotFound);
    }

    [Fact]
    public async Task GetByIdAsync_SoftDeleted_ReturnsNotFound()
    {
        var city = TestData.City(isDeleted: true);
        CityRepository.Setup(r => r.GetByIdAsync(city.CityId, 
            It.IsAny<CancellationToken>())).ReturnsAsync(city);

        var result = await Service.GetByIdAsync(city.CityId);

        result.AssertFailure("CITY_NOT_FOUND", ResultErrorType.NotFound);
    }
}
