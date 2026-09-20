using Triply.Application.Interfaces.Repositories.General;
using Triply.Domain.Entities;

namespace Triply.Application.Interfaces.Repositories;

/// <summary>Defines the persistence operations for hotel image.</summary>
public interface IHotelImageRepository : IGenericRepository<HotelImage>
{
    /// <summary>Gets the next display order.</summary>
    Task<short> GetNextDisplayOrderAsync(Guid hotelId, CancellationToken cancellationToken = default);
    /// <summary>Gets the hotel image by its hotel identifier.</summary>
    Task<List<HotelImage>> GetByHotelIdAsync(Guid hotelId, CancellationToken cancellationToken = default);
}
