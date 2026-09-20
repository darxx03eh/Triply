using Sieve.Models;
using Triply.Application.Interfaces.Repositories.General;
using Triply.Domain.Entities;

namespace Triply.Application.Interfaces.Repositories;

/// <summary>Defines the persistence operations for city.</summary>
public interface ICityRepository : IGenericRepository<City>
{
    /// <summary>Gets a paginated list of cities.</summary>
    Task<(List<City> Cities, int TotalCount)> GetPagedAsync(SieveModel sieveModel,
        bool isAdmin,
        CancellationToken cancellationToken = default);

    /// <summary>Checks whether the city exists.</summary>
    Task<bool> IsCityExistsAsync(string name, string country, CancellationToken cancellationToken = default);
    /// <summary>Checks whether the city identifier exists.</summary>
    Task<bool> IsCityIdExistsAsync(Guid cityId, CancellationToken cancellationToken = default);

    /// <summary>Checks whether the city exists exclude identifier.</summary>
    Task<bool> IsCityExistsExcludeId(string name, string country, Guid cityId,
        CancellationToken cancellationToken = default);
    /// <summary>Gets the hotels count.</summary>
    Task<Dictionary<Guid, int>> GetHotelsCountAsync(IEnumerable<Guid> cityIds,
        CancellationToken cancellationToken = default);
    /// <summary>Sets the original row version.</summary>
    void SetOriginalRowVersion(City city, byte[] rowVersion);
}