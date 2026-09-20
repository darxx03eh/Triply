namespace Triply.Application.Common.Models;

/// <summary>Gets or sets the paged result t.</summary>
public class PagedResult<T>
{
    /// <summary>Gets or sets the items.</summary>
    public List<T> Items { get; set; } = [];
    /// <summary>Gets or sets the page.</summary>
    public int Page { get; set; }
    /// <summary>Gets or sets the page size.</summary>
    public int PageSize { get; set; }
    /// <summary>Gets or sets the number of total.</summary>
    public int TotalCount { get; set; }
    /// <summary>Gets the total pages.</summary>
    public int TotalPages => PageSize == 0 ? 0 : (int)Math.Ceiling(TotalCount / (double)PageSize);
}