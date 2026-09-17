namespace Triply.Application.Features.Cities.Commands.CreateCity;

/// <summary>Data required to create a city.</summary>
/// <param name="Name">The city name.</param>
/// <param name="Country">The country containing the city.</param>
/// <param name="PostOffice">The optional post-office identifier.</param>
public class CreateCityRequest
{
    public string Name { get; set; }
    public string Country { get; set; }
    public string? PostOffice { get; set; }
}