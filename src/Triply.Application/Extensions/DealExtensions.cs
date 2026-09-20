using Triply.Application.DTOs.Deals;
using Triply.Application.Features.Deals.Commands.CreateDeal;
using Triply.Domain.Entities;

namespace Triply.Application.Extensions;

/// <summary>Extension methods for deal.</summary>
public static class DealExtensions
{
    /// <summary>Maps the deal to a deal response.</summary>
    public static DealResponse ToDealResponse(this Deal deal)
        => new DealResponse()
        {
            DealId = deal.DealId,
            RoomId = deal.RoomId,
            Title = deal.Title,
            DiscountPercentage = deal.DiscountPercentage,
            StartsAt = deal.StartsAt,
            EndsAt = deal.EndsAt,
            IsFeatured = deal.IsFeatured,
            CreatedAt = deal.CreatedAt
        };

    /// <summary>Maps the deal to a deal.</summary>
    public static Deal ToDeal(this CreateDealRequest request)
        => new Deal()
        {
            RoomId = request.RoomId,
            Title = request.Title.Trim(),
            DiscountPercentage = request.DiscountPercentage,
            StartsAt = request.StartsAt,
            EndsAt = request.EndsAt,
            IsFeatured = request.IsFeatured
        };
    
    /// <summary>Applies the discount.</summary>
    public static decimal ApplyDiscount(this decimal price, decimal discountPercentage)
        => Math.Round(price * (100 - discountPercentage) / 100, 2);
}