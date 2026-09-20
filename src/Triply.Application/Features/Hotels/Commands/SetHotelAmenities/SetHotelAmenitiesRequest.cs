namespace Triply.Application.Features.Hotels.Commands.SetHotelAmenities;

/// <summary>Request payload used to set hotel amenities.</summary>
public class SetHotelAmenitiesRequest
{
    /// <summary>Gets or sets the amenity identifiers.</summary>
    public List<Guid> AmenityIds { get; set; } = [];
}
