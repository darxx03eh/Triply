using Microsoft.EntityFrameworkCore;
using Triply.Application.Interfaces.Repositories;
using Triply.Domain.Entities;
using Triply.Infrastructure.Db;
using Triply.Infrastructure.Repositories.General;

namespace Triply.Infrastructure.Repositories;

public class HotelImageRepository(TriplyDbContext context) : GenericRepository<HotelImage>(context),
    IHotelImageRepository
{
    public async Task<short> GetNextDisplayOrderAsync(Guid hotelId, CancellationToken cancellationToken = default)
    {
        short? max = await context.HotelImages
            .Where(i => i.HotelId == hotelId)
            .MaxAsync(i => (short?)i.DisplayOrder, cancellationToken);
        return (short)((max ?? 0) + 1);
    }

    public async Task<List<HotelImage>> GetByHotelIdAsync(Guid hotelId, CancellationToken cancellationToken = default)
        => await context.HotelImages
            .AsNoTracking()
            .Where(i => i.HotelId == hotelId)
            .OrderBy(i => i.DisplayOrder)
            .ToListAsync(cancellationToken);
}
