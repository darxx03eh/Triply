using Triply.Domain.Enums.Images;
namespace Triply.Domain.Entities;

/// <summary>Represents the hotel image.</summary>
public sealed class HotelImage
{
    /// <summary>Initializes a new instance of the hotel image.</summary>
    public HotelImage()
    {
        ImageId = Guid.NewGuid();
        Status = ImageStatus.Pending;
    }
    /// <summary>Gets or sets the identifier of the image.</summary>
    public Guid ImageId { get; set; }
    /// <summary>Gets or sets the identifier of the hotel.</summary>
    public Guid HotelId { get; set; }
    /// <summary>Gets or sets the URL.</summary>
    public string? Url { get; set; }
    /// <summary>Cloudinary public id, needed to delete the asset. Null until the upload completes.</summary>
    public string? PublicId { get; set; }
    /// <summary>Gets or sets the display order.</summary>
    public short DisplayOrder { get; set; }
    /// <summary>Gets or sets the status.</summary>
    public ImageStatus Status { get; set; }
    /// <summary>Gets or sets the navigation property for hotel.</summary>
    public Hotel Hotel { get; set; } = null!;
}