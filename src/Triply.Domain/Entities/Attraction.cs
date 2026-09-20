using Triply.Domain.Entities.Base;

namespace Triply.Domain.Entities;

/// <summary>Represents the attraction.</summary>
public class Attraction : BaseEntity
{
    /// <summary>Initializes a new instance of the attraction.</summary>
    public Attraction() => AttractionId = Guid.NewGuid();

    /// <summary>Gets or sets the identifier of the attraction.</summary>
    public Guid AttractionId { get; set; }
    /// <summary>Gets or sets the identifier of the hotel.</summary>
    public Guid HotelId { get; set; }
    /// <summary>Gets or sets the name.</summary>
    public string Name { get; set; } = null!;
    /// <summary>Gets or sets the category.</summary>
    public string Category { get; set; } = null!;
    /// <summary>Gets or sets the distance km.</summary>
    public decimal DistanceKm { get; set; }
    /// <summary>Gets or sets the navigation property for hotel.</summary>
    public Hotel Hotel { get; set; } = null!;
}