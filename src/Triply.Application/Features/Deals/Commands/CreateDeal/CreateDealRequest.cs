namespace Triply.Application.Features.Deals.Commands.CreateDeal;

/// <summary>Request payload used to create deal.</summary>
public class CreateDealRequest
{
    /// <summary>Gets or sets the identifier of the room.</summary>
    public Guid RoomId { get; set; }
    /// <summary>Gets or sets the title.</summary>
    public string Title { get; set; }
    /// <summary>Gets or sets the discount percentage.</summary>
    public decimal DiscountPercentage { get; set; }
    /// <summary>Gets or sets when the create deal starts.</summary>
    public DateTime StartsAt { get; set; }
    /// <summary>Gets or sets when the create deal ends.</summary>
    public DateTime EndsAt { get; set; }
    /// <summary>Gets or sets whether the create deal is featured.</summary>
    public bool IsFeatured { get; set; } = true;
}