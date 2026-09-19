namespace Triply.Application.Features.Deals.Commands.UpdateDeal;

public class UpdateDealRequest
{
    public string Title { get; set; }
    public decimal DiscountPercentage { get; set; }
    public DateTime StartsAt { get; set; }
    public DateTime EndsAt { get; set; }
    public bool IsFeatured { get; set; }
}