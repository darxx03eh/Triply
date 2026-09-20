namespace Triply.Application.DTOs.Home;

/// <summary>Response returned for the trending city.</summary>
public class TrendingCityResponse
{
    /// <summary>Gets the identifier of the city.</summary>
    public Guid CityId { get; init; }
    /// <summary>Gets the name.</summary>
    public string Name { get; init; }
    /// <summary>Gets the country.</summary>
    public string Country { get; init; }
    /// <summary>Gets the thumbnail URL.</summary>
    public string? ThumbnailUrl { get; init; }
    /// <summary>Gets the number of visits.</summary>
    public int VisitsCount { get; init; }
    /// <summary>Gets the number of hotels.</summary>
    public int HotelsCount { get; init; }
}