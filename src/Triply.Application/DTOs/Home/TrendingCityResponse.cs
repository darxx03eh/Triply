namespace Triply.Application.DTOs.Home;

public class TrendingCityResponse
{
    public Guid CityId { get; init; }
    public string Name { get; init; }
    public string Country { get; init; }
    public string? ThumbnailUrl { get; init; }
    public int VisitsCount { get; init; }
    public int HotelsCount { get; init; }
}