using Triply.Api.DependencyInjection;
using Triply.Api.Middlewares;
using Triply.Infrastructure.DependencyInjection;
using MessageQueue.DependencyInjection;
using Triply.Api.Endpoints;
using Logging.DependencyInjection;
using Serilog;

namespace Triply.Api;

/// <summary>Represents the program.</summary>
public class Program
{
    /// <summary>Mains.</summary>
    public static async Task Main(string[] args)
    {
        TriplyLoggingExtensions.CreateBootstrapLogger();
        try
        {
            Log.Information("Starting Triply API");
            await RunAsync(args);
        }
        catch (Exception exception) when (exception is not HostAbortedException)
        {
            Log.Fatal(exception, "Triply API terminated unexpectedly");
        }
        finally
        {
            await Log.CloseAndFlushAsync();
        }
    }

    private static async Task RunAsync(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        // Add Serilog (Console + Elasticsearch)
        builder.AddTriplyLogging("api");
        
        // Add booking options.
        builder.Services.AddBookingOptions(builder.Configuration);
        
        // Add Triply DbContext
        builder.Services.AddTriplyDbContext(builder.Configuration);
        
        // Add Redis Service
        builder.Services.AddRedisService(builder.Configuration);
        
        // Add Sieve Service
        builder.Services.AddSieveService(builder.Configuration);
        
        // Add Dependencies
        builder.Services.AddInfrastructureDependencies();
        
        // Add Decorators
        builder.Services.ApplyDecorators();
        
        // Add RabbitMqMessaging Service
        builder.Services.AddRabbitMqMessaging(builder.Configuration);
        
        // Add Identity Settings
        builder.Services.AddIdentityServices();
        
        // Add JWT Authentication Settings
        builder.Services.AddJwtAuthentication(builder.Configuration);
        
        // Add Generate Invoices PDF Configuration
        builder.Services.AddGenerateInvoiceConfigurations();
        
        // Add Payment Gateway
        builder.Services.AddPaymentServices(builder.Configuration);
        
        // Add Health Checks Service
        builder.Services.AddHealthChecks();

        // Add services to the container.
        builder.Services.AddAuthorization();
        
        // Add Swagger
        builder.Services.AddSwagger();
        
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();

        var app = builder.Build();

        await app.Services.SeedAsync();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Triply API v1");
                options.RoutePrefix = "swagger";
                options.DisplayRequestDuration();
            });
        }

        // Correlation id + one log per request
        app.UseTriplyRequestLogging();
        app.UseHttpsRedirection();
        app.UseMiddleware<ErrorHandlerMiddleware>();
        
        app.MapHealthChecks("/health");
        app.UseAuthorization();

        // Map all endpoints
        app.MapEndpoints();
        // When route not found return 404 Not Found
        app.Map404NotFoundEndpoints();
        await app.RunAsync();
    }
}