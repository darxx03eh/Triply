namespace Triply.Application.Options;

public class ImageUploadOptions
{
    public string SharedStoragePath { get; set; } = "/app/uploads";
    public long MaxFileSizeBytes { get; set; } = 10485760;
}