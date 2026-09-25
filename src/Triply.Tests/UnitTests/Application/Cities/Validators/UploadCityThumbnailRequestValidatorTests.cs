using FluentValidation.TestHelper;
using Microsoft.Extensions.Options;
using Triply.Application.Features.Cities.Commands.UploadCityThumbnail;
using Triply.Application.Options;
using Triply.Tests.UnitTests.Common.Fakes;

namespace Triply.Tests.UnitTests.Application.Cities.Validators;

public class UploadCityThumbnailRequestValidatorTests
{
    private readonly UploadCityThumbnailRequestValidator _validator =
        new(Options.Create(new ImageUploadOptions { MaxFileSizeBytes = 1024 }));

    [Theory]
    [InlineData("city.png")]
    [InlineData("city.JPG")]
    [InlineData("city.jpeg")]
    [InlineData("city.webp")]
    public void Validate_AllowedImage_HasNoErrors(string fileName)
    {
        var result = _validator.TestValidate(new UploadCityThumbnailRequest 
            { File = TestImages.File(fileName, TestImages.Png) });

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_NoFile_HasError()
    {
        var result = _validator.TestValidate(new UploadCityThumbnailRequest { File = null! });

        result.ShouldHaveValidationErrorFor(x => x.File);
    }

    [Theory]
    [InlineData("city.gif")]
    [InlineData("city.exe")]
    [InlineData("city")]
    public void Validate_DisallowedExtension_HasError(string fileName)
    {
        var result = _validator.TestValidate(new UploadCityThumbnailRequest 
            { File = TestImages.File(fileName, TestImages.Png) });

        result.ShouldHaveValidationErrorFor(x => x.File.FileName);
    }

    [Fact]
    public void Validate_EmptyFile_HasError()
    {
        var result = _validator.TestValidate(new UploadCityThumbnailRequest 
            { File = TestImages.File("city.png", []) });

        result.ShouldHaveValidationErrorFor(x => x.File.Length);
    }

    [Fact]
    public void Validate_FileTooLarge_HasError()
    {
        var result = _validator.TestValidate(new UploadCityThumbnailRequest 
            { File = TestImages.File("city.png", new byte[2048]) });

        result.ShouldHaveValidationErrorFor(x => x.File.Length);
    }
}
