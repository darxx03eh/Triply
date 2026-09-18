using System.Text.Json.Serialization;

namespace Triply.Application.Features.Cities.Commands.UpdateCity;

// <summary>Data required to update a city.</summary>
/// <param name="Name">The city name.</param>
/// <param name="Country">The country containing the city.</param>
/// <param name="PostOffice">The optional post-office identifier.</param>
/// <param name="RowVersion">The concurrency version returned with the city.</param>
public class UpdateCityRequest
{
    [JsonIgnore]
    public Guid CityId { get; set; }
    public string Name { get; set; }
    public string Country { get; set; }
    public string? PostOffice { get; set; }
    public byte[] RowVersion { get; set; }
}