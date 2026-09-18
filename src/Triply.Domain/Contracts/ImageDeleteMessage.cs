namespace Triply.Domain.Contracts;

public class ImageDeleteMessage
{
    public Guid ImageId { get; set; }
    public Guid HotelId { get; set; }
    public string PublicId { get; set; } = null!;
}
