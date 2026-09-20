namespace Triply.Application.DTOs.Attractions;

/// <summary>Response returned for the attraction.</summary>
public record AttractionResponse
{
    /// <summary>Gets the identifier of the attraction.</summary>
    public Guid AttractionId { get; init; }
    /// <summary>Gets the identifier of the hotel.</summary>
    public Guid HotelId { get; init; }
    /// <summary>Gets the name.</summary>
    public string Name { get; init; }
    /// <summary>Gets the category.</summary>
    public string Category { get; init; }
    /// <summary>Gets the distance km.</summary>
    public decimal DistanceKm { get; init; }
}