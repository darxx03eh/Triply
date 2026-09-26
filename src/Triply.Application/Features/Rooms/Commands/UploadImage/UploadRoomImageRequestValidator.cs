using FluentValidation;
using Microsoft.Extensions.Options;
using Triply.Application.Options;

namespace Triply.Application.Features.Rooms.Commands.UploadImage;

/// <summary>Validates the upload room image request.</summary>
public class UploadRoomImageRequestValidator : AbstractValidator<UploadRoomImageRequest>
{
    private static readonly HashSet<string> AllowedExtensions =
        new(StringComparer.OrdinalIgnoreCase) { ".jpg", ".jpeg", ".png", ".webp" };

    /// <summary>Initializes a new instance of the upload room image request validator.</summary>
    public UploadRoomImageRequestValidator(IOptions<ImageUploadOptions> uploadOptions)
    {
        long maxBytes = uploadOptions.Value.MaxFileSizeBytes;

        RuleFor(x => x.File)
            .NotNull().WithMessage("File is required.");

        When(x => x.File is not null, () =>
        {
            RuleFor(x => x.File.Length)
                .GreaterThan(0).WithMessage("File is empty.")
                .LessThanOrEqualTo(maxBytes)
                .WithMessage($"File exceeds the {maxBytes / 1024 / 1024} MB limit.");

            RuleFor(x => x.File.FileName)
                .Must(name => AllowedExtensions.Contains(Path.GetExtension(name)))
                .WithMessage("Only JPG, PNG, and WEBP files are allowed.");
        });

        // Matches the CK_DisplayOrder_Positive check constraint on the RoomImages table.
        RuleFor(x => x.DisplayOrder)
            .GreaterThan((short)0)
            .When(x => x.DisplayOrder.HasValue)
            .WithMessage("Display order must be greater than 0.");
    }
}