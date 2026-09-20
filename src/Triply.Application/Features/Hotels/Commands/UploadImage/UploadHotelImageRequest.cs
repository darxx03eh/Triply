using Microsoft.AspNetCore.Http;

namespace Triply.Application.Features.Hotels.Commands.UploadImage;

/// <summary>Request payload used to upload hotel image.</summary>
public class UploadHotelImageRequest
{
    /// <summary>Gets or sets the file.</summary>
    public IFormFile File { get; set; } = default!;
    /// <summary>Position of the image in the gallery. When omitted, the image is appended at the end.</summary>
    public short? DisplayOrder { get; set; }
}
