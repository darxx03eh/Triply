using FluentValidation.TestHelper;
using Triply.Application.Features.Hotels.Queries.GetHotels;
using Triply.Domain.Results;

namespace Triply.Tests.UnitTests.Application.Hotels.Validators;

public class GetHotelsValidatorTests
{
    private readonly GetHotelsValidator _validator = new();

    [Theory]
    [InlineData(null, null)]
    [InlineData(1, 50)]
    public void Validate_ValidPaging_HasNoErrors(int? page, int? pageSize)
    {
        var result = _validator.TestValidate(new GetHotelsRequest { Page = page, PageSize = pageSize });

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_PageZero_UsesHotelMessage()
    {
        var result = _validator.TestValidate(new GetHotelsRequest { Page = 0 });

        result.ShouldHaveValidationErrorFor(x => x.Page)
            .WithErrorMessage(ResultResponseMessages.Hotels.Validation.PageInvalid.Message);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(51)]
    public void Validate_PageSizeOutOfRange_HasError(int pageSize)
    {
        var result = _validator.TestValidate(new GetHotelsRequest { PageSize = pageSize });

        result.ShouldHaveValidationErrorFor(x => x.PageSize)
            .WithErrorMessage(ResultResponseMessages.Hotels.Validation.PageSizeInvalid.Message);
    }
}
