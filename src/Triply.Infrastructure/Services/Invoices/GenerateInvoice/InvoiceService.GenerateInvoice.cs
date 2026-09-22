using Microsoft.Extensions.Logging;
using QuestPDF.Fluent;
using Triply.Application.DTOs.Bookings;
using Triply.Domain.Enums.Bookings;
using Triply.Domain.Results;
using Triply.Domain.Results.Enums;
using Triply.Infrastructure.Documents;

namespace Triply.Infrastructure.Services.Invoices;

public partial class InvoiceService
{
    /// <summary>Generates the invoice file for the booking with the given confirmation number.</summary>
    public async Task<Result<InvoiceFileResponse>> GenerateAsync(string confirmationNumber, Guid userId, bool isAdmin,
        CancellationToken cancellationToken = default)
    {
        var bookings = await bookingRepository
            .GetByConfirmationNumberAsync(confirmationNumber, cancellationToken);
        if (bookings.Count == 0)
        {
            logger.LogWarning(
                "Generate invoice failed: booking {ConfirmationNumber} was not found (user {UserId}, admin: {IsAdmin})",
                confirmationNumber, userId, isAdmin);
            return Result<InvoiceFileResponse>.Failure(
                "BOOKING_NOT_FOUND",
                $"The requested booking with confirmation number: {confirmationNumber} was not found.",
                ResultErrorType.NotFound);
        }

        if (!isAdmin && bookings[0].UserId != userId)
        {
            logger.LogWarning(
                "Generate invoice failed: booking {ConfirmationNumber} does not belong to user {UserId}",
                confirmationNumber, userId);
            return Result<InvoiceFileResponse>.Failure(
                "BOOKING_NOT_FOUND",
                $"The requested booking with confirmation number: {confirmationNumber} was not found.",
                ResultErrorType.NotFound);
        }

        var status = bookings[0].Status;
        if (status is not (BookingStatus.Confirmed or BookingStatus.Completed))
        {
            logger.LogWarning(
                "Generate invoice failed: booking {ConfirmationNumber} is not paid yet (status {Status}, user {UserId})",
                confirmationNumber, status, userId);
            return Result<InvoiceFileResponse>.Failure(
                "BOOKING_NOT_PAID", "The invoice is available after the booking is paid.");
        }

        var content = new BookingInvoiceDocument(bookings, paymentOptions.Value.Currency).GeneratePdf();
        logger.LogInformation(
            "Invoice for booking {ConfirmationNumber} generated for user {UserId} (admin: {IsAdmin}, {RoomCount} rooms," +
            " {Size} bytes)",
            confirmationNumber, userId, isAdmin, bookings.Count, content.Length);

        return Result<InvoiceFileResponse>.Success(
            new InvoiceFileResponse(content, $"invoice-{confirmationNumber}.pdf"));
    }
}