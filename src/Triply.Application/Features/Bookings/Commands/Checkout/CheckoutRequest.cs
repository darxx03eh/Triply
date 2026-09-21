using System.Text.Json.Serialization;

namespace Triply.Application.Features.Bookings.Commands.Checkout;

/// <summary>Data required to check out.</summary>
public class CheckoutRequest
{
    /// <summary>Gets or sets the identifier of the user.</summary>
    [JsonIgnore]
    public Guid UserId { get; set; }
    /// <summary>Gets or sets the guest full name.</summary>
    public string GuestFullName { get; set; }
    /// <summary>Gets or sets the guest email.</summary>
    public string GuestEmail { get; set; }
    /// <summary>Gets or sets the guest phone number.</summary>
    public string? GuestPhoneNumber { get; set; }
    /// <summary>Gets or sets the special request.</summary>
    public string? SpecialRequests { get; set; }
}