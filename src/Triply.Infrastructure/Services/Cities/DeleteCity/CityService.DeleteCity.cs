using Triply.Domain.Results;
using Triply.Domain.Results.Enums;

namespace Triply.Infrastructure.Services.Cities;

public partial class CityService
{
    public async Task<Result<bool>> DeleteAsync(Guid cityId, CancellationToken cancellationToken = default)
    {
        var city = await cityRepository.GetByIdAsync(cityId, cancellationToken);
        if (city is null || city.IsDeleted)
            return Result<bool>.Failure(
                "CITY_NOT_FOUND", $"The requested city with id: {cityId.ToString()} was not found.", 
                ResultErrorType.NotFound);

        city.IsDeleted = true;
        city.ModifiedAt = DateTime.UtcNow;
        await cityRepository.UpdateAsync(city);
        await cityRepository.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true, 
            ResultSuccessType.NoContent, 
            new ("CITY_DELETED", "City deleted successfully."));
    }
}