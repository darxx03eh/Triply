using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using ImageUploader.IProviders;
using ImageUploader.Options;
using Microsoft.Extensions.Options;

namespace ImageUploader.Providers;

public class CloudinaryUploader(IOptions<CloudinaryOptions> options) : ICloudinaryUploader
{
    private readonly Cloudinary _cloudinary = new(new Account(
        options.Value.CloudName, 
        options.Value.ApiKey, 
        options.Value.ApiSecret));

    public async Task<string> UploadAsync(string filePath, string originalFileName,
        CancellationToken cancellationToken = default)
    {
        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(filePath),
            Folder = "triply/hotels",
            UniqueFilename = true,
            Overwrite = false
        };

        var result = await _cloudinary.UploadAsync(uploadParams, cancellationToken: cancellationToken);

        if (result.Error is not null)
            throw new InvalidOperationException($"Cloudinary upload failed: {result.Error.Message}");

        return result.SecureUrl.ToString();
    }
}