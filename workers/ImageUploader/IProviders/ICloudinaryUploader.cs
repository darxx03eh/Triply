namespace ImageUploader.IProviders;

public interface ICloudinaryUploader
{
    Task<CloudinaryUploadResult> UploadAsync(string filePath, string originalFileName, string folder,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(string publicId, CancellationToken cancellationToken = default);
}

public sealed record CloudinaryUploadResult(string Url, string PublicId);
