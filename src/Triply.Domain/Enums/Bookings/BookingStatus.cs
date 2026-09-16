namespace Triply.Domain.Enums.Bookings;

public enum BookingStatus : byte
{
    Pending = 0,
    Confirmed = 1,
    Cancelled = 2,
    Completed = 3
}