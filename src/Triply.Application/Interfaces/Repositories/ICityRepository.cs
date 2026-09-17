using Sieve.Models;
using Triply.Application.Interfaces.Repositories.General;
using Triply.Domain.Entities;

namespace Triply.Application.Interfaces.Repositories;

public interface ICityRepository : IGenericRepository<City>
{
    Task<(List<City> Cities, int TotalCount)> GetPagedAsync(SieveModel sieveModel,
        bool isAdmin,
        CancellationToken cancellationToken = default);

    Task<bool> IsCityExistsAsync(string name, string country, CancellationToken cancellationToken = default);

    Task<bool> IsCityExistsExcludeId(string name, string country, Guid cityId,
        CancellationToken cancellationToken = default);
    void SetOriginalRowVersion(City city, byte[] rowVersion);
}