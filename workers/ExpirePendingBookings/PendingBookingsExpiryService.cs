using Microsoft.Extensions.Options;
using Triply.Application.Interfaces.Repositories;
using Triply.Application.Options;

/// <summary>Cancels the pending bookings that were not paid in time so their rooms become available again.</summary>
public class PendingBookingsExpiryService(
    IServiceScopeFactory scopeFactory,
    IOptions<BookingOptions> options,
    ILogger<PendingBookingsExpiryService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromMinutes(options.Value.ExpiryCheckIntervalMinutes));
        do
        {
            try
            {
                using var scope = scopeFactory.CreateScope();
                var bookingRepository = scope.ServiceProvider.GetRequiredService<IBookingRepository>();
                var expired = await bookingRepository.ExpirePendingAsync(
                    DateTime.UtcNow.AddMinutes(-options.Value.PendingExpiryMinutes), stoppingToken);
                if (expired > 0)
                    logger.LogInformation("{Count} unpaid pending bookings were expired.", expired);
            }
            catch (Exception exception) when (exception is not OperationCanceledException)
            {
                logger.LogError(exception, "Failed to expire the unpaid pending bookings.");
            }
        } while (await timer.WaitForNextTickAsync(stoppingToken));
    }
}