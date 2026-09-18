using Microsoft.AspNetCore.Http;

namespace Triply.Application.Features.Cities.Commands.UploadCityThumbnail;

public class UploadCityThumbnailRequest
{
    public IFormFile File { get; set; } = default!;
}
