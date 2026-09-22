using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Triply.Application.Interfaces.Repositories;
using Triply.Application.Interfaces.Services;
using Triply.Application.Options;

namespace Triply.Infrastructure.Services.Invoices;

/// <summary>Gets or sets the invoice service.</summary>
/// <summary>Implements the invoice operations.</summary>
public partial class InvoiceService(
    IBookingRepository bookingRepository,
    IOptions<PaymentOptions> paymentOptions,
    ILogger<InvoiceService> logger) : IInvoiceService {}