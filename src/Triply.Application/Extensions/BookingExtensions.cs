using Triply.Application.DTOs.Bookings;
using Triply.Application.DTOs.Cart;
using Triply.Application.Features.Bookings.Commands.Checkout;
using Triply.Domain.Entities;

namespace Triply.Application.Extensions;

/// <summary>Extension methods for booking.</summary>
public static class BookingExtensions
{
    /// <summary>Maps the booking to a booking response.</summary>
    public static BookingResponse ToBookingResponse(this Booking booking)
        => new BookingResponse()
        {
            BookingId = booking.BookingId,
            ConfirmationNumber = booking.ConfirmationNumber,
            HotelId = booking.Room.HotelId,
            HotelName = booking.Room.Hotel.Name,
            CityName = booking.Room.Hotel.City.Name,
            HotelAddress = booking.Room.Hotel.Address,
            RoomId = booking.RoomId,
            RoomNumber = booking.Room.Number,
            RoomType = booking.Room.RoomType,
            CheckIn = DateOnly.FromDateTime(booking.CheckIn),
            CheckOut = DateOnly.FromDateTime(booking.CheckOut),
            Nights = booking.CheckIn.GetNights(booking.CheckOut),
            Adults = booking.Adults,
            Children = booking.Children,
            DiscountAmount = booking.DiscountAmount,
            TotalPrice = booking.TotalPrice,
            Status = booking.Status,
            CreatedAt = booking.CreatedAt
        };
    
    /// <summary>Maps the list of bookings to a booking confirmation response.</summary>
    public static BookingConfirmationResponse ToBookingConfirmationResponse(this IReadOnlyList<Booking> bookings)
    {
        var first = bookings[0];
        return new BookingConfirmationResponse()
        {
            ConfirmationNumber = first.ConfirmationNumber,
            Status = first.Status,
            GuestFullName = first.GuestFullName,
            GuestEmail = first.GuestEmail,
            GuestPhoneNumber = first.GuestPhoneNumber,
            SpecialRequests = first.SpecialRequests,
            DiscountAmount = bookings.Sum(b => b.DiscountAmount),
            TotalPrice = bookings.Sum(b => b.TotalPrice),
            CreatedAt = first.CreatedAt,
            Bookings = bookings.Select(b => b.ToBookingResponse()).ToList()
        };
    }
    
    public static Booking ToBooking(this CheckoutRequest request, string confirmationNumber, CartItem item, 
        CartItemResponse price)
    => new Booking
    {
        ConfirmationNumber = confirmationNumber,
        UserId = request.UserId,
        RoomId = item.RoomId,
        Room = item.Room,
        CheckIn = item.CheckIn,
        CheckOut = item.CheckOut,
        Adults = item.Adults,
        Children = item.Children,
        DiscountAmount = price.DiscountAmount,
        TotalPrice = price.TotalPrice,
        GuestFullName = request.GuestFullName.Trim(),
        GuestEmail = request.GuestEmail.Trim(),
        GuestPhoneNumber = string.IsNullOrWhiteSpace(request.GuestPhoneNumber)
            ? null
            : request.GuestPhoneNumber.Trim(),
        SpecialRequests = string.IsNullOrWhiteSpace(request.SpecialRequests)
            ? null
            : request.SpecialRequests.Trim()
    };
}