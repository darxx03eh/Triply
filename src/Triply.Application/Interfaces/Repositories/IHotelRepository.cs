using Sieve.Models;
using Triply.Application.Interfaces.Repositories.General;
using Triply.Domain.Entities;

namespace Triply.Application.Interfaces.Repositories;

/// <summary>Defines the persistence operations for hotel.</summary>
public interface IHotelRepository : IGenericRepository<Hotel>
{
    /// <summary>Checks whether the locations exists.</summary>
    Task<bool> IsLocationsExistsAsync(decimal? latitude, decimal? longitude, CancellationToken cancellationToken);
    /// <summary>Checks whether the hotel identifier exists.</summary>
    Task<bool> IsHotelIdExistsAsync(Guid hotelId, CancellationToken cancellationToken = default);
    /// <summary>Gets the thumbnails.</summary>
    Task<Dictionary<Guid, string>> GetThumbnailsAsync(IEnumerable<Guid> hotelIds,
        CancellationToken cancellationToken = default);
    /// <summary>Gets the rooms count.</summary>
    Task<Dictionary<Guid, int>> GetRoomsCountAsync(IEnumerable<Guid> hotelIds,
        CancellationToken cancellationToken = default);
    /// <summary>Checks whether the hotel exists.</summary>
    Task<bool> IsHotelExistsAsync(string name, Guid cityId, CancellationToken cancellationToken);
    /// <summary>Checks whether the location exists exclude identifier.</summary>
    Task<bool> IsLocationExistsExcludeIdAsync(decimal? latitude, decimal? longitude, Guid hotelId,
        CancellationToken cancellationToken = default);
    /// <summary>Checks whether the hotel exists exclude identifier.</summary>
    Task<bool> IsHotelExistsExcludeId(string name, Guid cityId, Guid hotelId,
        CancellationToken cancellationToken = default);
    /// <summary>Gets the hotel by its identifier including the images.</summary>
    Task<Hotel?> GetByIdWithImagesAsync(Guid id, CancellationToken cancellationToken = default);
    /// <summary>Gets a paginated list of hotels.</summary>
    Task<(List<Hotel> Hotels, int TotalCount)> GetPagedAsync(SieveModel sieveModel, 
        bool isAdmin,
        CancellationToken cancellationToken = default);
    /// <summary>Softs the delete rooms.</summary>
    Task SoftDeleteRoomsAsync(Guid hotelId, CancellationToken cancellationToken = default);
    /// <summary>Sets the original row version.</summary>
    void SetOriginalRowVersion(Hotel hotel, byte[] rowVersion);

    /// <summary>Gets the review stats.</summary>
    Task<(decimal? AverageRating, int ReviewsCount)> GetReviewStatsAsync(Guid hotelId,
        CancellationToken cancellationToken = default);
}