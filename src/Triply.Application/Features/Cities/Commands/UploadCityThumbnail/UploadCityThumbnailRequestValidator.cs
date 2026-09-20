using FluentValidation;
using Microsoft.Extensions.Options;
using Triply.Application.Options;

namespace Triply.Application.Features.Cities.Commands.UploadCityThumbnail;

/// <summary>Validates the upload city thumbnail request.</summary>
public class UploadCityThumbnailRequestValidator : AbstractValidator<UploadCityThumbnailRequest>
{
    private static readonly HashSet<string> AllowedExtensions =
        new(StringComparer.OrdinalIgnoreCase) { ".jpg", ".jpeg", ".png", ".webp" };

    /// <summary>Initializes a new instance of the upload city thumbnail request validator.</summary>
    public UploadCityThumbnailRequestValidator(IOptions<ImageUploadOptions> uploadOptions)
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
    }
}
