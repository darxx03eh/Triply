namespace Triply.Application.Options;

/// <summary>Configuration options for booking.</summary>
public class BookingOptions
{
    /// <summary>Number of minutes before a pending booking expires.</summary>
    public int PendingExpiryMinutes { get; set; } = 45;
    /// <summary>Interval in minutes for checking expired pending bookings.</summary>
    public int ExpiryCheckIntervalMinutes { get; set; } = 5;
}