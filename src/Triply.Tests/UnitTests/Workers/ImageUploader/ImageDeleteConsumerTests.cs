using ImageUploader;
using ImageUploader.IProviders;
using MessageQueue.IRabbitMQ;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Triply.Domain.Contracts;

namespace Triply.Tests.UnitTests.Workers.ImageUploader;

public class ImageDeleteConsumerTests
{
    private readonly Mock<IMessageConsumer> _consumer = new();
    private readonly Mock<ICloudinaryUploader> _cloudinary = new();
    private Func<ImageDeleteMessage, string, CancellationToken, Task> _handle = null!;
    private Func<ImageDeleteMessage, Exception, CancellationToken, Task> _onExhausted = null!;
    private readonly ImageDeleteConsumerHostedService _worker;

    public ImageDeleteConsumerTests()
    {
        _consumer.Setup(c => c.SubscribeAsync(
                "image.delete",
                It.IsAny<Func<ImageDeleteMessage, string, CancellationToken, Task>>(),
                It.IsAny<Func<ImageDeleteMessage, Exception, CancellationToken, Task>?>(),
                It.IsAny<CancellationToken>()))
            .Callback<string, Func<ImageDeleteMessage, 
                string, CancellationToken, Task>, Func<ImageDeleteMessage, Exception, CancellationToken, Task>?, CancellationToken>(
                (_, handle, exhausted, _) => 
                { _handle = handle; _onExhausted = exhausted!; })
            .Returns(Task.CompletedTask);
        _worker = new ImageDeleteConsumerHostedService(_consumer.Object, _cloudinary.Object,
            NullLogger<ImageDeleteConsumerHostedService>.Instance);
        _worker.StartAsync(CancellationToken.None).GetAwaiter().GetResult();
        _worker.ExecuteTask!.GetAwaiter().GetResult();
    }

    [Fact]
    public async Task Handle_DeletesAssetByPublicId()
    {
        await _handle(new ImageDeleteMessage { ImageId = Guid.NewGuid(), PublicId = "triply/hotels/x" }, 
            "image.delete", CancellationToken.None);

        _cloudinary.Verify(c => c.DeleteAsync("triply/hotels/x", 
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_CloudinaryFails_ThrowsSoTheMessageIsRetried()
    {
        _cloudinary.Setup(c => c.DeleteAsync(It.IsAny<string>(), 
            It.IsAny<CancellationToken>())).ThrowsAsync(new InvalidOperationException());

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _handle(new ImageDeleteMessage { PublicId = "x" }, "image.delete", CancellationToken.None));
    }

    [Fact]
    public async Task OnRetriesExhausted_DoesNotThrow()
    {
        await _onExhausted(new ImageDeleteMessage { PublicId = "x" }, new Exception(), CancellationToken.None);
    }

    [Fact]
    public async Task StopAsync_DisposesItsConsumer()
    {
        await _worker.StopAsync(CancellationToken.None);

        _consumer.Verify(c => c.DisposeAsync(), Times.Once);
    }
}
