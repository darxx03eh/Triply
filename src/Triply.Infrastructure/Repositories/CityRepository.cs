using Microsoft.EntityFrameworkCore;
using Sieve.Models;
using Sieve.Services;
using Triply.Application.Interfaces.Repositories;
using Triply.Domain.Entities;
using Triply.Infrastructure.Db;
using Triply.Infrastructure.Repositories.General;

namespace Triply.Infrastructure.Repositories;

public class CityRepository(TriplyDbContext context, ISieveProcessor sieveProcessor) 
    : GenericRepository<City>(context),
    ICityRepository
{
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

    public async Task<bool> IsCityExistsAsync(string name, string country,
        CancellationToken cancellationToken = default)
        => await context.Cities.AnyAsync(c =>
            c.Name.ToUpper() == name.ToUpper() 
            && c.Country.ToUpper() == country.ToUpper()
            , cancellationToken);

    public async Task<bool> IsCityExistsExcludeId(string name, string country, Guid cityId,
        CancellationToken cancellationToken = default)
        => await context.Cities.AnyAsync(c =>
                (c.Name.ToUpper() == name.ToUpper()
                 && c.Country.ToUpper() == country.ToUpper()) 
                && c.CityId != cityId
            , cancellationToken);

    public void SetOriginalRowVersion(City city, byte[] rowVersion)
        => context.Entry(city).Property(c => c.RowVersion).OriginalValue = rowVersion;
}