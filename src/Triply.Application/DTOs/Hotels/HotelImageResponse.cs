using System.Text.Json.Serialization;
using Triply.Domain.Enums.HotleImages;

namespace Triply.Application.DTOs.Hotels;

/// <summary>Response returned for the hotel image.</summary>
public record HotelImageResponse
{
    /// <summary>Gets the identifier of the image.</summary>
    public Guid ImageId { get; init; }
    /// <summary>Gets the identifier of the hotel.</summary>
    public Guid HotelId { get; init; }
    /// <summary>Gets the URL.</summary>
    public string? Url { get; init; }
    /// <summary>Gets the display order.</summary>
    public short DisplayOrder { get; init; }
    /// <summary>Gets the status.</summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public HotelImageStatus Status { get; init; }
}
