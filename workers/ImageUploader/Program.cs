using ImageUploader;
using ImageUploader.IProviders;
using ImageUploader.Options;
using ImageUploader.Providers;
using MessageQueue.DependencyInjection;
using Triply.Application.Interfaces.Repositories;
using Triply.Infrastructure.DependencyInjection;
using Triply.Infrastructure.Repositories;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddRabbitMqMessaging(builder.Configuration);
builder.Services.AddTriplyDbContext(builder.Configuration);

builder.Services.AddOptions<CloudinaryOptions>()
    .Bind(builder.Configuration.GetSection("Cloudinary"))
    .Validate(o => !string.IsNullOrWhiteSpace(o.CloudName)
                   && !string.IsNullOrWhiteSpace(o.ApiKey)
                   && !string.IsNullOrWhiteSpace(o.ApiSecret),
        "Cloudinary CloudName, ApiKey and ApiSecret must be configured.")
    .ValidateOnStart();

builder.Services.AddSingleton<ICloudinaryUploader, CloudinaryUploader>();
builder.Services.AddScoped<IHotelImageRepository, HotelImageRepository>();
builder.Services.AddHostedService<ImageUploadConsumerHostedService>();

var host = builder.Build();
host.Run();
