using Microsoft.EntityFrameworkCore;
using Sieve.Models;
using Sieve.Services;
using Triply.Application.Interfaces.Repositories;
using Triply.Domain.Entities;
using Triply.Infrastructure.Db;
using Triply.Infrastructure.Repositories.General;

namespace Triply.Infrastructure.Repositories;

/// <summary>Gets or sets the review repository.</summary>
/// <summary>Persistence operations for review.</summary>
public class ReviewRepository(TriplyDbContext context, ISieveProcessor sieveProcessor)
    : GenericRepository<Review>(context),
        IReviewRepository
{
    /// <summary>Gets the review by its identifier including the user.</summary>
    public async Task<Review?> GetByIdWithUserAsync(Guid reviewId, CancellationToken cancellationToken = default)
        => await context.Reviews
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.ReviewId == reviewId, cancellationToken);

    /// <summary>Gets the hotel reviews.</summary>
    public async Task<(List<Review> Reviews, int TotalCount)> GetHotelReviewsAsync(Guid hotelId, SieveModel sieveModel, CancellationToken cancellationToken = default)
    {
        var query = context.Reviews
            .AsNoTracking()
            .Include(r => r.User)
            .Where(r => r.HotelId == hotelId);
        
        if (string.IsNullOrWhiteSpace(sieveModel.Sorts))
            query = query.OrderByDescending(r => r.CreatedAt);
        
        query = sieveProcessor.Apply(sieveModel, query, applyPagination: false);
        var totalCount = await  query.CountAsync(cancellationToken);
        
        query = sieveProcessor.Apply(sieveModel, query, applyFiltering: false, applySorting: false);
        var reviews = await query.ToListAsync(cancellationToken);
        
        return (reviews, totalCount);
    }

    /// <summary>Checks whether the review exists.</summary>
    public async Task<bool> IsReviewExistsAsync(Guid hotelId, Guid userId,
        CancellationToken cancellationToken = default)
        => await context.Reviews.AnyAsync(
            r => r.HotelId == hotelId
                 && r.UserId == userId,
            cancellationToken);
}