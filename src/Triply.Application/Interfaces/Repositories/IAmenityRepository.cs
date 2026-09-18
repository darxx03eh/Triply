using Triply.Application.Interfaces.Repositories.General;
using Triply.Domain.Entities;

namespace Triply.Application.Interfaces.Repositories;

public interface IAmenityRepository : IGenericRepository<Amenity>
{
    Task<List<Amenity>> GetAllOrderedAsync(CancellationToken cancellationToken = default);
    Task<List<Amenity>> GetByHotelIdAsync(Guid hotelId, CancellationToken cancellationToken = default);
    Task<bool> IsAmenityExistsAsync(string name, CancellationToken cancellationToken = default);

    Task<bool> IsAmenityExistsExcludeIdAsync(string name, Guid amenityId,
        CancellationToken cancellationToken = default);
    Task<int> CountExistingAsync(IEnumerable<Guid> amenityIds, CancellationToken cancellationToken = default);

    Task ReplaceHotelAmenitiesAsync(Guid hotelId, IEnumerable<Guid> amenityIds,
        CancellationToken cancellationToken = default);
}
