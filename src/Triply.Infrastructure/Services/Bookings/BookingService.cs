using Microsoft.Extensions.Logging;
using Triply.Application.Interfaces.Payments;
using Triply.Application.Interfaces.Repositories;
using Triply.Application.Interfaces.Services;

namespace Triply.Infrastructure.Services.Bookings;

/// <summary>Gets or sets the booking service.</summary>
/// <summary>Implements the booking operations.</summary>
public partial class BookingService(
    IBookingRepository bookingRepository,
    ICartRepository cartRepository,
    IDealRepository dealRepository,
    IPaymentGateway paymentGateway,
    ILogger<BookingService> logger
    ) : IBookingService { }