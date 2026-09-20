namespace Triply.Application.Features.Cities.Commands.CreateCity;

/// <summary>Data required to create a city.</summary>
public class CreateCityRequest
{
    /// <summary>Gets or sets the name.</summary>
    public string Name { get; set; }
    /// <summary>Gets or sets the country.</summary>
    public string Country { get; set; }
    /// <summary>Gets or sets the post office.</summary>
    public string? PostOffice { get; set; }
}