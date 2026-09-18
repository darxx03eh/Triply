using Sieve.Models;
using Triply.Application.Interfaces.Repositories.General;
using Triply.Domain.Entities;

namespace Triply.Application.Interfaces.Repositories;

public interface IHotelRepository : IGenericRepository<Hotel>
{
    Task<bool> IsLocationsExistsAsync(decimal? latitude, decimal? longitude, CancellationToken cancellationToken);
    Task<bool> IsHotelExistsAsync(string name, Guid cityId, CancellationToken cancellationToken);
    Task<bool> IsLocationExistsExcludeIdAsync(decimal? latitude, decimal? longitude, Guid hotelId,
        CancellationToken cancellationToken = default);
    Task<bool> IsHotelExistsExcludeId(string name, Guid cityId, Guid hotelId,
        CancellationToken cancellationToken = default);
    Task<Hotel?> GetByIdWithImagesAsync(Guid id, CancellationToken cancellationToken = default);
    Task<(List<Hotel> Hotels, int TotalCount)> GetPagedAsync(SieveModel sieveModel, 
        bool isAdmin,
        CancellationToken cancellationToken = default);
    void SetOriginalRowVersion(Hotel hotel, byte[] rowVersion);
}