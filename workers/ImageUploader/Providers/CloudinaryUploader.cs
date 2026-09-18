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

    public async Task<CloudinaryUploadResult> UploadAsync(string filePath, string originalFileName, string folder,
        CancellationToken cancellationToken = default)
    {
        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(filePath),
            Folder = folder,
            UniqueFilename = true,
            Overwrite = false
        };

        var result = await _cloudinary.UploadAsync(uploadParams, cancellationToken: cancellationToken);

        if (result.Error is not null)
            throw new InvalidOperationException($"Cloudinary upload failed: {result.Error.Message}");

        return new CloudinaryUploadResult(result.SecureUrl.ToString(), result.PublicId);
    }

    public async Task DeleteAsync(string publicId, CancellationToken cancellationToken = default)
    {
        var result = await _cloudinary.DestroyAsync(new DeletionParams(publicId)
        {
            ResourceType = ResourceType.Image,
            Invalidate = true
        });

        if (result.Error is not null)
            throw new InvalidOperationException($"Cloudinary delete failed: {result.Error.Message}");

        // "not found" means it is already gone, which is the desired end state.
        if (result.Result is not ("ok" or "not found"))
            throw new InvalidOperationException($"Cloudinary delete returned '{result.Result}'.");
    }
}
