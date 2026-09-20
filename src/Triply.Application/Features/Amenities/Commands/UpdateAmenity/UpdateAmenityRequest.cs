using System.Text.Json.Serialization;

namespace Triply.Application.Features.Amenities.Commands.UpdateAmenity;

/// <summary>Request payload used to update amenity.</summary>
public class UpdateAmenityRequest
{
    /// <summary>Gets or sets the identifier of the amenity.</summary>
    [JsonIgnore]
    public Guid AmenityId { get; set; }
    /// <summary>Gets or sets the name.</summary>
    public string Name { get; set; }
}
