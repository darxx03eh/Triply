using Microsoft.AspNetCore.Http;

namespace Triply.Application.Features.Rooms.Commands.UploadImage;

/// <summary>Request payload used to upload room image.</summary>
public class UploadRoomImageRequest
{
    /// <summary>Gets or sets the file.</summary>
    public IFormFile File { get; set; } = default!;
    /// <summary>Position of the image in the gallery. When omitted, the image is appended at the end.</summary>
    public short? DisplayOrder { get; set; }
}