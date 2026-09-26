using Moq;
using Triply.Application.Features.Rooms.Commands.UploadImage;
using Triply.Domain.Contracts;
using Triply.Domain.Contracts.Enums;
using Triply.Domain.Entities;
using Triply.Domain.Enums.Images;
using Triply.Domain.Results.Enums;
using Triply.Tests.UnitTests.Common.Assertions;
using Triply.Tests.UnitTests.Common.Fakes;

namespace Triply.Tests.UnitTests.Infrastructure.Services.Rooms;

public class RoomImageTests : RoomServiceTestBase
{
    private readonly Room _room;
    private RoomImage? _savedImage;

    public RoomImageTests()
    {
        _room = ExistingRoom();
        ImageRepository.Setup(repository => repository.AddAsync(It.IsAny<RoomImage>(), It.IsAny<CancellationToken>()))
            .Callback<RoomImage, CancellationToken>((image, _) => _savedImage = image);
        ImageRepository.Setup(repository => repository.GetNextDisplayOrderAsync(_room.RoomId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((short)2);
    }

    [Fact]
    public async Task InitiateUploadAsync_ValidImage_QueuesRoomImageUpload()
    {
        ImageUploadMessage? message = null;
        Publisher.Setup(publisher => publisher.PublishAsync("image.upload", It.IsAny<ImageUploadMessage>(), null,
                It.IsAny<CancellationToken>()))
            .Callback<string, ImageUploadMessage, string?, CancellationToken>((_, value, _, _) => message = value);

        var result = await Service.InitiateUploadAsync(_room.RoomId, Request());

        var response = result.AssertSuccess(ResultSuccessType.Accepted);
        Assert.Equal(ImageStatus.Pending, response.Status);
        Assert.Equal(2, _savedImage!.DisplayOrder);
        Assert.Equal(ImageTarget.RoomImage, message!.Target);
        Assert.Equal(_room.RoomId, message.Id);
        Assert.True(File.Exists(message.FilePath));
    }

    [Fact]
    public async Task InitiateUploadAsync_MissingRoom_ReturnsNotFound()
    {
        var result = await Service.InitiateUploadAsync(Guid.NewGuid(), Request());

        result.AssertFailure("ROOM_NOT_FOUND", ResultErrorType.NotFound);
        Assert.Empty(Storage.Files);
    }

    [Fact]
    public async Task InitiateUploadAsync_InvalidImageContent_ReturnsValidationError()
    {
        var result = await Service.InitiateUploadAsync(_room.RoomId, Request("invalid.png", TestImages.Text));

        result.AssertFailure("FILE_CONTENT_INVALID", ResultErrorType.Validation);
        ImageRepository.Verify(repository => repository.AddAsync(It.IsAny<RoomImage>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GetImagesAsync_ExistingRoom_ReturnsAllImageStatuses()
    {
        ImageRepository.Setup(repository => repository.GetByRoomIdAsync(_room.RoomId, It.IsAny<CancellationToken>()))
            .ReturnsAsync([
                new RoomImage { RoomId = _room.RoomId, DisplayOrder = 1, Status = ImageStatus.Uploaded, Url = "https://cdn.test/1.jpg" },
                new RoomImage { RoomId = _room.RoomId, DisplayOrder = 2, Status = ImageStatus.Pending }
            ]);

        var images = (await Service.GetImagesAsync(_room.RoomId)).AssertSuccess();

        Assert.Equal([ImageStatus.Uploaded, ImageStatus.Pending], images.Select(image => image.Status));
    }

    [Fact]
    public async Task GetImagesAsync_MissingRoom_ReturnsNotFound()
    {
        var result = await Service.GetImagesAsync(Guid.NewGuid());

        result.AssertFailure("ROOM_NOT_FOUND", ResultErrorType.NotFound);
    }

    [Fact]
    public async Task DeleteImageAsync_UploadedImage_DeletesAndQueuesCloudDeletion()
    {
        var image = new RoomImage { RoomId = _room.RoomId, PublicId = "triply/rooms/1", DisplayOrder = 1 };
        ImageRepository.Setup(repository => repository.GetByIdAsync(image.ImageId, It.IsAny<CancellationToken>())).ReturnsAsync(image);

        var result = await Service.DeleteImageAsync(_room.RoomId, image.ImageId);

        result.AssertSuccess(ResultSuccessType.NoContent);
        ImageRepository.Verify(repository => repository.DeleteAsync(image), Times.Once);
        Publisher.Verify(publisher => publisher.PublishAsync("image.delete",
            It.Is<ImageDeleteMessage>(message => message.ImageId == image.ImageId && message.Id == _room.RoomId),
            null, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteImageAsync_ImageOfAnotherRoom_ReturnsNotFound()
    {
        var image = new RoomImage { RoomId = Guid.NewGuid(), DisplayOrder = 1 };
        ImageRepository.Setup(repository => repository.GetByIdAsync(image.ImageId, It.IsAny<CancellationToken>())).ReturnsAsync(image);

        var result = await Service.DeleteImageAsync(_room.RoomId, image.ImageId);

        result.AssertFailure("ROOM_IMAGE_NOT_FOUND", ResultErrorType.NotFound);
        ImageRepository.Verify(repository => repository.DeleteAsync(It.IsAny<RoomImage>()), Times.Never);
    }

    private static UploadRoomImageRequest Request(string fileName = "room.png", byte[]? content = null)
        => new() { File = TestImages.File(fileName, content ?? TestImages.Png) };
}
