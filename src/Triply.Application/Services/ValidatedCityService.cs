using FluentValidation;
using Triply.Application.Common.Models;
using Triply.Application.DTOs.Cities;
using Triply.Application.Extensions;
using Triply.Application.Features.Cities.Commands.CreateCity;
using Triply.Application.Features.Cities.Commands.UpdateCity;
using Triply.Application.Features.Cities.Commands.UploadCityThumbnail;
using Triply.Application.Features.Cities.Queries.GetCitiesRequest;
using Triply.Application.Interfaces.Services;
using Triply.Domain.Results;

namespace Triply.Application.Services;

/// <summary>Gets or sets the validated city service.</summary>
/// <summary>Validates the requests before delegating to the city service.</summary>
public class ValidatedCityService(
    ICityService inner,
    IEnumerable<IValidator<CreateCityRequest>> createValidators,
    IEnumerable<IValidator<UpdateCityRequest>> updateValidators,
    IEnumerable<IValidator<GetCitiesRequest>> getCitiesValidators,
    IEnumerable<IValidator<UploadCityThumbnailRequest>> uploadThumbnailValidators) : ICityService
{
    /// <summary>Creates a new city.</summary>
    public async Task<Result<CityResponse>> CreateAsync(CreateCityRequest request,
        CancellationToken cancellationToken = default)
    {
        await createValidators.ValidateAndThrowAsync(request, cancellationToken);
        return await inner.CreateAsync(request, cancellationToken);
    }

    /// <summary>Gets the city by its identifier.</summary>
    public async Task<Result<CityResponse>> GetByIdAsync(Guid cityId, CancellationToken cancellationToken = default)
        => await inner.GetByIdAsync(cityId, cancellationToken);


    /// <summary>Gets a paginated list of cities.</summary>
    public async Task<Result<PagedResult<CityResponse>>> GetPagedAsync(GetCitiesRequest request,
        bool isAdmin,
        CancellationToken cancellationToken = default)
    {
        await getCitiesValidators.ValidateAndThrowAsync(request, cancellationToken);
        return await  inner.GetPagedAsync(request, isAdmin, cancellationToken);
    }

    /// <summary>Updates an existing city.</summary>
    public async Task<Result<CityResponse>> UpdateAsync(Guid cityId, UpdateCityRequest request,
        CancellationToken cancellationToken = default)
    {
        await updateValidators.ValidateAndThrowAsync(request, cancellationToken);
        return await inner.UpdateAsync(cityId, request, cancellationToken);
    }

    /// <summary>Deletes the city.</summary>
    public async Task<Result<bool>> DeleteAsync(Guid cityId, CancellationToken cancellationToken = default)
        => await inner.DeleteAsync(cityId, cancellationToken);

    /// <summary>Uploads the thumbnail.</summary>
    public async Task<Result<CityResponse>> UploadThumbnailAsync(Guid cityId, UploadCityThumbnailRequest request,
        CancellationToken cancellationToken = default)
    {
        await uploadThumbnailValidators.ValidateAndThrowAsync(request, cancellationToken);
        return await inner.UploadThumbnailAsync(cityId, request, cancellationToken);
    }

    /// <summary>Deletes the thumbnail.</summary>
    public async Task<Result<bool>> DeleteThumbnailAsync(Guid cityId, CancellationToken cancellationToken = default)
        => await inner.DeleteThumbnailAsync(cityId, cancellationToken);
}