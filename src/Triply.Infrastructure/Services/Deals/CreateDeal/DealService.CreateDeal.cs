using Microsoft.Extensions.Logging;
using Triply.Application.DTOs.Deals;
using Triply.Application.Extensions;
using Triply.Application.Features.Deals.Commands.CreateDeal;
using Triply.Domain.Results;
using Triply.Domain.Results.Enums;

namespace Triply.Infrastructure.Services.Deals;

/// <summary>Implements the deal operations.</summary>
public partial class DealService
{
    /// <summary>Creates a new deal.</summary>
    public async Task<Result<DealResponse>> CreateAsync(CreateDealRequest request,
        CancellationToken cancellationToken = default)
    {
        var room = await roomRepository.GetByIdAsync(request.RoomId, cancellationToken);
        if (room is null || room.IsDeleted)
        {
            logger.LogWarning("Create deal {DealTitle} failed: room {RoomId} was not found", request.Title,
                request.RoomId);
            return Result<DealResponse>.Failure(
                "ROOM_NOT_FOUND",
                $"The requested room with id: {request.RoomId.ToString()} was not found.",
                ResultErrorType.NotFound);
        }

        var isDealOverlapping = await dealRepository.IsDealOverlappingAsync(request.RoomId,
            request.StartsAt, request.EndsAt,
            cancellationToken: cancellationToken);
        if (isDealOverlapping)
        {
            logger.LogWarning(
                "Create deal {DealTitle} rejected: room {RoomId} already has a deal between {StartsAt} and {EndsAt}",
                request.Title, request.RoomId, request.StartsAt, request.EndsAt);
            return Result<DealResponse>.Failure(
                "DEAL_OVERLAPPING",
                "This room already has a deal in the same period.",
                ResultErrorType.Conflict);
        }
        
        var deal = request.ToDeal();

        await dealRepository.AddAsync(deal, cancellationToken);
        await dealRepository.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Deal {DealId} ({DealTitle}, -{DiscountPercentage}%) created on room {RoomId} " +
                              "from {StartsAt} to {EndsAt}, featured: {IsFeatured}",
            deal.DealId, deal.Title, deal.DiscountPercentage, deal.RoomId, deal.StartsAt, deal.EndsAt, deal.IsFeatured);

        return Result<DealResponse>.Success(
            deal.ToDealResponse(), ResultSuccessType.Created,
            new ResultSuccess("DEAL_CREATED", "Deal created successfully."));
    }
}