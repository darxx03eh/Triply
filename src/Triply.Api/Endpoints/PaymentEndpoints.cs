using Triply.Api.Extensions;
using Triply.Api.Responses;
using Triply.Application.DTOs.Bookings;
using Triply.Application.DTOs.Payments;
using Triply.Application.Interfaces.Services;
using Triply.Infrastructure.Routes;

namespace Triply.Api.Endpoints;

/// <summary>Maps the payment endpoints.</summary>
public static class PaymentEndpoints
{
    extension(IEndpointRouteBuilder app)
    {
        /// <summary>Maps the payment endpoints.</summary>
        public void MapPaymentEndpoints()
        {
            var group = app.MapGroup("")
                .WithTags("Payments");

            group.MapPost(Router.BookingRoutes.Pay, async (
                    string confirmationNumber, ICurrentUserAccessor user, IPaymentService paymentService,
                    CancellationToken cancellationToken) =>
                {
                    var result = await paymentService.PayAsync(confirmationNumber, user.UserId, cancellationToken);
                    return result.ToMinimalApiResult();
                }).RequireAuthorization()
                .WithName("PayBooking")
                .WithDisplayName("Pay Booking")
                .WithSummary("Starts the payment of a pending booking")
                .WithDescription("""
                                 Starts the payment of a pending booking. With Stripe it returns a checkout url
                                 the user is redirected to, the booking is confirmed later by the webhook.
                                 With the Mock provider the booking is paid and confirmed immediately.
                                 """)
                .Produces<ApiResponse<PaymentResponse>>(StatusCodes.Status200OK)
                .Produces<ApiResponse<object>>(StatusCodes.Status400BadRequest)
                .Produces<ApiResponse<object>>(StatusCodes.Status404NotFound);

            group.MapPost(Router.PaymentRoutes.Webhook, async (
                    HttpRequest request, IPaymentService paymentService, CancellationToken cancellationToken) =>
                {
                    using var reader = new StreamReader(request.Body);
                    var payload = await reader.ReadToEndAsync(cancellationToken);
                    var result = await paymentService.HandleWebhookAsync(
                        payload, request.Headers["Stripe-Signature"].ToString(), cancellationToken);
                    return result.ToMinimalApiResult();
                }).AllowAnonymous()
                .WithName("PaymentWebhook")
                .WithDisplayName("Payment Webhook")
                .WithSummary("Receives the payment provider events")
                .WithDescription("""
                                 Receives the checkout events from the payment provider, verifies the signature,
                                 confirms the paid bookings and sends the booking confirmation email.
                                 """)
                .Produces<ApiResponse<bool>>(StatusCodes.Status200OK)
                .Produces<ApiResponse<object>>(StatusCodes.Status400BadRequest);

            group.MapGet(Router.BookingRoutes.Invoice, async (
                    string confirmationNumber, ICurrentUserAccessor user, IInvoiceService invoiceService,
                    CancellationToken cancellationToken) =>
                {
                    var result = await invoiceService.GenerateAsync(
                        confirmationNumber, user.UserId, user.IsAdmin, cancellationToken);
                    if (!result.IsSuccess)
                        return result.ToMinimalApiResult();
                    return Results.File(result.Value!.Content, "application/pdf", result.Value.FileName);
                }).RequireAuthorization()
                .WithName("GetBookingInvoice")
                .WithDisplayName("Get Booking Invoice")
                .WithSummary("Downloads the invoice of a paid booking")
                .WithDescription("""
                                 Generates a PDF invoice for a paid booking with the guest details, rooms,
                                 dates, discounts and the total paid.
                                 """)
                .Produces<ApiResponse<InvoiceFileResponse>>(
                    StatusCodes.Status200OK, 
                    contentType: "application/pdf")
                .Produces<ApiResponse<object>>(StatusCodes.Status400BadRequest)
                .Produces<ApiResponse<object>>(StatusCodes.Status404NotFound);
        }
    }
}