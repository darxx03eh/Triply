using Triply.Application.Interfaces.Repositories.General;
using Triply.Domain.Entities;

namespace Triply.Application.Interfaces.Repositories;

public interface IAttractionRepository : IGenericRepository<Attraction>
{
    Task<List<Attraction>> GetByHotelIdAsync(Guid hotelId, CancellationToken cancellationToken = default);
}