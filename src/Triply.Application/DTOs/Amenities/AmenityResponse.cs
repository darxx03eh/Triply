namespace Triply.Application.DTOs.Amenities;

public record AmenityResponse
{
    public Guid AmenityId { get; init; }
    public string Name { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? ModifiedAt { get; init; }
}
