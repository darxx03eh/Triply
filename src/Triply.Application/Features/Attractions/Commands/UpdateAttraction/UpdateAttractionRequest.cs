namespace Triply.Application.Features.Attractions.Commands.UpdateAttraction;

/// <summary>Request payload used to update attraction.</summary>
public class UpdateAttractionRequest
{
    /// <summary>Gets or sets the name.</summary>
    public string Name { get; set; }
    /// <summary>Gets or sets the category.</summary>
    public string Category { get; set; }
    /// <summary>Gets or sets the distance km.</summary>
    public decimal DistanceKm { get; set; }
}