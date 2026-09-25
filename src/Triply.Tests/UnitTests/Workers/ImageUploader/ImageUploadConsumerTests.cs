using ImageUploader;
using ImageUploader.IProviders;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Triply.Domain.Contracts;
using Triply.Domain.Contracts.Enums;
using Triply.Domain.Entities;
using Triply.Domain.Enums.HotleImages;
using Triply.Tests.UnitTests.Common.Builders;
using Triply.XUnitTests.Workers.ImageUploader;

namespace Triply.Tests.UnitTests.Workers.ImageUploader;

public class ImageUploadConsumerTests : ImageWorkerTestBase
{
    private Func<ImageUploadMessage, string, CancellationToken, Task> _handle = null!;
    private Func<ImageUploadMessage, Exception, CancellationToken, Task> _onExhausted = null!;
    private readonly Hotel _hotel;
    private readonly City _city;

    public ImageUploadConsumerTests()
    {
        Consumer.Setup(c => c.SubscribeAsync(
                "image.upload",
                It.IsAny<Func<ImageUploadMessage, string, CancellationToken, Task>>(),
                It.IsAny<Func<ImageUploadMessage, Exception, CancellationToken, Task>?>(),
                It.IsAny<CancellationToken>()))
            .Callback<string, Func<ImageUploadMessage, string, CancellationToken, Task>, Func<ImageUploadMessage, 
                Exception, CancellationToken, Task>?, CancellationToken>(
                (_, handle, exhausted, _) => { _handle = handle; _onExhausted = exhausted!; })
            .Returns(Task.CompletedTask);
        Cloudinary.Setup(c => c.UploadAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CloudinaryUploadResult("https://cdn.test/new.png", "folder/new"));

        _city = TestData.City();
        _hotel = TestData.Hotel(_city);
        using (var db = Db())
        {
            db.AddRange(_city, _hotel);
            db.SaveChanges();
        }

        var worker = new ImageUploadConsumerHostedService(
            Consumer.Object, Cloudinary.Object, Services.GetRequiredService<IServiceScopeFactory>(),
            NullLogger<ImageUploadConsumerHostedService>.Instance);
        worker.StartAsync(CancellationToken.None).GetAwaiter().GetResult();
        worker.ExecuteTask!.GetAwaiter().GetResult();
    }

    private HotelImage SeedImage(HotelImageStatus status = HotelImageStatus.Pending)
    {
        var image = TestData.Image(_hotel, status: status);
        image.Hotel = null!;
        using var db = Db();
        db.HotelImages.Add(image);
        db.SaveChanges();
        return image;
    }

    private HotelImage Reload(Guid imageId)
    {
        using var db = Db();
        return db.HotelImages.Single(i => i.ImageId == imageId);
    }

    private static ImageUploadMessage HotelMessage(HotelImage image, string path) => new()
    {
        ImageId = image.ImageId, HotelId = image.HotelId, FilePath = path, OriginalFileName = "photo.png"
    };

    [Fact]
    public async Task HotelImage_Pending_UploadsToHotelsFolderAndMarksUploaded()
    {
        var image = SeedImage();
        var path = StageFile();

        await _handle(HotelMessage(image, path), "image.upload", CancellationToken.None);

        var saved = Reload(image.ImageId);
        Assert.Equal(HotelImageStatus.Uploaded, saved.Status);
        Assert.Equal("https://cdn.test/new.png", saved.Url);
        Assert.Equal("folder/new", saved.PublicId);
        Assert.False(File.Exists(path));
        Cloudinary.Verify(c => c.UploadAsync(path, "photo.png", "triply/hotels", It.IsAny<CancellationToken>()), 
            Times.Once);
    }

