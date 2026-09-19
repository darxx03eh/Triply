using Microsoft.EntityFrameworkCore;
using Triply.Application.Interfaces.Repositories;
using Triply.Domain.Entities;
using Triply.Infrastructure.Db;
using Triply.Infrastructure.Repositories.General;

namespace Triply.Infrastructure.Repositories;

public class AttractionRepository(TriplyDbContext context) : GenericRepository<Attraction>(context),
    IAttractionRepository
{
    public async Task<List<Attraction>> GetByHotelIdAsync(Guid hotelId, CancellationToken cancellationToken = default)
        => await context.Attractions
            .AsNoTracking()
            .Where(a => a.HotelId == hotelId)
            .OrderBy(a => a.DistanceKm)
            .ThenBy(a => a.Name)
            .ToListAsync(cancellationToken);

}