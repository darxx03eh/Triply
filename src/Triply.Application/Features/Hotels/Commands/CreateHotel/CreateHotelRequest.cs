using System.Text.Json.Serialization;
using Triply.Domain.Enums.Hotels;

namespace Triply.Application.Features.Hotels.Commands.CreateHotel;

/// <summary>Request payload used to create hotel.</summary>
public class CreateHotelRequest
{
    /// <summary>Gets or sets the name.</summary>
    public string Name { get; set; }
    /// <summary>Gets or sets the identifier of the city.</summary>
    public Guid CityId { get; set; }
    /// <summary>Gets or sets the star rating.</summary>
    public byte StarRating  { get; set; }
    /// <summary>Gets or sets the hotel type.</summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public HotelType HotelType { get; set; }
    /// <summary>Gets or sets the address.</summary>
    public string Address { get; set; }
    /// <summary>Gets or sets the description.</summary>
    public string Description { get; set; }
    /// <summary>Gets or sets the latitude.</summary>
    public decimal? Latitude { get; set; }
    /// <summary>Gets or sets the longitude.</summary>
    public decimal? Longitude { get; set; }
}