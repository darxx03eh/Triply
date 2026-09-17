using FluentValidation;
using Triply.Application.Common.Models;
using Triply.Application.DTOs.Cities;
using Triply.Application.Extensions;
using Triply.Application.Features.Cities.Commands.CreateCity;
using Triply.Application.Features.Cities.Commands.UpdateCity;
using Triply.Application.Features.Cities.Queries.GetCitiesRequest;
using Triply.Application.Interfaces.Services;
using Triply.Domain.Results;

namespace Triply.Application.Services;

public class ValidatedCityService(
    ICityService inner,
    IEnumerable<IValidator<CreateCityRequest>> createValidators,
    IEnumerable<IValidator<UpdateCityRequest>> updateValidators,
    IEnumerable<IValidator<GetCitiesRequest>> getCitiesValidators) : ICityService
{
    public async Task<Result<CityResponse>> CreateAsync(CreateCityRequest request,
        CancellationToken cancellationToken = default)
    {
        await createValidators.ValidateAndThrowAsync(request, cancellationToken);
        return await inner.CreateAsync(request, cancellationToken);
    }

    public async Task<Result<CityResponse>> GetByIdAsync(Guid cityId, CancellationToken cancellationToken = default)
        => await inner.GetByIdAsync(cityId, cancellationToken);


    public async Task<Result<PagedResult<CityResponse>>> GetPagedAsync(GetCitiesRequest request,
        bool isAdmin,
        CancellationToken cancellationToken = default)
    {
        await getCitiesValidators.ValidateAndThrowAsync(request, cancellationToken);
        return await  inner.GetPagedAsync(request, isAdmin, cancellationToken);
    }

    public async Task<Result<CityResponse>> UpdateAsync(Guid cityId, UpdateCityRequest request,
        CancellationToken cancellationToken = default)
    {
        await updateValidators.ValidateAndThrowAsync(request, cancellationToken);
        return await inner.UpdateAsync(cityId, request, cancellationToken);
    }

    public async Task<Result<bool>> DeleteAsync(Guid cityId, CancellationToken cancellationToken = default)
        => await inner.DeleteAsync(cityId, cancellationToken);
}