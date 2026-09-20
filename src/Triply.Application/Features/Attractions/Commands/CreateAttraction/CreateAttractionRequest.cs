using System.Text.Json.Serialization;

namespace Triply.Application.Features.Attractions.Commands.CreateAttraction;

/// <summary>Request payload used to create attraction.</summary>
public class CreateAttractionRequest
{
    /// <summary>Gets or sets the identifier of the hotel.</summary>
    [JsonIgnore]
    public Guid HotelId { get; set; }
    /// <summary>Gets or sets the name.</summary>
    public string Name { get; set; }
    /// <summary>Gets or sets the category.</summary>
    public string Category { get; set; }
    /// <summary>Gets or sets the distance km.</summary>
    public decimal DistanceKm { get; set; }
}