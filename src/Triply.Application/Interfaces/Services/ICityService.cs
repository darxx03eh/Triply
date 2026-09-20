using Triply.Application.Common.Models;
using Triply.Application.DTOs.Cities;
using Triply.Application.Features.Cities.Commands.CreateCity;
using Triply.Application.Features.Cities.Commands.UpdateCity;
using Triply.Application.Features.Cities.Commands.UploadCityThumbnail;
using Triply.Application.Features.Cities.Queries.GetCitiesRequest;
using Triply.Domain.Results;

namespace Triply.Application.Interfaces.Services;

/// <summary>Defines the city operations.</summary>
public interface ICityService
{
    /// <summary>Creates a new city.</summary>
    Task<Result<CityResponse>> CreateAsync(CreateCityRequest request, CancellationToken cancellationToken = default);
    /// <summary>Gets the city by its identifier.</summary>
    Task<Result<CityResponse>> GetByIdAsync(Guid cityId, CancellationToken cancellationToken = default);

    /// <summary>Gets a paginated list of cities.</summary>
    Task<Result<PagedResult<CityResponse>>> GetPagedAsync(GetCitiesRequest request,
        bool isAdmin,
        CancellationToken cancellationToken = default);

    /// <summary>Updates an existing city.</summary>
    Task<Result<CityResponse>> UpdateAsync(Guid cityId, UpdateCityRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>Deletes the city.</summary>
    Task<Result<bool>> DeleteAsync(Guid cityId, CancellationToken cancellationToken = default);

    /// <summary>Uploads the thumbnail.</summary>
    Task<Result<CityResponse>> UploadThumbnailAsync(Guid cityId, UploadCityThumbnailRequest request,
        CancellationToken cancellationToken = default);
    /// <summary>Deletes the thumbnail.</summary>
    Task<Result<bool>> DeleteThumbnailAsync(Guid cityId, CancellationToken cancellationToken = default);
}