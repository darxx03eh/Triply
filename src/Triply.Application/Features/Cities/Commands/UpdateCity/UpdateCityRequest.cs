using System.Text.Json.Serialization;

namespace Triply.Application.Features.Cities.Commands.UpdateCity;

/// <summary>Data required to update a city.</summary>
public class UpdateCityRequest
{
    /// <summary>Gets or sets the identifier of the city.</summary>
    [JsonIgnore]
    public Guid CityId { get; set; }
    /// <summary>Gets or sets the name.</summary>
    public string Name { get; set; }
    /// <summary>Gets or sets the country.</summary>
    public string Country { get; set; }
    /// <summary>Gets or sets the post office.</summary>
    public string? PostOffice { get; set; }
    /// <summary>Gets or sets the row version.</summary>
    public byte[] RowVersion { get; set; }
}