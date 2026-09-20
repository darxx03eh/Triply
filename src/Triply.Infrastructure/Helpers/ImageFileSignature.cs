namespace Triply.Infrastructure.Helpers;

/// <summary>Represents the image file signature.</summary>
public static class ImageFileSignature
{
    private static readonly Dictionary<string, byte[]> Signatures = new()
    {
        [".jpg"] = [0xFF, 0xD8, 0xFF],
        [".jpeg"] = [0xFF, 0xD8, 0xFF],
        [".png"] = [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A],
        [".webp"] = [0x52, 0x49, 0x46, 0x46]
    };

    private static readonly byte[] WebpMarker = "WEBP"u8.ToArray();

    /// <summary>Checks whether the valid.</summary>
    public static bool IsValid(Stream stream, string extension)
    {
        if (!Signatures.TryGetValue(extension.ToLowerInvariant(), out var signature))
            return false;

        var header = new byte[12];
        stream.Position = 0;
        int bytesRead = stream.ReadAtLeast(header, header.Length, throwOnEndOfStream: false);
        stream.Position = 0;

        if (bytesRead < signature.Length || !header.AsSpan(0, signature.Length).SequenceEqual(signature))
            return false;

        return extension != ".webp"
               || (bytesRead >= 12 && header.AsSpan(8, 4).SequenceEqual(WebpMarker));
    }
}