    [Fact]
    public async Task HotelImage_RowDeleted_DiscardsFileWithoutUploading()
    {
        var path = StageFile();

        await _handle(new ImageUploadMessage { ImageId = Guid.NewGuid(), FilePath = path, OriginalFileName = "x.png" },
            "image.upload", CancellationToken.None);

        Assert.False(File.Exists(path));
        Cloudinary.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HotelImage_AlreadyUploaded_IsIdempotent()
    {
        var image = SeedImage(HotelImageStatus.Uploaded);
        var path = StageFile();

        await _handle(HotelMessage(image, path), "image.upload", CancellationToken.None);

        Assert.False(File.Exists(path));
        Cloudinary.VerifyNoOtherCalls();
        Assert.Equal("https://cdn.test/image.png", Reload(image.ImageId).Url);
    }

    [Fact]
    public async Task HotelImage_StagedFileMissing_MarksFailedWithoutRetrying()
    {
        var image = SeedImage();

        await _handle(HotelMessage(image, Path.Combine(Storage.Path, "missing.png")), "image.upload", 
            CancellationToken.None);

        Assert.Equal(HotelImageStatus.Failed, Reload(image.ImageId).Status);
        Cloudinary.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HotelImage_CloudinaryFails_ThrowsSoTheMessageIsRetried()
    {
        var image = SeedImage();
        var path = StageFile();
        Cloudinary.Setup(c => c.UploadAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), 
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new HttpRequestException("timeout"));

        await Assert.ThrowsAsync<HttpRequestException>(() => _handle(HotelMessage(image, path), 
            "image.upload", CancellationToken.None));

        Assert.Equal(HotelImageStatus.Pending, Reload(image.ImageId).Status);
        Assert.True(File.Exists(path));
    }

    [Fact]
    public async Task HotelImage_RetriesExhausted_MarksFailedAndDeletesFile()
    {
        var image = SeedImage();
        var path = StageFile();

        await _onExhausted(HotelMessage(image, path), new HttpRequestException(), CancellationToken.None);

        Assert.Equal(HotelImageStatus.Failed, Reload(image.ImageId).Status);
        Assert.False(File.Exists(path));
    }

    [Fact]
    public async Task HotelImage_RetriesExhaustedAfterUpload_KeepsUploadedStatus()
    {
        var image = SeedImage(HotelImageStatus.Uploaded);

        await _onExhausted(HotelMessage(image, StageFile()), new Exception(), CancellationToken.None);

        Assert.Equal(HotelImageStatus.Uploaded, Reload(image.ImageId).Status);
    }

    private ImageUploadMessage CityMessage(string path) => new()
    {
        ImageId = Guid.NewGuid(), CityId = _city.CityId, Target = ImageTarget.CityThumbnail, FilePath = path, 
        OriginalFileName = "city.png"
    };

    private City ReloadCity()
    {
        using var db = Db();
        return db.Cities.Single(c => c.CityId == _city.CityId);
    }

    [Fact]
    public async Task CityThumbnail_FirstUpload_SetsThumbnailInCitiesFolder()
    {
        var path = StageFile("city.png");

        await _handle(CityMessage(path), "image.upload", CancellationToken.None);

        var city = ReloadCity();
        Assert.Equal("https://cdn.test/new.png", city.ThumbnailUrl);
        Assert.Equal("folder/new", city.ThumbnailPublicId);
        Assert.NotNull(city.ModifiedAt);
        Assert.False(File.Exists(path));
        Cloudinary.Verify(c => c.UploadAsync(path, "city.png", "triply/cities", It.IsAny<CancellationToken>()), 
            Times.Once);
        Cloudinary.Verify(c => c.DeleteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CityThumbnail_Replacement_DeletesPreviousAsset()
    {
        using (var db = Db())
        {
            var city = db.Cities.Single();
            city.ThumbnailUrl = "https://cdn.test/old.png";
            city.ThumbnailPublicId = "folder/old";
            db.SaveChanges();
        }

        await _handle(CityMessage(StageFile()), "image.upload", CancellationToken.None);

        Assert.Equal("folder/new", ReloadCity().ThumbnailPublicId);
        Cloudinary.Verify(c => c.DeleteAsync("folder/old", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CityThumbnail_OldAssetDeleteFails_StillSucceeds()
    {
        using (var db = Db())
        {
            db.Cities.Single().ThumbnailPublicId = "folder/old";
            db.SaveChanges();
        }
        Cloudinary.Setup(c => c.DeleteAsync("folder/old", It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("cdn"));

        await _handle(CityMessage(StageFile()), "image.upload", CancellationToken.None);

        Assert.Equal("folder/new", ReloadCity().ThumbnailPublicId);
    }

    [Fact]
    public async Task CityThumbnail_CityMissing_DiscardsFile()
    {
        var path = StageFile();
        var message = CityMessage(path);
        message.CityId = Guid.NewGuid();

        await _handle(message, "image.upload", CancellationToken.None);

        Assert.False(File.Exists(path));
        Cloudinary.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task CityThumbnail_StagedFileMissing_DoesNothing()
    {
        await _handle(CityMessage(Path.Combine(Storage.Path, "gone.png")), "image.upload", CancellationToken.None);

        Assert.Null(ReloadCity().ThumbnailUrl);
        Cloudinary.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task CityThumbnail_RetriesExhausted_OnlyDeletesFile()
    {
        var path = StageFile();

        await _onExhausted(CityMessage(path), new Exception(), CancellationToken.None);

        Assert.False(File.Exists(path));
        Assert.Null(ReloadCity().ThumbnailUrl);
    }
}
