using Moq;
using Triply.Domain.Results.Enums;
using Triply.Tests.UnitTests.Common.Assertions;
using Triply.Tests.UnitTests.Common.Builders;

namespace Triply.Tests.UnitTests.Infrastructure.Services.Cities;

public class DeleteCityTests : CityServiceTestBase
{
    [Fact]
    public async Task DeleteAsync_Existing_SoftDeletesAndReturnsNoContent()
    {
        var city = TestData.City();
        CityRepository.Setup(r => r.GetByIdAsync(city.CityId, 
            It.IsAny<CancellationToken>())).ReturnsAsync(city);

        var result = await Service.DeleteAsync(city.CityId);

        result.AssertSuccess(ResultSuccessType.NoContent);
        Assert.True(city.IsDeleted);
        Assert.NotNull(city.ModifiedAt);
        CityRepository.Verify(r => r.DeleteAsync(It.IsAny<Triply.Domain.Entities.City>()), Times.Never);
        CityRepository.Verify(r => r.SaveChangesAsync(
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_Missing_ReturnsNotFound()
    {
        var result = await Service.DeleteAsync(Guid.NewGuid());

        result.AssertFailure("CITY_NOT_FOUND", ResultErrorType.NotFound);
    }

    [Fact]
    public async Task DeleteAsync_AlreadyDeleted_ReturnsNotFound()
    {
        var city = TestData.City(isDeleted: true);
        CityRepository.Setup(r => r.GetByIdAsync(city.CityId, 
            It.IsAny<CancellationToken>())).ReturnsAsync(city);

        var result = await Service.DeleteAsync(city.CityId);

        result.AssertFailure("CITY_NOT_FOUND", ResultErrorType.NotFound);
        CityRepository.Verify(r => r.SaveChangesAsync(
            It.IsAny<CancellationToken>()), Times.Never);
    }
}
