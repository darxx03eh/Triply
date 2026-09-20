using Triply.Application.Common.Models;

namespace Triply.Application.DTOs.Search;

/// <summary>Response returned for the search hotels.</summary>
public class SearchHotelsResponse : PagedResult<HotelSearchItemResponse>
{
    /// <summary>Gets the check in.</summary>
    public DateOnly CheckIn { get; init; }
    /// <summary>Gets the check out.</summary>
    public DateOnly CheckOut { get; init; }
    /// <summary>Gets the nights.</summary>
    public int Nights { get; init; }
    /// <summary>Gets the adults.</summary>
    public int Adults { get; init; }
    /// <summary>Gets the children.</summary>
    public int Children { get; init; }
    /// <summary>Gets the rooms.</summary>
    public int Rooms { get; init; }
}
