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
    public string? Url { get; set; } = null!;
    public short DisplayOrder { get; set; }
    public HotelImageStatus Status { get; set; }
    public Hotel Hotel { get; set; } = null!;
}