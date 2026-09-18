using Triply.Domain.Enums.HotleImages;

namespace Triply.Domain.Entities;

public sealed class HotelImage
{
    public HotelImage()
    {
        ImageId = Guid.NewGuid();
        Status = HotelImageStatus.Pending;
    }
    public Guid ImageId { get; set; }
    public Guid HotelId { get; set; }
    public string? Url { get; set; }
    /// <summary>Cloudinary public id, needed to delete the asset. Null until the upload completes.</summary>
    public string? PublicId { get; set; }
    public short DisplayOrder { get; set; }
    public HotelImageStatus Status { get; set; }
    public Hotel Hotel { get; set; } = null!;
}