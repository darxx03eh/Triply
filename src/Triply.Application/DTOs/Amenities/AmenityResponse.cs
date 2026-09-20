namespace Triply.Application.DTOs.Amenities;

/// <summary>Response returned for the amenity.</summary>
public record AmenityResponse
{
    /// <summary>Gets the identifier of the amenity.</summary>
    public Guid AmenityId { get; init; }
    /// <summary>Gets the name.</summary>
    public string Name { get; init; }
    /// <summary>Gets when the amenity was created.</summary>
    public DateTime CreatedAt { get; init; }
    /// <summary>Gets when the amenity was modified.</summary>
    public DateTime? ModifiedAt { get; init; }
}
