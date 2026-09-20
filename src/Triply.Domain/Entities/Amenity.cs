using Triply.Domain.Entities.Base;

namespace Triply.Domain.Entities;

/// <summary>Represents the amenity.</summary>
public sealed class Amenity : BaseEntity
{
    /// <summary>Initializes a new instance of the amenity.</summary>
    public Amenity()
    {
        AmenityId = Guid.NewGuid();
        HotelAmenities =  new HashSet<HotelAmenities>();
    }
    /// <summary>Gets or sets the identifier of the amenity.</summary>
    public Guid AmenityId { get; set; }
    /// <summary>Gets or sets the name.</summary>
    public string Name { get; set; } = null!;
    /// <summary>Gets or sets the navigation property for hotel amenities.</summary>
    public ICollection<HotelAmenities> HotelAmenities { get; set; }
}