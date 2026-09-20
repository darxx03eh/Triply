namespace Triply.Domain.Enums.Bookings;

/// <summary>The booking status values.</summary>
public enum BookingStatus : byte
{
    /// <summary>The pending.</summary>
    Pending = 0,
    /// <summary>The confirmed.</summary>
    Confirmed = 1,
    /// <summary>The cancelled.</summary>
    Cancelled = 2,
    /// <summary>The completed.</summary>
    Completed = 3
}