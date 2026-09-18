using System.Text.Json.Serialization;
using Triply.Domain.Enums.HotleImages;

namespace Triply.Application.DTOs.Hotels;

public record HotelImageResponse
{
    public Guid ImageId { get; init; }
    public Guid HotelId { get; init; }
    public string? Url { get; init; }
    public short DisplayOrder { get; init; }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public HotelImageStatus Status { get; init; }
}
