namespace Triply.Application.DTOs.Cities;

/// <summary>Public representation of a city.</summary>
public sealed record CityResponse
{
    /// <summary>Gets the identifier of the city.</summary>
    public Guid CityId { get; init; }
    /// <summary>Gets the name.</summary>
    public string Name { get; init; }
    /// <summary>Gets the country.</summary>
    public string Country { get; init; }
    /// <summary>Gets the post office.</summary>
    public string? PostOffice { get; init; }
    /// <summary>Gets the number of hotels.</summary>
    public int HotelsCount { get; init; }
    /// <summary>Gets the thumbnail URL.</summary>
    public string? ThumbnailUrl { get; init; }
    /// <summary>Gets whether the city is deleted.</summary>
    public bool IsDeleted { get; init; }
    /// <summary>Gets when the city was created.</summary>
    public DateTime CreatedAt { get; init; }
    /// <summary>Gets when the city was modified.</summary>
    public DateTime? ModifiedAt { get; init; }
    /// <summary>Gets the row version.</summary>
    public byte[] RowVersion { get; init; } = [];
}