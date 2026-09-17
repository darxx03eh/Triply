namespace Triply.Application.DTOs.Cities;

/// <summary>Public representation of a city.</summary>
/// <param name="CityId">The city identifier.</param>
/// <param name="Name">The city name.</param>
/// <param name="Country">The country containing the city.</param>
/// <param name="PostOffice">The optional post-office identifier.</param>
/// <param name="CreatedAt">The UTC creation time.</param>
/// <param name="ModifiedAt">The UTC modification time, when available.</param>
public sealed record CityResponse
{
    public Guid CityId { get; init; }
    public string Name { get; init; }
    public string Country { get; init; }
    public string? PostOffice { get; init; }
    public bool IsDeleted { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? ModifiedAt { get; init; }
}