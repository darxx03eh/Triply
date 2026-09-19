using System.Text.Json.Serialization;

namespace Triply.Application.Features.Attractions.Commands.CreateAttraction;

public class CreateAttractionRequest
{
    [JsonIgnore]
    public Guid HotelId { get; set; }
    public string Name { get; set; }
    public string Category { get; set; }
    public decimal DistanceKm { get; set; }
}