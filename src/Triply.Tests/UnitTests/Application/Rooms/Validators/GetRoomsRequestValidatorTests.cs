using FluentValidation.TestHelper;
using Triply.Application.Features.Rooms.Queries.GetRooms;

namespace Triply.Tests.UnitTests.Application.Rooms.Validators;

public class GetRoomsRequestValidatorTests
{
    private readonly GetRoomsRequestValidator _validator = new();

    [Theory]
    [InlineData(null, null)]
    [InlineData(2, 25)]
    public void Validate_ValidPaging_HasNoErrors(int? page, int? pageSize)
    {
        var result = _validator.TestValidate(new GetRoomsRequest { Page = page, PageSize = pageSize });

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_PageZero_HasError()
    {
        var result = _validator.TestValidate(new GetRoomsRequest { Page = 0 });

        result.ShouldHaveValidationErrorFor(x => x.Page);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(100)]
    public void Validate_PageSizeOutOfRange_HasError(int pageSize)
    {
        var result = _validator.TestValidate(new GetRoomsRequest { PageSize = pageSize });

        result.ShouldHaveValidationErrorFor(x => x.PageSize);
    }
}
