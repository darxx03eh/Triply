namespace Triply.Application.Features.Search.Queries.SearchHotels;

/// <summary>Request payload used to search hotels.</summary>
public class SearchHotelsRequest
{
    /// <summary>Gets or sets the q.</summary>
    public string? Q { get; set; }
    /// <summary>Gets or sets the identifier of the city.</summary>
    public Guid? CityId { get; set; }
    /// <summary>Gets or sets the check in.</summary>
    public DateOnly? CheckIn { get; set; }
    /// <summary>Gets or sets the check out.</summary>
    public DateOnly? CheckOut { get; set; }
    /// <summary>Gets or sets the adults.</summary>
    public int? Adults { get; set; }
    /// <summary>Gets or sets the children.</summary>
    public int? Children { get; set; }
    /// <summary>Gets or sets the rooms.</summary>
    public int? Rooms { get; set; }
    /// <summary>Gets or sets the min price.</summary>
    public decimal? MinPrice { get; set; }
    /// <summary>Gets or sets the max price.</summary>
    public decimal? MaxPrice { get; set; }
    /// <summary>Gets or sets the stars.</summary>
    public int[]? Stars { get; set; }
    /// <summary>Gets or sets the types.</summary>
    public string[]? Types { get; set; }
    /// <summary>Gets or sets the amenities.</summary>
    public Guid[]? Amenities { get; set; }
    /// <summary>Gets or sets the sort.</summary>
    public string? Sort { get; set; }
    /// <summary>Gets or sets the page.</summary>
    public int? Page { get; set; }
    /// <summary>Gets or sets the page size.</summary>
    public int? PageSize { get; set; }
}
