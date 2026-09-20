using Microsoft.Extensions.Logging;
using Triply.Application.DTOs.Deals;
using Triply.Application.Extensions;
using Triply.Application.Features.Deals.Commands.UpdateDeal;
using Triply.Domain.Results;
using Triply.Domain.Results.Enums;

namespace Triply.Infrastructure.Services.Deals;

public partial class DealService
{
    /// <summary>Updates an existing deal.</summary>
    public async Task<Result<DealResponse>> UpdateAsync(Guid dealId, UpdateDealRequest request,
        CancellationToken cancellationToken = default)
    {
        var deal = await dealRepository.GetByIdAsync(dealId, cancellationToken);
        if (deal is null)
        {
            logger.LogWarning("Update deal failed: deal {DealId} was not found", dealId);
            return Result<DealResponse>.Failure(
                "DEAL_NOT_FOUND",
                $"The requested deal with id: {dealId.ToString()} was not found.",
                ResultErrorType.NotFound);
        }

        var isDealOverlapping = await dealRepository.IsDealOverlappingAsync(deal.RoomId, request.StartsAt,
            request.EndsAt, dealId,
            cancellationToken);
        if (isDealOverlapping)
        {
            logger.LogWarning(
                "Update deal {DealId} rejected: room {RoomId} already has another deal between {StartsAt} and {EndsAt}",
                dealId, deal.RoomId, request.StartsAt, request.EndsAt);
            return Result<DealResponse>.Failure(
                "DEAL_OVERLAPPING",
                "This room already has a deal in the same period.",
                ResultErrorType.Conflict);
        }

        deal.Title = request.Title.Trim();
        deal.DiscountPercentage = request.DiscountPercentage;
        deal.StartsAt = request.StartsAt;
        deal.EndsAt = request.EndsAt;
        deal.IsFeatured = request.IsFeatured;
        deal.ModifiedAt = DateTime.UtcNow;
        await dealRepository.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Deal {DealId} ({DealTitle}, -{DiscountPercentage}%) updated, " +
                              "from {StartsAt} to {EndsAt}, featured: {IsFeatured}",
            dealId, deal.Title, deal.DiscountPercentage, deal.StartsAt, deal.EndsAt, deal.IsFeatured);

        return Result<DealResponse>.Success(
            deal.ToDealResponse(), ResultSuccessType.Ok,
            new ResultSuccess("DEAL_UPDATED", "Deal updated successfully."));
    }
}