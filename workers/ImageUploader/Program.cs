using ImageUploader;
using ImageUploader.IProviders;
using ImageUploader.Options;
using ImageUploader.Providers;
using Logging.DependencyInjection;
using MessageQueue.DependencyInjection;
using MessageQueue.Options;
using MessageQueue.RabbitMQ;
using Microsoft.Extensions.Options;
using Triply.Application.Interfaces.Repositories;
using Triply.Infrastructure.DependencyInjection;
using Triply.Infrastructure.Repositories;

var builder = Host.CreateApplicationBuilder(args);

builder.AddTriplyLogging("image-uploader");

builder.Services.AddRabbitMqMessaging(builder.Configuration);
builder.Services.AddTriplyDbContext(builder.Configuration);
builder.Services.AddSieveService(builder.Configuration);

builder.Services.AddOptions<CloudinaryOptions>()
    .Bind(builder.Configuration.GetSection("Cloudinary"))
    .Validate(o => !string.IsNullOrWhiteSpace(o.CloudName)
                   && !string.IsNullOrWhiteSpace(o.ApiKey)
                   && !string.IsNullOrWhiteSpace(o.ApiSecret),
        "Cloudinary CloudName, ApiKey and ApiSecret must be configured.")
    .ValidateOnStart();

builder.Services.AddSingleton<ICloudinaryUploader, CloudinaryUploader>();
builder.Services.AddScoped<IHotelImageRepository, HotelImageRepository>();
builder.Services.AddScoped<IRoomImageRepository, RoomImageRepository>();
builder.Services.AddScoped<ICityRepository, CityRepository>();
builder.Services.AddHostedService<ImageUploadConsumerHostedService>();

// Deletions get a dedicated consumer and queue, derived from the main RabbitMq settings
// (e.g. image.queue -> image.queue.delete) so they never mix with upload messages.
builder.Services.AddHostedService(sp =>
{
    var options = builder.Configuration.GetSection("RabbitMq").Get<RabbitMqOptions>() ?? new RabbitMqOptions();
    options.QueueName = $"{options.QueueName ?? "image.queue"}.delete";
    options.DeadLetterQueueName = $"{options.DeadLetterQueueName ?? "image.dlq"}.delete";
    options.DeadLetterRoutingKey = "image.delete";

    return new ImageDeleteConsumerHostedService(
        new RabbitMqConsumer(Options.Create(options), sp.GetRequiredService<ILogger<RabbitMqConsumer>>()),
        sp.GetRequiredService<ICloudinaryUploader>(),
        sp.GetRequiredService<ILogger<ImageDeleteConsumerHostedService>>());
});

var host = builder.Build();
host.Run();
