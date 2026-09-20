namespace ImageUploader.Options;

/// <summary>Configuration options of Cloudinary.</summary>
public class CloudinaryOptions
{
    /// <summary>Gets or sets the cloud name.</summary>
    public string CloudName { get; set; } = default!;
    /// <summary>Gets or sets the API key.</summary>
    public string ApiKey { get; set; } = default!;
    /// <summary>Gets or sets the API secret.</summary>
    public string ApiSecret { get; set; } = default!;
}