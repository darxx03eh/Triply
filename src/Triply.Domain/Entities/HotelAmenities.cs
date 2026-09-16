namespace Triply.Domain.Entities;

public sealed class HotelAmenities
{
    public HotelAmenities() => HotelAmenitiesId = Guid.NewGuid();
    public Guid HotelAmenitiesId { get; set; }
    public Guid HotelId { get; set; }
    public Guid AmenityId { get; set; }
    public Hotel Hotel { get; set; } = null!;
    public Amenity Amenity { get; set; } = null!;
}