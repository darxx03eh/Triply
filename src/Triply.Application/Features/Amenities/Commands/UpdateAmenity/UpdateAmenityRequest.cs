using System.Text.Json.Serialization;

namespace Triply.Application.Features.Amenities.Commands.UpdateAmenity;

public class UpdateAmenityRequest
{
    [JsonIgnore]
    public Guid AmenityId { get; set; }
    public string Name { get; set; }
}
