namespace Triply.Application.Features.Search.Queries.SearchHotels;

public class SearchHotelsRequest
{
    public string? Q { get; set; }
    public Guid? CityId { get; set; }
    public DateOnly? CheckIn { get; set; }
    public DateOnly? CheckOut { get; set; }
    public int? Adults { get; set; }
    public int? Children { get; set; }
    public int? Rooms { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public int[]? Stars { get; set; }
    public string[]? Types { get; set; }
    public Guid[]? Amenities { get; set; }
    public string? Sort { get; set; }
    public int? Page { get; set; }
    public int? PageSize { get; set; }
}
