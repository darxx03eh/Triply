namespace Triply.Domain.Entities;

public sealed class HotelImage
{
    public Guid ImageId { get; set; } = Guid.NewGuid();

    public Guid HotelId { get; set; }

    public string Url { get; set; } = null!;

    public short DisplayOrder { get; set; }

    public Hotel Hotel { get; set; } = null!;
}