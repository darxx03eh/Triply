using Microsoft.EntityFrameworkCore;
using Triply.Application.Interfaces.Repositories;
using Triply.Domain.Entities;
using Triply.Infrastructure.Db;
using Triply.Infrastructure.Repositories.General;

namespace Triply.Infrastructure.Repositories;

/// <summary>Gets or sets the attraction repository.</summary>
/// <summary>Persistence operations for attraction.</summary>
public class AttractionRepository(TriplyDbContext context) : GenericRepository<Attraction>(context),
    IAttractionRepository
{
    /// <summary>Gets the attraction by its hotel identifier.</summary>
    public async Task<List<Attraction>> GetByHotelIdAsync(Guid hotelId, CancellationToken cancellationToken = default)
        => await context.Attractions
            .AsNoTracking()
            .Where(a => a.HotelId == hotelId)
            .OrderBy(a => a.DistanceKm)
            .ThenBy(a => a.Name)
            .ToListAsync(cancellationToken);

}