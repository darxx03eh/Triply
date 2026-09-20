using Microsoft.EntityFrameworkCore;
using Triply.Application.Interfaces.Repositories;
using Triply.Domain.Entities;
using Triply.Infrastructure.Db;
using Triply.Infrastructure.Repositories.General;

namespace Triply.Infrastructure.Repositories;

/// <summary>Gets or sets the amenity repository.</summary>
/// <summary>Persistence operations for amenity.</summary>
public class AmenityRepository(TriplyDbContext context) : GenericRepository<Amenity>(context),
    IAmenityRepository
{
    /// <summary>Gets the all ordered.</summary>
    public async Task<List<Amenity>> GetAllOrderedAsync(CancellationToken cancellationToken = default)
        => await context.Amenities
            .AsNoTracking()
            .OrderBy(a => a.Name)
            .ToListAsync(cancellationToken);

    /// <summary>Gets the amenity by its hotel identifier.</summary>
    public async Task<List<Amenity>> GetByHotelIdAsync(Guid hotelId, CancellationToken cancellationToken = default)
        => await context.HotelAmenities
            .AsNoTracking()
            .Where(ha => ha.HotelId == hotelId)
            .Select(ha => ha.Amenity)
            .OrderBy(a => a.Name)
            .ToListAsync(cancellationToken);

    /// <summary>Checks whether the amenity exists.</summary>
    public async Task<bool> IsAmenityExistsAsync(string name, CancellationToken cancellationToken = default)
        => await context.Amenities.AnyAsync(a => a.Name.ToUpper() == name.ToUpper(), cancellationToken);

    /// <summary>Checks whether the amenity exists exclude identifier.</summary>
    public async Task<bool> IsAmenityExistsExcludeIdAsync(string name, Guid amenityId,
        CancellationToken cancellationToken = default)
        => await context.Amenities.AnyAsync(a =>
            a.Name.ToUpper() == name.ToUpper() && a.AmenityId != amenityId, cancellationToken);

    /// <summary>Counts the existing.</summary>
    public async Task<int> CountExistingAsync(IEnumerable<Guid> amenityIds,
        CancellationToken cancellationToken = default)
    {
        var ids = amenityIds.Distinct().ToList();
        return await context.Amenities.CountAsync(a => ids.Contains(a.AmenityId), cancellationToken);
    }

    /// <summary>Replaces the hotel amenities.</summary>
    public async Task ReplaceHotelAmenitiesAsync(Guid hotelId, IEnumerable<Guid> amenityIds,
        CancellationToken cancellationToken = default)
    {
        var newIds = amenityIds.ToHashSet();
        var current = await context.HotelAmenities
            .Where(ha => ha.HotelId == hotelId)
            .ToListAsync(cancellationToken);

        context.HotelAmenities.RemoveRange(current.Where(ha => !newIds.Contains(ha.AmenityId)));

        var currentIds = current.Select(ha => ha.AmenityId).ToHashSet();
        await context.HotelAmenities.AddRangeAsync(newIds
            .Where(id => !currentIds.Contains(id))
            .Select(id => new HotelAmenities { HotelId = hotelId, AmenityId = id }), cancellationToken);
    }
}
