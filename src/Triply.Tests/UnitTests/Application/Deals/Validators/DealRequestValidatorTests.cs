using FluentValidation.TestHelper;
using Triply.Application.Features.Deals.Commands.CreateDeal;
using Triply.Application.Features.Deals.Commands.UpdateDeal;
using Triply.Application.Features.Deals.Queries.GetDeals;
using Triply.Domain.Results;

namespace Triply.Tests.UnitTests.Application.Deals.Validators;

public class DealRequestValidatorTests
{
    [Fact]
    public void Validate_CreateDealWithValidValues_HasNoErrors()
    {
        var now = DateTime.UtcNow.AddHours(1);
        var result = new CreateDealRequestValidator().TestValidate(new CreateDealRequest
        { RoomId = Guid.NewGuid(), Title = "Early booking", DiscountPercentage = 15, StartsAt = now, EndsAt = now.AddDays(2) });

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_UpdateDealEndingBeforeStart_HasEndsAtError()
    {
        var now = DateTime.UtcNow.AddHours(1);
        var result = new UpdateDealRequestValidator().TestValidate(new UpdateDealRequest
        { Title = "Weekend", DiscountPercentage = 10, StartsAt = now, EndsAt = now.AddMinutes(-1) });

        result.ShouldHaveValidationErrorFor(x => x.EndsAt)
            .WithErrorMessage(ResultResponseMessages.Deals.Validation.EndsBeforeStarts.Message);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(51)]
    public void Validate_GetDealsWithInvalidPageSize_HasError(int pageSize)
    {
        var result = new GetDealsRequestValidator().TestValidate(new GetDealsRequest { PageSize = pageSize });

        result.ShouldHaveValidationErrorFor(x => x.PageSize);
    }
}
