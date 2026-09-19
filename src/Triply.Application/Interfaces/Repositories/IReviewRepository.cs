using Sieve.Models;
using Triply.Application.Interfaces.Repositories.General;
using Triply.Domain.Entities;

namespace Triply.Application.Interfaces.Repositories;

public interface IReviewRepository : IGenericRepository<Review>
{
    Task<Review?> GetByIdWithUserAsync(Guid reviewId, CancellationToken cancellationToken = default);

    Task<(List<Review> Reviews, int TotalCount)> GetHotelReviewsAsync(Guid hotelId, SieveModel sieveModel,
        CancellationToken cancellationToken = default);

    Task<bool> IsReviewExistsAsync(Guid hotelId, Guid userId, CancellationToken cancellationToken = default);
}