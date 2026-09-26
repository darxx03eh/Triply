using Triply.Domain.Contracts.Enums;

namespace Triply.Domain.Contracts;

/// <summary>Message payload published for the image upload.</summary>
public class ImageUploadMessage
{
    /// <summary>Gets or sets the identifier of the image.</summary>
    public Guid ImageId { get; set; }
    /// <summary>Gets or sets the identifier.</summary>
    public Guid Id { get; set; }
    /// <summary>Gets or sets the identifier of the city.</summary>
    public Guid? CityId { get; set; }
    /// <summary>Gets or sets the target.</summary>
    public ImageTarget Target { get; set; } = ImageTarget.HotelImage;
    /// <summary>Gets or sets the file path.</summary>
    public string FilePath { get; set; }
    /// <summary>Gets or sets the original file name.</summary>
    public string OriginalFileName { get; set; }
}