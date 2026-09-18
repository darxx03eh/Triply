using Triply.Application.Common.Models;
using Triply.Application.DTOs.Cities;
using Triply.Application.Features.Cities.Commands.CreateCity;
using Triply.Application.Features.Cities.Commands.UpdateCity;
using Triply.Application.Features.Cities.Commands.UploadCityThumbnail;
using Triply.Application.Features.Cities.Queries.GetCitiesRequest;
using Triply.Domain.Results;

namespace Triply.Application.Interfaces.Services;

public interface ICityService
{
    Task<Result<CityResponse>> CreateAsync(CreateCityRequest request, CancellationToken cancellationToken = default);
    Task<Result<CityResponse>> GetByIdAsync(Guid cityId, CancellationToken cancellationToken = default);

    Task<Result<PagedResult<CityResponse>>> GetPagedAsync(GetCitiesRequest request,
        bool isAdmin,
        CancellationToken cancellationToken = default);

    Task<Result<CityResponse>> UpdateAsync(Guid cityId, UpdateCityRequest request,
        CancellationToken cancellationToken = default);

    Task<Result<bool>> DeleteAsync(Guid cityId, CancellationToken cancellationToken = default);

    Task<Result<CityResponse>> UploadThumbnailAsync(Guid cityId, UploadCityThumbnailRequest request,
        CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteThumbnailAsync(Guid cityId, CancellationToken cancellationToken = default);
}