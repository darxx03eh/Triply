namespace Triply.Application.DTOs.Hotels;

public record HotelResponse()
{
    public Guid HotelId { get; init; }
    public string Name { get; init; }
    public Guid CityId { get; init; }
    public string CityName { get; init; }
    public Guid? OwnerId { get; init; }
    public byte StarRating { get; init; }
    public string Description { get; init; }
    public decimal? Latitude { get; init; }
    public decimal? Longitude { get; init; }
    public IReadOnlyList<string> ImageUrls { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? ModifiedAt { get; init; }
    /// <summary>Concurrency token (base64 in JSON); send it back unchanged when updating the hotel.</summary>
    public byte[] RowVersion { get; init; } = [];
}