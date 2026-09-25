using FluentValidation.TestHelper;
using Microsoft.Extensions.Options;
using Triply.Application.Features.Hotels.Commands.UploadImage;
using Triply.Application.Options;
using Triply.Tests.UnitTests.Common.Fakes;

namespace Triply.Tests.UnitTests.Application.Hotels.Validators;

public class UploadHotelImageRequestValidatorTests
{
    private readonly UploadHotelImageRequestValidator _validator =
        new(Options.Create(new ImageUploadOptions { MaxFileSizeBytes = 1024 }));

    private static UploadHotelImageRequest Request(string fileName = "hotel.png", 
        byte[]? content = null, short? order = null)
        => new() { File = TestImages.File(fileName, content ?? TestImages.Png), DisplayOrder = order };

    [Theory]
    [InlineData("hotel.png")]
    [InlineData("hotel.jpg")]
    [InlineData("hotel.JPEG")]
    [InlineData("hotel.webp")]
    public void Validate_AllowedImage_HasNoErrors(string fileName)
    {
        var result = _validator.TestValidate(Request(fileName));

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithoutDisplayOrder_IsValid()
    {
        var result = _validator.TestValidate(Request(order: null));

        result.ShouldNotHaveValidationErrorFor(x => x.DisplayOrder);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-3)]
    public void Validate_DisplayOrderNotPositive_HasError(short order)
    {
        var result = _validator.TestValidate(Request(order: order));

        result.ShouldHaveValidationErrorFor(x => x.DisplayOrder);
    }

    [Fact]
    public void Validate_NoFile_HasError()
    {
        var result = _validator.TestValidate(new UploadHotelImageRequest { File = null! });

        result.ShouldHaveValidationErrorFor(x => x.File);
    }

    [Theory]
    [InlineData("hotel.gif")]
    [InlineData("hotel.pdf")]
    [InlineData("hotel")]
    public void Validate_DisallowedExtension_HasError(string fileName)
    {
        var result = _validator.TestValidate(Request(fileName));

        result.ShouldHaveValidationErrorFor(x => x.File.FileName);
    }

    [Fact]
    public void Validate_EmptyFile_HasError()
    {
        var result = _validator.TestValidate(Request(content: []));

        result.ShouldHaveValidationErrorFor(x => x.File.Length);
    }

    [Fact]
    public void Validate_FileTooLarge_HasError()
    {
        var result = _validator.TestValidate(Request(content: new byte[1025]));

        result.ShouldHaveValidationErrorFor(x => x.File.Length);
    }
}
