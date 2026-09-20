namespace Triply.Application.Options;

/// <summary>Configuration options of image upload.</summary>
public class ImageUploadOptions
{
    /// <summary>Gets or sets the shared storage path.</summary>
    public string SharedStoragePath { get; set; } = "/app/uploads";
    /// <summary>Gets or sets the max file size bytes.</summary>
    public long MaxFileSizeBytes { get; set; } = 10485760;
}