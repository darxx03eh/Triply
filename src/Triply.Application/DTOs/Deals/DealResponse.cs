namespace Triply.Application.DTOs.Deals;

/// <summary>Gets or sets the deal response.</summary>
/// <summary>Response returned for the deal.</summary>
public record DealResponse()
{
    /// <summary>Gets the identifier of the deal.</summary>
    public Guid DealId { get; init; }
    /// <summary>Gets the identifier of the room.</summary>
    public Guid RoomId { get; init; }
    /// <summary>Gets the title.</summary>
    public string Title { get; init; }
    /// <summary>Gets the discount percentage.</summary>
    public decimal DiscountPercentage { get; init; }
    /// <summary>Gets when the deal starts.</summary>
    public DateTime StartsAt { get; init; }
    /// <summary>Gets when the deal ends.</summary>
    public DateTime EndsAt { get; init; }
    /// <summary>Gets whether the deal is featured.</summary>
    public bool IsFeatured { get; init; }
    /// <summary>Gets when the deal was created.</summary>
    public DateTime CreatedAt { get; init; }
}