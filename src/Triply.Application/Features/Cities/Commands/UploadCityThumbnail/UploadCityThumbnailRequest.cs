using Microsoft.AspNetCore.Http;

namespace Triply.Application.Features.Cities.Commands.UploadCityThumbnail;

/// <summary>Request payload used to upload city thumbnail.</summary>
public class UploadCityThumbnailRequest
{
    /// <summary>Gets or sets the file.</summary>
    public IFormFile File { get; set; } = default!;
}
