using Logging.DependencyInjection;
using Triply.Application.Interfaces.Repositories;
using Triply.Infrastructure.DependencyInjection;
using Triply.Infrastructure.Repositories;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddTriplyDbContext(builder.Configuration);
builder.Services.AddSieveService(builder.Configuration);
builder.Services.AddBookingOptions(builder.Configuration);

builder.Services.AddScoped<IBookingRepository, BookingRepository>();

builder.AddTriplyLogging("expire-pending-booking-worker");
builder.Services.AddHostedService<PendingBookingsExpiryService>();

var host = builder.Build();
host.Run();