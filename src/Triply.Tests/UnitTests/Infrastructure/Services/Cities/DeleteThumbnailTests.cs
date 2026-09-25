using Moq;
using Triply.Domain.Contracts;
using Triply.Domain.Results.Enums;
using Triply.Tests.UnitTests.Common.Assertions;
using Triply.Tests.UnitTests.Common.Builders;

namespace Triply.Tests.UnitTests.Infrastructure.Services.Cities;

public class DeleteThumbnailTests : CityServiceTestBase
{
    [Fact]
    public async Task DeleteThumbnailAsync_WithThumbnail_ClearsFieldsAndQueuesCloudDeletion()
    {
        var city = TestData.City();
        city.ThumbnailUrl = "https://cdn.test/city.png";
        city.ThumbnailPublicId = "triply/cities/abc";
        CityRepository.Setup(r => 
            r.GetByIdAsync(city.CityId, It.IsAny<CancellationToken>())).ReturnsAsync(city);

        var result = await Service.DeleteThumbnailAsync(city.CityId);

        result.AssertSuccess(ResultSuccessType.NoContent);
        Assert.Null(city.ThumbnailUrl);
        Assert.Null(city.ThumbnailPublicId);
        CityRepository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), 
            Times.Once);
        Publisher.Verify(p => p.PublishAsync("image.delete",
            It.Is<ImageDeleteMessage>(m => m.PublicId == "triply/cities/abc"), 
            null, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteThumbnailAsync_WithoutPublicId_ClearsUrlWithoutQueueing()
    {
        var city = TestData.City();
        city.ThumbnailUrl = "https://cdn.test/legacy.png";
        CityRepository.Setup(r => r.GetByIdAsync(city.CityId, 
            It.IsAny<CancellationToken>())).ReturnsAsync(city);

        var result = await Service.DeleteThumbnailAsync(city.CityId);

        result.AssertSuccess(ResultSuccessType.NoContent);
        Publisher.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task DeleteThumbnailAsync_PublishFails_StillSucceeds()
    {
        var city = TestData.City();
        city.ThumbnailUrl = "https://cdn.test/city.png";
        city.ThumbnailPublicId = "triply/cities/abc";
        CityRepository.Setup(r => r.GetByIdAsync(city.CityId, 
            It.IsAny<CancellationToken>())).ReturnsAsync(city);
        Publisher.Setup(p => p.PublishAsync(It.IsAny<string>(), 
                It.IsAny<ImageDeleteMessage>(), null, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException());

        var result = await Service.DeleteThumbnailAsync(city.CityId);

        result.AssertSuccess(ResultSuccessType.NoContent);
        Assert.Null(city.ThumbnailUrl);
    }

    [Fact]
    public async Task DeleteThumbnailAsync_NoThumbnail_ReturnsNotFound()
    {
        var city = TestData.City();
        CityRepository.Setup(r => r.GetByIdAsync(city.CityId, 
            It.IsAny<CancellationToken>())).ReturnsAsync(city);

        var result = await Service.DeleteThumbnailAsync(city.CityId);

        result.AssertFailure("CITY_THUMBNAIL_NOT_FOUND", ResultErrorType.NotFound);
        CityRepository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), 
            Times.Never);
    }

    [Fact]
    public async Task DeleteThumbnailAsync_MissingCity_ReturnsNotFound()
    {
        var result = await Service.DeleteThumbnailAsync(Guid.NewGuid());

        result.AssertFailure("CITY_NOT_FOUND", ResultErrorType.NotFound);
    }
}
