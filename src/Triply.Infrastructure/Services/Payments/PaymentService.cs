using MessageQueue.IRabbitMQ;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Triply.Application.Interfaces.Payments;
using Triply.Application.Interfaces.Repositories;
using Triply.Application.Interfaces.Repositories.General;
using Triply.Application.Interfaces.Services;
using Triply.Application.Options;
using Triply.Domain.Contracts;
using Triply.Domain.Contracts.Enums;
using Triply.Domain.Entities;
using Triply.Domain.Enums.Bookings;
using Triply.Domain.Enums.Payments;

namespace Triply.Infrastructure.Services.Payments;

/// <summary>Handles the bookings-confirmation side effects of a successful payment.</summary>
public partial class PaymentService(
    IBookingRepository bookingRepository,
    IGenericRepository<Payment> paymentRepository,
    IPaymentGateway paymentGateway,
    IMessagePublisher publisher,
    IOptions<PaymentOptions> paymentOptions,
    ILogger<PaymentService> logger) : IPaymentService
{
    /// <summary>Marks the given bookings and their payments as paid/confirmed, persists the change, and attempts to publish a confirmation email.</summary>
    /// <param name="bookings">The bookings to confirm. Each must already have a non-null <see cref="Booking.Payment"/>.</param>
    /// <param name="transactionId">The payment provider's transaction identifier to record on each booking's payment, if available.</param>
    private async Task ConfirmBookingsAsync(List<Booking> bookings, string? transactionId)
    {
        var now = DateTime.UtcNow;
        foreach (var booking in bookings)
        {
            booking.Status = BookingStatus.Confirmed;
            booking.ModifiedAt = now;
            booking.Payment!.Status = PaymentStatus.Paid;
            booking.Payment.PaidAt = now;
            if (transactionId is not null)
                booking.Payment.TransactionId = transactionId;
        }
        await bookingRepository.SaveChangesAsync(CancellationToken.None);
        logger.LogInformation(
            "Booking {ConfirmationNumber} confirmed and marked as paid ({RoomCount} rooms, transaction {TransactionId})",
            bookings[0].ConfirmationNumber, bookings.Count, transactionId);

        try
        {
            await PublishBookingConfirmationEmail(bookings);
            logger.LogInformation("Confirmation email of booking {ConfirmationNumber} queued",
                bookings[0].ConfirmationNumber);
        }
        catch (Exception exception)
        {
            logger.LogError(exception,
                "Booking {ConfirmationNumber} was confirmed but its confirmation email could not be queued",
                bookings[0].ConfirmationNumber);
        }
    }

    /// <summary>Publishes a booking-confirmation email message to the message queue, summarizing all bookings under the same confirmation number.</summary>
    /// <param name="bookings">The confirmed bookings to include in the confirmation email.</param>
    private async Task PublishBookingConfirmationEmail(List<Booking> bookings)
    {
        var first = bookings[0];
        await publisher.PublishAsync("email.send", new EmailMessage
        {
            Type = EmailType.BookingConfirmation,
            To = first.GuestEmail,
            TemplateData = new Dictionary<string, string>
            {
                ["user_name"] = first.GuestFullName,
                ["hotel_name"] = string.Join(", ", bookings.Select(b => b.Room.Hotel.Name).Distinct()),
                ["check_in"] = bookings.Min(b => b.CheckIn).ToString("dd MMM yyyy"),
                ["check_out"] = bookings.Max(b => b.CheckOut).ToString("dd MMM yyyy"),
                ["total_price"] = $"{bookings.Sum(b => b.TotalPrice):N2} {paymentOptions.Value.Currency.ToUpper()}",
                ["confirmation_number"] = first.ConfirmationNumber
            }
        });
    }
}
