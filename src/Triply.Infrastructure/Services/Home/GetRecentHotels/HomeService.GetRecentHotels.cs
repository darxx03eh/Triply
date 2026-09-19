using Triply.Application.DTOs.Home;
using Triply.Domain.Results;

namespace Triply.Infrastructure.Services.Home;

public partial class HomeService
{
    public async Task<Result<IReadOnlyList<RecentHotelResponse>>> GetRecentHotelsAsync(Guid userId, int? count,
        CancellationToken cancellationToken = default)
    {
        var hotels = await recentVisitRepository.GetRecentHotelsAsync(
            userId, Math.Clamp(count ?? 5, 1, 20), cancellationToken);

        return Result<IReadOnlyList<RecentHotelResponse>>.Success(hotels, success: hotels.Count == 0
            ? new("RECENT_HOTELS_EMPTY", "You have not visited any hotel yet.")
            : new("RECENT_HOTELS_FOUND", "Recently visited hotels were retrieved successfully."));
    }
}