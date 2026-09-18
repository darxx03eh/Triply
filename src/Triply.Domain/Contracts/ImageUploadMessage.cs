using Triply.Domain.Contracts.Enums;

namespace Triply.Domain.Contracts;

public class ImageUploadMessage
{
    public Guid ImageId { get; set; }
    public Guid HotelId { get; set; }
    public Guid? CityId { get; set; }
    public ImageTarget Target { get; set; } = ImageTarget.HotelImage;
    public string FilePath { get; set; }
    public string OriginalFileName { get; set; }
}