using Moq;
using Triply.Application.Features.Cities.Commands.UploadCityThumbnail;
using Triply.Domain.Contracts;
using Triply.Domain.Contracts.Enums;
using Triply.Domain.Results.Enums;
using Triply.Tests.UnitTests.Common.Assertions;
using Triply.Tests.UnitTests.Common.Builders;
using Triply.Tests.UnitTests.Common.Fakes;

namespace Triply.Tests.UnitTests.Infrastructure.Services.Cities;

public class UploadThumbnailTests : CityServiceTestBase
{
    private readonly Triply.Domain.Entities.City _city = TestData.City();

    public UploadThumbnailTests()
    {
        CityRepository.Setup(r => 
            r.GetByIdAsync(_city.CityId, It.IsAny<CancellationToken>())).ReturnsAsync(_city);
    }

    private static UploadCityThumbnailRequest Request(string fileName, byte[] content)
        => new() { File = TestImages.File(fileName, content) };

    [Fact]
    public async Task UploadThumbnailAsync_ValidImage_StagesFileAndQueuesCityThumbnailMessage()
    {
        ImageUploadMessage? message = null;
        Publisher.Setup(p => p.PublishAsync("image.upload", 
                It.IsAny<ImageUploadMessage>(), null, It.IsAny<CancellationToken>()))
            .Callback<string, ImageUploadMessage, string?, CancellationToken>((_, m, _, _) => message = m);

        var result = await Service.UploadThumbnailAsync(_city.CityId, Request("nablus.PNG", TestImages.Png));

        result.AssertSuccess(ResultSuccessType.Accepted);
        Assert.NotNull(message);
        Assert.Equal(ImageTarget.CityThumbnail, message!.Target);
        Assert.Equal(_city.CityId, message.CityId);
        Assert.Equal("nablus.PNG", message.OriginalFileName);
        Assert.True(File.Exists(message.FilePath));
        Assert.EndsWith(".png", message.FilePath);
        Assert.Equal(TestImages.Png, await File.ReadAllBytesAsync(message.FilePath));
    }

    [Fact]
    public async Task UploadThumbnailAsync_MissingCity_ReturnsNotFoundWithoutStaging()
    {
        var result = await Service.UploadThumbnailAsync(Guid.NewGuid(), Request("x.png", TestImages.Png));

        result.AssertFailure("CITY_NOT_FOUND", ResultErrorType.NotFound);
        Assert.Empty(Storage.Files);
        Publisher.VerifyNoOtherCalls();
    }

    [Theory]
    [InlineData("fake.png")]
    [InlineData("fake.jpg")]
    [InlineData("fake.webp")]
    public async Task UploadThumbnailAsync_ContentNotMatchingExtension_ReturnsValidationError(string fileName)
    {
        var result = await Service.UploadThumbnailAsync(_city.CityId, Request(fileName, TestImages.Text));

        result.AssertFailure("FILE_CONTENT_INVALID", ResultErrorType.Validation);
        Assert.Empty(Storage.Files);
        Publisher.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task UploadThumbnailAsync_PublishFails_DeletesStagedFileAndReturnsFailure()
    {
        Publisher.Setup(p => p.PublishAsync(It.IsAny<string>(), It.IsAny<ImageUploadMessage>(), null, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("broker down"));

        var result = await Service.UploadThumbnailAsync(_city.CityId, Request("nablus.png", TestImages.Png));

        result.AssertFailure("CITY_THUMBNAIL_QUEUE_FAILED", ResultErrorType.BusinessRule);
        Assert.Empty(Storage.Files);
    }
}
