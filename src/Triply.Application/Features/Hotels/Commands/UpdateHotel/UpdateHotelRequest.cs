using System.Text.Json.Serialization;
using Triply.Domain.Enums.Hotels;

namespace Triply.Application.Features.Hotels.Commands.UpdateHotel;

public class UpdateHotelRequest
{
    public string Name { get; set; }
    public Guid CityId { get; set; }
    [JsonIgnore]
    public Guid HotelId { get; set; }
    public Guid? OwnerId { get; set; }
    public byte StarRating { get; set; }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public HotelType HotelType { get; set; }
    public string Address { get; set; }
    public string? Description { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public byte[] RowVersion { get; set; }
}