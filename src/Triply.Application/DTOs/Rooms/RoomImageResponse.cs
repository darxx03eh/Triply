using System.Text.Json.Serialization;
using Triply.Domain.Enums.Images;

namespace Triply.Application.DTOs.Rooms;

/// <summary>Response returned for the room image.</summary>
public record RoomImageResponse()
{
    /// <summary>Gets the identifier of the image.</summary>
    public Guid ImageId { get; init; }
    /// <summary>Gets the identifier of the room.</summary>
    public Guid RoomId { get; init; }
    /// <summary>Gets the URL.</summary>
    public string? Url { get; init; }
    /// <summary>Gets the display order.</summary>
    public short DisplayOrder { get; init; }
    /// <summary>Gets the status.</summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ImageStatus Status { get; init; }
}