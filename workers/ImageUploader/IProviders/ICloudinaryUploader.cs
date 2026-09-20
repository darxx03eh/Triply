namespace ImageUploader.IProviders;

/// <summary>Defines the Cloudinary uploader operations.</summary>
public interface ICloudinaryUploader
{
    /// <summary>Uploads.</summary>
    Task<CloudinaryUploadResult> UploadAsync(string filePath, string originalFileName, string folder,
        CancellationToken cancellationToken = default);

    /// <summary>Deletes the Cloudinary uploader.</summary>
    Task DeleteAsync(string publicId, CancellationToken cancellationToken = default);
}

/// <summary>Gets or sets the Cloudinary upload result.</summary>
/// <summary>Gets or sets the Cloudinary upload result.</summary>
/// <summary>Gets or sets the identifier of the public.</summary>
/// <summary>Gets or sets the URL.</summary>
public sealed record CloudinaryUploadResult(string Url, string PublicId);
