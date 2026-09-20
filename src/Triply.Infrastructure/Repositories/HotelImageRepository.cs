using Microsoft.EntityFrameworkCore;
using Triply.Application.Interfaces.Repositories;
using Triply.Domain.Entities;
using Triply.Infrastructure.Db;
using Triply.Infrastructure.Repositories.General;

namespace Triply.Infrastructure.Repositories;

/// <summary>Gets or sets the hotel image repository.</summary>
/// <summary>Persistence operations for hotel image.</summary>
public class HotelImageRepository(TriplyDbContext context) : GenericRepository<HotelImage>(context),
    IHotelImageRepository
{
    /// <summary>Gets the next display order.</summary>
    public async Task<short> GetNextDisplayOrderAsync(Guid hotelId, CancellationToken cancellationToken = default)
    {
        short? max = await context.HotelImages
            .Where(i => i.HotelId == hotelId)
            .MaxAsync(i => (short?)i.DisplayOrder, cancellationToken);
        return (short)((max ?? 0) + 1);
    }

    /// <summary>Gets the hotel image by its hotel identifier.</summary>
    public async Task<List<HotelImage>> GetByHotelIdAsync(Guid hotelId, CancellationToken cancellationToken = default)
        => await context.HotelImages
            .AsNoTracking()
            .Where(i => i.HotelId == hotelId)
            .OrderBy(i => i.DisplayOrder)
            .ToListAsync(cancellationToken);
}
