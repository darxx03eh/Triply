namespace Triply.Domain.Entities;

/// <summary>Represents the hotel amenities.</summary>
public sealed class HotelAmenities
{
    /// <summary>Initializes a new instance of the hotel amenities.</summary>
    public HotelAmenities() => HotelAmenitiesId = Guid.NewGuid();
    /// <summary>Gets or sets the identifier of the hotel amenities.</summary>
    public Guid HotelAmenitiesId { get; set; }
    /// <summary>Gets or sets the identifier of the hotel.</summary>
    public Guid HotelId { get; set; }
    /// <summary>Gets or sets the identifier of the amenity.</summary>
    public Guid AmenityId { get; set; }
    /// <summary>Gets or sets the navigation property for hotel.</summary>
    public Hotel Hotel { get; set; } = null!;
    /// <summary>Gets or sets the navigation property for amenity.</summary>
    public Amenity Amenity { get; set; } = null!;
}