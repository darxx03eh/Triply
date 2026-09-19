using Triply.Domain.Entities.Base;

namespace Triply.Domain.Entities;

public class Attraction : BaseEntity
{
    public Attraction() => AttractionId = Guid.NewGuid();

    public Guid AttractionId { get; set; }
    public Guid HotelId { get; set; }
    public string Name { get; set; } = null!;
    public string Category { get; set; } = null!;
    public decimal DistanceKm { get; set; }
    public Hotel Hotel { get; set; } = null!;
}