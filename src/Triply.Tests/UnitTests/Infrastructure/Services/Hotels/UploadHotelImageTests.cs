using Moq;
using Triply.Application.Features.Hotels.Commands.UploadImage;
using Triply.Domain.Contracts;
using Triply.Domain.Contracts.Enums;
using Triply.Domain.Entities;
using Triply.Domain.Enums.HotleImages;
using Triply.Domain.Results.Enums;
using Triply.Tests.UnitTests.Common.Assertions;
using Triply.Tests.UnitTests.Common.Fakes;

namespace Triply.Tests.UnitTests.Infrastructure.Services.Hotels;

public class UploadHotelImageTests : HotelServiceTestBase
{
    private readonly Hotel _hotel;
    private HotelImage? _saved;

    public UploadHotelImageTests()
    {
        _hotel = ExistingHotel();
        ImageRepository.Setup(r => r.AddAsync(It.IsAny<HotelImage>(), 
                It.IsAny<CancellationToken>()))
            .Callback<HotelImage, CancellationToken>((image, _) => _saved = image);
        ImageRepository.Setup(r => r.GetNextDisplayOrderAsync(_hotel.HotelId, 
            It.IsAny<CancellationToken>())).ReturnsAsync((short)4);
    }

    private static UploadHotelImageRequest Request(string fileName = "room.png", byte[]? 
        content = null, short? order = null)
        => new() { File = TestImages.File(fileName, content ?? TestImages.Png), DisplayOrder = order };

    [Fact]
    public async Task InitiateUploadAsync_Valid_SavesPendingImageStagesFileAndQueuesMessage()
    {
        ImageUploadMessage? message = null;
        Publisher.Setup(p => p.PublishAsync("image.upload", 
                It.IsAny<ImageUploadMessage>(), null, It.IsAny<CancellationToken>()))
            .Callback<string, ImageUploadMessage, string?, CancellationToken>((_, m, _, _) => message = m);

        var result = await Service.InitiateUploadAsync(_hotel.HotelId, Request(order: 2));

        var response = result.AssertSuccess(ResultSuccessType.Accepted);
        Assert.Equal(HotelImageStatus.Pending, response.Status);
        Assert.Equal(2, _saved!.DisplayOrder);
        Assert.Equal(ImageTarget.HotelImage, message!.Target);
        Assert.Equal(_saved.ImageId, message.ImageId);
        Assert.Equal(_hotel.HotelId, message.HotelId);
        Assert.Equal(Path.Combine(Storage.Path, $"{_saved.ImageId}.png"), message.FilePath);
        Assert.True(File.Exists(message.FilePath));
    }

    [Fact]
    public async Task InitiateUploadAsync_WithoutDisplayOrder_AppendsToGallery()
    {
        await Service.InitiateUploadAsync(_hotel.HotelId, Request(order: null));

        Assert.Equal(4, _saved!.DisplayOrder);
    }

    [Fact]
    public async Task InitiateUploadAsync_WithDisplayOrder_DoesNotQueryNextOrder()
    {
        await Service.InitiateUploadAsync(_hotel.HotelId, Request(order: 1));

        ImageRepository.Verify(r => 
            r.GetNextDisplayOrderAsync(It.IsAny<Guid>(), 
                It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task InitiateUploadAsync_MissingHotel_ReturnsNotFound()
    {
        var result = await Service.InitiateUploadAsync(Guid.NewGuid(), Request());

        result.AssertFailure("HOTEL_NOT_FOUND", ResultErrorType.NotFound);
        Assert.Empty(Storage.Files);
    }

    [Theory]
    [InlineData("image.png")]
    [InlineData("image.jpeg")]
    public async Task InitiateUploadAsync_FakeImage_ReturnsValidationErrorAndSavesNothing(string fileName)
    {
        var result = await Service.InitiateUploadAsync(_hotel.HotelId, Request(fileName, TestImages.Text));

        result.AssertFailure("FILE_CONTENT_INVALID", ResultErrorType.Validation);
        Assert.Null(_saved);
        Assert.Empty(Storage.Files);
    }

    [Fact]
    public async Task InitiateUploadAsync_DatabaseFails_DeletesStagedFileAndRethrows()
    {
        ImageRepository.Setup(r => r.SaveChangesAsync(
            It.IsAny<CancellationToken>())).ThrowsAsync(new InvalidOperationException("db"));

        await Assert.ThrowsAsync<InvalidOperationException>(() => Service.InitiateUploadAsync(_hotel.HotelId, Request()));

        Assert.Empty(Storage.Files);
        Publisher.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task InitiateUploadAsync_PublishFails_MarksImageFailedAndDeletesFile()
    {
        Publisher.Setup(p => p.PublishAsync(It.IsAny<string>(), 
                It.IsAny<ImageUploadMessage>(), null, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("broker down"));

        var result = await Service.InitiateUploadAsync(_hotel.HotelId, Request());

        result.AssertFailure("HOTEL_IMAGE_QUEUE_FAILED", ResultErrorType.BusinessRule);
        Assert.Equal(HotelImageStatus.Failed, _saved!.Status);
        Assert.Empty(Storage.Files);
        ImageRepository.Verify(r => r.SaveChangesAsync(
            It.IsAny<CancellationToken>()), Times.Exactly(2));
    }
}
