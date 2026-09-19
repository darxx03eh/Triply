namespace Triply.Application.Features.Attractions.Commands.UpdateAttraction;

public class UpdateAttractionRequest
{
    public string Name { get; set; }
    public string Category { get; set; }
    public decimal DistanceKm { get; set; }
}