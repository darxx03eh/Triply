using System.Text;
using Microsoft.AspNetCore.Http;

namespace Triply.Tests.UnitTests.Common.Fakes;

/// <summary>Minimal byte headers that pass (or fail) the image signature checks.</summary>
public static class TestImages
{
    public static byte[] Png => [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A, 0, 0, 0, 0, 1, 2, 3];
    public static byte[] Jpeg => [0xFF, 0xD8, 0xFF, 0xE0, 0, 0x10, 0x4A, 0x46, 0x49, 0x46, 0, 1];
    public static byte[] Webp => [.. "RIFF"u8.ToArray(), 0x24, 0, 0, 0, .. "WEBP"u8.ToArray(), 0x56, 0x50];
    public static byte[] Wav => [.. "RIFF"u8.ToArray(), 0x24, 0, 0, 0, .. "WAVE"u8.ToArray(), 0x66, 0x6D];
    public static byte[] Text => Encoding.UTF8.GetBytes("this is not an image at all");

    public static IFormFile File(string fileName, byte[] content)
    {
        var stream = new MemoryStream(content);
        return new FormFile(stream, 0, content.Length, "file", fileName)
        {
            Headers = new HeaderDictionary(),
            ContentType = "application/octet-stream"
        };
    }
}
