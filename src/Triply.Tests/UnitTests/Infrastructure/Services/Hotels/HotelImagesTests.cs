using Moq;
using Triply.Domain.Contracts;
using Triply.Domain.Entities;
using Triply.Domain.Enums.Images;
using Triply.Domain.Results.Enums;
using Triply.Tests.UnitTests.Common.Assertions;
using Triply.Tests.UnitTests.Common.Builders;

namespace Triply.Tests.UnitTests.Infrastructure.Services.Hotels;

public class HotelImagesTests : HotelServiceTestBase
{
    [Fact]
    public async Task GetImagesAsync_Existing_ReturnsAllStatuses()
    {
        var hotel = ExistingHotel();
        ImageRepository.Setup(r => r.GetByHotelIdAsync(hotel.HotelId, It.IsAny<CancellationToken>())).ReturnsAsync(
        [
            TestData.Image(hotel, 1),
            TestData.Image(hotel, 2, ImageStatus.Pending),
            TestData.Image(hotel, 3, ImageStatus.Failed)
        ]);

        var result = await Service.GetImagesAsync(hotel.HotelId);

        var images = result.AssertSuccess();
        Assert.Equal([ImageStatus.Uploaded, ImageStatus.Pending, ImageStatus.Failed], 
            images.Select(i => i.Status));
    }

    [Fact]
    public async Task GetImagesAsync_MissingHotel_ReturnsNotFound()
    {
        var result = await Service.GetImagesAsync(Guid.NewGuid());

        result.AssertFailure("HOTEL_NOT_FOUND", ResultErrorType.NotFound);
    }

    [Fact]
    public async Task DeleteImageAsync_Uploaded_DeletesRowAndQueuesCloudDeletion()
    {
        var hotel = TestData.Hotel(City);
        var image = TestData.Image(hotel, publicId: "triply/hotels/xyz");
        ImageRepository.Setup(r => r.GetByIdAsync(image.ImageId, It.IsAny<CancellationToken>())).ReturnsAsync(image);

        var result = await Service.DeleteImageAsync(hotel.HotelId, image.ImageId);

        result.AssertSuccess(ResultSuccessType.NoContent);
        ImageRepository.Verify(r => r.DeleteAsync(image), Times.Once);
        ImageRepository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        Publisher.Verify(p => p.PublishAsync("image.delete",
            It.Is<ImageDeleteMessage>(m => m.PublicId == "triply/hotels/xyz" && 
                                           m.ImageId == image.ImageId),
            null, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteImageAsync_PendingImage_DeletesRowWithoutQueueing()
    {
        var hotel = TestData.Hotel(City);
        var image = TestData.Image(hotel, status: ImageStatus.Pending);
        ImageRepository.Setup(r => r.GetByIdAsync(image.ImageId, It.IsAny<CancellationToken>())).ReturnsAsync(image);

        var result = await Service.DeleteImageAsync(hotel.HotelId, image.ImageId);

        result.AssertSuccess(ResultSuccessType.NoContent);
        Publisher.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task DeleteImageAsync_PublishFails_StillSucceeds()
    {
        var hotel = TestData.Hotel(City);
        var image = TestData.Image(hotel);
        ImageRepository.Setup(r => r.GetByIdAsync(image.ImageId, It.IsAny<CancellationToken>())).ReturnsAsync(image);
        Publisher.Setup(p => p.PublishAsync(It.IsAny<string>(), It.IsAny<ImageDeleteMessage>(), null, 
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException());

        var result = await Service.DeleteImageAsync(hotel.HotelId, image.ImageId);

        result.AssertSuccess(ResultSuccessType.NoContent);
    }

    [Fact]
    public async Task DeleteImageAsync_ImageOfAnotherHotel_ReturnsNotFound()
    {
        var image = TestData.Image(TestData.Hotel(City));
        ImageRepository.Setup(r => r.GetByIdAsync(image.ImageId, It.IsAny<CancellationToken>())).ReturnsAsync(image);

        var result = await Service.DeleteImageAsync(Guid.NewGuid(), image.ImageId);

        result.AssertFailure("HOTEL_IMAGE_NOT_FOUND", ResultErrorType.NotFound);
        ImageRepository.Verify(r => r.DeleteAsync(It.IsAny<HotelImage>()), Times.Never);
    }

    [Fact]
    public async Task DeleteImageAsync_MissingImage_ReturnsNotFound()
    {
        var result = await Service.DeleteImageAsync(Guid.NewGuid(), Guid.NewGuid());

        result.AssertFailure("HOTEL_IMAGE_NOT_FOUND", ResultErrorType.NotFound);
    }
}
