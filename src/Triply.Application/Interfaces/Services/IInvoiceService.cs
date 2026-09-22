using Triply.Application.DTOs.Bookings;
using Triply.Domain.Results;

namespace Triply.Application.Interfaces.Services;

/// <summary>Defines the invoice operations.</summary>
public interface IInvoiceService
{
    /// <summary>Generates the invoice file for the booking with the given confirmation number.</summary>
    Task<Result<InvoiceFileResponse>> GenerateAsync(string confirmationNumber, Guid userId, bool isAdmin,
        CancellationToken cancellationToken = default);
}