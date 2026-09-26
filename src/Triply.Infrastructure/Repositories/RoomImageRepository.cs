using Microsoft.EntityFrameworkCore;
using Triply.Application.Interfaces.Repositories;
using Triply.Domain.Entities;
using Triply.Infrastructure.Db;
using Triply.Infrastructure.Repositories.General;

namespace Triply.Infrastructure.Repositories;

/// <summary>Gets or sets the hotel image repository.</summary>
/// <summary>Persistence operations for hotel image.</summary>
public class RoomImageRepository(TriplyDbContext context) : GenericRepository<RoomImage>(context),
    IRoomImageRepository
{
    /// <summary>Gets the next display order.</summary>
    public async Task<short> GetNextDisplayOrderAsync(Guid roomId, CancellationToken cancellationToken = default)
    {
        short? max = await context.RoomImages
            .Where(i => i.RoomId == roomId)
            .MaxAsync(i => (short?)i.DisplayOrder, cancellationToken);
        return (short)((max ?? 0) + 1);
    }

    /// <summary>Gets the hotel image by its room identifier.</summary>
    public async Task<List<RoomImage>> GetByRoomIdAsync(Guid roomId, CancellationToken cancellationToken = default)
        => await context.RoomImages
            .AsNoTracking()
            .Where(i => i.RoomId == roomId)
            .OrderBy(i => i.DisplayOrder)
            .ToListAsync(cancellationToken);
}