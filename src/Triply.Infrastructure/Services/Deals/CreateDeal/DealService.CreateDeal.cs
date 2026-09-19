using Triply.Application.DTOs.Deals;
using Triply.Application.Extensions;
using Triply.Application.Features.Deals.Commands.CreateDeal;
using Triply.Domain.Results;
using Triply.Domain.Results.Enums;

namespace Triply.Infrastructure.Services.Deals;

public partial class DealService
{
    public async Task<Result<DealResponse>> CreateAsync(CreateDealRequest request,
        CancellationToken cancellationToken = default)
    {
        var room = await roomRepository.GetByIdAsync(request.RoomId, cancellationToken);
        if (room is null || room.IsDeleted)
            return Result<DealResponse>.Failure(
                "ROOM_NOT_FOUND",
                $"The requested room with id: {request.RoomId.ToString()} was not found.",
                ResultErrorType.NotFound);

        if (await dealRepository.IsDealOverlappingAsync(request.RoomId, request.StartsAt, request.EndsAt,
                cancellationToken: cancellationToken))
            return Result<DealResponse>.Failure(
                "DEAL_OVERLAPPING",
                "This room already has a deal in the same period.",
                ResultErrorType.Conflict);

        var deal = request.ToDeal();

        await dealRepository.AddAsync(deal, cancellationToken);
        await dealRepository.SaveChangesAsync(cancellationToken);

        return Result<DealResponse>.Success(
            deal.ToDealResponse(), ResultSuccessType.Created,
            new ResultSuccess("DEAL_CREATED", "Deal created successfully."));
    }
}