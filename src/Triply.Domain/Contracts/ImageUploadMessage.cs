namespace Triply.Domain.Contracts;

public class ImageUploadMessage
{
    public Guid ImageId { get; set; }
    public Guid HotelId { get; set; }
    public string FilePath { get; set; }
    public string OriginalFileName { get; set; }
}