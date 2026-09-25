using EmailWorker.Contracts.Enums;
using EmailWorker.Providers;

namespace Triply.Tests.UnitTests.Workers.EmailWorker;

public class ScribanEmailTemplateProviderTests
{
    private readonly ScribanEmailTemplateProvider _provider = new();

    [Fact]
    public async Task RenderAsync_ConfirmationEmail_FillsPlaceholders()
    {
        var (subject, body) = await 
            _provider.RenderAsync(EmailType.ConfirmationEmail, new()
        {
            ["user_name"] = "mahmoud",
            ["confirmation_link"] = "http://localhost:3000/confirm?token=abc"
        });

        Assert.Equal("Confirm your account", subject);
        Assert.Contains("mahmoud", body);
        Assert.Contains("http://localhost:3000/confirm?token=abc", body);
        Assert.DoesNotContain("{{", body);
    }

    [Fact]
    public async Task RenderAsync_BookingConfirmation_RendersSubjectWithHotelName()
    {
        var (subject, body) = await _provider.RenderAsync(EmailType.BookingConfirmation, new()
        {
            ["user_name"] = "lina",
            ["hotel_name"] = "Red Sea Resort",
            ["check_in"] = "2026-10-02",
            ["check_out"] = "2026-10-05",
            ["total_price"] = "$957"
        });

        Assert.Equal("Booking Confirmed - Red Sea Resort", subject);
        Assert.Contains("2026-10-05", body);
        Assert.Contains("$957", body);
    }

    [Fact]
    public async Task RenderAsync_ForgotPassword_RendersResetLink()
    {
        var (subject, body) = await _provider.RenderAsync(EmailType.ForgotPassword, new()
        {
            ["user_name"] = "omar", ["reset_link"] = "http://reset", ["expiry_minutes"] = "30"
        });

        Assert.Equal("Reset your password", subject);
        Assert.Contains("http://reset", body);
    }

    [Fact]
    public async Task RenderAsync_UnknownType_Throws()
    {
        await Assert.ThrowsAsync<InvalidOperationException>(() => _provider.RenderAsync((EmailType)99, new()));
    }
}
