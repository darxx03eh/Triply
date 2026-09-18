using Triply.Application.Common.Models;

namespace Triply.Application.DTOs.Search;

public class SearchHotelsResponse : PagedResult<HotelSearchItemResponse>
{
    public DateOnly CheckIn { get; init; }
    public DateOnly CheckOut { get; init; }
    public int Nights { get; init; }
    public int Adults { get; init; }
    public int Children { get; init; }
    public int Rooms { get; init; }
}
