using FluentValidation.TestHelper;
using Triply.Application.Features.Cities.Queries.GetCitiesRequest;

namespace Triply.Tests.UnitTests.Application.Cities.Validators;

public class GetCitiesRequestValidatorTests
{
    private readonly GetCitiesRequestValidator _validator = new();

    [Theory]
    [InlineData(null, null)]
    [InlineData(1, 1)]
    [InlineData(3, 50)]
    public void Validate_ValidPaging_HasNoErrors(int? page, int? pageSize)
    {
        var result = _validator.TestValidate(new GetCitiesRequest { Page = page, PageSize = pageSize });

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_PageBelowOne_HasError(int page)
    {
        var result = _validator.TestValidate(new GetCitiesRequest { Page = page });

        result.ShouldHaveValidationErrorFor(x => x.Page);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(51)]
    public void Validate_PageSizeOutOfRange_HasError(int pageSize)
    {
        var result = _validator.TestValidate(new GetCitiesRequest { PageSize = pageSize });

        result.ShouldHaveValidationErrorFor(x => x.PageSize);
    }
}
