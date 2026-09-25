using Triply.Infrastructure.Helpers;
using Triply.Tests.UnitTests.Common.Fakes;

namespace Triply.Tests.UnitTests.Infrastructure.Helpers;

public class ImageFileSignatureTests
{
    public static TheoryData<string, byte[]> ValidImages => new()
    {
        { ".png", TestImages.Png },
        { ".PNG", TestImages.Png },
        { ".jpg", TestImages.Jpeg },
        { ".jpeg", TestImages.Jpeg },
        { ".webp", TestImages.Webp }
    };

    public static TheoryData<string, byte[]> InvalidImages => new()
    {
        { ".png", TestImages.Jpeg },
        { ".jpg", TestImages.Png },
        { ".png", TestImages.Text },
        { ".webp", TestImages.Wav },
        { ".webp", TestImages.Png },
        { ".gif", TestImages.Png },
        { ".exe", TestImages.Png },
        { "", TestImages.Png },
        { ".png", [0x89, 0x50] },
        { ".png", [] }
    };

    [Theory]
    [MemberData(nameof(ValidImages))]
    public void IsValid_ContentMatchesExtension_ReturnsTrue(string extension, byte[] content)
    {
        using var stream = new MemoryStream(content);

        Assert.True(ImageFileSignature.IsValid(stream, extension));
    }

    [Theory]
    [MemberData(nameof(InvalidImages))]
    public void IsValid_ContentDoesNotMatchExtension_ReturnsFalse(string extension, byte[] content)
    {
        using var stream = new MemoryStream(content);

        Assert.False(ImageFileSignature.IsValid(stream, extension));
    }

    [Fact]
    public void IsValid_Always_RewindsTheStream()
    {
        using var stream = new MemoryStream(TestImages.Png);
        stream.Position = 5;

        ImageFileSignature.IsValid(stream, ".png");

        Assert.Equal(0, stream.Position);
    }
}
