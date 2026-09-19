using Microsoft.EntityFrameworkCore;
using Triply.Domain.Entities;

namespace Triply.Infrastructure.Services.Home;

public partial class HomeService
{
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
        }
        catch (DbUpdateException)
        {
            throw;
        }
    }
}