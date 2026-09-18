namespace ImageUploader.IProviders;

public interface ICloudinaryUploader
{
    Task<string> UploadAsync(string filePath, string originalFileName, CancellationToken cancellationToken = default);
}