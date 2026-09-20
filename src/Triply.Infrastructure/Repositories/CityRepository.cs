using Microsoft.EntityFrameworkCore;
using Sieve.Models;
using Sieve.Services;
using Triply.Application.Interfaces.Repositories;
using Triply.Domain.Entities;
using Triply.Infrastructure.Db;
using Triply.Infrastructure.Repositories.General;

namespace Triply.Infrastructure.Repositories;

/// <summary>Gets or sets the city repository.</summary>
/// <summary>Persistence operations for city.</summary>
public class CityRepository(TriplyDbContext context, ISieveProcessor sieveProcessor) 
    : GenericRepository<City>(context),
    ICityRepository
{
    /// <summary>Gets a paginated list of cities.</summary>
    public async Task<(List<City> Cities, int TotalCount)> GetPagedAsync(
        SieveModel sieveModel, 
        bool isAdmin,
        CancellationToken cancellationToken = default)
    {
        var query = context.Cities.AsQueryable();
        if (isAdmin) query = query.IgnoreQueryFilters();
        query = sieveProcessor.Apply(sieveModel, query, applyPagination: false);
        var totalCount = await query.CountAsync(cancellationToken);
        query = sieveProcessor.Apply(sieveModel, query, applyFiltering: false, applySorting: false);
        var cities = await query.ToListAsync(cancellationToken);

        return (cities, totalCount);
    }

    /// <summary>Checks whether the city exists.</summary>
    public async Task<bool> IsCityExistsAsync(string name, string country,
        CancellationToken cancellationToken = default)
        => await context.Cities.AnyAsync(c =>
            c.Name.ToUpper() == name.ToUpper() 
            && c.Country.ToUpper() == country.ToUpper()
            , cancellationToken);

    /// <summary>Checks whether the city identifier exists.</summary>
    public async Task<bool> IsCityIdExistsAsync(Guid cityId, CancellationToken cancellationToken = default)
        => await context.Cities.AnyAsync(c => c.CityId == cityId, cancellationToken);

    /// <summary>Checks whether the city exists exclude identifier.</summary>
    public async Task<bool> IsCityExistsExcludeId(string name, string country, Guid cityId,
        CancellationToken cancellationToken = default)
        => await context.Cities.AnyAsync(c =>
                (c.Name.ToUpper() == name.ToUpper()
                 && c.Country.ToUpper() == country.ToUpper()) 
                && c.CityId != cityId
            , cancellationToken);

    /// <summary>Gets the hotels count.</summary>
    public async Task<Dictionary<Guid, int>> GetHotelsCountAsync(IEnumerable<Guid> cityIds,
        CancellationToken cancellationToken = default)
        => await context.Hotels
            .Where(h => cityIds.Contains(h.CityId))
            .GroupBy(h => h.CityId)
            .Select(g => new { CityId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.CityId, x => x.Count, cancellationToken);

    /// <summary>Sets the original row version.</summary>
    public void SetOriginalRowVersion(City city, byte[] rowVersion)
        => context.Entry(city).Property(c => c.RowVersion).OriginalValue = rowVersion;
}