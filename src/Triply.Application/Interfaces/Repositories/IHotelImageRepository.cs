using Triply.Application.Interfaces.Repositories.General;
using Triply.Domain.Entities;

namespace Triply.Application.Interfaces.Repositories;

public interface IHotelImageRepository : IGenericRepository<HotelImage>
{
    Task<short> GetNextDisplayOrderAsync(Guid hotelId, CancellationToken cancellationToken = default);
    Task<List<HotelImage>> GetByHotelIdAsync(Guid hotelId, CancellationToken cancellationToken = default);
}
