using Triply.Domain.Entities.Base;

namespace Triply.Domain.Entities;

public sealed class Amenity : BaseEntity
{
    public Amenity()
    {
        AmenityId = Guid.NewGuid();
        HotelAmenities =  new HashSet<HotelAmenities>();
    }
    public Guid AmenityId { get; set; }
    public string Name { get; set; } = null!;
    public ICollection<HotelAmenities> HotelAmenities { get; set; }
}