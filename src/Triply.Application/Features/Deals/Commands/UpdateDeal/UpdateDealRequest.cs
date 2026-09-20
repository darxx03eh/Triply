namespace Triply.Application.Features.Deals.Commands.UpdateDeal;

/// <summary>Request payload used to update deal.</summary>
public class UpdateDealRequest
{
    /// <summary>Gets or sets the title.</summary>
    public string Title { get; set; }
    /// <summary>Gets or sets the discount percentage.</summary>
    public decimal DiscountPercentage { get; set; }
    /// <summary>Gets or sets when the update deal starts.</summary>
    public DateTime StartsAt { get; set; }
    /// <summary>Gets or sets when the update deal ends.</summary>
    public DateTime EndsAt { get; set; }
    /// <summary>Gets or sets whether the update deal is featured.</summary>
    public bool IsFeatured { get; set; }
}