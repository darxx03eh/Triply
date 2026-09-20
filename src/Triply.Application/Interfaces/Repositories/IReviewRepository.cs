using Sieve.Models;
using Triply.Application.Interfaces.Repositories.General;
using Triply.Domain.Entities;

namespace Triply.Application.Interfaces.Repositories;

/// <summary>Defines the persistence operations for review.</summary>
public interface IReviewRepository : IGenericRepository<Review>
{
    /// <summary>Gets the review by its identifier including the user.</summary>
    Task<Review?> GetByIdWithUserAsync(Guid reviewId, CancellationToken cancellationToken = default);

    /// <summary>Gets the hotel reviews.</summary>
    Task<(List<Review> Reviews, int TotalCount)> GetHotelReviewsAsync(Guid hotelId, SieveModel sieveModel,
        CancellationToken cancellationToken = default);

    /// <summary>Checks whether the review exists.</summary>
    Task<bool> IsReviewExistsAsync(Guid hotelId, Guid userId, CancellationToken cancellationToken = default);
}