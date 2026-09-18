using Microsoft.EntityFrameworkCore;
using Sieve.Models;
using Sieve.Services;
using Triply.Application.Interfaces.Repositories;
using Triply.Domain.Entities;
using Triply.Infrastructure.Db;
using Triply.Infrastructure.Repositories.General;

namespace Triply.Infrastructure.Repositories;

public class HotelRepository(TriplyDbContext context, ISieveProcessor sieveProcessor) 
    : GenericRepository<Hotel>(context),
    IHotelRepository
{
    public async Task<bool> IsLocationsExistsAsync(decimal? latitude, decimal? longitude,
        CancellationToken cancellationToken)
        => await context.Hotels
            .AnyAsync(h => h.Latitude == latitude && h.Longitude == longitude, cancellationToken);

    public async Task<bool> IsHotelExistsAsync(string name, Guid cityId, CancellationToken cancellationToken)
        => await context.Hotels.AnyAsync(h => h.Name.ToUpper() == name.ToUpper() && h.CityId == cityId,
            cancellationToken);

    public async Task<bool> IsHotelExistsExcludeId(string name, Guid cityId, Guid hotelId,
        CancellationToken cancellationToken = default)
        => await context.Hotels.AnyAsync(h =>
                (h.Name.ToUpper() == name.ToUpper() && h.CityId == cityId)
                && h.HotelId != hotelId,
            cancellationToken);

    public async Task<Hotel?> GetByIdWithImagesAsync(Guid id, CancellationToken cancellationToken = default)
    => await context.Hotels
        .Include(h => h.City)
        .Include(h => h.Images.OrderBy(i => i.DisplayOrder))
        .FirstOrDefaultAsync(h => h.HotelId == id, cancellationToken);

    public async Task<(List<Hotel> Hotels, int TotalCount)> GetPagedAsync(SieveModel sieveModel,
        bool isAdmin,
        CancellationToken cancellationToken = default)
    {
        var query = context.Hotels.Include(h => h.City).AsQueryable();

        if (isAdmin) query = query.IgnoreQueryFilters();

        query = sieveProcessor.Apply(sieveModel, query, applyPagination: false);
        var totalCount = await query.CountAsync(cancellationToken);

        query = sieveProcessor.Apply(sieveModel, query, applyFiltering: false, applySorting: false);
        var hotels = await query.ToListAsync(cancellationToken);

        return (hotels, totalCount);
    }

    public void SetOriginalRowVersion(Hotel hotel, byte[] rowVersion)
        => context.Entry(hotel).Property(c => c.RowVersion).OriginalValue = rowVersion;
}