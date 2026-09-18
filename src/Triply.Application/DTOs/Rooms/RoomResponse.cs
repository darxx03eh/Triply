using System.Text.Json.Serialization;
using Triply.Domain.Enums.Rooms;

namespace Triply.Application.DTOs.Rooms;

public record RoomResponse
{
    public Guid RoomId { get; init; }
    public Guid HotelId { get; init; }
    public string HotelName { get; init; }
    public string Number { get; init; }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public RoomType RoomType { get; init; }
    public short AdultCapacity { get; init; }
    public short ChildCapacity { get; init; }
    public decimal PricePerNight { get; init; }
    public bool IsAvailable { get; init; }
    public string? Description { get; init; }
    public bool IsDeleted { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? ModifiedAt { get; init; }
    public byte[] RowVersion { get; init; } = [];
}
