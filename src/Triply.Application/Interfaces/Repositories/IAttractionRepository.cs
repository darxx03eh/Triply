using Triply.Application.Interfaces.Repositories.General;
using Triply.Domain.Entities;

namespace Triply.Application.Interfaces.Repositories;

/// <summary>Defines the persistence operations for attraction.</summary>
public interface IAttractionRepository : IGenericRepository<Attraction>
{
    /// <summary>Gets the attraction by its hotel identifier.</summary>
    Task<List<Attraction>> GetByHotelIdAsync(Guid hotelId, CancellationToken cancellationToken = default);
}