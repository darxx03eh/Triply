using Triply.Application.Interfaces.Repositories.General;
using Triply.Domain.Entities;

namespace Triply.Application.Interfaces.Repositories;

/// <summary>Defines the persistence operations for amenity.</summary>
public interface IAmenityRepository : IGenericRepository<Amenity>
{
    /// <summary>Gets the all ordered.</summary>
    Task<List<Amenity>> GetAllOrderedAsync(CancellationToken cancellationToken = default);
    /// <summary>Gets the amenity by its hotel identifier.</summary>
    Task<List<Amenity>> GetByHotelIdAsync(Guid hotelId, CancellationToken cancellationToken = default);
    /// <summary>Checks whether the amenity exists.</summary>
    Task<bool> IsAmenityExistsAsync(string name, CancellationToken cancellationToken = default);

    /// <summary>Checks whether the amenity exists exclude identifier.</summary>
    Task<bool> IsAmenityExistsExcludeIdAsync(string name, Guid amenityId,
        CancellationToken cancellationToken = default);
    /// <summary>Counts the existing.</summary>
    Task<int> CountExistingAsync(IEnumerable<Guid> amenityIds, CancellationToken cancellationToken = default);

    /// <summary>Replaces the hotel amenities.</summary>
    Task ReplaceHotelAmenitiesAsync(Guid hotelId, IEnumerable<Guid> amenityIds,
        CancellationToken cancellationToken = default);
}
