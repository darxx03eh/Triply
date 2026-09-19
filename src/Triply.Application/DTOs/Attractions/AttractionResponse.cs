namespace Triply.Application.DTOs.Attractions;

public record AttractionResponse
{
    public Guid AttractionId { get; init; }
    public Guid HotelId { get; init; }
    public string Name { get; init; }
    public string Category { get; init; }
    public decimal DistanceKm { get; init; }
}