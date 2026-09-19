namespace Triply.Application.DTOs.Home;

public record RecentHotelResponse()
{
    public Guid HotelId { get; init; }
    public string Name { get; init; }
    public string CityName { get; init; }
    public string Country { get; init; }
    public byte StarRating { get; init; }
    public string? ThumbnailUrl { get; init; }
    public decimal? MinPricePerNight { get; init; }
    public DateTime VisitedAt { get; init; }
}