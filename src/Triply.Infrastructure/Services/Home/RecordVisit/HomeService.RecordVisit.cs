using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Triply.Domain.Entities;

namespace Triply.Infrastructure.Services.Home;

public partial class HomeService
{
    /// <summary>Records the visit.</summary>
    public async Task RecordVisitAsync(Guid userId, Guid hotelId, CancellationToken cancellationToken = default)
    {
        var visit = await recentVisitRepository.GetUserVisitAsync(userId, hotelId, cancellationToken);
        if (visit is null)
            await recentVisitRepository.AddAsync(
                new UserRecentVisit
            {
                UserId = userId,
                HotelId = hotelId
            }, cancellationToken);
        else visit.VisitedAt = DateTime.UtcNow;
        try
        {
            await recentVisitRepository.SaveChangesAsync(cancellationToken);
            logger.LogDebug("Visit of user {UserId} to hotel {HotelId} recorded (first visit: {IsFirstVisit})",
                userId, hotelId, visit is null);
        }
        catch (DbUpdateException exception)
        {
            logger.LogWarning(exception, "Could not record the visit of user {UserId} to hotel {HotelId}", 
                userId, hotelId);
            throw;
        }
    }
}