using FluentValidation.TestHelper;
using Triply.Application.Features.Bookings.Commands.Checkout;
using Triply.Application.Features.Bookings.Queries.GetBookings;
using Triply.Domain.Results;

namespace Triply.Tests.UnitTests.Application.Bookings.Validators;

public class BookingRequestValidatorTests
{
    [Fact]
    public void Validate_CheckoutRequestWithValidGuest_HasNoErrors()
    {
        var result = new CheckoutRequestValidator().TestValidate(new CheckoutRequest
        {
            GuestFullName = "Mahmoud Darawsheh", GuestEmail = "mahmoud@triply.com", GuestPhoneNumber = "+970599123456"
        });

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("not-an-email")]
    [InlineData("")]
    public void Validate_CheckoutRequestWithInvalidEmail_HasGuestEmailError(string email)
    {
        var result = new CheckoutRequestValidator().TestValidate(new CheckoutRequest
        { GuestFullName = "Mahmoud", GuestEmail = email });

        result.ShouldHaveValidationErrorFor(x => x.GuestEmail)
            .WithErrorMessage(ResultResponseMessages.Bookings.Validation.GuestEmailInvalid.Message);
    }

    [Fact]
    public void Validate_CheckoutRequestWithoutGuestName_HasRequiredNameError()
    {
        var result = new CheckoutRequestValidator().TestValidate(new CheckoutRequest
        { GuestFullName = "", GuestEmail = "mahmoud@triply.com" });

        result.ShouldHaveValidationErrorFor(x => x.GuestFullName)
            .WithErrorMessage(ResultResponseMessages.Bookings.Validation.GuestFullNameRequired.Message);
    }

    [Fact]
    public void Validate_CheckoutRequestWithTooLongFields_HasLengthErrors()
    {
        var result = new CheckoutRequestValidator().TestValidate(new CheckoutRequest
        {
            GuestFullName = new string('a', 101), GuestEmail = "mahmoud@triply.com",
            SpecialRequests = new string('a', 2001)
        });

        result.ShouldHaveValidationErrorFor(x => x.GuestFullName)
            .WithErrorMessage(ResultResponseMessages.Bookings.Validation.GuestFullNameMaxLength.Message);
        result.ShouldHaveValidationErrorFor(x => x.SpecialRequests)
            .WithErrorMessage(ResultResponseMessages.Bookings.Validation.SpecialRequestsMaxLength.Message);
    }

    [Theory]
    [InlineData("0599123456")]
    [InlineData("+0123456789")]
    [InlineData("+9705")]
    public void Validate_CheckoutRequestWithInvalidPhone_HasPhoneError(string phone)
    {
        var result = new CheckoutRequestValidator().TestValidate(new CheckoutRequest
        { GuestFullName = "Mahmoud", GuestEmail = "mahmoud@triply.com", GuestPhoneNumber = phone });

        result.ShouldHaveValidationErrorFor(x => x.GuestPhoneNumber)
            .WithErrorMessage(ResultResponseMessages.Bookings.Validation.GuestPhoneNumberInvalid.Message);
    }

    [Theory]
    [InlineData(0, "Page")]
    [InlineData(51, "PageSize")]
    public void Validate_GetBookingsWithInvalidPaging_HasTheExpectedError(int value, string property)
    {
        var result = new GetBookingsRequestValidator().TestValidate(new GetBookingsRequest
        { Page = property == "Page" ? value : 1, PageSize = property == "PageSize" ? value : 10 });

        Assert.Contains(result.Errors, error => error.PropertyName == property);
    }
}
