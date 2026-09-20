using System.Text.Json.Serialization;
using Triply.Domain.Enums.Hotels;

namespace Triply.Application.Features.Hotels.Commands.UpdateHotel;

/// <summary>Request payload used to update hotel.</summary>
public class UpdateHotelRequest
{
    /// <summary>Gets or sets the name.</summary>
    public string Name { get; set; }
    /// <summary>Gets or sets the identifier of the city.</summary>
    public Guid CityId { get; set; }
    /// <summary>Gets or sets the identifier of the hotel.</summary>
    [JsonIgnore]
    public Guid HotelId { get; set; }
    /// <summary>Gets or sets the identifier of the owner.</summary>
    public Guid? OwnerId { get; set; }
    /// <summary>Gets or sets the star rating.</summary>
    public byte StarRating { get; set; }
    /// <summary>Gets or sets the hotel type.</summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public HotelType HotelType { get; set; }
    /// <summary>Gets or sets the address.</summary>
    public string Address { get; set; }
    /// <summary>Gets or sets the description.</summary>
    public string? Description { get; set; }
    /// <summary>Gets or sets the latitude.</summary>
    public decimal? Latitude { get; set; }
    /// <summary>Gets or sets the longitude.</summary>
    public decimal? Longitude { get; set; }
    /// <summary>Gets or sets the row version.</summary>
    public byte[] RowVersion { get; set; }
}