using FluentValidation.TestHelper;
using Triply.Application.Extensions;
using Triply.Application.Features.Search.Queries.SearchHotels;
using Triply.Domain.Results;

namespace Triply.Tests.UnitTests.Application.Search;

public class SearchHotelsRequestValidatorTests
{
    private readonly SearchHotelsRequestValidator _validator = new();
    private static DateOnly Day(int offset) => SearchExtensions.Today().AddDays(offset);

    [Fact]
    public void Validate_EmptyRequest_UsesDefaultsAndIsValid()
    {
        var result = _validator.TestValidate(new SearchHotelsRequest());

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_FullValidRequest_HasNoErrors()
    {
        var request = new SearchHotelsRequest
        {
            Q = "Amman",
            CityId = Guid.NewGuid(),
            CheckIn = Day(2),
            CheckOut = Day(5),
            Adults = 4,
            Children = 2,
            Rooms = 2,
            MinPrice = 50,
            MaxPrice = 300,
            Stars = [4, 5],
            Types = ["Luxury", "boutique"],
            Amenities = [Guid.NewGuid()],
            Sort = "PRICE_ASC",
            Page = 2,
            PageSize = 20
        };

        var result = _validator.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_QueryTooLong_HasError()
    {
        var result = _validator.TestValidate(new SearchHotelsRequest { Q = new string('a', 101) });

        result.ShouldHaveValidationErrorFor(x => x.Q);
    }

    [Fact]
    public void Validate_CheckInToday_IsValid()
    {
        var result = _validator.TestValidate(new SearchHotelsRequest { CheckIn = Day(0) });

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_CheckInInPast_HasError()
    {
        var result = _validator.TestValidate(new SearchHotelsRequest { CheckIn = Day(-1) });

        result.ShouldHaveValidationErrorFor(x => x.CheckIn)
            .WithErrorMessage(ResultResponseMessages.Search.Validation.CheckInInPast.Message);
    }

    [Theory]
    [InlineData(5, 5)]
    [InlineData(5, 3)]
    public void Validate_CheckOutNotAfterCheckIn_HasError(int checkIn, int checkOut)
    {
        var result = _validator.TestValidate(new SearchHotelsRequest { CheckIn = Day(checkIn), CheckOut = Day(checkOut) });

        result.ShouldHaveValidationErrorFor(x => x.CheckOut)
            .WithErrorMessage(ResultResponseMessages.Search.Validation.CheckOutBeforeCheckIn.Message);
    }

    [Fact]
    public void Validate_CheckOutOnlyBeforeDefaultCheckIn_HasError()
    {
        var result = _validator.TestValidate(new SearchHotelsRequest { CheckOut = Day(0) });

        result.ShouldHaveValidationErrorFor(x => x.CheckOut);
    }

    [Fact]
    public void Validate_StayOfExactly30Nights_IsValid()
    {
        var result = _validator.TestValidate(new SearchHotelsRequest { CheckIn = Day(1), CheckOut = Day(31) });

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_StayLongerThan30Nights_HasError()
    {
        var result = _validator.TestValidate(new SearchHotelsRequest { CheckIn = Day(1), CheckOut = Day(32) });

        result.ShouldHaveValidationErrorFor(x => x.CheckOut)
            .WithErrorMessage(ResultResponseMessages.Search.Validation.StayTooLong.Message);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(21)]
    public void Validate_AdultsOutOfRange_HasError(int adults)
    {
        var result = _validator.TestValidate(new SearchHotelsRequest { Adults = adults, Rooms = 1 });

        result.ShouldHaveValidationErrorFor(x => x.Adults);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(21)]
    public void Validate_ChildrenOutOfRange_HasError(int children)
    {
        var result = _validator.TestValidate(new SearchHotelsRequest { Children = children });

        result.ShouldHaveValidationErrorFor(x => x.Children);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(11)]
    public void Validate_RoomsOutOfRange_HasError(int rooms)
    {
        var result = _validator.TestValidate(new SearchHotelsRequest { Adults = 20, Rooms = rooms });

        result.ShouldHaveValidationErrorFor(x => x.Rooms);
    }

    [Fact]
    public void Validate_MoreRoomsThanAdults_HasError()
    {
        var result = _validator.TestValidate(new SearchHotelsRequest { Adults = 1, Rooms = 2 });

        result.ShouldHaveValidationErrorFor(x => x.Rooms)
            .WithErrorMessage(ResultResponseMessages.Search.Validation.RoomsMoreThanAdults.Message);
    }

    [Fact]
    public void Validate_MoreRoomsThanDefaultAdults_HasError()
    {
        var result = _validator.TestValidate(new SearchHotelsRequest { Rooms = 3 });

        result.ShouldHaveValidationErrorFor(x => x.Rooms);
    }

    [Fact]
    public void Validate_NegativePrices_HaveErrors()
    {
        var result = _validator.TestValidate(new SearchHotelsRequest { MinPrice = -1, MaxPrice = -1 });

        result.ShouldHaveValidationErrorFor(x => x.MinPrice);
        result.ShouldHaveValidationErrorFor(x => x.MaxPrice);
    }

    [Fact]
    public void Validate_MaxPriceBelowMinPrice_HasError()
    {
        var result = _validator.TestValidate(new SearchHotelsRequest { MinPrice = 200, MaxPrice = 100 });

        result.ShouldHaveValidationErrorFor(x => x.MaxPrice)
            .WithErrorMessage(ResultResponseMessages.Search.Validation.PriceRangeInvalid.Message);
    }

    [Fact]
    public void Validate_EqualMinAndMaxPrice_IsValid()
    {
        var result = _validator.TestValidate(new SearchHotelsRequest { MinPrice = 100, MaxPrice = 100 });

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(6)]
    public void Validate_StarOutOfRange_HasError(int star)
    {
        var result = _validator.TestValidate(new SearchHotelsRequest { Stars = [5, star] });

        Assert.Contains(result.Errors, e => e.PropertyName == "Stars[1]");
    }

    [Theory]
    [InlineData("Castle")]
    [InlineData("1")]
    [InlineData("")]
    public void Validate_UnknownHotelType_HasError(string type)
    {
        var result = _validator.TestValidate(new SearchHotelsRequest { Types = [type] });

        Assert.Contains(result.Errors, e => e.PropertyName == "Types[0]");
    }

    [Fact]
    public void Validate_MoreThan20Amenities_HasError()
    {
        var amenities = Enumerable.Range(0, 21).Select(_ => Guid.NewGuid()).ToArray();

        var result = _validator.TestValidate(new SearchHotelsRequest { Amenities = amenities });

        result.ShouldHaveValidationErrorFor(x => x.Amenities);
    }

    [Theory]
    [InlineData("recommended")]
    [InlineData("price_asc")]
    [InlineData("price_desc")]
    [InlineData("stars_desc")]
    [InlineData("stars_asc")]
    [InlineData("Name")]
    public void Validate_KnownSort_IsValid(string sort)
    {
        var result = _validator.TestValidate(new SearchHotelsRequest { Sort = sort });

        result.ShouldNotHaveValidationErrorFor(x => x.Sort);
    }

    [Fact]
    public void Validate_UnknownSort_HasError()
    {
        var result = _validator.TestValidate(new SearchHotelsRequest { Sort = "cheapest" });

        result.ShouldHaveValidationErrorFor(x => x.Sort)
            .WithErrorMessage(ResultResponseMessages.Search.Validation.SortInvalid.Message);
    }

    [Theory]
    [InlineData(0, null)]
    [InlineData(null, 0)]
    [InlineData(null, 51)]
    public void Validate_InvalidPaging_HasError(int? page, int? pageSize)
    {
        var result = _validator.TestValidate(new SearchHotelsRequest { Page = page, PageSize = pageSize });

        Assert.False(result.IsValid);
    }
}
